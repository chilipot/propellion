using UnityEngine;
using UnityEngine.UI;

public class GaugeBehavior : MonoBehaviour
{
    public Image gauge;
    public float lowPercentage = 0.33f;
    
    private static readonly int NotLow = Animator.StringToHash("NotLow");
    private static readonly int Low = Animator.StringToHash("Low");
    
    private Animator animator;

    private void Awake()
    {
        animator = gameObject.GetComponent<Animator>();
    }
    
    public void SetVal(float capacity, float maxCapacity)
    {
        var clampedCapacity = Mathf.Clamp(capacity, 0f, maxCapacity);
        var percentageOfMax = clampedCapacity / maxCapacity;
        gauge.fillAmount = percentageOfMax * (180.0f / 360);
        if (percentageOfMax <= lowPercentage)
        {
            animator.ResetTrigger(NotLow);
            animator.SetTrigger(Low);
        }
        else
        {
            animator.ResetTrigger(Low);
            animator.SetTrigger(NotLow);
        }
    }
}
