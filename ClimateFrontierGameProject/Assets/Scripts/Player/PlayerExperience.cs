using UnityEngine;

public class PlayerExperience : MonoBehaviour
{
    public int currentLevel = 1;
    public int currentExperience = 0;
    public int experienceToNextLevel = 10; // Starting value

    // Leveling parameters
    public int baseExperienceRequirement = 10; // Initial XP requirement
    public int experienceIncreasePerLevel = 10; // XP increase per level

    public void GainExperience(int amount)
    {
        currentExperience += amount;
        Debug.Log($"Gained {amount} XP. Total XP: {currentExperience}/{experienceToNextLevel}");

        // Level up as many times as possible with current XP
        while (currentExperience >= experienceToNextLevel)
        {
            LevelUp();
        }
    }

    private void LevelUp()
    {
        currentExperience -= experienceToNextLevel;
        currentLevel++;
        Debug.Log($"Leveled up to Level {currentLevel}!");

        // Optionally increase player stats here

        // Calculate XP needed for next level
        experienceToNextLevel = CalculateExperienceForNextLevel(currentLevel);
    }

    private int CalculateExperienceForNextLevel(int level)
    {
        // Linear increase: XP required increases by a fixed amount each level
        return baseExperienceRequirement + experienceIncreasePerLevel * (level - 1);
    }

}
