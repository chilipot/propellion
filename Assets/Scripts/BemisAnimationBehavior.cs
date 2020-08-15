using UnityEngine;

public class BemisAnimationBehavior : StateMachineBehaviour
{
    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        FindObjectOfType<IntroCutsceneController>().StartBemisDialogue();
    }
}
