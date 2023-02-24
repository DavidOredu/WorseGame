using MoreMountains.Feedbacks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Player : MonoBehaviour, IDestructible, IFeedbacks
{
    public int health;
    public Color color;
    public bool isBig;
    public bool isInvulnerable;
    public bool viralHit;
    public bool superMultiplier;
    
    private List<Image> healthIconsStack = new List<Image>();

    public SpriteRenderer spriteRenderer;
    public List<TrailRenderer> trails;
    public LineRenderer ropeRenderer;

    [SerializeField]
    private Material shieldMat;

    private PlayerData playerData;

    public GameObject bulletPrefab;

    public int killMeter;
    private bool canSpawnBullet;
    private Timer bulletCooldownTimer;
    private float bulletCooldownTime;

    public GameObject shield;
    public GameObject destructionEffect;
    public MMFeedbacks collisionFeedback;
    public MMFeedbacks damageFeedback;
    public MMFeedbacks collisionSoundFeedback;
    public MMFeedbacks bulletSoundFeedback;
    public MMFeedbackSound damageCollisionSoundFeedback;
    // Start is called before the first frame update
    private void Awake()
    {
        playerData = Resources.Load<PlayerData>("PlayerData");

        isBig = playerData.isBig;
        spriteRenderer.color = playerData.color;
        foreach (var trail in trails)
        {
            trail.startColor = playerData.color;
            trail.endColor = playerData.color;
        }
        ropeRenderer.startColor = playerData.ropeColor;
        ropeRenderer.endColor = playerData.ropeColor;
        shield.SetActive(false);
        shieldMat.SetColor("_Color", playerData.color);

        bulletCooldownTime = playerData.bulletCooldownTime;

        bulletCooldownTimer = new Timer(bulletCooldownTime);
        bulletCooldownTimer.SetTimer();
    }
    void Start()
    {
        InitialzeHealth();
        SetIconsStack();

        collisionSoundFeedback.Feedbacks[0].Active = AudioManager.instance.gameSettings.sound;
        bulletSoundFeedback.Feedbacks[0].Active = AudioManager.instance.gameSettings.sound;
        damageCollisionSoundFeedback.Active = AudioManager.instance.gameSettings.sound;
    }
    
    // Update is called once per frame
    void FixedUpdate()
    {
        if (playerData.isBulletUnlocked)
        {
            ControlBulletCooldown();
            SpawnBullets();
        }
    }
    private void SpawnBullets()
    {
        if(killMeter >= playerData.killsToSpawnBullet)
        {
            if (canSpawnBullet)
            {
                bulletSoundFeedback.PlayFeedbacks();
                ShootBullet(playerData.bulletCount, transform.position);
                killMeter = 0;
                canSpawnBullet = false;
                bulletCooldownTimer.ResetTimer();
            }
        }
    }
    public void ShootBullet(int bulletCount, Vector3 position)
    {
        for (int i = 0; i < bulletCount; i++)
        {
            var bullet = Instantiate(bulletPrefab, position, Quaternion.identity);
            var bulletSc = bullet.GetComponent<Bullet>();
            if (bulletSc.playerRB != null)
                bulletSc.CreateNewDirection();
            bulletSc.lifetimeTimer.ResetTimer();
        }
    }
    private void ControlBulletCooldown()
    {
        if (bulletCooldownTimer.IsTimeUp)
        {
            canSpawnBullet = true;
        }
        else
        {
            bulletCooldownTimer.UpdateTimer();
        }
    }
    private void InitialzeHealth()
    {
        health = playerData.health;

        for (int i = 0; i < GameManager.instance.healthIcons.Count; i++)
        {
            GameManager.instance.healthIcons[i].gameObject.SetActive(false);
        }
        for (int i = 0; i < health; i++)
        {
            GameManager.instance.healthIcons[i].gameObject.SetActive(true);
            GameManager.instance.healthIcons[i].color = playerData.color;
        }
    }
    void UpdateHealthBar(int healthRemaining, bool active, bool totalDamage = false)
    {
        if(!totalDamage)
            healthIconsStack[healthRemaining].gameObject.SetActive(active);
        else
            for (int i = 0; i < playerData.health; i++)
                healthIconsStack[i].gameObject.SetActive(false);
    }
    void SetIconsStack()
    {
        foreach (var healthIcon in GameManager.instance.healthIcons)
        {
            if(healthIcon.gameObject.activeSelf)
                healthIconsStack.Add(healthIcon);
        }
    }
    public void IncreaseHealth(int amount)
    {
        if(health == playerData.health) { return; }

        UpdateHealthBar(health, true);

        health += amount;
    }
    public void Damage(bool totalDamage, bool lavaPit = false)
    {
        if (isInvulnerable && !lavaPit) { return; }
        // Player Damage Feedback
        PlayDamageFeedback();

        if (totalDamage)
        {
            health = 0;
            UpdateHealthBar(health, false, totalDamage);
        }
        else
            health--;

        UpdateHealthBar(health, false, totalDamage);

        if(health == 0)
        {
            // Player destruction feedback
            InstantiateDestructParticle();
            UIManager.instance.ClosePopUp();
            GameManager.instance.GameEnd();
            transform.parent.gameObject.SetActive(false);
        }
    }

    public void InstantiateDestructParticle()
    {
        var destructionGO = Instantiate(destructionEffect, transform.position, Quaternion.identity);
        var main = destructionGO.GetComponent<ParticleSystem>().main;
        main.startColor = GetComponent<SpriteRenderer>().color;
    }

    public void PlayCollsionFeedback()
    {
        collisionFeedback.PlayFeedbacks();
    }

    public void PlayDamageFeedback()
    {
        damageFeedback.PlayFeedbacks();
    }
    public void PlayCollisionSoundFeedback()
    {
        collisionSoundFeedback.PlayFeedbacks();
    }
}
