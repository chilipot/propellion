using UnityEngine;

public class AsteroidMovement : MonoBehaviour
{

    private float speed;
    
    private void Start()
    {
        transform.Rotate(RandomAngle(), RandomAngle(), RandomAngle());
        speed = Random.Range(0.2f, 2f);
    }
    
    private float RandomAngle()
    {
        return Random.Range(0f, 360f);
    }

    private void Update()
    {
        // TODO: make this physics-based instead?
        transform.Translate(Time.deltaTime * speed * Vector3.forward);
    }

}
