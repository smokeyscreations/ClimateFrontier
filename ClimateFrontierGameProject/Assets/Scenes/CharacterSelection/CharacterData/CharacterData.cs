// CharacterData.cs
using UnityEngine;

[CreateAssetMenu(fileName = "CharacterData", menuName = "Character Selection/Character Data")]
public class CharacterData : ScriptableObject
{
    public string characterName;
    public string description;
    public Sprite icon; // For the selection buttons
    public GameObject selectionPrefab; // 3D model for rendering in the selection screen
    public GameObject inGamePrefab; // Prefab used in the game scene

    // Common attributes
    public float maxHealth = 100f;
    public float baseAttackDamage = 50f;
    public float movementSpeed = 5f;
    public float abilityCooldown = 1f;
    public float attackRange = 10f;
    public LayerMask enemyLayerMask;

    // Character-specific abilities
    public float ability1Damage = 20f;
    public float ability2Damage = 25f;
    public float ability3Damage = 30f;

    // Add any additional attributes as needed
}
