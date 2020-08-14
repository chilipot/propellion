using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuBehavior : MonoBehaviour
{
    // THIS IS A PLACEHOLDER BEFORE WE HAVE A LEVEL SELECT
    // TODO: REPLACE THIS WITH A PROPER LEVEL SELECT
    
    public Animator fadeToBlackAnimation;
    public AudioClip buttonClickSfx;

    private static readonly int FadeOut = Animator.StringToHash("FadeOut");

    private Transform mainCam;
    private bool menuOptionSelected = false;

    private void Awake()
    {
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        mainCam = Camera.main.transform;
    }

    private IEnumerator LoadLevel(int levelIndex)
    {
        if (menuOptionSelected) yield break;
        menuOptionSelected = true;
        fadeToBlackAnimation.SetTrigger(FadeOut);
        var animationDuration = fadeToBlackAnimation.GetCurrentAnimatorStateInfo(0).length;
        yield return new WaitForSeconds(animationDuration);
        SceneManager.LoadScene(levelIndex);
    }
    
    public void PlayGame()
    {
        if (menuOptionSelected) return;
        AudioSource.PlayClipAtPoint(buttonClickSfx, mainCam.position);
        StartCoroutine(nameof(LoadLevel), SceneManager.GetActiveScene().buildIndex + 1);
    }

    public void ExitGame()
    {
        Application.Quit();
    }
}