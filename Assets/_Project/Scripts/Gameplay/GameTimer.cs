using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class GameTimer : MonoBehaviour
{
    [SerializeField] private Slider slider;
    [SerializeField] private Image fillImage;
    [SerializeField] private Gradient gradient;

    private float maxTime;
    private Tween countDown;
    public void Setup(float time)
    {
        maxTime = time;
    }

    public void PlayCountDown()
    {
        countDown = slider.DOValue(0, maxTime).SetEase(Ease.Linear);
        slider.onValueChanged.AddListener(OnSlideValueChange);
    }

    public void OnSlideValueChange(float value)
    {
        Color color = gradient.Evaluate(value);
        if (fillImage.color != color)
            fillImage.color = color;
    }

    public void ToggleCountDown(bool isOn)
    {
        if (countDown.IsPlaying() != isOn)
            countDown.TogglePause();
    }
}
