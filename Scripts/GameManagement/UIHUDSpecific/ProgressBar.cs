using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;


public class ProgressBar : MonoBehaviour
{
    public Slider slider;

    public Gradient gradient;

    public Image fill;

    public Image Icon;

    public void SetMaxValue(int value)
    {
        slider.maxValue = value;

        slider.value = value;

        fill.color = gradient.Evaluate(1f);

        Icon.color = gradient.Evaluate(1f);
    }

    public void SetValue(int value)
    {
        if (slider == null)
        {
            Debug.LogError("Slider reference is missing on ProgressBar!", this);
            return;
        }
        
        slider.value = value;
        
        if (fill != null)
        {
            fill.color = gradient.Evaluate(slider.normalizedValue);
        }
    
        if (Icon != null)
        {
            Icon.color = gradient.Evaluate(slider.normalizedValue);
        }
    }
}
