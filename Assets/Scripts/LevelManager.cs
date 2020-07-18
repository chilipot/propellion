using UnityEngine;
using UnityEngine.SceneManagement;

// TODO: determine best practice for level managers (all static members? always use FindObjectOfType<LevelManager>()? always use inspector variable?)
public class LevelManager : MonoBehaviour
{
    
    public UIManager ui;
    
    private bool levelOver;
    private Rigidbody playerBody;
    
    private void Start()
    {
        levelOver = false;
        playerBody = GameObject.FindGameObjectWithTag("Player").GetComponent<Rigidbody>();
    }

    private void Update()
    {
        if (levelOver && Input.anyKeyDown || Input.GetKeyDown(KeyCode.R))
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
    }

    public void Win()
    {
        if (levelOver) return;
        ui.SetLevelStatus("You win!");
        levelOver = true;
        playerBody.constraints = RigidbodyConstraints.FreezeAll;
    }

    public void Lose()
    {
        if (levelOver) return;
        ui.SetLevelStatus("Game over!");
        levelOver = true;
        
        // TODO: here and everywhere else both FreeCams are changed, use a more general method instead
        CameraController.FreeCam = false;
        PhysicsCameraController.FreeCam = false;
    }

    public bool LevelIsOver()
    {
        return levelOver;
    }
}
