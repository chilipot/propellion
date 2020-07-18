using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public Text healthBar;
    public Text thrustCapacityBar;
    public Text levelStatus;

    public void SetHealthBar(float currHealth, float maxHealth)
    {
        healthBar.text = Mathf.RoundToInt(currHealth).ToString();
        healthBar.color = Color.Lerp(Color.red, Color.green, currHealth / maxHealth);
    }

    public void SetThrustCapacityBar(float currCapacity, float maxCapacity)
    {
        float capcityLeft = currCapacity / maxCapacity;
        if (Mathf.Approximately(capcityLeft, 0f))
        {
            thrustCapacityBar.text = "EMPTY";
            thrustCapacityBar.color = Color.red;
            thrustCapacityBar.fontStyle = FontStyle.Bold;

        }
        else
        {
            thrustCapacityBar.text = Mathf.RoundToInt(currCapacity).ToString();
            thrustCapacityBar.color = (capcityLeft < 0.5f) ? Color.yellow : Color.blue;
        }
    }

    public void SetLevelStatus(string text)
    {
        levelStatus.text = text;
        levelStatus.gameObject.SetActive(true);
    }
}