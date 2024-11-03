using UnityEngine;

public class PlayerRunState : IState
{
    protected BasePlayer player;

    public PlayerRunState(BasePlayer player)
    {
        this.player = player;
    }

    public void Tick()
    {
        // Calculate the movement speed based on input magnitude
        float speedValue = player.MovementInput.magnitude * player.MovementSpeed;
        player.animator.SetFloat("speed", speedValue);

    }

    public void OnEnter()
    {
        Debug.Log("Entering Run State");
    }

    public void OnExit()
    {
        Debug.Log("Exiting Run State");
        // Optionally reset the speed parameter
        player.animator.SetFloat("speed", 0f);
    }
}
