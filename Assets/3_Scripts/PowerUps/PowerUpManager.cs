using System;
using System.Collections;
using System.Collections.Generic;
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
            PowerUpsButtonList[i].SetUpPUButton(PowerUpsList[i]);
            PowerUpsButtonList[i].OnPowerUpUsedCallback += OnPowerUpUsed;
        }
        /*
        for (int i = 0; i < PowerUpsList.Count; i++)
        {
            switch (i)
            {
                case 0:
                    PowerUpsList[i].SetUpPowerUp(RemoteConfig.POWER_UP_ONE_USES);
                    PowerUpsButtonList[i].gameObject.SetActive(RemoteConfig.POWER_UP_ONE_ENABLED);
                    PowerUpsButtonList[i].SetEnabled(PowerUpsList[i].IsAvailable());
                    PowerUpsButtonList[i].OnPowerUpUsedCallback += OnPowerUpUsed;
                    break;
                case 1:
                    PowerUpsList[i].SetUpPowerUp(RemoteConfig.POWER_UP_TWO_USES);
                    PowerUpsButtonList[i].gameObject.SetActive(RemoteConfig.POWER_UP_TWO_ENABLED);
                    PowerUpsButtonList[i].SetEnabled(PowerUpsList[i].IsAvailable());
                    PowerUpsButtonList[i].OnPowerUpUsedCallback += OnPowerUpUsed;
                    break;
                case 2:
                    PowerUpsList[i].SetUpPowerUp(RemoteConfig.POWER_UP_THREE_USES);
                    PowerUpsButtonList[i].gameObject.SetActive(RemoteConfig.POWER_UP_THREE_ENABLED);
                    PowerUpsButtonList[i].SetEnabled(PowerUpsList[i].IsAvailable());
                    PowerUpsButtonList[i].OnPowerUpUsedCallback += OnPowerUpUsed;
                    break;
                default:
                    PowerUpsList[i].SetUpPowerUp(RemoteConfig.POWER_UP_THREE_USES);
                    PowerUpsButtonList[i].gameObject.SetActive(RemoteConfig.POWER_UP_TWO_ENABLED);
                    PowerUpsButtonList[i].SetEnabled(PowerUpsList[i].IsAvailable());
                    PowerUpsButtonList[i].OnPowerUpUsedCallback += OnPowerUpUsed;
                    break;
            }

        }
        */
    }

    public void OnPowerUpUsed(PowerUpConfig config, PowerUpButton button)
    {
        if (config.IsAvailable())
        {
            config.UsePowerUp();
            GameManager.Instance.HandlePowerUp(config);
            button.UpdatePUButton();
        }
    }

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
