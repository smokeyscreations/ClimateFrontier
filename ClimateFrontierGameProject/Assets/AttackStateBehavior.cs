using UnityEngine;

public class AttackStateBehaviour : StateMachineBehaviour
{
    public int comboStep;

    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        animator.SetInteger("ComboStep", comboStep);
    }

    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        // Reset ComboStep when exiting the last attack
        if (comboStep == 3)
        {
            animator.SetInteger("ComboStep", 0);
        }
    }
}
