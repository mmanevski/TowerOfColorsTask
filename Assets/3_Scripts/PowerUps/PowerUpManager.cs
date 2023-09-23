using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerUpManager : MonoBehaviour
{
    [SerializeField]
    List<PowerUpConfig> PowerUpsList;
    [SerializeField]
    List<PowerUpButton> PowerUpsButtonList;
    void Start()
    {
        for (int i = 0; i < PowerUpsList.Count; i++)
        {
            switch (i)
            {
                case 0:
                    PowerUpsList[i].SetUpPowerUp(RemoteConfig.POWER_UP_MULTIBALLS_AMOUNT, RemoteConfig.POWER_UP_ONE_USES);
                    PowerUpsButtonList[i].gameObject.SetActive(RemoteConfig.POWER_UP_ONE_ENABLED);
                    PowerUpsButtonList[i].SetEnabled(PowerUpsList[i].IsAvailable());
                    PowerUpsButtonList[i].OnPowerUpUsedCallback += OnPowerUpUsed;
                    break;
                case 1:
                    PowerUpsList[i].SetUpPowerUp(RemoteConfig.POWER_UP_MULTIBALLS_AMOUNT, RemoteConfig.POWER_UP_TWO_USES);
                    PowerUpsButtonList[i].gameObject.SetActive(RemoteConfig.POWER_UP_TWO_ENABLED);
                    PowerUpsButtonList[i].SetEnabled(PowerUpsList[i].IsAvailable());
                    PowerUpsButtonList[i].OnPowerUpUsedCallback += OnPowerUpUsed;
                    break;
                case 2:
                    PowerUpsList[i].SetUpPowerUp(RemoteConfig.POWER_UP_MULTIBALLS_AMOUNT, RemoteConfig.POWER_UP_THREE_USES);
                    PowerUpsButtonList[i].gameObject.SetActive(RemoteConfig.POWER_UP_THREE_ENABLED);
                    PowerUpsButtonList[i].SetEnabled(PowerUpsList[i].IsAvailable());
                    PowerUpsButtonList[i].OnPowerUpUsedCallback += OnPowerUpUsed;
                    break;
                default:
                    PowerUpsList[i].SetUpPowerUp(RemoteConfig.POWER_UP_MULTIBALLS_AMOUNT, RemoteConfig.POWER_UP_THREE_USES);
                    PowerUpsButtonList[i].gameObject.SetActive(RemoteConfig.POWER_UP_TWO_ENABLED);
                    PowerUpsButtonList[i].SetEnabled(PowerUpsList[i].IsAvailable());
                    PowerUpsButtonList[i].OnPowerUpUsedCallback += OnPowerUpUsed;
                    break;
            }
        }
    }

    public void OnPowerUpUsed(int powerUpID)
    {
        if (PowerUpsList[powerUpID-1].IsAvailable())
        {
            UsePowerUp(powerUpID);
            PowerUpsButtonList[powerUpID - 1].SetEnabled(PowerUpsList[powerUpID - 1].IsAvailable());
        }
    }
    
    private void UsePowerUp(int powerUpID)
    {
        PowerUpsList[powerUpID-1].UsePowerUp();
    }
}
