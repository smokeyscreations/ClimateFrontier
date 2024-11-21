using UnityEngine;

public class SpellManager : MonoBehaviour
{
    public SpellData[] spells; // Assigned from CharacterData

    private float[] spellCooldownTimers;

    private BasePlayer player;

    private void Awake()
    {
        player = GetComponent<BasePlayer>();
        if (player == null)
        {
            Debug.LogError("SpellManager requires a BasePlayer component.");
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
        HandleSpellCasting();
    }

    private void HandleSpellCasting()
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

            if (spell.isAutomatic)
            {
                HandleAutomaticSpell(i, spell);
            }
            else
            {
                HandleManualSpell(i, spell);
            }
        }
    }

    private void HandleManualSpell(int index, SpellData spell)
    {
        if (Input.GetKeyDown(spell.hotkey))
        {
            if (Time.time >= spellCooldownTimers[index])
            {
                CastSpell(spell);
                spellCooldownTimers[index] = Time.time + spell.cooldown;
            }
            else
            {
                float remaining = spellCooldownTimers[index] - Time.time;
                Debug.Log($"{spell.spellName} is on cooldown for {remaining:F1} more seconds.");
            }
        }
    }

    private void HandleAutomaticSpell(int index, SpellData spell)
    {
        if (Time.time >= spellCooldownTimers[index])
        {
            // For spells that require a target, find the nearest enemy
            if (spell.prefab.GetComponent<ISpell>() is AOE)
            {
                Transform target = FindNearestEnemy(spell.spellAttackRange);
                if (target != null)
                {
                    CastSpellAtTarget(spell, target);
                    spellCooldownTimers[index] = Time.time + spell.cooldown;
                }
            }
            else
            {
                // For spells that don't require a target
                CastSpell(spell);
                spellCooldownTimers[index] = Time.time + spell.cooldown;
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

    private void CastSpell(SpellData spell)
    {
        if (spell.prefab != null)
        {
            Vector3 spawnPosition = player.transform.position + Vector3.up * spell.spawnOffset;
            Quaternion spawnRotation = player.transform.rotation;

            GameObject spellObject = ObjectPooler.Instance.SpawnFromPool(spell.tag, spawnPosition, spawnRotation);
            if (spellObject != null)
            {
                IPoolable poolable = spellObject.GetComponent<IPoolable>();
                if (poolable != null)
                {
                    poolable.OnObjectSpawn();
                }

                ISpell spellScript = spellObject.GetComponent<ISpell>();
                if (spellScript != null)
                {
                    spellScript.Initialize(spell, player, null);
                }

                Debug.Log($"Casted {spell.spellName}.");
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

    private void CastSpellAtTarget(SpellData spell, Transform target)
    {
        if (spell.prefab != null)
        {
            Vector3 directionToTarget = (target.position - player.transform.position).normalized;
            Vector3 spawnPosition = player.transform.position + directionToTarget * spell.spawnOffset;
            Quaternion spawnRotation = Quaternion.LookRotation(directionToTarget, Vector3.up);

            GameObject spellObject = ObjectPooler.Instance.SpawnFromPool(spell.tag, spawnPosition, spawnRotation);
            if (spellObject != null)
            {
                IPoolable poolable = spellObject.GetComponent<IPoolable>();
                if (poolable != null)
                {
                    poolable.OnObjectSpawn();
                }

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
