using UnityEngine;

public class BlackHoleConsumption : MonoBehaviour
{

    public BlackHolePull blackHolePull;
    
    private LevelManager levelManager;
    private ProceduralGeneration entityManager;
    private void Start()
    {
        entityManager = FindObjectOfType<ProceduralGeneration>();
        levelManager = FindObjectOfType<LevelManager>();
    }

    private void OnTriggerEnter(Collider other)
    {
        var consumedBody = other.gameObject.GetComponent<Rigidbody>();
        if (!consumedBody) return;
        blackHolePull.RemovePulledBody(consumedBody);
        if (other.gameObject.CompareTag("Player"))
        {
            // TODO: if this behavior is necessary past the prototype, make it a public method on a Player script, instead of sporadically duplicating this line
            consumedBody.constraints = RigidbodyConstraints.FreezeAll;
            levelManager.Lose();
        }
        else
        {
            entityManager.RemoveEntity(other.gameObject);
            Destroy(other.gameObject, 1);
        }
    }
}
