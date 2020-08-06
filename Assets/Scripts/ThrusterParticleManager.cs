using UnityEngine;

public class ThrusterParticleManager : MonoBehaviour
{

    public bool ExhaustTrailActive { get; private set; } = false;
    
    private ParticleSystem[] exhaustTrails;
    
    private void Start()
    {
        exhaustTrails = GetComponentsInChildren<ParticleSystem>();
    }

    public void StartExhaustTrail()
    {
        foreach (var exhaustTrail in exhaustTrails) exhaustTrail.Play();
        ExhaustTrailActive = true;
    }
    
    public void StopExhaustTrail()
    {
        foreach (var exhaustTrail in exhaustTrails) exhaustTrail.Stop();
        ExhaustTrailActive = false;
    }
}