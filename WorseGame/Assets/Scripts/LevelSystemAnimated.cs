using System;
using UnityEngine;
[Serializable]
public class LevelSystemAnimated
{
    public int level;
    public int amountOfBuyingXP;


    //  private int experienceToNextLevel;

    public event EventHandler OnExperienceChanged;
    public event EventHandler OnLevelChanged;

    private LevelSystem levelSystem;
    public bool isAnimating;

    private float updateTimer;
    private float updateTimerMax;

    public int levelAttributeExperience;
    public int experienceToNextLevel;
    public LevelSystemAnimated(LevelSystem levelSystem)
    {
        SetLevelSystem(levelSystem);
        updateTimerMax = .016f;

        levelSystem.OnExperienceChanged += LevelSystem_OnExperienceChanged;
        levelSystem.OnLevelChanged += LevelSystem_OnLevelChanged;
    }
    public void Update()
    {
        if (isAnimating)
        {
            //Check if its time to update
            updateTimer += Time.deltaTime;
            while (updateTimer >= updateTimerMax)
            {
                //time to update
                updateTimer -= updateTimerMax;

                UpdateAddExperience();
            }
        }
        Debug.Log(level + " " + levelAttributeExperience);

    }
    private void UpdateAddExperience()
    {
        //local level equals the target level

        if (level < levelSystem.GetLevelNumber())
        {
            AddExperience();
        }
        else
        {
            if (levelAttributeExperience < levelSystem.GetExperience())
            {
                AddExperience();
            }
            else
            {
                isAnimating = false;
            }
        }
    }
    public void LevelUp()
    {
        if (!levelSystem.IsMaxLevel())
        {
            level++;
            levelSystem.levelAttribute.experience -= levelSystem.levelAttribute.experienceToNextLevel;
            OnLevelChanged?.Invoke(this, EventArgs.Empty);
            Debug.Log("has invoked level changed in level system animated");
        }
        else if (levelSystem.IsMaxLevel())
        {
            level = 100;
            OnLevelChanged?.Invoke(this, EventArgs.Empty);
        }
    }

    public void AddExperience()
    {
        levelAttributeExperience += Mathf.CeilToInt((.5f * experienceToNextLevel) / 100);

        if(levelAttributeExperience >= experienceToNextLevel)
        {
            level++;
            levelAttributeExperience = 0;
            experienceToNextLevel = levelSystem.GetExperienceToNextLevel(level);
            OnLevelChanged?.Invoke(this, EventArgs.Empty);
        }
        OnExperienceChanged?.Invoke(this, EventArgs.Empty);
        Debug.Log("has invoked experience changed in level system animated");
    }
    public void SetLevelSystem(LevelSystem levelSystem)
    {
        this.levelSystem = levelSystem;

        level = levelSystem.GetLevelNumber();

        levelAttributeExperience = levelSystem.levelAttribute.experience;
        experienceToNextLevel = levelSystem.GetExperienceToNextLevel(level);
    }
    public void SetLevelSystem()
    {
        level = levelSystem.GetLevelNumber();

        levelAttributeExperience = levelSystem.levelAttribute.experience;
        experienceToNextLevel = levelSystem.GetExperienceToNextLevel(level);
    }

    private void LevelSystem_OnLevelChanged(object sender, EventArgs e)
    {
        isAnimating = true;
    }

    private void LevelSystem_OnExperienceChanged(object sender, EventArgs e)
    {
        isAnimating = true;
    }
    public int GetLevelNumber()
    {
        return level;
    }

    //public float GetExperienceNormalized()
    //{
    //    if (levelSystem.IsMaxLevel())
    //    {
    //        return 1f;
    //    }
    //    else
    //    {
    //        return (float)experience / levelSystem.GetExperienceToNextLevel(level);
    //    }
    //}
    public float GetAttributeNormalized(float experience)
    {
        if (levelSystem.IsMaxLevel())
        {
            return 1f;
        }
        else
        {
            return (float)experience / levelSystem.GetExperienceToNextLevel(level);
        }
    }
}
