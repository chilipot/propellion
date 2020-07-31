using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public GaugeBehavior healthGauge;
    public GaugeBehavior fuelGauge;
    public Text levelStatus;


    public void SetLevelStatus(bool won)
    {
        levelStatus.text = won ? "You win!" : "Game over!";
        levelStatus.gameObject.SetActive(true);
    }
}