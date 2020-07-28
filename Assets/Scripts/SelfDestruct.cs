using UnityEngine;

public class SelfDestruct : MonoBehaviour
{

    public float lifetime = 2f;
    
    private void Start()
    {
        Destroy(gameObject, lifetime);
    }

}