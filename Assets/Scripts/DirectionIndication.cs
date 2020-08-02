using UnityEngine;

public class DirectionIndication : MonoBehaviour
{
    private Transform exitPortal;
    
    private void Start()
    {
        exitPortal = GameObject.FindWithTag("ExitPortal").transform;
    }

    private void Update()
    {
        var tf = transform;
        var targetDirection = (exitPortal.position - tf.position).normalized;
        tf.rotation = Quaternion.LookRotation(targetDirection, tf.parent.up);
    }
}