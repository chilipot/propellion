using UnityEngine;
using UnityEngine.SceneManagement;

// TODO: determine best practice for level managers (all static members? always use FindObjectOfType<LevelManager>()? always use inspector variable?)
public class LevelManager : MonoBehaviour
{
    public static string LevelName => SceneManager.GetActiveScene().name;
    public static int LevelIndex => SceneManager.GetActiveScene().buildIndex;
    public static bool LevelIsOver { get; private set; }
    public static bool DebugMode { get; private set; }
    public static bool GodMode { get; private set; }
    public static Camera MainCamera { get; private set; }
    public static Transform Player { get; private set; }
    public static Rigidbody PlayerRb { get; private set; }

    public static bool LevelIsActive => ProceduralGeneration.FinishedGenerating && !LevelIsOver;

    public bool enableDebugMode = false;
    public bool enableGodMode = false;

    private UIManager ui;
    private AudioSource bemis;
    private GrappleGunBehavior grappleGun;
    private AudioSource winSfx, loseSfx;
    private bool levelWon;

    private void Awake()
    {
        LevelIsOver = false;
        DebugMode = enableDebugMode;
        GodMode = enableGodMode;
        MainCamera = Camera.main;
        Player = GameObject.FindWithTag("Player").transform;
        PlayerRb = Player.GetComponent<Rigidbody>();
    }

    private void Start()
    {
        bemis = GameObject.FindGameObjectWithTag("B3M1S").GetComponent<AudioSource>();
        ui = FindObjectOfType<UIManager>();
        grappleGun = FindObjectOfType<GrappleGunBehavior>();
        var audioSources = MainCamera.GetComponents<AudioSource>();
        winSfx = audioSources[1];
        loseSfx = audioSources[2];
        levelWon = false;
    }

    private void Update()
    {
        if (bemis.isPlaying) return; // LET BEMIS FINISH!
        var isLastLevel = LevelIndex == 3; // TODO: come up with a story-relevant endgame instead of just hardcodedly repeating last level
        if (LevelIsOver && (!levelWon || isLastLevel) && Input.anyKeyDown || !LevelIsOver && Input.GetKeyDown(KeyCode.R)) ReloadCurrentLevel();
        else if (LevelIsOver && levelWon && Input.anyKeyDown) LoadNextLevel();
    }

    public void Win()
    {
        if (!LevelIsActive) return;
        EndLevel(true);
        winSfx.Play();
        PlayerRb.constraints = RigidbodyConstraints.FreezeAll;
        levelWon = true;
    }

    public void Lose()
    {
        if (GodMode || !LevelIsActive) return;
        EndLevel(false);
        loseSfx.Play();
        
        // TODO: here and everywhere else both FreeCams are changed, use a more general method instead
        CameraController.FreeCam = false;
        PhysicsCameraController.FreeCam = false;
    }

    private void EndLevel(bool won)
    {
        ui.SetLevelStatus(won ? UIManager.LevelStatus.Win : UIManager.LevelStatus.Lose);
        LevelIsOver = true;
        grappleGun.StopGrapple();
    }

    private void LoadNextLevel()
    {
        SceneManager.LoadScene(LevelIndex + 1);
    }

    private void ReloadCurrentLevel()
    {
        SceneManager.LoadScene(LevelIndex);
    }
}
