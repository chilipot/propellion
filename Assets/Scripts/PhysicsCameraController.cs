using UnityEngine;
using Vector3 = UnityEngine.Vector3;

public class PhysicsCameraController : MonoBehaviour
{
    public float mouseSensitivity = 200;

    public static bool FreeCam = true; // TODO: make this an instance variable (see TODO below)

    private Vector3 rotationForce;
    private Rigidbody rb;
    
    private void Start()
    {
        FreeCam = true;
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        rb = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        // TODO: instead of checking LevelIsOver here, encapsulate that logic into FreeCam by making it a
        // a private instance variable w/ public setter, which refuses to set it to true if LevelIsOver
        if (!FreeCam || LevelManager.LevelInactive) return;
        var moveX = Input.GetAxis("Mouse X") * mouseSensitivity;
        var moveY = Input.GetAxis("Mouse Y") * mouseSensitivity * -1;
        rotationForce = new Vector3(moveY, moveX, 0);
    }

    private void FixedUpdate()
    {
        rb.AddRelativeTorque(rotationForce, ForceMode.Acceleration);
    }
}
