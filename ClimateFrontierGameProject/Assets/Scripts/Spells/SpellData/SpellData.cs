using UnityEngine;

[CreateAssetMenu(fileName = "SpellData", menuName = "Abilities/Spell Data")]
public class SpellData : ScriptableObject
{
    public string spellName;
    [TextArea]
    public string description;
    public Sprite icon;
    

    public GameObject aoePrefab;      // Prefab for the AOE effect
    public float damage;
    public float cooldown;
    public float spellAttackRange;
    public KeyCode hotkey;             // Key to activate the spell
    public float activeDuration;

}
