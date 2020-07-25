using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class GUIFloat
{
    public Action<float> effect;
    public float min = 0;
    public float max = 1;
    public string name = "undefined";

    public float value;

    public void DrawGUI(Rect sliderRect)
    {
        value = GUI.HorizontalSlider(sliderRect, value, min, max);
        GUI.Label(sliderRect, name);
        effect(value);
    }
}


public class GUITrigger
{
    public Action effect;
    public string name = "undefined";

    public void DrawGUI(Rect sliderRect)
    {
        if(GUI.Button(sliderRect, name))
        {
            effect();
        }
    }
}

