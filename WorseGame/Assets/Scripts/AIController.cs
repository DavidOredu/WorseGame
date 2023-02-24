using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EnemyType
{
    Enemy,
    BigEnemy,
    Shooter,
    Kamikaze,
}
public class AIController : MonoBehaviour
{
    private Box box;

    public DifficultyData difficultyData;

    public EnemyType enemyType;

    private ShooterEnemy shooterEnemy;
    private KamikazeEnemy kamikazeEnemy;

    private bool hasInit;

    private float recentXpNormalized;

    // Start is called before the first frame update
    void InitVariables()
    {
        if (hasInit) { return; }
        box = GetComponent<Box>();

        switch (enemyType)
        {
            case EnemyType.Shooter:
                shooterEnemy = GetComponent<ShooterEnemy>();
                break;
            case EnemyType.Kamikaze:
                kamikazeEnemy = GetComponent<KamikazeEnemy>();
                break;
            default:
                break;
        }
        hasInit = true;
    }
    // Update is called once per frame
    void Update()
    {
        InitVariables();
        if (!hasInit) { return; }

        if (recentXpNormalized != GameManager.instance.xpToNextLevelNormalized)
        {
            for (int i = 2; i == 0; i--)
            {
                if (GameManager.instance.xpToNextLevelNormalized >= difficultyData.xpToSpawnProbability[i])
                {
                    box.boxSpawnProbability.ChangeAnimationCurve(difficultyData.spawnProbabilityCurves[i]);
                    break;
                }
                else
                    continue;
            }


            switch (enemyType)
            {
                case EnemyType.Shooter:
                    shooterEnemy.playerSearchRadius = difficultyData.shooterSearchRadius.Evaluate(GameManager.instance.xpToNextLevelNormalized);
                    shooterEnemy.alertTime = difficultyData.shooterAlertTime.Evaluate(GameManager.instance.xpToNextLevelNormalized);
                    shooterEnemy.shotCount = Mathf.FloorToInt(difficultyData.shooterShotCount.Evaluate(GameManager.instance.xpToNextLevelNormalized));
                    shooterEnemy.btwShotsTime = difficultyData.shooterTimeBtwShots.Evaluate(difficultyData.shooterShotCount.Evaluate(GameManager.instance.xpToNextLevelNormalized));
                    shooterEnemy.shotTime = difficultyData.shooterShotTime.Evaluate(difficultyData.shooterShotCount.Evaluate(GameManager.instance.xpToNextLevelNormalized));
                    break;
                case EnemyType.Kamikaze:
                    kamikazeEnemy.playerSearchRadius = difficultyData.kamikazeSearchRadius.Evaluate(GameManager.instance.xpToNextLevelNormalized);
                    kamikazeEnemy.alertTime = difficultyData.kamikazeAlertTime.Evaluate(GameManager.instance.xpToNextLevelNormalized);
                    kamikazeEnemy.speed = difficultyData.kamikazeSpeed.Evaluate(GameManager.instance.xpToNextLevelNormalized);
                    kamikazeEnemy.acceleration = difficultyData.kamikazeAcceleration.Evaluate(GameManager.instance.xpToNextLevelNormalized);
                    kamikazeEnemy.deceleration = difficultyData.kamikazeDeceleration.Evaluate(GameManager.instance.xpToNextLevelNormalized);
                    kamikazeEnemy.rotationalControl = difficultyData.kamikazeManeuvarability.Evaluate(GameManager.instance.xpToNextLevelNormalized);
                    break;
                default:
                    break;
            }
            recentXpNormalized = GameManager.instance.xpToNextLevelNormalized;
        }
    }
}
