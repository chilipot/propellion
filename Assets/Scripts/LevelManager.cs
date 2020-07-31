using UnityEngine;
using UnityEngine.SceneManagement;

// TODO: determine best practice for level managers (all static members? always use FindObjectOfType<LevelManager>()? always use inspector variable?)
public class LevelManager : MonoBehaviour
{
    
    private UIManager ui;
    private GrappleGunBehavior grappleGun;
    private bool levelOver;
    private Rigidbody playerBody;
    private AudioSource winSfx, loseSfx;
    
    private void Start()
    {
        ui = FindObjectOfType<UIManager>();
        grappleGun = FindObjectOfType<GrappleGunBehavior>();
        levelOver = false;
        playerBody = GameObject.FindGameObjectWithTag("Player").GetComponent<Rigidbody>();
        var audioSources = GetComponents<AudioSource>();
        winSfx = audioSources[1];
        loseSfx = audioSources[2];
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
        winSfx.Play();
        playerBody.constraints = RigidbodyConstraints.FreezeAll;
    }

    public void Lose()
    {
        if (levelOver) return;
        EndLevel(false);
        loseSfx.Play();
        
        // TODO: here and everywhere else both FreeCams are changed, use a more general method instead
        CameraController.FreeCam = false;
        PhysicsCameraController.FreeCam = false;
    }

    private void EndLevel(bool won)
    {
        ui.SetLevelStatus(won ? UIManager.LevelStatus.Win : UIManager.LevelStatus.Lose);
        levelOver = true;
        grappleGun.StopGrapple();
    }

    public bool LevelIsOver()
    {
        return levelOver;
    }
}
