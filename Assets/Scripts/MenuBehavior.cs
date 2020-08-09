using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuBehavior : MonoBehaviour
{
    // THIS IS A PLACEHOLDER BEFORE WE HAVE A LEVEL SELECT
    // TODO: REPLACE THIS WITH A PROPER LEVEL SELECT
    
    public string firstLevel;
    public Animator fadeToBlackAnimation;
    public AudioClip buttonClickSfx;

    private static readonly int FadeOut = Animator.StringToHash("FadeOut");

    private Transform mainCam;
    private bool menuOptionSelected = false;

    private void Awake()
    {
        mainCam = Camera.main.transform;
    }

    private IEnumerator LoadLevel(string levelName)
    {
        if (menuOptionSelected) yield break;
        menuOptionSelected = true;
        fadeToBlackAnimation.SetTrigger(FadeOut);
        var animationDuration = fadeToBlackAnimation.GetCurrentAnimatorStateInfo(0).length;
        yield return new WaitForSeconds(animationDuration);
        SceneManager.LoadScene(levelName);
    }
    
    public void PlayGame()
    {
        AudioSource.PlayClipAtPoint(buttonClickSfx, mainCam.position);
        StartCoroutine(nameof(LoadLevel), firstLevel);
    }

    public void ExitGame()
    {
        Application.Quit();
    }
}