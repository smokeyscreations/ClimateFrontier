using UnityEngine;

public class PlayerIdleState : IState
{
    protected BasePlayer player;

    public PlayerIdleState(BasePlayer player)
    {
        this.player = player;
    }

    public void Tick()
    {
        // Set speed to 0 to ensure the Animator stays in Idle state
        player.animator.SetFloat("speed", 0f);
    }

    public void OnEnter()
    {
        Debug.Log("Entering Idle State");
        // No need to set animator parameters here since it's handled in Tick
    }

    public void OnExit()
    {
        Debug.Log("Exiting Idle State");
    }
}
