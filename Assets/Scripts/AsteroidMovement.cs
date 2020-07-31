using System;
using UnityEngine;
using Random = UnityEngine.Random;

public class AsteroidMovement : MonoBehaviour
{

    public float minSpeed = 200f;
    public float maxSpeed = 800f;
    
    private float speed;
    private Rigidbody rb;
    
    private void Start()
    {
        speed = Random.Range(minSpeed, maxSpeed);
        rb = GetComponent<Rigidbody>();
        var tf = transform;
        rb.AddForce(tf.forward * speed, ForceMode.Impulse);
    }
}
