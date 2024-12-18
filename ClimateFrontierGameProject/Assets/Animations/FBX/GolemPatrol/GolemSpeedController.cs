using UnityEngine;
using UnityEngine.AI;

public class GolemSpeedController : MonoBehaviour
{
    private NavMeshAgent agent;       // Reference to the NavMeshAgent
    private Animator animator;        // Reference to the Animator
    private static readonly int SpeedHash = Animator.StringToHash("speed"); // Animator parameter hash

    void Start()
    {
        // Get components
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();

        if (agent == null)
            Debug.LogError("NavMeshAgent not found on " + gameObject.name);
        if (animator == null)
            Debug.LogError("Animator not found on " + gameObject.name);
    }

    void Update()
    {
        if (agent != null && animator != null)
        {
            // Calculate the magnitude of NavMeshAgent's velocity
            float speed = agent.velocity.magnitude;

            // Update the Animator's "Speed" parameter
            animator.SetFloat(SpeedHash, speed);
        }
    }
}
