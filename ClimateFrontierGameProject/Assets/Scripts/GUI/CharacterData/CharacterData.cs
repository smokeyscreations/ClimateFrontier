// CharacterData.cs
using UnityEngine;

[CreateAssetMenu(fileName = "CharacterData", menuName = "Character Selection/Character Data")]
public class CharacterData : ScriptableObject
{
    public string characterName;
    public string description;
    public Sprite icon; // For the selection buttons
    public GameObject characterPrefab; // 3D model for rendering
}
