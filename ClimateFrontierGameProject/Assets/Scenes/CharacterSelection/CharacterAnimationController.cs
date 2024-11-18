using UnityEngine;

public class CharacterAnimationController : MonoBehaviour
{
    private Animator animator;

    void Start()
    {
        animator = GetComponent<Animator>();

        if (animator == null)
        {
            Debug.LogError("Animator component not found on the character.");
            return;
        }

        // Optionally, ensure the KickEntrance animation plays on start
        animator.Play("KickEntrance");
    }

    // Optional: Method to replay the KickEntrance animation
    public void PlayKickEntrance()
    {
        animator.Play("KickEntrance");
    }
}
