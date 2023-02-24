using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MoreMountains.Feedbacks;

[CreateAssetMenu(fileName = "DifficultyData", menuName = "Data/Difficulty Data")]
public class DifficultyData : ScriptableObject
{
    [Header("GENERAL")]
    public List<float> xpToSpawnProbability;
    public List<AnimationCurve> spawnProbabilityCurves;

    [Header("SHOOTER")]
    public AnimationCurve shooterSearchRadius;
    public AnimationCurve shooterAlertTime;
    public AnimationCurve shooterShotCount;
    public AnimationCurve shooterTimeBtwShots;
    public AnimationCurve shooterShotTime;

    [Header("KAMIKAZE")]
    public AnimationCurve kamikazeSearchRadius;
    public AnimationCurve kamikazeAlertTime;
    public AnimationCurve kamikazeSpeed;
    public AnimationCurve kamikazeAcceleration;
    public AnimationCurve kamikazeDeceleration;
    public AnimationCurve kamikazeManeuvarability;
}
