using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour, IDestructible
{
    private GameObject player;
    public Rigidbody2D playerRB;
    private SpriteRenderer spriteRenderer;
    private PlayerData playerData;
    private TrailRenderer trail;


    public float speed;
    public Timer lifetimeTimer;
    public float maxLifetime;
    private Vector3 direction;
    private float directionRange = .5f;
    public float distanceToHit;
    public GameObject destructionEffect;
    // Start is called before the first frame update
    void Start()
    {
        //   GameManager.OnGameEnd += DestroyOnGameEnd;
        lifetimeTimer = new Timer(maxLifetime);
        lifetimeTimer.SetTimer();

        spriteRenderer = GetComponent<SpriteRenderer>();
        playerData = Resources.Load<PlayerData>("PlayerData");
        player = GameObject.FindGameObjectWithTag("Player");
        playerRB = player.GetComponent<Rigidbody2D>();
        trail = GetComponent<TrailRenderer>();

        spriteRenderer.color = playerData.color;
        trail.startColor = playerData.color;
        trail.endColor = playerData.color;
        CreateNewDirection();
    }
    public void CreateNewDirection()
    {
        var direction = playerRB.velocity.normalized;
        var newDir = new Vector3(Random.Range(direction.x - directionRange, direction.x + directionRange), Random.Range(direction.y - directionRange, direction.y + directionRange), 0);
        this.direction = newDir;
      //  direction = new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f), 0f);
        trail.enabled = true;
    }
    void FixedUpdate()
    {
        transform.Translate(direction * speed * Time.deltaTime);

        if (!lifetimeTimer.isTimeUp)
        {
            lifetimeTimer.UpdateTimer();
            Collide(false);
        }
        else
        {
            Collide(true);
        }
    }
    void Collide(bool forcedCollision)
    {
        Collider2D hit = Physics2D.OverlapCircle(transform.position, distanceToHit, LayerMask.GetMask("Box", "Ground", "LavaPit"));
        if (hit)
        {
            if (hit.CompareTag("Box"))
            {
                var box = hit.GetComponent<Box>();
                box.Damage(player, false);
            }
            // play collision feedback
            InstantiateDestructParticle();
            lifetimeTimer.StopTimer();
            gameObject.SetActive(false);
            Destroy(gameObject, 3f);
        }
        if (forcedCollision)
        {
            InstantiateDestructParticle();
            gameObject.SetActive(false);
            Destroy(gameObject, 3f);
        }
    }

    public void InstantiateDestructParticle()
    {
        var destructionGO = Instantiate(destructionEffect, transform.position, Quaternion.identity);
        var main = destructionGO.GetComponent<ParticleSystem>().main;
        main.startColor = GetComponent<SpriteRenderer>().color;
        trail.enabled = false;
    }
}
