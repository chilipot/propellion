using UnityEngine;

public class MedicalCanisterPickup : MonoBehaviour
{
    public GameObject healAura;
    
    public static readonly StatEvent PickupEvent = new StatEvent(StatEventType.MedicalCanisterPickedUp);

    private GrappleGunBehavior grapple;
    private ProceduralGeneration entityManager;

    private void Start()
    {
        grapple = FindObjectOfType<GrappleGunBehavior>();
        entityManager = FindObjectOfType<ProceduralGeneration>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;
        grapple.StopGrapple();
        Instantiate(healAura, other.transform);
        var medicalCanister = gameObject;
        medicalCanister.SetActive(false);
        entityManager.RemoveEntity(medicalCanister);
        PickupEvent.Trigger();
        Destroy(medicalCanister, 2);
    }
}