using UnityEngine;
using UnityEngine.UI;

// TODO: make this a singleton
public class UIManager : MonoBehaviour
{
    public GaugeBehavior healthGauge;
    public GaugeBehavior fuelGauge;

    public GameObject levelStatus, reticle;
    public Sprite missionSuccess, missionFailed, loadingSimulation;
    public Color reticleImageHitColor = new Color(255, 92, 94);
    
    private Image levelStatusImage;
    private Image reticleImage;

    public enum LevelStatus
    {
        Playing,
        Loading,
        Win,
        Lose
    }
    
    private void Awake()
    {
        levelStatusImage = levelStatus.GetComponent<Image>();
        reticleImage = reticle.GetComponent<Image>();
    }

    public void SetReticleFocus(bool focused)
    {
        if (focused)
        {
            reticleImage.color = reticleImageHitColor;
            reticleImage.transform.localScale = Vector3.Lerp(reticleImage.transform.localScale,
                new Vector3(0.5f, 0.5f, 0.5f), Time.deltaTime * 4);
        }
        else
        {
            reticleImage.color = Color.white;
            reticleImage.transform.localScale = Vector3.Lerp(reticleImage.transform.localScale,
                new Vector3(1f, 1f, 1f), Time.deltaTime * 4);
        }
    }

    public void SetLevelStatus(LevelStatus status) {
        Sprite statusSprite;
        switch (status)
        {
            case LevelStatus.Playing:
                levelStatus.SetActive(false);
                reticleImage.enabled = true;
                return;
            case LevelStatus.Loading:
                statusSprite = loadingSimulation;
                reticleImage.enabled = false;
                break;
            case LevelStatus.Win:
                statusSprite = missionSuccess;
                reticleImage.enabled = false;
                break;
            default:
                statusSprite = missionFailed;
                reticleImage.enabled = false;
                break;
        }
        levelStatusImage.sprite = statusSprite;
        levelStatus.SetActive(true);
    }
}