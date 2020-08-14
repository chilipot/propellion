using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class FadeToBlackPlayer : MonoBehaviour
{
    private static readonly int FadeOut = Animator.StringToHash("FadeOut");
    
    private bool startedFade = false;
    
    // TODO: remove if never used
    public void FadeAndLoadScene(int sceneIndex)
    {
        StartCoroutine(nameof(FadeAndLoadSceneCoroutine), sceneIndex);
    }
    
    public void FadeAndLoadNextScene()
    {
        FadeAndLoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }
    
    private IEnumerator FadeAndLoadSceneCoroutine(int sceneIndex)
    {
        if (startedFade) yield break;
        startedFade = true;
        var fadeToBlackAnimation = GetComponent<Animator>();
        fadeToBlackAnimation.SetTrigger(FadeOut);
        var animationDuration = fadeToBlackAnimation.GetCurrentAnimatorStateInfo(0).length;
        yield return new WaitForSeconds(animationDuration);
        SceneManager.LoadScene(sceneIndex);
    }
}