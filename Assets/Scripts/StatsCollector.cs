using System;
using System.Collections.Generic;
using UnityEngine;

public enum Stat
{
    AliensSlain,
    MedicalCanistersPickedUp,
    AsteroidsBumped,
    BlackHoleDeaths,
    AsteroidCollisionDeaths,
    EnemyProjectileDeaths,
    Successes,
    EmptyThrusters,
    TotalDeaths
}

public enum StatEventType
{
    AlienSlayed,
    MedicalCanisterPickedUp,
    AsteroidBumped,
    DeathToBlackHole,
    SpaceSuitDamageDeath,
    Success,
    ThrusterEmptied
}

public class StatEvent
{
    public delegate void StatAction(StatEventType eventType, object payload);
    public event StatAction OnTrigger;

    public StatEventType EventType { get; }

    public StatEvent(StatEventType eventType)
    {
        EventType = eventType;
    }

    public void Trigger(object payload = null)
    {
        OnTrigger?.Invoke(EventType, payload);
    }
}

public class StatChangeRecord
{
    public StatChangeRecord(DateTime timestamp, Stat stat)
    {
        Timestamp = timestamp;
        ChangedStat = stat;
    }

    public Stat ChangedStat { get; }
    public DateTime Timestamp { get; }
}

public class StatsCollector : MonoBehaviour
{
    public static readonly Dictionary<Stat, int> GlobalStats = new Dictionary<Stat, int>();
    public static readonly Dictionary<int, Dictionary<Stat, int>> PerLevelStats = new Dictionary<int, Dictionary<Stat, int>>();
    public static readonly Dictionary<Stat, int> PerLevelAttemptStats = new Dictionary<Stat, int>();
    public static readonly List<StatChangeRecord> StatEventTimeline = new List<StatChangeRecord>();

    public delegate void StatUpdate(StatChangeRecord statChange);

    public static event StatUpdate OnStatUpdate;

    private static void InitPerLevelAttemptStats()
    {
        foreach (Stat _stat in Enum.GetValues(typeof(Stat)))
        {
            PerLevelAttemptStats[_stat] = 0;
        }
    }
    
    private static void InitGlobalStats()
    {
        foreach (Stat _stat in Enum.GetValues(typeof(Stat)))
        {
            GlobalStats[_stat] = 0;
        }
    }

    private static void InitPerLevelStats(int levelIndex)
    {
        PerLevelStats[levelIndex] = new Dictionary<Stat, int>();
        foreach (Stat _stat in Enum.GetValues(typeof(Stat)))
        {
            PerLevelStats[levelIndex][_stat] = 0;
        }
    }
    
    public static int GetGlobalStat(Stat stat)
    {
        if (GlobalStats.Count == 0) InitGlobalStats();
        return GlobalStats[stat];
    }

    public static int GetLevelStat(Stat stat)
    {
        var levelIndex = LevelManager.LevelIndex;
        if (PerLevelStats.Count == 0) InitPerLevelStats(levelIndex);

        return PerLevelStats[levelIndex][stat];
    }

    private void OnEnable()
    {
        PerLevelAttemptStats.Clear();
        InitPerLevelAttemptStats();
        MedicalCanisterPickup.PickupEvent.OnTrigger += HandleEvent;
        CollisionPhysicsBehavior.AsteroidBumpEvent.OnTrigger += HandleEvent;
        BlackHoleConsumption.ConsumptionEvent.OnTrigger += HandleEvent;
        ExitBehavior.WinEvent.OnTrigger += HandleEvent;
        SpaceSuitManager.DamageDeathEvent.OnTrigger += HandleEvent;
        EnemyAI.SlayAlienEvent.OnTrigger += HandleEvent;
        ThrusterManager.EmptyThrusterEvent.OnTrigger += HandleEvent;
    }

    private void OnDisable()
    {
        MedicalCanisterPickup.PickupEvent.OnTrigger -= HandleEvent;
        CollisionPhysicsBehavior.AsteroidBumpEvent.OnTrigger -= HandleEvent;
        BlackHoleConsumption.ConsumptionEvent.OnTrigger -= HandleEvent;
        ExitBehavior.WinEvent.OnTrigger -= HandleEvent;
        SpaceSuitManager.DamageDeathEvent.OnTrigger -= HandleEvent;
        EnemyAI.SlayAlienEvent.OnTrigger -= HandleEvent;
        ThrusterManager.EmptyThrusterEvent.OnTrigger -= HandleEvent;
    }

    private static Stat? ParseChangedStat(StatEventType eventType, object payload)
    {
        var eventResultMapping = new Dictionary<StatEventType, Stat>()
        {
            [StatEventType.AlienSlayed] = Stat.AliensSlain,
            [StatEventType.MedicalCanisterPickedUp] = Stat.MedicalCanistersPickedUp,
            [StatEventType.AsteroidBumped] = Stat.AsteroidsBumped,
            [StatEventType.DeathToBlackHole] = Stat.BlackHoleDeaths,
            [StatEventType.Success] = Stat.Successes,
            [StatEventType.ThrusterEmptied] = Stat.EmptyThrusters
        };
        
        Stat? changedStat = null;
        if (eventType == StatEventType.SpaceSuitDamageDeath)
        {
            if (payload is HealthEffectSource typedPayload)
            {
                changedStat = (typedPayload == HealthEffectSource.Asteroid)
                    ? Stat.AsteroidCollisionDeaths
                    : Stat.EnemyProjectileDeaths;
            }
        }
        else
        {
            changedStat = eventResultMapping[eventType];
            if (changedStat == Stat.AsteroidCollisionDeaths
                || changedStat == Stat.BlackHoleDeaths
                || changedStat == Stat.EnemyProjectileDeaths)
            {
                
            }
        }

        return changedStat;
    }

    private static void IncrementChangedStat(Stat changedStat)
    {
        if (GlobalStats.Count == 0) InitGlobalStats();
        GlobalStats[changedStat]++;

        var levelIndex = LevelManager.LevelIndex;
        if (!PerLevelStats.ContainsKey(levelIndex)) InitPerLevelStats(levelIndex);
        PerLevelStats[levelIndex][changedStat]++;

        PerLevelAttemptStats[changedStat]++;
    }

    private static void HandleEvent(StatEventType eventType, object payload)
    {
        var changedStat = ParseChangedStat(eventType, payload);
        if (changedStat == null) return;

        IncrementChangedStat(changedStat.Value);

        // TODO: Make this all support multiple changed stats
        if (eventType == StatEventType.DeathToBlackHole || eventType == StatEventType.SpaceSuitDamageDeath)
        {
            IncrementChangedStat(Stat.TotalDeaths);
        }

        var newStatChange = new StatChangeRecord(DateTime.Now, changedStat.Value);
        StatEventTimeline.Insert(0, newStatChange);

        OnStatUpdate?.Invoke(newStatChange);
    }
}