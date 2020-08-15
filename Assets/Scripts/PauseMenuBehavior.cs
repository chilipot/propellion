using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenuBehavior : MonoBehaviour
{
    public GameObject PauseMenu;
    public static bool Paused { get; private set; } = false;

    private UIManager ui;
    private LevelManager levelManager;
    
    private void Start()
    {
        levelManager = FindObjectOfType<LevelManager>();
        ui = FindObjectOfType<UIManager>();
        // Paused = false;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab) 
            && (LevelManager.CurrentLevelStatus == LevelManager.LevelStatus.Playing 
                || LevelManager.CurrentLevelStatus == LevelManager.LevelStatus.Paused))
        {
            TogglePause();
        }
    }

    private void SetPause(bool paused)
    {
        Paused = paused;
        Cursor.visible = Paused;
        Cursor.lockState = (Paused) ? CursorLockMode.None : CursorLockMode.Locked;
        PauseMenu.SetActive(Paused);
        levelManager.SetLevelStatus((Paused) ? LevelManager.LevelStatus.Paused : LevelManager.LevelStatus.Playing);
    }

    public void TogglePause()
    {
        SetPause(!Paused);
    }
    
    public void Restart()
    {
        SetPause(false);
        levelManager.ReloadCurrentLevel();
    }

    public void ReturnToMainMenu()
    {
        SetPause(false);
        SceneManager.LoadScene(0); // First Scene is the main menu
    }
}