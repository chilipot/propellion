using UnityEngine;

public class Rotation : MonoBehaviour
{

    public float degreesPerSecond = 90f;

    private void Update()
    {
        transform.Rotate(Vector3.up, degreesPerSecond * Time.deltaTime);
    }
}