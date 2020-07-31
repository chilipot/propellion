using UnityEngine;
using UnityEngine.SceneManagement;

// TODO: determine best practice for level managers (all static members? always use FindObjectOfType<LevelManager>()? always use inspector variable?)
public class LevelManager : MonoBehaviour
{
    public static bool LevelInactive { get; private set; }
    
    private UIManager ui;
    private GrappleGunBehavior grappleGun;
    private Rigidbody playerBody;
    private AudioSource winSfx, loseSfx;
    private bool levelStarted;
    
    private void Start()
    {
        LevelInactive = true;
        ui = FindObjectOfType<UIManager>();
        grappleGun = FindObjectOfType<GrappleGunBehavior>();
        playerBody = GameObject.FindGameObjectWithTag("Player").GetComponent<Rigidbody>();
        var audioSources = GetComponents<AudioSource>();
        winSfx = audioSources[1];
        loseSfx = audioSources[2];
        levelStarted = false;
    }

    private void Update()
    {
        if (!levelStarted && ProceduralGeneration.FinishedGenerating)
        {
            LevelInactive = false;
            levelStarted = true;
        }
        if (LevelInactive && levelStarted && Input.anyKeyDown || Input.GetKeyDown(KeyCode.R))
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
    }

    public void Win()
    {
        if (LevelInactive) return;
        EndLevel(true);
        winSfx.Play();
        playerBody.constraints = RigidbodyConstraints.FreezeAll;
    }

    public void Lose()
    {
        if (LevelInactive) return;
        EndLevel(false);
        loseSfx.Play();
        
        // TODO: here and everywhere else both FreeCams are changed, use a more general method instead
        CameraController.FreeCam = false;
        PhysicsCameraController.FreeCam = false;
    }

    private void EndLevel(bool won)
    {
        ui.SetLevelStatus(won);
        LevelInactive = true;
        grappleGun.StopGrapple();
    }
}
