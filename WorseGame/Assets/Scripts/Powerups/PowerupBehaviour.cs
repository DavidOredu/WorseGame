using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerupBehaviour : MonoBehaviour
{
    public PowerupController powerupController;
    public PowerupActions powerupActions;

    [SerializeField]
    public Powerup powerup;

    private void Start()
    {
        powerupController = GameManager.instance.powerupController;
        powerupActions = GameManager.instance.powerupActions;
    }
    public void ActivatePowerup()
    {
        SetPowerupEvents(powerupActions);       
        if (powerup.isInstant)
        {
            powerupController.ActivatePowerup(powerup);
        }
    }

    public void SetPowerupEvents(PowerupActions powerupActions)
    {
        powerup.startAction.RemoveAllListeners();
        powerup.activeAction.RemoveAllListeners();
        powerup.endAction.RemoveAllListeners();

        switch (powerup.name)
        {
            case "Health":
                powerup.startAction.AddListener(powerupActions.HealthStartAction);
                powerup.endAction.AddListener(powerupActions.HealthEndAction);
                break;
            case "Shield":
                powerup.startAction.AddListener(powerupActions.ShieldStartAction);
                powerup.endAction.AddListener(powerupActions.ShieldEndAction);
                break;
            case "Viral Hit":
                powerup.startAction.AddListener(powerupActions.ViralHitStartAction);
                powerup.endAction.AddListener(powerupActions.ViralHitEndAction);
                break;
            case "Super Multiplier":
                powerup.startAction.AddListener(powerupActions.SuperMultiplierStartAction);
                powerup.endAction.AddListener(powerupActions.SuperMultiplierEndAction);
                break;
        }
    }
}
