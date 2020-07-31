using System;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public GaugeBehavior healthGauge;
    public GaugeBehavior fuelGauge;

    public GameObject levelStatus, reticle;
    public Sprite missionSuccess, missionFailed, loadingSimulation;

    private Image levelStatusImage;

    public enum LevelStatus
    {
        Playing,
        Loading,
        Win,
        Lose
    }

    private void Start()
    {
        levelStatusImage = levelStatus.GetComponent<Image>();
    }

    public void SetLevelStatus(LevelStatus status) {
        Sprite statusSprite;
        switch (status)
        {
            case LevelStatus.Playing:
                levelStatus.SetActive(false);
                reticle.SetActive(true);
                return;
            case LevelStatus.Loading:
                statusSprite = loadingSimulation;
                break;
            case LevelStatus.Win:
                statusSprite = missionSuccess;
                break;
            default:
                statusSprite = missionFailed;
                break;
        }
        reticle.SetActive(false);
        levelStatusImage.sprite = statusSprite;
        levelStatus.SetActive(true);
    }
}