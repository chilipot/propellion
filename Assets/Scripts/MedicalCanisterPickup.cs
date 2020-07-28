using UnityEngine;

public class MedicalCanisterPickup : MonoBehaviour
{

    public GameObject healAura;

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;
        other.GetComponentInChildren<GrappleGunBehavior>().StopGrapple();
        Instantiate(healAura, other.transform);
        var medicalCanister = gameObject;
        medicalCanister.SetActive(false);
        Destroy(medicalCanister, 2);
    }
}