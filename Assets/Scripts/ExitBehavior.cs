using UnityEngine;

public class ExitBehavior : MonoBehaviour
{
    public static readonly StatEvent WinEvent = new StatEvent(StatEventType.Success);
    
    private LevelManager levelManager;
    
    private void Start()
    {
        levelManager = FindObjectOfType<LevelManager>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            WinEvent.Trigger();
            levelManager.Win();
        }
    }
}
