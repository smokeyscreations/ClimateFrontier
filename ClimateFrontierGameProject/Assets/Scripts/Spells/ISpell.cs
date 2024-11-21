using UnityEngine;

public interface ISpell
{
    void Initialize(SpellData spellData, BasePlayer player, Transform target);
}
