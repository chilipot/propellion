using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// TODO: DELETE ME
public class TestCollision : MonoBehaviour
{
    public float force;
    void Start()
    {
        GetComponent<Rigidbody>().AddForce(Vector3.forward * force, ForceMode.Impulse);
    }

    void Update()
    {
        
    }
}
