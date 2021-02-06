using UnityEngine;
using Random = UnityEngine.Random;

public class AsteroidMovement : MonoBehaviour
{

    public Vector2 speedRange;
    public float superspeedChance;
    public float superspeedMultiplier;
    
    private float speed;
    private Rigidbody rb;
    
    private void Start()
    {
        speed = Random.Range(speedRange[0], speedRange[1]);
        var superspeed = Random.Range(0F, 1F) <= superspeedChance;
        if (superspeed)
        {
            speed *= superspeedMultiplier;
        }
        rb = GetComponent<Rigidbody>();
        var tf = transform;
        rb.AddForce(tf.forward * speed, ForceMode.Impulse);
    }
}
