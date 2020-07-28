using UnityEngine;

public class MedicalCanisterPickup : MonoBehaviour
{

    public GameObject healAura;

    private GrappleGunBehavior grapple;

    private void Start()
    {
        grapple = FindObjectOfType<GrappleGunBehavior>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;
        grapple.StopGrapple();
        Instantiate(healAura, other.transform);
        var medicalCanister = gameObject;
        medicalCanister.SetActive(false);
        Destroy(medicalCanister, 2);
    }
}