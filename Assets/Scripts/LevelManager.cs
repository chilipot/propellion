using UnityEngine;
using UnityEngine.SceneManagement;

// TODO: determine best practice for level managers (all static members? always use FindObjectOfType<LevelManager>()? always use inspector variable?)
public class LevelManager : MonoBehaviour
{
    
    private UIManager ui;
    private GrappleGunBehavior grappleGun;
    private bool levelOver;
    private Rigidbody playerBody;
    
    private void Start()
    {
        ui = FindObjectOfType<UIManager>();
        grappleGun = FindObjectOfType<GrappleGunBehavior>();
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
        EndLevel(true);
        playerBody.constraints = RigidbodyConstraints.FreezeAll;
    }

    public void Lose()
    {
        if (levelOver) return;
        EndLevel(false);
        
        // TODO: here and everywhere else both FreeCams are changed, use a more general method instead
        CameraController.FreeCam = false;
        PhysicsCameraController.FreeCam = false;
    }

    private void EndLevel(bool won)
    {
        ui.SetLevelStatus(won);
        levelOver = true;
        grappleGun.StopGrapple();
    }

    public bool LevelIsOver()
    {
        return levelOver;
    }
}
