using System;
using UnityEngine;
using UnityEngine.SceneManagement;

// TODO: determine best practice for level managers (all static members? always use FindObjectOfType<LevelManager>()? always use inspector variable?)
public class LevelManager : MonoBehaviour
{
    public static bool LevelInactive { get; private set; } = true;
    public static bool DebugMode { get; private set; } = false;
    public static Camera MainCamera { get; private set; }
    public static Transform Player { get; private set; }
    public static Rigidbody PlayerRb { get; private set; }

    public bool enableDebugMode = false;
    
    private UIManager ui;
    private GrappleGunBehavior grappleGun;
    private AudioSource winSfx, loseSfx;
    private bool levelStarted;

    private void Awake()
    {
        LevelInactive = true;
        DebugMode = enableDebugMode;
        MainCamera = Camera.main;
        Player = GameObject.FindWithTag("Player").transform;
        PlayerRb = Player.GetComponent<Rigidbody>();
    }

    private void Start()
    {
        ui = FindObjectOfType<UIManager>();
        grappleGun = FindObjectOfType<GrappleGunBehavior>();
        var audioSources = MainCamera.GetComponents<AudioSource>();
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
        if ((LevelInactive && levelStarted && Input.anyKeyDown) || Input.GetKeyDown(KeyCode.R))
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
    }

    public void Win()
    {
        if (LevelInactive) return;
        EndLevel(true);
        winSfx.Play();
        PlayerRb.constraints = RigidbodyConstraints.FreezeAll;
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
        ui.SetLevelStatus(won ? UIManager.LevelStatus.Win : UIManager.LevelStatus.Lose);
        LevelInactive = true;
        grappleGun.StopGrapple();
    }
}
