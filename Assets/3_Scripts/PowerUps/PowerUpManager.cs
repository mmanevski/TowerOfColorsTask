using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public enum PowerUpType
{ 
    Multyball,
    TimerBoost,
    ExtraBalls
}

public class PowerUpManager : MonoBehaviour
{
    [SerializeField]
    List<PowerUpConfig> PowerUpsList;
    [SerializeField]
    List<PowerUpButton> PowerUpsButtonList;
    void Start()
    {
        //Set up the powerUp buttons, and also the Powerups themselves because they need to get info from RemoteConfig. 
        for (int i = 0; i < PowerUpsButtonList.Count; i++)
        {
            int powerUpUses = 0;
            bool powerUpEnabled = false;

            switch (i)
            {
                case 0:
                    powerUpUses = RemoteConfig.POWER_UP_ONE_USES;
                    powerUpEnabled = RemoteConfig.POWER_UP_ONE_ENABLED;
                    break;
                case 1:
                    powerUpUses = RemoteConfig.POWER_UP_TWO_USES;
                    powerUpEnabled = RemoteConfig.POWER_UP_TWO_ENABLED;
                    break;
                case 2:
                    powerUpUses = RemoteConfig.POWER_UP_THREE_USES;
                    powerUpEnabled = RemoteConfig.POWER_UP_THREE_ENABLED;
                    break;
                default:
                    powerUpUses = RemoteConfig.POWER_UP_THREE_USES;
                    powerUpEnabled = RemoteConfig.POWER_UP_THREE_ENABLED;
                    break;
            }

            if (PowerUpsList[i].type == PowerUpType.Multyball)
            {
                PowerUpsList[i].SetUpPowerUp(powerUpUses, RemoteConfig.POWER_UP_MULTIBALLS_AMOUNT);
            }
            else
            {
                PowerUpsList[i].SetUpPowerUp(powerUpUses);
            }

            PowerUpsButtonList[i].gameObject.SetActive(powerUpEnabled);
            PowerUpsButtonList[i].SetUpPUButton(PowerUpsList[i].type, PowerUpsList[i].image, PowerUpsList[i].GetNumOfUses());
            PowerUpsButtonList[i].OnPowerUpUsedCallback += OnPowerUpUsed;
        }
    }

    //Fired when a powerUp button is clicked. 
    public void OnPowerUpUsed(PowerUpType puType, PowerUpButton button)
    {
        PowerUpConfig config = PowerUpsList.FirstOrDefault<PowerUpConfig>(i => i.type == puType);

        if (config.IsAvailable())
        {
            config.UsePowerUp();
            GameManager.Instance.HandlePowerUp(config);
            button.UpdatePUButton(config.GetNumOfUses());
        }
    }

    //For adding aditional powerUps during gameplay, from mission rewards.
    public void AddPowerUp(PowerUpType type, int amount)
    {
        foreach (var powerUp in PowerUpsList)
        {
            if (type == powerUp.type)
            {
                powerUp.AddNumberOfUses(amount);
                break;
            }
        }
    }

}
