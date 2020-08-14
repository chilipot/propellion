using UnityEngine;

public class IntroCutsceneController : MonoBehaviour
{
    public AudioSource labChairSfx;
    public AudioSource bemisVoice;
    public AudioClip bemisDialogue2;
    public AudioClip enterSimulationSfx;
    public Animator simulationBoothLightingAnim;

    public float spaceStationFlightSpeed = 10f;
    
    private static readonly int EnterSimulationBoothTrigger = Animator.StringToHash("EnterSimulationBooth");
    private static readonly int StartSimulationTrigger = Animator.StringToHash("StartSimulation");
    private static readonly int Rotation = Shader.PropertyToID("_Rotation");

    private bool skyboxShouldRotate = true;

    private void Awake()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    private void Update()
    {
        // press space or return to skip cutscene (TODO: make this the ESC key if not on WebGL, and add an inspector variable for array of skip keycodes)
        if (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.Return)) LoadFirstLevel();
        
        // rotate the skybox to simulate space station moving through space when player looks out the window
        if (skyboxShouldRotate) RenderSettings.skybox.SetFloat(Rotation, Time.time * spaceStationFlightSpeed);
    }

    public void StartBemisDialogue()
    {
        bemisVoice.Play();
        Invoke(nameof(EnterSimulationBooth), bemisVoice.clip.length);
    }

    private void EnterSimulationBooth()
    {
        var labChairAnimation = GetComponentInParent<Animator>();
        labChairAnimation.SetTrigger(EnterSimulationBoothTrigger);
        var animationDuration = labChairAnimation.GetCurrentAnimatorStateInfo(0).length;
        labChairSfx.Play();
        Invoke(nameof(StopLabChairSfx), labChairSfx.clip.length * 2f);
        Invoke(nameof(StartBemisDialogue2), animationDuration / 2f);
    }

    private void StopLabChairSfx()
    {
        labChairSfx.Stop();
    }

    private void StartBemisDialogue2()
    {
        bemisVoice.clip = bemisDialogue2;
        bemisVoice.Play();
        Invoke(nameof(StartSimulation), bemisDialogue2.length);
    }

    private void StartSimulation()
    {
        // TODO: use camera shake effect from Unique Projectiles package (or even better, some kind of glitchy matrixy vfx)
        simulationBoothLightingAnim.SetTrigger(StartSimulationTrigger);
        var enterSimulationSfxSource = AudioSourceExtension.PlayClipAndGetSource(enterSimulationSfx, transform.position);
        enterSimulationSfxSource.volume = 0.3f;
        Invoke(nameof(LoadFirstLevel), enterSimulationSfx.length / 2f);
    }

    private void LoadFirstLevel()
    {
        skyboxShouldRotate = false;
        RenderSettings.skybox.SetFloat(Rotation, 0f);
        FindObjectOfType<FadeToBlackPlayer>().FadeAndLoadNextScene();
    }
}