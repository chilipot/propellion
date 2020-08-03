using UnityEngine;

// TODO: give a thruster prefab to the player and have their ThrusterManager (which should probably be renamed to PlayerThrusterManager) call this API in StartEngine()/StopEngine()
// TODO: angle exhaust trail correctly, based on movement
public class ThrusterParticleManager : MonoBehaviour
{

    public bool ExhaustTrailActive { get; private set; } = false;
    
    // should be size 2, one for each engine
    private ParticleSystem[] exhaustTrails;
    
    private void Start()
    {
        exhaustTrails = GetComponentsInChildren<ParticleSystem>();
    }

    public void StartExhaustTrail()
    {
        foreach (var exhaustTrail in exhaustTrails)
        {
            exhaustTrail.Play();
        }
        ExhaustTrailActive = true;
    }
    
    public void StopExhaustTrail()
    {
        foreach (var exhaustTrail in exhaustTrails)
        {
            exhaustTrail.Stop();
        }
        ExhaustTrailActive = false;
    }
    
}