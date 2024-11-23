using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class FlickerStrike : MonoBehaviour, ISpell, IPoolable
{
    private FlickerStrikeData flickerData;
    private BasePlayer player;
    private List<Transform> enemiesToAttack;
    private int targetsHit = 0;

    // Reference to the Dash VFX GameObject (assumed to be a child named "DashVFX")
    private GameObject dashVFX;

    // Coroutine reference for the Flicker Strike process
    private Coroutine flickerCoroutine;

    // Tag for Slice VFX pooling
    private const string SliceVFXTag = "SliceVFX";

    public void Initialize(SpellData spellData, BasePlayer player, Transform target)
    {
        flickerData = (FlickerStrikeData)spellData;
        this.player = player;
        enemiesToAttack = new List<Transform>();
        targetsHit = 0;

        // Find the Dash VFX child object
        dashVFX = transform.Find("DashVFX")?.gameObject;
        if (dashVFX == null)
        {
            Debug.LogError("DashVFX child not found in FlickerStrike prefab.");
        }

        // Activate Dash VFX and parent it to the player for movement
        if (dashVFX != null)
        {
            dashVFX.SetActive(true);
            dashVFX.transform.SetParent(player.transform, false);
        }

        // Start the Flicker Strike coroutine
        flickerCoroutine = StartCoroutine(PerformFlickerStrike());
    }

    public void OnObjectSpawn()
    {
        // Additional initialization if needed when spawned from the pool
    }

    public void OnObjectReturn()
    {
        // Deactivate Dash VFX and reparent it back to the FlickerStrike prefab
        if (dashVFX != null)
        {
            dashVFX.SetActive(false);
            dashVFX.transform.SetParent(transform, false);
        }

        // Stop the Flicker Strike coroutine if it's running
        if (flickerCoroutine != null)
        {
            StopCoroutine(flickerCoroutine);
            flickerCoroutine = null;
        }

        // Reset spell state
        enemiesToAttack.Clear();
        targetsHit = 0;
    }

    private IEnumerator PerformFlickerStrike()
    {
        float elapsedTime = 0f;

        while (elapsedTime < flickerData.activeDuration && targetsHit < flickerData.maxTargets)
        {
            Transform enemy = FindNearestEnemy();

            if (enemy != null)
            {
                yield return TeleportToEnemy(enemy);
                DealDamage(enemy);
                targetsHit++;
                yield return new WaitForSeconds(flickerData.attackInterval);
            }
            else
            {
                // No more enemies to attack
                break;
            }

            elapsedTime += flickerData.dashDuration + flickerData.attackInterval;
        }

        EndSpell();
    }

    private Transform FindNearestEnemy()
    {
        Collider[] colliders = Physics.OverlapSphere(player.transform.position, flickerData.teleportRange, player.EnemyLayerMask);

        Transform nearestEnemy = null;
        float minDistance = Mathf.Infinity;

        foreach (Collider collider in colliders)
        {
            if (enemiesToAttack.Contains(collider.transform))
                continue; // Skip already attacked enemies

            float distance = Vector3.Distance(player.transform.position, collider.transform.position);
            if (distance < minDistance)
            {
                minDistance = distance;
                nearestEnemy = collider.transform;
            }
        }

        if (nearestEnemy != null)
        {
            enemiesToAttack.Add(nearestEnemy);
        }

        return nearestEnemy;
    }

    private IEnumerator TeleportToEnemy(Transform enemy)
    {
        // Play dash animation
        Animator animator = player.animator;
        animator.SetBool("IsDashing", true);

        float dashDuration = flickerData.dashDuration;

        Vector3 startPosition = player.transform.position;
        Vector3 endPosition = enemy.position;

        float elapsedTime = 0f;

        while (elapsedTime < dashDuration)
        {
            float t = elapsedTime / dashDuration;
            player.transform.position = Vector3.Lerp(startPosition, endPosition, t);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // Ensure player reaches the exact enemy position
        player.transform.position = endPosition;

        // Stop dash animation
        animator.SetBool("IsDashing", false);
    }

    private void DealDamage(Transform enemy)
    {
        BaseEnemy enemyComponent = enemy.GetComponent<BaseEnemy>();
        if (enemyComponent != null)
        {
            enemyComponent.TakeDamage(Mathf.RoundToInt(flickerData.damage));

            // Camera shake
            if (CameraShake.Instance != null)
            {
                CameraShake.Instance.TriggerShake(0.3f, 0.7f); // Increased duration and magnitude
            }

            // Spawn Slice VFX from the pool
            SpawnSliceVFX(enemy.position, enemy.forward);
        }
    }

    private void SpawnSliceVFX(Vector3 position, Vector3 forward)
    {
        GameObject sliceVFX = ObjectPooler.Instance.SpawnFromPool(SliceVFXTag, position, Quaternion.LookRotation(forward));
        if (sliceVFX != null)
        {
            Debug.Log("SliceVFX spawned at position: " + position);
            // SliceVFX handles its own sound and returns to pool
        }
        else
        {
            Debug.LogWarning("Failed to spawn SliceVFX.");
        }
    }

    private IEnumerator ReturnSliceVFXToPool(GameObject sliceVFX, float delay)
    {
        yield return new WaitForSeconds(delay);
        ObjectPooler.Instance.ReturnToPool(SliceVFXTag, sliceVFX);
        Debug.Log("SliceVFX returned to pool.");
    }

    private void EndSpell()
    {
        // Return the FlickerStrike object to the pool
        if (!string.IsNullOrEmpty(flickerData.tag) && ObjectPooler.Instance != null)
        {
            ObjectPooler.Instance.ReturnToPool(flickerData.tag, gameObject);
        }
        else
        {
            gameObject.SetActive(false); // Fallback if pooling is not set up correctly
        }
    }
}
