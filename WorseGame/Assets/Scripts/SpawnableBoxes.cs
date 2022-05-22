using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SpawnableBoxes", menuName = "Data/Spawnable Boxes")]
public class SpawnableBoxes : ScriptableObject
{
    public List<ObjectPooler.PoolTag> poolTags = new List<ObjectPooler.PoolTag>();
}
