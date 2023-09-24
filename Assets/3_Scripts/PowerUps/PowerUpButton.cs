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
    GameObject DisabledGraphic;

    PowerUpType type;

    public System.Action<PowerUpType, PowerUpButton> OnPowerUpUsedCallback;

    public void SetUpPUButton(PowerUpType puType, Sprite icon, int num)
    {
        type = puType;
        image.sprite = icon;
        UpdatePUButton(num);
    }

    public void UpdatePUButton(int num)
    {
        valueText.text = "x" + num;
        SetEnabled(num != 0);
    }

    public void SetEnabled(bool isEnabled)
    {
        DisabledGraphic.SetActive(!isEnabled);
    }

    public void OnPowerUpButtonPressed()
    {
        OnPowerUpUsedCallback.Invoke(type, this);
    }

}
