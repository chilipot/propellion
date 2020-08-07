using UnityEngine;

public class KatanaSlice : MonoBehaviour
{
    public BoxCollider hitbox;
    
    private static readonly int ShouldSwing = Animator.StringToHash("shouldSwing");

    private Animator swingAnimation;
    private AudioSource swingSfx;

    private void Start()
    {
        swingAnimation = GetComponent<Animator>();
        swingSfx = GetComponent<AudioSource>();
    }

    private void Update()
    {
        if (!LevelManager.LevelIsActive || 
            swingAnimation.GetCurrentAnimatorStateInfo(0).IsName("Swing") || 
            !Input.GetMouseButtonDown(1)) return;
        
        swingSfx.PlayOneShot(swingSfx.clip);
        swingAnimation.SetTrigger(ShouldSwing);
        hitbox.enabled = true;
        Debug.Log("enabling collider!"); // TODO: delete
        Invoke(nameof(DisableCollider), swingAnimation.GetCurrentAnimatorStateInfo(0).length); // TODO: only enable collider while katana is swinging DOWN, not for the full animation (maybe split into separate states?)
    }

    private void DisableCollider()
    {
        Debug.Log("disabling collider!"); // TODO: delete
        hitbox.enabled = false;
    }
    
}