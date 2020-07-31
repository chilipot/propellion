using UnityEngine;

public class MedicalCanisterPickup : MonoBehaviour
{
    public GameObject healAura;

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
        Destroy(medicalCanister, 2);
    }
}