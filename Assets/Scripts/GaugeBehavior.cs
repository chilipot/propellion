using System;
using System.Collections;
using System.Collections.Generic;
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

    public void SetVal(float capacity, float maxCapacity)
    {
        float percentageOfMax = capacity / maxCapacity;
        gauge.fillAmount = (percentageOfMax) * (180.0f / 360);
        if (percentageOfMax <= lowPercentage)
        {
            BlinkRed();
        }
    }

    public void BlinkRed()
    {
        animator.SetTrigger("Low");
    }
}
