using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    public Slider slider;
    public Gradient gradient;
    public Image fill;

    public void SetMaxLife(float maxLife)
    {
        slider.maxValue = maxLife;
        slider.value = maxLife;

        fill.color = gradient.Evaluate(1);
    }
    public void SetLife(float life)
    {
        slider.value = life;

        fill.color = gradient.Evaluate(slider.normalizedValue);
    }
}
