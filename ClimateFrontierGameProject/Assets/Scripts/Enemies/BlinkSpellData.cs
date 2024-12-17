using UnityEngine;

[CreateAssetMenu(fileName = "New BlinkSpellData", menuName = "Spells/BlinkSpellData")]
public class BlinkSpellData : SpellData
{
    public float blinkDistance = 10f;  // Distance to blink forward
    public float blinkDuration = 0.2f; // Duration of the blink effect
}
