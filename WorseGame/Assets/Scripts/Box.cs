using MoreMountains.Feedbacks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Box : MonoBehaviour, IDestructible
{
    public BoxType boxType;
    public BoxData boxData;

    private int score;
    private int currency;

    public GameObject destructionEffect;

    public MMFeedbacks destructionFeedbacks;
    public MMFeedbackFloatingText valueText;
    public MMFeedbackFloatingText currencyText;

    SpriteRenderer spriteRenderer;
    // Start is called before the first frame update
    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        SetBoxScores();

        currencyText.Channel = 1;
        
    }
    private void OnTriggerEnter2D(Collider2D other)
    {

        if (other.CompareTag("Player"))
        {
            if (boxType == BoxType.BigEnemy || boxType == BoxType.Enemy)
                Damage(other.gameObject, true);
            else
            {
                other.GetComponent<IFeedbacks>().PlayCollsionFeedback();
                Damage(other.gameObject, false);
            }
        }
    }
    // Update is called once per frame
    void Update()
    {
        
    }
    public void Damage(GameObject player, bool damagePlayer)
    {
        for (int i = 0; i < valueText.AnimateColorGradient.colorKeys.Length; i++)
        {
            valueText.AnimateColorGradient.colorKeys[i].color = spriteRenderer.color;
        }
        valueText.Value = (score * ScoreSystem.GameScore.currentScoreMultiplier).ToString("N0");
        currencyText.Value = "$" + currency.ToString("N0");

        GameManager.instance.AddScore(score);
        RunCollisionLogic(player, damagePlayer);
        InstantiateDestructParticle();
        
        destructionFeedbacks.PlayFeedbacks();
        currencyText.Play(transform.position);
        gameObject.SetActive(false);
    }
    private void SetBoxScores()
    {
        score = boxData.score;
        currency = boxData.currency * LevelingManager.instance.levelSystem.GetLevelNumber();
    }
    private void RunCollisionLogic(GameObject player, bool damagePlayer)
    {
        GameManager.instance.scoreUI.GetComponent<IFeedbacks>().PlayCollsionFeedback();

        var playerSc = player.GetComponentInChildren<Player>();
        playerSc.killMeter++;
        playerSc.PlayCollisionSoundFeedback();
        switch (boxType)
        {
            case BoxType.Enemy:
                if (damagePlayer)
                {
                    if (!playerSc.isBig)
                        playerSc.Damage(false);
                    else
                        playerSc.GetComponent<IFeedbacks>().PlayCollsionFeedback();
                    GameManager.instance.AddCurrency(currency, false);
                }
                break;
            case BoxType.BigEnemy:
                if (damagePlayer)
                {
                    if (!playerSc.isBig)
                        playerSc.Damage(true);
                    else
                        playerSc.Damage(false);
                    GameManager.instance.AddCurrency(currency, false);
                }
                break;
            case BoxType.Special:
                GameManager.instance.AddCurrency(currency, true);
                break;
            case BoxType.MoneyDeluxe:
                GameManager.instance.AddCurrency(currency, true);
                break;
            default:
                GameManager.instance.AddCurrency(currency, false);
                break;
        }
    }

    public void InstantiateDestructParticle()
    {
        var destructionGO = Instantiate(destructionEffect, transform.position, Quaternion.identity);
        var main = destructionGO.GetComponent<ParticleSystem>().main;
        main.startColor = GetComponent<SpriteRenderer>().color;
    }

    public enum BoxType
    {
        Normal,
        Enemy,
        BigEnemy,
        Special,
        Deluxe,
        MoneyDeluxe,
    }
}
