using UnityEngine;

public class MenuBehavior : MonoBehaviour
{
    // THIS IS A PLACEHOLDER BEFORE WE HAVE A LEVEL SELECT
    // TODO: REPLACE THIS WITH A PROPER LEVEL SELECT
    
    public AudioClip buttonClickSfx;

    private Transform mainCam;
    private bool menuOptionSelected = false;

    private void Awake()
    {
        mainCam = Camera.main.transform;
    }

    public void PlayGame()
    {
        if (menuOptionSelected) return;
        menuOptionSelected = true;
        AudioSource.PlayClipAtPoint(buttonClickSfx, mainCam.position);
        FindObjectOfType<FadeToBlackPlayer>().FadeAndLoadNextScene();
    }

    public void ExitGame()
    {
        Application.Quit();
    }
}