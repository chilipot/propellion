using System.Collections.Generic;
using UnityEngine;

public class BlackHoleExpansion : MonoBehaviour
{

    public float expansionRate = 10f;

    private void Update()
    {
        transform.localScale += Time.deltaTime * expansionRate * Vector3.one;
    }
}
