using UnityEngine;

public class BlackHoleConsumption : MonoBehaviour
{

    private LevelManager levelManager;

    private void Start()
    {
        levelManager = FindObjectOfType<LevelManager>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            levelManager.Lose();
        }
        else
        {
            Destroy(other.gameObject);
        }
    }
}
