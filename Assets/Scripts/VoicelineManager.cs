using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public interface IVoiceline
{
   void Play();
}

public class VoicelineManager : MonoBehaviour
{
    public string[] voicelines;

    private ProceduralGeneration entityManager;
    private Dictionary<string, bool> playedVoicelines;

    private void Awake()
    {
        playedVoicelines = new Dictionary<string, bool>();
        foreach (var voiceline in voicelines)
        {
            playedVoicelines[voiceline] = false;
        }
    }

    private void Start()
    {
        entityManager = FindObjectOfType<ProceduralGeneration>();
    }

    private void OnEnable()
    {
        StatsCollector.OnStatUpdate += PlayVoiceline;
    }

    private void OnDisable()
    {
        StatsCollector.OnStatUpdate -= PlayVoiceline;
    }

    private bool AllLinesPlayed(IEnumerable<int> voicelineIndices) =>
        voicelineIndices.All((n) => playedVoicelines[voicelines[n]]);
    
    private IVoiceline TraverseOnAlienSlaid(StatChangeRecord statChangeRecord)
    {
        var aliensSlain = StatsCollector.GetLevelStat(Stat.AliensSlain);
        if (aliensSlain == 1)
        {
            // Play #14
        }
        else if (!playedVoicelines[voicelines[15]])
        {
            // Random Play: #15, #16, Nothing
        }
        else
        {
            // Random Play: #15 or Nothing
        }

        return null;
    }
    
    private IVoiceline TraverseOnAsteroidBumped(StatChangeRecord statChangeRecord)
    {
        var asteroidsBumpedInSuccession = 0;
        foreach (var record in StatsCollector.StatEventTimeline)
        {
            if (DateTime.Now.Subtract(record.Timestamp) > TimeSpan.FromSeconds(15)) break;

            if (record.ChangedStat == Stat.AsteroidsBumped)
            {
                asteroidsBumpedInSuccession++;
            }
        }

        if (asteroidsBumpedInSuccession >= 4)
        {
            // Play #2
        } else if (StatsCollector.PerLevelAttemptStats[Stat.AsteroidsBumped] == 10)
        {
            // Random Play: #21, #6, #8
        } else if (playedVoicelines[voicelines[5]] && !playedVoicelines[voicelines[6]])
        {
            // Play #7
        }

        return null;
    }
    
    private IVoiceline TraverseOnDeath(StatChangeRecord statChangeRecord)
    {
        var numDeaths = StatsCollector.GetLevelStat(Stat.TotalDeaths);
        if (10 <= numDeaths && numDeaths <= 25 && numDeaths % 5 == 0)
        {
            if (AllLinesPlayed(new[] {0, 30, 21, 23}) && !playedVoicelines[voicelines[28]])
            {
                // Play #29
            }
            else
            {
                // Random Play (unplayed): #1, #31, #22, #24
            }
        } else if (numDeaths >= 30 && numDeaths % 5 == 0)
        {
            // Play #38
        }
        else
        {
            var playedTwentyNine = playedVoicelines[voicelines[28]];
            switch (statChangeRecord.ChangedStat)
            {
                case Stat.AsteroidCollisionDeaths when AllLinesPlayed(new []{ 32, 31, 27, 26, 22, 18, 17, 9, 2, 3, 10}) && !playedTwentyNine:
                case Stat.EnemyProjectileDeaths when AllLinesPlayed(new[] {33, 35, 25, 2, 3, 10}) && !playedTwentyNine:
                case Stat.BlackHoleDeaths when AllLinesPlayed(new[] {11, 29, 3, 10}) && !playedTwentyNine:
                    // Play #29
                    break;
                case Stat.BlackHoleDeaths when StatsCollector.GetLevelStat(Stat.BlackHoleDeaths) == 5:
                    // Play #12
                    break;
                case Stat.BlackHoleDeaths:
                    // Random Play: #30, #4, #11
                    break;
                case Stat.EnemyProjectileDeaths when StatsCollector.GetLevelStat(Stat.EnemyProjectileDeaths) >= 3:
                    // Random Play: #34, #36, #26, #3, #4, #11
                    break;
                case Stat.AsteroidCollisionDeaths:
                    // Random Play: #33, #32, #28, #27, #23, #19, #18, #11, #10, #4, #3
                    break;
            }
        }
        return null;
    }
    
    private IVoiceline TraverseOnMedicalCanisterPickedUp(StatChangeRecord statChangeRecord)
    {
        if (StatsCollector.PerLevelAttemptStats[Stat.MedicalCanistersPickedUp] == 8)
        {
            // Play #5
        }
        return null;
    }
    
    private IVoiceline TraverseOnEmptyThruster(StatChangeRecord statChangeRecord)
    {
        // TODO: Make this wait for 5 mins between playing when thruster refueling is added, otherwise this plays once
        if (StatsCollector.PerLevelAttemptStats[Stat.EmptyThrusters] == 1)
        {
            // Random Play: #20 or Nothing
        }
        return null;
    }
    
    private IVoiceline TraverseOnBeatLevel(StatChangeRecord statChangeRecord)
    {
        if (!entityManager.disableBlackHole && StatsCollector.GetLevelStat(Stat.Successes) == 1
                                            && StatsCollector.GetLevelStat(Stat.TotalDeaths) == 0)
        {
            // Play #25
        }

        return null;
    }

    private void PlayVoiceline(StatChangeRecord statChangeRecord)
    {
        IVoiceline triggeredVoiceline = null;
        switch (statChangeRecord.ChangedStat)
        {
            case Stat.AliensSlain:
                triggeredVoiceline = TraverseOnAlienSlaid(statChangeRecord);
                break;
            case Stat.MedicalCanistersPickedUp:
                triggeredVoiceline = TraverseOnMedicalCanisterPickedUp(statChangeRecord);
                break;
            case Stat.AsteroidsBumped:
                triggeredVoiceline = TraverseOnAsteroidBumped(statChangeRecord);
                break;
            case Stat.BlackHoleDeaths:
            case Stat.AsteroidCollisionDeaths:
            case Stat.EnemyProjectileDeaths:
                triggeredVoiceline = TraverseOnDeath(statChangeRecord);
                break;
            case Stat.Successes:
                triggeredVoiceline = TraverseOnBeatLevel(statChangeRecord);
                break;
            case Stat.EmptyThrusters:
                triggeredVoiceline = TraverseOnEmptyThruster(statChangeRecord);
                break;
        }
        triggeredVoiceline?.Play();
    }
}