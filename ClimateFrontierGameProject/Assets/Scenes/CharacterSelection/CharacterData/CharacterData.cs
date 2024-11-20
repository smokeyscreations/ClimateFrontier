// CharacterData.cs
using UnityEngine;

[CreateAssetMenu(fileName = "CharacterData", menuName = "Character Selection/Character Data")]
public class CharacterData : ScriptableObject
{
    public string characterName;
    public string description;
    public string classString;
    public Sprite icon; // For the selection buttons
    public GameObject selectionPrefab; // 3D model for rendering in the selection screen
    public GameObject inGamePrefab; // Prefab used in the game scene
    public string basicAttackVFXTag = "SlashVFX";
    public float basicAttackVFXOffset = 1.0f;

    // Common attributes
    public float maxHealth = 100f;
    public float baseAttackDamage = 50f;
    public float baseWalkingSpeed = 2.5f;
    public float baseRunningSpeed = 5f;
    public float abilityCooldown = 1f;
    public float attackRange = 10f;
    public LayerMask enemyLayerMask;


    public SpellData[] abilities; // Array to hold abilities

}
