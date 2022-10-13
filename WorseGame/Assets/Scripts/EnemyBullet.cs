using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBullet : MonoBehaviour, IDestructible
{
    public Timer lifetimeTimer;
    public float speed;
    public float maxLifetime;
    public float distanceToHit;
    public GameObject destructionEffect;
    public bool isActive;

    public TrailRenderer trail;
    public Vector3 direction;
    // Start is called before the first frame update
    void Awake()
    {
        lifetimeTimer = new Timer(maxLifetime);
        lifetimeTimer.SetTimer();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        transform.Translate(direction * speed * Time.deltaTime);

        if (isActive)
        {
            if (!lifetimeTimer.isTimeUp)
            {
                lifetimeTimer.UpdateTimer();
            }
            else
            {
                Collide(null, true);
            }
        }
    }
    void Collide(Collider2D hit, bool forcedCollision)
    {
        if (hit)
        {
            if (hit.CompareTag("Player"))
            {
                var player = hit.GetComponent<Player>();
                if (!player.isBig)
                    player.Damage(true);
                else
                    player.Damage(false);
            }
            // play collision feedback
            InstantiateDestructParticle();
            lifetimeTimer.StopTimer();
            lifetimeTimer.ResetTimer();
            gameObject.SetActive(false);
            Destroy(gameObject, 3f);
        }
        if (forcedCollision)
        {
            InstantiateDestructParticle();
            gameObject.SetActive(false);
            lifetimeTimer.ResetTimer();
            Destroy(gameObject, 3f);
        }
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        Collide(other, false);
    }
    public void InstantiateDestructParticle()
    {
        var destructionGO = Instantiate(destructionEffect, transform.position, Quaternion.identity);
        var main = destructionGO.GetComponent<ParticleSystem>().main;
        main.startColor = GetComponent<SpriteRenderer>().color;
        trail.enabled = false;
    }
}
