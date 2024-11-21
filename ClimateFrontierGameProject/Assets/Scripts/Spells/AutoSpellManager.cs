using UnityEngine;

public class AutoSpellManager : MonoBehaviour
{
    public SpellData[] spells; // Assigned from CharacterData or manually

    private float[] spellCooldownTimers;

    private BasePlayer player;

    private void Awake()
    {
        player = GetComponent<BasePlayer>();
        if (player == null)
        {
            Debug.LogError("AutoSpellManager requires a BasePlayer component.");
        }

        InitializeCooldownTimers();
    }

    // Initialize cooldown timers based on the number of spells
    public void InitializeCooldownTimers()
    {
        if (spells != null)
        {
            spellCooldownTimers = new float[spells.Length];
            for (int i = 0; i < spellCooldownTimers.Length; i++)
            {
                spellCooldownTimers[i] = 0f;
            }
        }
        else
        {
            spellCooldownTimers = new float[0];
        }
    }

    private void Update()
    {
        HandleAutoCasting();
    }

    private void HandleAutoCasting()
    {
        if (spells == null || spellCooldownTimers == null)
        {
            Debug.LogWarning("Spells or spellCooldownTimers not initialized.");
            return;
        }

        for (int i = 0; i < spells.Length; i++)
        {
            if (i >= spellCooldownTimers.Length)
            {
                Debug.LogError($"spellCooldownTimers length ({spellCooldownTimers.Length}) is less than spells length ({spells.Length}).");
                break;
            }

            SpellData spell = spells[i];
            if (spell == null)
            {
                Debug.LogWarning($"Spell at index {i} is null.");
                continue;
            }

            // Check if the cooldown has elapsed
            if (Time.time >= spellCooldownTimers[i])
            {
                Transform target = FindNearestEnemy(spell.spellAttackRange);
                if (target != null)
                {
                    CastSpellAtTarget(spell, target);
                    spellCooldownTimers[i] = Time.time + spell.cooldown;
                }
                else
                {
                    Debug.Log($"No enemy within range ({spell.spellAttackRange}) for spell '{spell.spellName}'.");
                }
            }
        }
    }

    private Transform FindNearestEnemy(float range)
    {
        Collider[] colliders = Physics.OverlapSphere(player.transform.position, range, player.EnemyLayerMask);
        Transform nearestEnemy = null;
        float minDistance = Mathf.Infinity;

        foreach (Collider collider in colliders)
        {
            float distance = Vector3.Distance(player.transform.position, collider.transform.position);
            if (distance < minDistance)
            {
                minDistance = distance;
                nearestEnemy = collider.transform;
            }
        }

        return nearestEnemy;
    }

    private void CastSpellAtTarget(SpellData spell, Transform target)
    {
        if (spell.prefab != null)
        {
            // Calculate direction to target
            Vector3 directionToTarget = (target.position - player.transform.position).normalized;

            // Calculate spawn position with offset
            Vector3 spawnPosition = player.transform.position + directionToTarget * spell.spawnOffset;

            // Align spell's rotation with direction to target
            Quaternion spawnRotation = Quaternion.LookRotation(directionToTarget, Vector3.up);

            // Spawn the spell from the pool using its unique tag
            GameObject spellObject = ObjectPooler.Instance.SpawnFromPool(spell.tag, spawnPosition, spawnRotation);
            if (spellObject != null)
            {
                // Initialize the spell object
                IPoolable poolable = spellObject.GetComponent<IPoolable>();
                if (poolable != null)
                {
                    poolable.OnObjectSpawn();
                }

                // If the spell has a specific script (e.g., AOE), initialize it
                ISpell spellScript = spellObject.GetComponent<ISpell>();
                if (spellScript != null)
                {
                    spellScript.Initialize(spell, player, target);
                }

                Debug.Log($"Casted {spell.spellName} at {target.name}.");
            }
            else
            {
                Debug.LogWarning($"Failed to spawn spell with tag '{spell.tag}'.");
            }
        }
        else
        {
            Debug.LogWarning($"Spell {spell.spellName} has no prefab assigned.");
        }
    }
}
