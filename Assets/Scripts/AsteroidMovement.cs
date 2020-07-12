using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AsteroidMovement : MonoBehaviour
{

    private float _speed;
    
    // Start is called before the first frame update
    void Start()
    {
        transform.Rotate(RandomAngle(), RandomAngle(), RandomAngle());
        _speed = Random.Range(0.2f, 2f);
    }
    
    private float RandomAngle()
    {
        return Random.Range(0f, 360f);
    }

    // Update is called once per frame
    void Update()
    {
        transform.Translate(Time.deltaTime * _speed * Vector3.forward);
    }

}
