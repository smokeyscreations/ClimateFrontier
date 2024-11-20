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

    private void Start()
    {
        // Ensure enemyLayerMask is set from player.characterData
        if (player != null && player.characterData != null)
        {
            // Use the enemyLayerMask from CharacterData
            player.characterData.enemyLayerMask = LayerMask.GetMask("Enemy");
        }
        else
        {
            Debug.LogWarning("Player or CharacterData is null in SpellManager.");
        }
    }

    private void Update()
    {
        HandleSpellInput();
    }

    private void HandleSpellInput()
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

            if (Input.GetKeyDown(spell.hotkey))
            {
                if (Time.time >= spellCooldownTimers[i])
                {
                    CastSpell(spell);
                    spellCooldownTimers[i] = Time.time + spell.cooldown;
                }
                else
                {
                    float remaining = spellCooldownTimers[i] - Time.time;
                    Debug.Log($"{spell.spellName} is on cooldown for {remaining:F1} more seconds.");
                }
            }
        }
    }

    private void CastSpell(SpellData spell)
    {
        if (spell.prefab != null)
        {
            // Calculate spawn position based on player's position and forward direction
            Vector3 spawnPosition = player.transform.position + player.transform.forward * spell.spawnOffset;

            // Align spell's rotation with player's rotation
            Quaternion spawnRotation = Quaternion.LookRotation(player.transform.forward, Vector3.up);

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
                    spellScript.Initialize(spell, player);
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
}
