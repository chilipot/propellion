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
                reticle.SetActive(true); // TODO: find a way to only enable it here and disable it everywhere else, without losing references because it's disabled
                return;
            case LevelStatus.Loading:
                statusSprite = loadingSimulation;
                reticle.SetActive(true);
                break;
            case LevelStatus.Win:
                statusSprite = missionSuccess;
                reticle.SetActive(false);
                break;
            default:
                statusSprite = missionFailed;
                reticle.SetActive(false);
                break;
        }
        levelStatusImage.sprite = statusSprite;
        levelStatus.SetActive(true);
    }
}