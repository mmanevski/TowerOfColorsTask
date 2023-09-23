using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TimerCounter : MonoBehaviour
{
    [SerializeField]
    Image border;
    [SerializeField]
    Image percentFill;
    [SerializeField]
    Slider percentSlider;
    [SerializeField]
    Image iconBg;
    [SerializeField]
    Image timerIcon;
    [SerializeField]
    float smoothTime = 0.5f;
    [SerializeField]
    float stepIncrement = 0.01f;

    Coroutine animateRoutine;

    public void SetColor(Color color)
    {
        border.color = color;
        percentFill.color = color;
        iconBg.color = color;
    }


    public void SetValueSmooth(float value)
    {
        if (animateRoutine != null)
            StopCoroutine(animateRoutine);
        animateRoutine = StartCoroutine(AnimateValue(value));
    }

    public void SetValue(float value)
    {
        //value = (float)System.Math.Truncate(value * 100) / 100;
        // fast patch to avoid reaching 100% prematurely as Unity doesn't likes doubles.
        if (value < 1 && value > .99f) {
            value = .99f;
        }
        percentSlider.normalizedValue = value;
        //percentText.text = value.ToString("P0");
    }

    IEnumerator AnimateValue(float endValue)
    {
        float p = 0;
        float e = 0;
        float startValue = percentSlider.normalizedValue;
        float value = percentSlider.normalizedValue;
        while (p < 1) {
            p = e / (smoothTime * Time.timeScale);
            value = Mathf.Clamp01(Mathf.Lerp(startValue, endValue, p));
            SetValue(value);
            yield return null;
            e += Time.deltaTime;
        }

        animateRoutine = null;
    }
}
