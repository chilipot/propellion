using UnityEngine;

using System;
using System.Collections.Generic;
using Random = UnityEngine.Random;

class WeightedRandomBag<T>
{
    private struct Entry
    {
        public float accumulatedWeight;
        public T item;
    }

    private List<Entry> entries = new List<Entry>();
    private float accumulatedWeight;

    public void AddEntry(T item, float weight)
    {
        accumulatedWeight += weight;
        entries.Add(new Entry {item = item, accumulatedWeight = accumulatedWeight});
    }

    public T GetRandom()
    {
        var r = Random.value * accumulatedWeight;

        foreach (var entry in entries)
        {
            if (entry.accumulatedWeight >= r)
            {
                return entry.item;
            }
        }

        return default(T); //should only happen when there are no entries
    }
}