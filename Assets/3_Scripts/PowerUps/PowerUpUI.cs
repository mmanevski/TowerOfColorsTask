using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PowerUpUI : MonoBehaviour
{
    [SerializeField]
    Transform powerUpText;
    [SerializeField]
    Transform powerUpCounter;
    [SerializeField]
    TextMeshProUGUI powerUpCounterText;

    public void PowerUpActive()
    {
        powerUpText.gameObject.SetActive(true);
        powerUpCounter.gameObject.SetActive(true);

        powerUpCounterText.text = "x" + RemoteConfig.POWER_UP_MULTIBALLS_AMOUNT;

    }

    internal void PowerUpInactive()
    {
        powerUpText.gameObject.SetActive(false);
        powerUpCounter.gameObject.SetActive(false);
    }

    internal void UpdatePowerUpCounter(int ballsLeft)
    {
        powerUpCounterText.text = "x" + ballsLeft;
        powerUpCounterText.GetComponent<Animation>().Play();
    }
}
