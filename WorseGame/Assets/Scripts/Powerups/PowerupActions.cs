using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;


public class PowerupActions : SerializedMonoBehaviour
{
    private GameManager gameManager;
    private GameManager GameManager
    {
        get
        {
            if (gameManager != null) { return gameManager; }
            return gameManager = GameManager.instance;
        }
    }

    #region Health Powerup
    public void HealthStartAction()
    {
        GameManager.player.IncreaseHealth(1);
    }

    public void HealthEndAction()
    {

    }
    #endregion
 
    #region Shield Powerup
    public void ShieldStartAction()
    {
        GameManager.player.shield.SetActive(true);
        GameManager.player.isInvulnerable = true;
    }
    public void ShieldEndAction()
    {
        GameManager.player.shield.SetActive(false);
        GameManager.player.isInvulnerable = false;
    }
    #endregion

    #region Viral Hit Powerup
    public void ViralHitStartAction()
    {
        GameManager.instance.player.viralHit = true;
    }
    public void ViralHitEndAction()
    {
        GameManager.instance.player.viralHit = false;
    }
    #endregion

    #region Super Multiplier Powerup
    public void SuperMultiplierStartAction()
    {
        GameManager.instance.player.superMultiplier = true;
    }
    public void SuperMultiplierEndAction()
    {
        GameManager.instance.player.superMultiplier = false;
    }
    #endregion

    #region Coins
    public void CoinStartAction()
    {

    }

    public void CoinEndAction()
    {

    }
    #endregion

    #region Other Functions

    private float Remap(float value, float from1, float to1, float from2, float to2)
    {
        return (value - from1) / (to1 - from1) * (to2 - from2) + from2;
    }

    #endregion
}
