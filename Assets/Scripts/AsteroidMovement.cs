using UnityEngine;
using Random = UnityEngine.Random;

public class AsteroidMovement : MonoBehaviour
{

    public Vector2 speedRange;
    
    private float speed;
    private Rigidbody rb;
    
    private void Start()
    {
        speed = Random.Range(speedRange[0], speedRange[1]);
        rb = GetComponent<Rigidbody>();
        var tf = transform;
        rb.AddForce(tf.forward * speed, ForceMode.Impulse);
    }
}
