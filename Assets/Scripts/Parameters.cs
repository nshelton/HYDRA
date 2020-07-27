using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.PlayerLoop;
using UnityEngine.UI;

public enum RoutingType
{
    None,
    AudioLow,
    AudioBypass,
    AudioHigh,
    Oscillator,
}

public enum OscillatorType
{
    Saw,
    Square,
    Sine
}

public class GUIFloat
{
    public Action<float> effect;
    public float min = 0;
    public float max = 1;
    public string name = "undefined";

    public float value;
    public RoutingType routingType;
    public OscillatorType oscillatorType;
    public int oscillatorFrequency = 1;

    public float lerp = 1;
    public float power = 1;
    public float duty = 1;

    private bool routingModal = false;

    Rect GetWindowRect(Rect button)
    {
        var width = 300;
        var height = 200;
        var x = button.x + button.width / 2;
        var y = button.y + button.height / 2;
        return new Rect(x - width / 2, y - height / 2,  width, height);
    }

    public void Update()
    {
        if (routingType == RoutingType.None)
            return;

        float newVal = 0; 
        switch (routingType)
        {
            case RoutingType.AudioBypass:
                newVal = RoutingServer.SampleBypass();
                break;
            case RoutingType.AudioHigh:
                newVal = RoutingServer.SampleTreble();
                break;
            case RoutingType.AudioLow:
                newVal = RoutingServer.SampleBass();
                break;
            case RoutingType.Oscillator:
                newVal = RoutingServer.SampleOscillator(oscillatorType, oscillatorFrequency);
                break;
        }
        newVal = min + newVal * (max - min);
        value = Mathf.Lerp(value, newVal, lerp * lerp * lerp * lerp);
    }

    public void DrawGUI(Rect sliderRect)
    {
        sliderRect.width -= sliderRect.height;
        value = GUI.HorizontalSlider(sliderRect, value, min, max);
        GUI.Label(sliderRect, name + "\t" + value.ToString("0.##"));

        sliderRect.x += sliderRect.width;
        sliderRect.width = sliderRect.height;

        if(GUI.Button(sliderRect, "O"))
        {
            routingModal = true;
        }

        if ( routingModal )
        {
             GUI.Window(0, GetWindowRect(sliderRect), RoutingWindow, "");
        }

        effect(Mathf.Pow(value, power));
    }

    void RoutingWindow(int windowID)
    {
        var selectedColor = Color.white * 0.9f;
        var normalColor = Color.white * 0.5f;

        GUI.color = routingType == RoutingType.AudioLow ? selectedColor : normalColor;

        if (GUI.Button(new Rect(0, 0, 67, 50), "low"))
        {
            routingType = RoutingType.AudioLow;
            routingModal = false;
        }

        GUI.color = routingType == RoutingType.AudioBypass ? selectedColor : normalColor;
        if (GUI.Button(new Rect(67, 0, 67, 50), "all"))
        {
            routingType = RoutingType.AudioBypass;
            routingModal = false;
        }

        GUI.color = routingType == RoutingType.AudioHigh ? selectedColor : normalColor;
        if (GUI.Button(new Rect(134, 0, 67, 50), "high"))
        {
            routingType = RoutingType.AudioHigh;
            routingModal = false;
        }

        GUI.color = normalColor;
        if (GUI.Button(new Rect(0, 50, 100, 50), "back"))
        {
            routingModal = false;
        }

        if (GUI.Button(new Rect(100, 50, 100, 50), "clear"))
        {
            routingType = RoutingType.None;
            routingModal = false;
        }

        GUI.color = routingType == RoutingType.Oscillator && oscillatorType == OscillatorType.Saw ? selectedColor : normalColor;
        if (GUI.Button(new Rect(0, 100, 67, 50), "saw"))
        {
            routingType = RoutingType.Oscillator;
            oscillatorType = OscillatorType.Saw;
        }

        GUI.color = routingType == RoutingType.Oscillator && oscillatorType == OscillatorType.Square ? selectedColor : normalColor;
        if (GUI.Button(new Rect(67, 100, 67, 50), "square"))
        {
            routingType = RoutingType.Oscillator;
            oscillatorType = OscillatorType.Square;
        }

        GUI.color = routingType == RoutingType.Oscillator && oscillatorType == OscillatorType.Sine ? selectedColor : normalColor;
        if (GUI.Button(new Rect(134, 100, 67, 50), "sine"))
        {
            routingType = RoutingType.Oscillator;
            oscillatorType = OscillatorType.Sine;
        }


        GUI.color = routingType == RoutingType.Oscillator && oscillatorFrequency == 1 ? selectedColor : normalColor;
        if (GUI.Button(new Rect(0, 150, 50, 50), "1"))
        {
            oscillatorFrequency = 1;
            routingModal = false;
        }

        GUI.color = routingType == RoutingType.Oscillator && oscillatorFrequency == 2 ? selectedColor : normalColor;
        if (GUI.Button(new Rect(50, 150, 50, 50), "2"))
        {
            oscillatorFrequency = 2;
            routingModal = false;
        }

        GUI.color = routingType == RoutingType.Oscillator && oscillatorFrequency == 4 ? selectedColor : normalColor;
        if (GUI.Button(new Rect(100, 150, 50, 50), "4"))
        {
            oscillatorFrequency = 4;
            routingModal = false;
        }

        GUI.color = routingType == RoutingType.Oscillator && oscillatorFrequency == 8 ? selectedColor : normalColor;
        if (GUI.Button(new Rect(150, 150, 50, 50), "8"))
        {
            oscillatorFrequency = 8;
            routingModal = false;
        }

        /// Parameter Controls

        lerp = GUI.VerticalSlider(new Rect(200, 0, 30, 150), lerp, 0.1f, 1.0f);
        power = GUI.VerticalSlider(new Rect(230, 0, 30, 150), power, 0.1f, 5.0f);
        duty = GUI.VerticalSlider(new Rect(260, 0, 30, 150), duty, 0f, 1f);

        GUI.Label(new Rect(200, 150, 100, 30),"lerp:\t" + lerp.ToString("0.##"));
        GUI.Label(new Rect(200, 170, 100, 30),"power:\t" + power.ToString("0.##"));
        GUI.Label(new Rect(200, 190, 100, 30),"duty:\t" + duty.ToString("0.##"));
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
    public void Update()
    {

    }
}

