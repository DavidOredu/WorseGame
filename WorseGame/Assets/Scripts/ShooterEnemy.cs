using System.Collections;
using UnityEngine;

public class ShooterEnemy : Box
{
    private Timer shotTimer;
    private Timer betweenShotTimer;
    public float shotTime;
    public float btwShotsTime;
    public int shotCount;
    public EnemyState currentShooterState;

    private Transform target;
    public GameObject bulletPrefab;

    private Timer alertTimer;
    public float alertTime;
    public override void Start()
    {
        base.Start();

        shotTimer = new Timer(shotTime);
        shotTimer.SetTimer();
        alertTimer = new Timer(alertTime);
        alertTimer.SetTimer();
        betweenShotTimer = new Timer(btwShotsTime);
        betweenShotTimer.SetTimer();

        spriteRenderer = transform.Find("BoxGFX").GetComponent<SpriteRenderer>();
    }
    public override void Update()
    {
        base.Update();


    }
    private void FixedUpdate()
    {
        switch (currentShooterState)
        {
            case EnemyState.Idle:
                FindPlayer();
                break;
            case EnemyState.Alert:
                TrackPlayer();
                RunAlertTimer();
                LosePlayer();
                break;
            case EnemyState.Attack:
                TrackPlayer();
                ShootPlayer();
                LosePlayer();
                break;
        }
    }
    private IEnumerator Shoot()
    {
        for (int i = 0; i < shotCount; i++)
        {
            if (target != null)
            {
                //bulletSoundFeedback.PlayFeedbacks();
                var bullet = Instantiate(bulletPrefab, transform.position, Quaternion.identity);
                var bulletSc = bullet.GetComponent<EnemyBullet>();
                bulletSc.direction = (target.transform.position - transform.position).normalized;

                yield return new WaitForSeconds(btwShotsTime);
            }
        }

    }
    private void FindPlayer()
    {
        Collider2D hit = Physics2D.OverlapCircle(transform.position, playerSearchRadius, whatToAttack);

        if (hit)
        {
            target = hit.transform;
            currentShooterState = EnemyState.Alert;
        }
        else
        {
            target = null;
            currentShooterState = EnemyState.Idle;
            alertTimer.ResetTimer();
        }
    }
    void TrackPlayer()
    {
        // if the player is connected to this box instance, stop rotation
        var dist = (transform.position - target.transform.position).normalized;
        float angle = Mathf.Atan2(dist.y, dist.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(Vector3.forward * (angle + 90f));
    }
    void ShootPlayer()
    {
        if (shotTimer.isTimeUp)
        {
            StartCoroutine(Shoot());
            shotTimer.ResetTimer();
        }
        else
        {
            shotTimer.UpdateTimer();
        }
    }
    void RunAlertTimer()
    {
        if (alertTimer.isTimeUp)
        {
            currentShooterState = EnemyState.Attack;
        }
        else
        {
            alertTimer.UpdateTimer();
        }
    }
    public void LosePlayer()
    {
        Collider2D hit = Physics2D.OverlapCircle(transform.position, playerMaxRadius, whatToAttack);

        if (!hit)
        {
            target = null;
            currentShooterState = EnemyState.Idle;
            alertTimer.ResetTimer();
        }
    }
}
