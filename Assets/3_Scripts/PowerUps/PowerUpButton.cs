using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PowerUpButton : MonoBehaviour
{
    [SerializeField]
    int PowerUpId;
    [SerializeField]
    GameObject EnabledGraphic;
    [SerializeField]
    GameObject DisabledGraphic;

    bool IsEnabled;

    public System.Action<int> OnPowerUpUsedCallback;

    public void SetEnabled(bool isEnabled)
    {
        IsEnabled = isEnabled;

        EnabledGraphic.SetActive(isEnabled);
        DisabledGraphic.SetActive(!isEnabled);

    }

    public void OnPowerUpButtonPressed()
    {
        OnPowerUpUsedCallback.Invoke(PowerUpId);
    }

}
