using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "BoxData", menuName = "Data/Box Data")]
public class BoxData : ScriptableObject
{
    public ObjectPooler.PoolTag boxType;
    public int score;
    public int currency;
    public bool isUnlocked;
    public int percentageToSpawn;
}
