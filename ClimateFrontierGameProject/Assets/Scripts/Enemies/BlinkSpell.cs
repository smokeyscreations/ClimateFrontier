using UnityEngine;
using DG.Tweening; // Import DOTween namespace

public class BlinkSpell : MonoBehaviour, ISpell, IPoolable
{
    private BlinkSpellData blinkData;
    private BasePlayer player;

    // Tag for Portal VFX pooling (if used)
    // private const string PortalVFXTag = "PortalVFX"; // Uncomment if using VFX

    // LayerMask for obstacle detection
    public LayerMask obstacleLayers;

    // Tween reference for the Blink process
    private Tween blinkTween;

    public void Initialize(SpellData spellData, BasePlayer player, Transform target)
    {
        blinkData = (BlinkSpellData)spellData;
        this.player = player;

        // Start the Blink process
        PerformBlink();
    }

    public void OnObjectSpawn()
    {
        // Additional initialization if needed when spawned from the pool
    }

    public void OnObjectReturn()
    {
        // Kill the tween if it's still running
        if (blinkTween != null && blinkTween.IsActive())
        {
            blinkTween.Kill();
            blinkTween = null;
        }
    }

    private void PerformBlink()
    {
        // Calculate end position
        Vector3 startPosition = player.transform.position;
        Vector3 forwardDirection = player.transform.forward;
        float blinkDistance = blinkData.blinkDistance;
        Vector3 endPosition = startPosition + forwardDirection * blinkDistance;

        // Play blink animation (optional)
        Animator animator = player.animator;
        if (animator != null)
        {
            animator.SetTrigger("Blink"); // Use a trigger parameter for the blink animation
        }

        float blinkDuration = blinkData.blinkDuration;

        // Use DOTween to move the player
        blinkTween = player.transform.DOMove(endPosition, blinkDuration)
            .SetEase(Ease.OutQuad)
            .OnComplete(() =>
            {
                // Ensure player reaches the exact end position
                player.transform.position = endPosition;

                // End the spell
                EndSpell();
            });

        // Optionally, you can spawn Portal VFX here if needed
        // SpawnPortalVFX(startPosition, player.transform.forward);
        // SpawnPortalVFX(endPosition, player.transform.forward);
    }

    private void EndSpell()
    {
        // Return the BlinkSpell object to the pool
        if (!string.IsNullOrEmpty(blinkData.tag) && ObjectPooler.Instance != null)
        {
            ObjectPooler.Instance.ReturnToPool(blinkData.tag, gameObject);
        }
        else
        {
            gameObject.SetActive(false); // Fallback if pooling is not set up correctly
        }
    }

    // Optional method if using Portal VFX
    /*
    private void SpawnPortalVFX(Vector3 position, Vector3 forward)
    {
        GameObject portalVFXInstance = ObjectPooler.Instance.SpawnFromPool(PortalVFXTag, position, Quaternion.LookRotation(forward));
        if (portalVFXInstance != null)
        {
            // PortalVFX handles its own behavior and returns to pool
        }
    }
    */
}
