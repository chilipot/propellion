using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

// TODO: make this a singleton
public class UIManager : MonoBehaviour
{
    public LevelEndMenuBehavior endMenu;
    public GaugeBehavior healthGauge;
    public GaugeBehavior fuelGauge;

    public GameObject levelStatus, reticle;
    public Sprite missionSuccess, missionFailed, loadingSimulation;
    public Color reticleImageHitColor = new Color(255, 92, 94);
    
    private Image levelStatusImage;
    private Image reticleImage;


    
    private void Awake()
    {
        endMenu = FindObjectOfType<LevelEndMenuBehavior>();
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

    public void HandleLevelStatus(LevelManager.LevelStatus status) {
        Sprite statusSprite;
        switch (status)
        {
            case LevelManager.LevelStatus.Playing:
                levelStatus.SetActive(false);
                reticleImage.enabled = true;
                break;
            case LevelManager.LevelStatus.Loading:
                statusSprite = loadingSimulation;
                reticleImage.enabled = false;
                
                levelStatusImage.sprite = statusSprite;
                levelStatus.SetActive(true);
                break;
            case LevelManager.LevelStatus.Win:
                // statusSprite = missionSuccess;
                reticleImage.enabled = false;
                
                // levelStatusImage.sprite = statusSprite;
                // levelStatus.SetActive(true);
                endMenu.Show();
                break;
            case LevelManager.LevelStatus.Lose:
                // statusSprite = missionFailed;
                reticleImage.enabled = false;
                endMenu.Show();
                // levelStatusImage.sprite = statusSprite;
                // levelStatus.SetActive(true);
                break;
            case LevelManager.LevelStatus.Paused:
                reticleImage.enabled = false;
                levelStatus.SetActive(false);
                break;
        }
    }
}