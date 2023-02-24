using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewLevelData", menuName = "Data/LevelData")]
public class LevelData : ScriptableObject
{
    [Header("LEVEL")]
    // current level of the player
    public int level = 1;

    // attribute value used to set the progress bar value, the value of the current experience foreach attribute
    [Header("ATTRIBUTE EXPERIENCES")]
    public int levelExperience = 0;

    [Header("EXPERIENCE TO NEXT LEVEL")]
    public AnimationCurve experienceToNextLevel;

    [Header("EXPERIENCE LEVEL UP SPEED")]
    public AnimationCurve xpBarSpeed;
}
