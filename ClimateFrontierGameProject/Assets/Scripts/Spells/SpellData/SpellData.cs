using UnityEngine;

public class SpellData : ScriptableObject
{
    public string spellName;
    public string tag;
    public GameObject prefab;
    public float damage;
    public float cooldown;
    public float spellAttackRange;
    public KeyCode hotkey;
    public float activeDuration;
    public float spawnOffset = 1f;
    public bool isAutomatic = false; 
}
