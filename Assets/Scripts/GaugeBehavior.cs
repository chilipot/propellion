using UnityEngine;
using UnityEngine.UI;

public class GaugeBehavior : MonoBehaviour
{
    public Image gauge;
    public float lowPercentage = 0.2f;
    private Animator animator;

    private void Start()
    {
        animator = gameObject.GetComponent<Animator>();
    }

    // TODO: fix nullreferenceexception here on game start
    public void SetVal(float capacity, float maxCapacity)
    {
        float percentageOfMax = capacity / maxCapacity;
        gauge.fillAmount = (percentageOfMax) * (180.0f / 360);
        if (percentageOfMax <= lowPercentage) animator.SetTrigger("Low");
        else animator.ResetTrigger("Low");
    }
}
