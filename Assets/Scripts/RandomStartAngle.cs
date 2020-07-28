using UnityEngine;

public class RandomStartAngle : MonoBehaviour
{

    private void Start()
    {
        transform.Rotate(RandomAngle(), RandomAngle(), RandomAngle());
    }
    
    private static float RandomAngle()
    {
        return Random.Range(0f, 360f);
    }
    
}