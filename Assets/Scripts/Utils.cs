using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using Random = UnityEngine.Random;

public class WeightedRandomBag<T>
{
    private struct Entry
    {
        public float AccumulatedWeight;
        public T Item;
    }

    private readonly List<Entry> entries = new List<Entry>();
    private float accumulatedWeight;

    public void AddEntry(T item, float weight)
    {
        accumulatedWeight += weight;
        entries.Add(new Entry {Item = item, AccumulatedWeight = accumulatedWeight});
    }

    public T GetRandom()
    {
        var r = Random.value * accumulatedWeight;

        foreach (var entry in entries.Where(entry => entry.AccumulatedWeight >= r))
        {
            return entry.Item;
        }

        return default; //should only happen when there are no entries
    }
}

public static class AudioSourceExtension
{
    public static AudioSource PlayClipAndGetSource(AudioClip clip, Vector3 pos){
        var tempObj = new GameObject("TempAudio");
        tempObj.transform.position = pos;
        var tempAudioSource = tempObj.AddComponent<AudioSource>();
        tempAudioSource.clip = clip;
        tempAudioSource.Play();
        Object.Destroy(tempObj, clip.length);
        return tempAudioSource;
    }
}