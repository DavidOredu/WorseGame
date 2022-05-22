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
    
    private Stack<Image> healthIconsStack = new Stack<Image>();

    public SpriteRenderer spriteRenderer;
    public LineRenderer ropeRenderer;
    private PlayerData playerData;

    public GameObject bulletPrefab;

    public int killMeter;

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
        ropeRenderer.startColor = playerData.ropeColor;
        ropeRenderer.endColor = playerData.ropeColor;
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
            SpawnBullets();
        }
    }
    void SpawnBullets()
    {
        if(killMeter >= playerData.killsToSpawnBullet)
        {
            bulletSoundFeedback.PlayFeedbacks();
            for (int i = 0; i < playerData.bulletCount; i++)
            {
                var bullet = ObjectPooler.instance.SpawnFromPool(ObjectPooler.PoolTag.Bullet, transform.position, Quaternion.identity);
                var bulletSc = bullet.GetComponent<Bullet>();
                bulletSc.CreateNewDirection();
                bulletSc.lifetimeTimer.ResetTimer();
            }
            killMeter = 0;
        }
    }
    void InitialzeHealth()
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
    void UpdateHealthBar(int healthLost)
    {
        for (int i = 0; i < healthLost; i++)
        {
            var currentImage = healthIconsStack.Peek();
            // use variable to run feedback logic on health bar

            currentImage.gameObject.SetActive(false);
            healthIconsStack.Pop();
        }
    }
    void SetIconsStack()
    {
        foreach (var healthIcon in GameManager.instance.healthIcons)
        {
            if(healthIcon.gameObject.activeSelf)
                healthIconsStack.Push(healthIcon);
        }
    }
    public void Damage(bool totalDamage)
    {
        // Player Damage Feedback
        PlayDamageFeedback();

        var previousHealth = health;

        if (totalDamage)
            health = 0;
        else
            health--;

        var difference = previousHealth - health;
        UpdateHealthBar(difference);

        if(health == 0)
        {
            // Player destruction feedback
            InstantiateDestructParticle();
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
