using System.Collections.Generic;
using UnityEngine;

public class BlackHoleExpansion : MonoBehaviour
{
    
    public float expansionRate = 0.5f; // the fraction of current size to increase by per second

    private void Update()
    {
        transform.localScale *= expansionRate * Time.deltaTime + 1;
    }
}
