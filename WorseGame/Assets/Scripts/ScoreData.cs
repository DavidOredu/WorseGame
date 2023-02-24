using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ScoreData", menuName = "Data/Score Data")]
public class ScoreData : ScriptableObject
{
    public int highScore = 0;
    public int maxKills;
    public int maxScoreMultiplier = 2;
    public int superMultiplierMultiplier = 2;
}
