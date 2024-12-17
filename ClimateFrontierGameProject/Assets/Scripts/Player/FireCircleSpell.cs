using UnityEngine;
using System.Collections;

public class FireCircleSpell : MonoBehaviour, ISpell, IPoolable
{
    private SpellData fireCircleData;
    private BasePlayer caster;

    private Coroutine lifeCoroutine;

    // If you want repeated damage, you can add a trigger or do OverlapSphere checks in Update/coroutine
    // For simplicity, let's just do an OverlapSphere approach in a coroutine.

    public void Initialize(SpellData spellData, BasePlayer player, Transform optionalTarget)
    {
        fireCircleData = spellData;  // store the data
        caster = player;

        // start a coroutine to handle the “activeDuration” countdown
        lifeCoroutine = StartCoroutine(FireCircleRoutine());
    }

    public void OnObjectSpawn()
    {
        // Reset state if needed
        gameObject.SetActive(true);
    }

    public void OnObjectReturn()
    {
        // Cleanup when returned to the pool
        if (lifeCoroutine != null)
        {
            StopCoroutine(lifeCoroutine);
            lifeCoroutine = null;
        }

        caster = null;
        fireCircleData = null;
        gameObject.SetActive(false);
    }

    private IEnumerator FireCircleRoutine()
    {
        float elapsedTime = 0f;
        float duration = fireCircleData.activeDuration;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;

            // For damage, do a quick OverlapSphere or OnTriggerStay approach
            DamageEnemiesWithinRadius();

            yield return null;
        }

        EndSpell();
    }

    private void DamageEnemiesWithinRadius()
    {
        // Example: OverlapSphere for enemies, deal damage each frame
        float radius = fireCircleData.spellAttackRange;  // or some other radius
        Collider[] hits = Physics.OverlapSphere(transform.position, radius, caster.EnemyLayerMask);

        foreach (Collider col in hits)
        {
            IDamageable enemyHealth = col.GetComponent<IDamageable>();
            if (enemyHealth != null)
            {
                enemyHealth.TakeDamage(fireCircleData.damage * Time.deltaTime);
                // or a small damage each frame, or at intervals
            }
        }
    }

    private void EndSpell()
    {
        // Return to the pool just like FlickerStrike
        if (!string.IsNullOrEmpty(fireCircleData.tag) && ObjectPooler.Instance != null)
        {
            ObjectPooler.Instance.ReturnToPool(fireCircleData.tag, gameObject);
        }
        else
        {
            // fallback
            gameObject.SetActive(false);
        }
    }
}
