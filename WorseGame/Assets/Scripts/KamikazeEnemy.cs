using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KamikazeEnemy : Box
{
    public Transform target;
    public Rigidbody2D rb;
    public TrailRenderer trail1;
    public TrailRenderer trail2;
    public EnemyState currentKamikazeState;
    private Timer alertTimer;
    public float alertTime = .8f;
    public float rotationalControl;
    public float acceleration;
    public float deceleration;
    public float speed;
    
    public override void Start()
    {
        base.Start();

        alertTimer = new Timer(alertTime);
        alertTimer.SetTimer();

        trail1.enabled = false;
        trail2.enabled = false;
    }

    private void FixedUpdate()
    {
        switch (currentKamikazeState)
        {
            case EnemyState.Idle:
                Decelerate();
                FindPlayer();
                break;
            case EnemyState.Alert:
                FindPlayer();
                RunAlertTimer();
                break;
            case EnemyState.Attack:
                ChasePlayer();
                LosePlayer();
                break;
        }
    }
    void Decelerate()
    {
        var velocityX = (rb.velocity.x - deceleration) * Time.deltaTime;
        var velocityY = (rb.velocity.y - deceleration) * Time.deltaTime;
        velocityX = Mathf.Max(velocityX, 0);
        velocityY = Mathf.Max(velocityY, 0);
        rb.velocity = new Vector2(velocityX, velocityY);
    }
    void RunAlertTimer()
    {
        if (alertTimer.isTimeUp)
        {
            trail1.enabled = true;
            trail2.enabled = true;
            currentKamikazeState = EnemyState.Attack;
        }
        else
        {
            alertTimer.UpdateTimer();
        }
    }
    private void FindPlayer()
    {
        Collider2D hit = Physics2D.OverlapCircle(transform.position, playerSearchRadius, whatToAttack);

        if (hit)
        {
            target = hit.transform;
            currentKamikazeState = EnemyState.Alert;
        }
        else
        {
            target = null;
            currentKamikazeState = EnemyState.Idle;
            alertTimer.ResetTimer();
        }
    }
    public void ChasePlayer()
    {
        Vector2 direction = transform.position - target.position;
        direction.Normalize();

        float cross = Vector3.Cross(direction, transform.right).z;
        rb.angularVelocity = rotationalControl * cross;

        Vector2 vel = transform.right * (acceleration);
        rb.AddForce(vel);

        float dir = Vector2.Dot(rb.velocity, rb.GetRelativeVector(Vector2.right));
        float thrustForce = Vector2.Dot(rb.velocity, rb.GetRelativeVector(Vector2.down)) * 2f;
        Vector2 relForce = Vector2.up * thrustForce;
        rb.AddForce(rb.GetRelativeVector(relForce));

        if (rb.velocity.magnitude > speed)
        {
            rb.velocity = rb.velocity.normalized * speed;
        }
    }
    public void LosePlayer()
    {
        Collider2D hit = Physics2D.OverlapCircle(transform.position, playerMaxRadius, whatToAttack);

        if (!hit)
        {
            target = null;
            currentKamikazeState = EnemyState.Idle;
            alertTimer.ResetTimer();
        }
    }
    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position, playerSearchRadius);
        Gizmos.DrawWireSphere(transform.position, playerMaxRadius);
    }
    
}
