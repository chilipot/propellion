using UnityEngine;
using Random = UnityEngine.Random;

public class AsteroidMovement : MonoBehaviour
{

    public float minSpeed = 200f;
    public float maxSpeed = 800f;
    
    private float speed;
    private bool appliedInitialForce;
    private Rigidbody rb;

    private void Start()
    {
        speed = Random.Range(minSpeed, maxSpeed);
        appliedInitialForce = false;
        rb = GetComponent<Rigidbody>();
        transform.Rotate(RandomAngle(), RandomAngle(), RandomAngle());
    }
    
    private static float RandomAngle()
    {
        return Random.Range(0f, 360f);
    }
    
    private void FixedUpdate()
    {
        if (appliedInitialForce) return;
        rb.AddForce(transform.forward * speed, ForceMode.Impulse);
        appliedInitialForce = true;
    }

}
