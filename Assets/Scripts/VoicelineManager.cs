using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;


public class VoicelinePlaybackRecord
{
    public int Voiceline { get; }
    public DateTime Timestamp { get; }

    public VoicelinePlaybackRecord(int index)
    {
        Voiceline = index;
        Timestamp = DateTime.Now;
    }
}

public class VoicelinePlaybackHistory
{
    public static List<VoicelinePlaybackRecord> GlobalTimeline = new List<VoicelinePlaybackRecord>();
    public static Dictionary<int, List<VoicelinePlaybackRecord>> LevelTimelines = new Dictionary<int, List<VoicelinePlaybackRecord>>();
    public static List<VoicelinePlaybackRecord> LevelAttemptTimeline =new List<VoicelinePlaybackRecord>();
    
    private static Dictionary<int, Dictionary<int, int>> LevelPlaybackCounts = new Dictionary<int, Dictionary<int, int>>();

    public void Initialize(int numVoicelines)
    {
        LevelAttemptTimeline.Clear();
        if (LevelTimelines.Count == 0 && LevelPlaybackCounts.Count == 0)
        {
            for (var i = 0; i < SceneManager.sceneCountInBuildSettings; i++)
            {
                LevelPlaybackCounts.Add(i, new Dictionary<int, int>()); 
                for (var j = 0; j < numVoicelines; j++)
                {
                    LevelPlaybackCounts[i].Add(j, 0);
                }
                LevelTimelines.Add(i, new List<VoicelinePlaybackRecord>());
            }
        }
    }

    private Dictionary<int, int> CurrentLevelPlaybackCountLookup => LevelPlaybackCounts[LevelManager.LevelIndex];
    public List<VoicelinePlaybackRecord> CurrentLevelTimeline => LevelTimelines[LevelManager.LevelIndex];
    public int GlobalCount(int voiceline) => LevelPlaybackCounts.Values.Sum(d => d[voiceline]);
    public int LevelCount(int voiceline) => CurrentLevelPlaybackCountLookup[voiceline];
    public int LevelAttemptCount(int voiceline) => LevelAttemptTimeline.Count(record => record.Voiceline == voiceline);

    public void AddRecord(int voiceline)
    {
        var record = new VoicelinePlaybackRecord(voiceline);
        GlobalTimeline.Insert(0, record);
        CurrentLevelTimeline.Insert(0, record);
        LevelAttemptTimeline.Insert(0, record);
        CurrentLevelPlaybackCountLookup[voiceline]++;
    }
}

public class VoicelineManager : MonoBehaviour
{
    public AudioClip[] setVoicelines;
    
    public static AudioClip[] voicelines;

    private ProceduralGeneration entityManager;

    private static AudioSource bemisAudioSource;
    
    private static VoicelinePlaybackHistory PlaybackHistory = new VoicelinePlaybackHistory();

    private interface IVoiceline
    {
        void Play();
    }

    private abstract class AVoiceline : IVoiceline
    {
        protected void _Play(int voicelineIndex)
        {
            var voiceline = voicelines[voicelineIndex];
            Debug.Log("PLAYED: " + voiceline.name);
            if (!bemisAudioSource.isPlaying)
            {
                bemisAudioSource.clip = voiceline;
                bemisAudioSource.PlayDelayed(1);
            }
            PlaybackHistory.AddRecord(voicelineIndex);
        }

        public abstract void Play();
    }

    private class Voiceline : AVoiceline
    {
        private int voicelineIndex;
        public Voiceline(int voicelineIndex)
        {
            this.voicelineIndex = voicelineIndex;
        }
    
        public override void Play()
        {
            // Play the voiceline
            _Play(voicelineIndex);
        }
    }

    private class RandomVoicelineGroup : AVoiceline
    {
        private int[] voicelineIndices;
        private bool withReplacement;
        private Func<int, bool> predicate;
        private bool includeNothing;
        public RandomVoicelineGroup(int[] voicelineIndices, Func<int, bool> predicate = null, bool withReplacement = false, bool includeNothing = true)
        {
            this.voicelineIndices = voicelineIndices;
            this.withReplacement = withReplacement;
            this.predicate = predicate ?? ((i) => PlaybackHistory.GlobalCount(i) == 0);
            this.includeNothing = includeNothing;
        }
        public override void Play()
        {
            var voicelinesToPlay= (withReplacement) ? voicelineIndices.ToList() : voicelineIndices.Where(voicelineIndex => predicate(voicelineIndex)).ToList();
            if (voicelinesToPlay.Count == 0) voicelinesToPlay = voicelineIndices.ToList();
            var randInd = Random.Range(0, (includeNothing ? voicelinesToPlay.Count : voicelinesToPlay.Count - 1));
            if (includeNothing && randInd == voicelinesToPlay.Count)
            {
                Debug.Log("PLAYED: NOTHING");
            }
            else
            {
                // Play the random voiceline
                var voicelineIndex = voicelinesToPlay[randInd];
                _Play(voicelineIndex);
            }
        }
    }

    private void Awake()
    {
        voicelines = setVoicelines;
        PlaybackHistory.Initialize(voicelines.Length); // Doesn't always initialize without persistence
    }

    private void Start()
    {
        entityManager = FindObjectOfType<ProceduralGeneration>();
    }

    private void OnEnable()
    {
        bemisAudioSource = GameObject.FindGameObjectWithTag("B3M1S").GetComponent<AudioSource>();
        StatsCollector.OnStatUpdate += PlayVoiceline;
    }

    private void OnDisable()
    {
        StatsCollector.OnStatUpdate -= PlayVoiceline;
    }

    private IVoiceline TraverseOnAlienSlayed(StatChangeRecord statChangeRecord)
    {
        var aliensSlain = StatsCollector.GetGlobalStat(Stat.AliensSlain);
        if (aliensSlain == 1)
        {
            // Play #14
            return new Voiceline(13);
        }
        else if (PlaybackHistory.GlobalCount(15) == 0 && !entityManager.disableBlackHole)
        {
            // Random Play: #15, #16, Nothing
            return new RandomVoicelineGroup(new []{14, 15});
        }
        else
        {
            // Random Play: #15 or Nothing
            return new RandomVoicelineGroup(new []{14});
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
            var asteroidsBumpedInSuccessionVoiceline = 1;
            var cooldown = 5; // in minutes
            var playedRecently = PlaybackHistory.CurrentLevelTimeline
                .TakeWhile(record => DateTime.Now.Subtract(record.Timestamp) <= TimeSpan.FromMinutes(cooldown))
                .Any(record => record.Voiceline == asteroidsBumpedInSuccessionVoiceline);
            if (!playedRecently)
            {
                return new Voiceline(asteroidsBumpedInSuccessionVoiceline);
            }
        } else if (StatsCollector.PerLevelAttemptStats[Stat.AsteroidsBumped] == 10)
        {
            // Random Play: #21, #6, #8
            return new RandomVoicelineGroup(new []{20, 5, 7});
        } else if (PlaybackHistory.LevelAttemptCount(5) > 0 && PlaybackHistory.LevelAttemptCount(6) == 0)
        {
            // Play #7
            return new Voiceline(6);
        }

        return null;
    }
    
    private IVoiceline TraverseOnDeath(StatChangeRecord statChangeRecord)
    {
        var numDeaths = StatsCollector.GetLevelStat(Stat.TotalDeaths);
        var initialDeathClips = new[] {0, 30, 21, 23};
        if (10 <= numDeaths && numDeaths <= (10 + 5 * (initialDeathClips.Length - 1)) && numDeaths % 5 == 0)
        {
            // Random Play: #1, #31, #22, #24
            return new RandomVoicelineGroup(initialDeathClips);
        } else if (numDeaths >= (10 + 5 * initialDeathClips.Length) && numDeaths % 5 == 0)
        {
            // Play #38
            return new Voiceline(38);
        }
        else
        {
            var playedTwentyNine = PlaybackHistory.GlobalCount(28) > 0;
            switch (statChangeRecord.ChangedStat)
            {
                case Stat.AsteroidCollisionDeaths
                    when (new[] {32, 31, 27, 26, 22, 18, 17, 9, 2, 3, 10}).All(v =>
                        PlaybackHistory.GlobalCount(v) > 0) && !playedTwentyNine:
                case Stat.EnemyProjectileDeaths
                    when (new[] {33, 35, 25, 36, 12, 2, 3, 10}).All(v => PlaybackHistory.GlobalCount(v) > 0) &&
                         !playedTwentyNine:
                case Stat.BlackHoleDeaths when (new[] {11, 29, 3, 10}).All(v => PlaybackHistory.GlobalCount(v) > 0) &&
                                               !playedTwentyNine:
                    // Play #29
                    return new Voiceline(28);
                    break;
                case Stat.BlackHoleDeaths when StatsCollector.GetLevelStat(Stat.BlackHoleDeaths) == 5:
                    // Play #12
                    return new Voiceline(11);
                    break;
                case Stat.BlackHoleDeaths:
                    // Random Play: #30, #4, #11
                    return new RandomVoicelineGroup(new [] {29, 3, 10});
                    break;
                case Stat.EnemyProjectileDeaths when StatsCollector.GetLevelStat(Stat.EnemyProjectileDeaths) >= 3:
                    // Random Play: #34, #36, #26, #37, #13, #3, #4, #11
                    return new RandomVoicelineGroup(new []{33, 35, 25, 36, 12, 2, 3, 10});
                    break;
                case Stat.EnemyProjectileDeaths:
                    // Random Play: #37, #13, #3, #4, #11
                    return new RandomVoicelineGroup(new []{36, 12, 2, 3, 10});
                case Stat.AsteroidCollisionDeaths:
                    // Random Play: #33, #32, #28, #27, #23, #19, #18, #11, #10, #4, #3
                    return new RandomVoicelineGroup(new []{32, 31, 27, 26, 22, 18, 17, 10, 9, 3, 2});
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
            return new Voiceline(4);
        }
        return null;
    }
    
    private IVoiceline TraverseOnEmptyThruster(StatChangeRecord statChangeRecord)
    {
        // TODO: Make this wait for 5 mins between playing when thruster refueling is added, otherwise this plays once
        if (StatsCollector.PerLevelAttemptStats[Stat.EmptyThrusters] == 1)
        {
            // Random Play: #20 or Nothing
            return new RandomVoicelineGroup(new []{19});
        }
        return null;
    }
    
    private IVoiceline TraverseOnBeatLevel(StatChangeRecord statChangeRecord)
    {
        if (!entityManager.disableBlackHole && StatsCollector.GetLevelStat(Stat.Successes) == 1
                                            && StatsCollector.GetLevelStat(Stat.TotalDeaths) == 0)
        {
            // Play #25
            return new Voiceline(24);
        }

        return null;
    }

    private void PlayVoiceline(StatChangeRecord statChangeRecord)
    {
        IVoiceline triggeredVoiceline = null;
        switch (statChangeRecord.ChangedStat)
        {
            case Stat.AliensSlain:
                triggeredVoiceline = TraverseOnAlienSlayed(statChangeRecord);
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