using UnityEngine;

public class SpellManager : MonoBehaviour
{
    public SpellData[] spells; // Assigned from CharacterData

    private float[] spellCooldownTimers;

    private BasePlayer player;


    public LayerMask enemyLayerMask;

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
        enemyLayerMask = LayerMask.GetMask("Enemy");
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
                    CastConeAOESpell(spell);
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

    private void CastConeAOESpell(SpellData spell)
    {
        if (spell.aoePrefab != null)
        {
            float spawnOffset = 1.0f; // Fixed offset

            // Calculate spawn position based on player's position and forward direction
            Vector3 spawnPosition = player.transform.position + player.transform.forward;

            // Align AOE's rotation with player's rotation
            Quaternion spawnRotation = Quaternion.LookRotation(player.transform.forward, Vector3.up);

            // Spawn the AOE from the pool
            GameObject aoe = AOEPooler.Instance.SpawnFromPool("AOE", spawnPosition, spawnRotation);
            if (aoe != null)
            {
                // Retrieve the AOE component from the spawned GameObject
                AOE aoeScript = aoe.GetComponent<AOE>();
                if (aoeScript != null)
                {
                    // Initialize the AOE with spell-specific parameters, including activeDuration
                    aoeScript.Initialize(
                        spell.damage,                           // Damage value from SpellData
                        spell.spellAttackRange,                // Range from SpellData
                        player.characterData.enemyLayerMask,    // Enemy Layer Mask from CharacterData
                        spell.activeDuration                    // Active duration from SpellData
                    );
                    Debug.Log($"Casted {spell.spellName}.");
                }
                else
                {
                    Debug.LogError("AOE prefab lacks an AOE script on the parent.");
                }
            }
            else
            {
                Debug.LogWarning($"Failed to spawn AOE with tag 'AOE'.");
            }
        }
        else
        {
            Debug.LogWarning($"Spell {spell.spellName} has no AOE prefab assigned.");
        }
    }


}
