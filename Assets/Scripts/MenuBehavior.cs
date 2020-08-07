using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuBehavior : MonoBehaviour
{
    // THIS IS A PLACEHOLDER BEFORE WE HAVE A LEVEL SELECT
    // TODO: REPLACE THIS WITH A PROPER LEVEL SELECT
    
    public string firstLevel;

    public void LoadLevel()
    {
        SceneManager.LoadScene(firstLevel);
    }

    public void ExitGame()
    {
        Application.Quit();
    }
}