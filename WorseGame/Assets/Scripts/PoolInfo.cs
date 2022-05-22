using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "PoolInfo", menuName = "Data/Pool Info")]
public class PoolInfo : ScriptableObject
{
    public ObjectPooler.Pool poolInfo;
}
