using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PowerUpButton : MonoBehaviour
{
    [SerializeField]
    Image image;
    [SerializeField]
    TextMeshProUGUI valueText;
    [SerializeField]
    GameObject EnabledGraphic;
    [SerializeField]
    GameObject DisabledGraphic;

    PowerUpConfig setConfig;

    bool IsEnabled;

    public System.Action<PowerUpConfig, PowerUpButton> OnPowerUpUsedCallback;

    public void SetUpPUButton(PowerUpConfig config)
    {
        setConfig = config;
        UpdatePUButton();
    }

    public void UpdatePUButton()
    {
        image.sprite = setConfig.image;
        valueText.text = "x" + setConfig.GetNumOfUses();
        SetEnabled(setConfig.GetNumOfUses() != 0);
    }

    public void SetEnabled(bool isEnabled)
    {
        IsEnabled = isEnabled;

        EnabledGraphic.SetActive(isEnabled);
        DisabledGraphic.SetActive(!isEnabled);

    }

    public void OnPowerUpButtonPressed()
    {
        OnPowerUpUsedCallback.Invoke(setConfig, this);
    }

}
