using System;
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

    public static bool LevelIsActive => ProceduralGeneration.FinishedGenerating && !LevelIsOver && !PauseMenuBehavior.Paused;
    public static LevelStatus CurrentLevelStatus { get; private set; }
    
    public bool enableDebugMode = false;
    public bool enableGodMode = false;
    public bool isLastLevel = false;

    private UIManager ui;
    private AudioSource bemis;
    private GrappleGunBehavior grappleGun;
    private AudioSource winSfx, loseSfx;
    private bool levelWon;
    
    public enum LevelStatus
    {
        Playing,
        Loading,
        Win,
        Lose,
        Paused
    }

    private void Awake()
    {
        ui = FindObjectOfType<UIManager>();
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
        grappleGun = FindObjectOfType<GrappleGunBehavior>();
        var audioSources = MainCamera.GetComponents<AudioSource>();
        winSfx = audioSources[1];
        loseSfx = audioSources[2];
        levelWon = false;
    }

    // private void Update()
    // {
    //     if (bemis.isPlaying) return; // LET BEMIS FINISH!
    //     var isLastLevel = LevelIndex == 3; // TODO: come up with a story-relevant endgame instead of just hardcodedly repeating last level
    //     if (LevelIsOver && (!levelWon || isLastLevel) && Input.anyKeyDown || !LevelIsOver && Input.GetKeyDown(KeyCode.R)) ReloadCurrentLevel();
    //     else if (LevelIsOver && levelWon && Input.anyKeyDown) LoadNextLevel();
    // }

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
        SetLevelStatus(won ? LevelStatus.Win : LevelStatus.Lose);
        LevelIsOver = true;
        grappleGun.StopGrapple();
    }

    public void SetLevelStatus(LevelStatus status)
    {
        CurrentLevelStatus = status;
        ui.HandleLevelStatus(status);
        switch (status)
        {
            case LevelStatus.Playing:
            case LevelStatus.Win:
            case LevelStatus.Lose:
                if (!PauseMenuBehavior.Paused) Time.timeScale = 1f;
                break;
            case LevelStatus.Loading:
            case LevelStatus.Paused:
                Time.timeScale = 0f;
                break;
        }
    }
    
    public void LoadNextLevel()
    {
        SceneManager.LoadScene(LevelIndex + 1);
    }

    public void ReloadCurrentLevel()
    {
        SceneManager.LoadScene(LevelIndex);
    }
}
