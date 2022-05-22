using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class LevelSystem
{
    private int level;

    private LevelData levelData;

    public PlayerAttribute levelAttribute { get; private set; }

    [SerializeField] public List<PlayerAttribute> playerAttributes = new List<PlayerAttribute>();


    public event EventHandler OnLevelChanged;
    public event EventHandler OnExperienceChanged;

    public LevelSystem(LevelData levelData)
    {
        SetLevelSystem(levelData);
    }
    public void SetLevelSystem()
    {
        level = levelData.level;

        levelAttribute = new PlayerAttribute(levelData.levelExperience, GetExperienceToNextLevel(level));
    }
    public void SetLevelSystem(LevelData levelData)
    {
        this.levelData = levelData;
        level = this.levelData.level;

        levelAttribute = new PlayerAttribute(levelData.levelExperience, GetExperienceToNextLevel(level));
    }
    public void Update()
    {
      //  levelAttribute.Update();
    }
    public void LevelUp()
    {
        if (!IsMaxLevel())
        {
            level++;
            if (!IsMaxLevel())
            {

                levelAttribute.experience -= levelAttribute.experienceToNextLevel;
                levelAttribute.experienceToNextLevel = GetExperienceToNextLevel(level);
                levelAttribute.isFull = false;

            }
            levelData.levelExperience = levelAttribute.experience;
            levelData.level = level;
            OnLevelChanged?.Invoke(this, EventArgs.Empty);
        }
    }
    public void AddExperience(int amount)
    {
        if (!IsMaxLevel())
        {
            levelAttribute.experience += amount;
            while(levelAttribute.experience >= levelAttribute.experienceToNextLevel)
            {
                LevelUp();
            }

            //    levelData.experience = experience;
            levelData.levelExperience = levelAttribute.experience;
            OnExperienceChanged?.Invoke(this, EventArgs.Empty);
            Debug.Log("Has invoked on experience changed in level system");
        }

    }
    public int GetLevelNumber()
    {
        return level;
    }
    public int GetExperience()
    {
        return levelAttribute.experience;
    }
    public bool IsMaxLevel()
    {
        return IsMaxLevel(level);
    }
    public bool IsMaxLevel(int level)
    {
        return level == 100;
    }

    public float GetAttributeNormalized(PlayerAttribute playerAttribute)
    {
        if (IsMaxLevel())
        {
            return 1f;
        }
        else
        {
            return playerAttribute.normalizedValue;
        }
    }
    public float GetExperienceNormalized()
    {
        if (IsMaxLevel())
        {
            return 1f;
        }
        else
        {
            return (float)levelAttribute.experience / GetExperienceToNextLevel(level);
        }
    }
    public int GetExperienceToNextLevel(int level)
    {
        var currentValue = levelData.experienceToNextLevel.Evaluate(level);
        var nextValue = levelData.experienceToNextLevel.Evaluate(level + 1);

        var experienceToNextLevel = nextValue - currentValue;

        return (int)experienceToNextLevel;
    }
}
