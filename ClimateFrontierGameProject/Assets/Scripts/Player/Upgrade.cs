using UnityEngine;

[System.Serializable]
public class Upgrade
{
    public UpgradeType upgradeType;
    public string description;
    public int value; // The value by which the upgrade modifies the stat
    public Sprite icon; // Optional: For UI representation
}
