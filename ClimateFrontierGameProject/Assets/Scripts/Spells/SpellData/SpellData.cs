using UnityEngine;

[CreateAssetMenu(fileName = "SpellData", menuName = "Abilities/Spell Data")]
public class SpellData : ScriptableObject
{
    public string spellName;
    public string tag;               // Unique tag for pooling
    [TextArea]
    public string description;
    public Sprite icon;

    public GameObject prefab;        // Prefab for the spell effect
    public float damage;
    public float cooldown;
    public float spellAttackRange;
    public KeyCode hotkey;
    public float activeDuration;
    public float spawnOffset = 1f; // Distance in front of the player where the spell spawns
}
