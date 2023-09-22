using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseUI : MonoBehaviour
{
    [SerializeField]
    Animator animator;
    [SerializeField]
    CustomToggle colorblindToggle;
    [SerializeField]
    CustomToggle vibrationToggle;

    void Awake()
    {
        //animator = GetComponent<Animator>();
        //animator.speed = 1.0f / Time.timeScale;

        colorblindToggle.SetEnabled(SaveData.CurrentColorList == 1);
        vibrationToggle.SetEnabled(SaveData.VibrationEnabled == 1);
    }

    public void ClosePopup()
    {

        GameManager.Instance.TogglePause(false);
        //gameObject.SetActive(false);

    }

    public void OnColorblindClick(bool value)
    {
        if (SaveData.CurrentColorList == 1 != value)
        {
            SaveData.CurrentColorList = value ? 1 : 0;
            Debug.Log(SaveData.CurrentColorList);
            TileColorManager.Instance.SetColorList(SaveData.CurrentColorList);
        }
    }

    public void OnVibrationClick(bool value)
    {
        if (SaveData.VibrationEnabled == 1 != value)
        {
            SaveData.VibrationEnabled = value ? 1 : 0;
            if (value)
                Handheld.Vibrate();
        }
    }
}
