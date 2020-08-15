using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class StatsScreenBehavior : MonoBehaviour
{
    private TextMeshProUGUI textbox;

    private void Start()
    {
    }

    private void OnEnable()
    {
        if (!textbox)
        {
            textbox = GetComponent<TextMeshProUGUI>();
        }

        textbox.SetText(GlobalStatsText());
    }

    private string GlobalStatsText()
    {
        var lookup = new Dictionary<string, Stat>()
        {
            ["Black Hole Deaths"] = Stat.BlackHoleDeaths,
            ["Asteroid Collision Deaths"] = Stat.AsteroidCollisionDeaths,
            ["Asteroids Bumped"] = Stat.AsteroidsBumped,
            ["Aliens Slain"] = Stat.AliensSlain,
            ["Medical Canisters Picked Up"] = Stat.MedicalCanistersPickedUp
        };
        var text = $"Time Played: {Mathf.RoundToInt(Time.realtimeSinceStartup / 60)} min\n\n";
        foreach (var entry in lookup)
        {
            int val = 0;
            if (StatsCollector.GlobalStats.ContainsKey(entry.Value))
            {
                val = StatsCollector.GlobalStats[entry.Value];
            }

            text += $"{entry.Key} : {val}\n\n";
        }

        return text;
    }
}