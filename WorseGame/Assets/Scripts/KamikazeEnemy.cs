using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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

    [SerializeField]
    private Canvas stateImageCanvas;
    [SerializeField]
    private Image stateImage;
    [SerializeField]
    private Sprite idleStateSprite;
    [SerializeField]
    private Sprite alertStateSprite;

    [SerializeField]
    private float alertStateSpriteSize = 5f;
    [SerializeField]
    private float idleStateSpriteSize = 2f;
    
    public override void Start()
    {
        base.Start();

        alertTimer = new Timer(alertTime);
        alertTimer.SetTimer();

        trail1.enabled = false;
        trail2.enabled = false;

        stateImageCanvas.worldCamera = Camera.main;
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
                UpdateAlertSlider();
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
        if (alertTimer.IsTimeUp)
        {
            trail1.enabled = true;
            trail2.enabled = true;
            stateImage.rectTransform.sizeDelta = new Vector2(alertStateSpriteSize, alertStateSpriteSize);
            stateImage.sprite = alertStateSprite;
            alertTimer.ResetTimer(alertTime);
            stateImage.fillAmount = 1f;
            currentKamikazeState = EnemyState.Attack;
        }
        else
        {
            alertTimer.UpdateTimer();
        }
    }
    void UpdateAlertSlider()
    {
        stateImage.fillAmount = 1 - alertTimer.CurrentTimeNormalized();
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
            stateImage.rectTransform.sizeDelta = new Vector2(idleStateSpriteSize, idleStateSpriteSize);
            stateImage.sprite = idleStateSprite;
            currentKamikazeState = EnemyState.Idle;
            alertTimer.ResetTimer(alertTime);
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
            stateImage.rectTransform.sizeDelta = new Vector2(idleStateSpriteSize, idleStateSpriteSize);
            stateImage.sprite = idleStateSprite;
            currentKamikazeState = EnemyState.Idle;
            alertTimer.ResetTimer(alertTime);
        }
    }
    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position, playerSearchRadius);
        Gizmos.DrawWireSphere(transform.position, playerMaxRadius);
    }
    
}
