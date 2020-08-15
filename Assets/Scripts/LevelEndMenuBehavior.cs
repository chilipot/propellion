using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LevelEndMenuBehavior : MonoBehaviour
{
    public GameObject menu;
    public Text title;
    public Button continueButton;
    public Text continueButtonText;

    public string continueOnWinText = "CONTINUE";
    public string continueOnLoseText = "RESTART";
    public string winMessage = "MISSION SUCCESS";
    public string loseMessage = "MISSION FAILED";

    private AudioSource bemis;
    private LevelManager levelManager;

    private void Start()
    {
        bemis = GameObject.FindGameObjectWithTag("B3M1S").GetComponent<AudioSource>();
        levelManager = FindObjectOfType<LevelManager>();
    }

    public void Show()
    {
        StartCoroutine(nameof(EnableContinueWhenBemisFinishes));
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        menu.SetActive(true);
        switch (LevelManager.CurrentLevelStatus)
        {
            case LevelManager.LevelStatus.Win:
                continueButtonText.text = continueOnWinText;
                title.text = winMessage;
                if (LevelManager.IsLastLevel)
                {
                    continueButton.gameObject.SetActive(false);
                }
                break;
            case LevelManager.LevelStatus.Lose:
                continueButtonText.text = continueOnLoseText;
                title.text = loseMessage;
                break;
        }
    }

    private IEnumerator EnableContinueWhenBemisFinishes()
    {
        
        continueButton.interactable = false;
        yield return new WaitWhile(() => bemis.isPlaying);
        continueButton.interactable = true;
    }

    public void Hide()
    {
        menu.SetActive(false);
    }

    public void Continue()
    {
        switch (LevelManager.CurrentLevelStatus)
        {
            case LevelManager.LevelStatus.Win:
                ProceedToNextLevel();
                break;
            case LevelManager.LevelStatus.Lose:
                Restart();
                break;
        }
    }

    private void ProceedToNextLevel()
    {
        levelManager.LoadNextLevel();
    }
    
    public void Restart()
    {
        levelManager.ReloadCurrentLevel();
    }

    public void ReturnToMainMenu()
    {
        SceneManager.LoadScene(0); // First Scene is the main menu
    }
}