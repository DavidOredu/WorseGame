using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "PlayerData", menuName = "Data/Player Data")]
public class PlayerData : ScriptableObject
{
    public int health = 3;
    public Color color;
    public Color ropeColor;
    public bool isBig = false;
    public bool isBulletUnlocked = false;
    public bool hasLaserRope = false;
    public float ropeDampingRatio = 1f;
    public float ropeGrapSpeed = .5f;
    public int killsToSpawnBullet = 10;
    public int bulletCount = 6;
    public int viralHitCount = 1;
    public float bulletCooldownTime = 3f;
}
