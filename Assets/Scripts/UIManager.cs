using System;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public Text healthBar;

    private void Start()
    {
        healthBar.text = "";
    }
}