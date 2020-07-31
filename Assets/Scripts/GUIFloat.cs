using System.Collections;
using System;
using UnityEngine;

[System.Serializable]
public class GUIFloat : GUIBase
{
    public Action<float> effect;
    public float min = 0;
    public float max = 1;
    public float value;
    public float defaultValue;

    private bool isActive = false;
    public Rect currentRect = new Rect();

    public GUIFloat(string name, float min, float max, float value, Action<float> effect)
    {
        this.min = min;
        this.max = max;
        this.value = value;
        this.effect = effect;
        this.name = name;
        this.defaultValue = value;
    }

    public override void ResetToDefault()
    {
        value = defaultValue;
        routingType = RoutingType.None;
    }
    public override void Update()
    {
        // handle routing
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
        value = Mathf.Lerp(value, newVal, lerp);
    }

    public override void UIUpdate()
    {
        ///Handle inputs
        Vector2 mouse = Input.mousePosition;
        mouse.y = Screen.height - mouse.y;
        if (isActive)
        {
            if (isActive)
            {
                float percent = (mouse.x - currentRect.min.x) / (currentRect.max.x - currentRect.min.x);
                value = min + (max - min) * Mathf.Clamp01(percent);

                if (Input.GetMouseButtonUp(0))
                {
                    isActive = false;
                    GUIUtility.ActiveControl = null;
                }
            }
        }

        if (currentRect.Contains(mouse))
        {
            if (Input.GetMouseButtonDown(0))
            {
                float percent = (mouse.x - currentRect.min.x) / (currentRect.max.x - currentRect.min.x);
                value = min + (max - min) * percent;
                isActive = true;
                GUIUtility.ActiveControl = this;
            }
            if (Input.GetMouseButtonDown(1))
            {
                GUIUtility.ActiveControl = this;
                GUIUtility.ControlModal = this;
            }
        }
    }

    public override void DrawGUI(Rect sliderRect)
    {
        base.DrawGUI(sliderRect);

        currentRect = sliderRect;

        var color = GUIUtility.WhiteTexture;

        if (routingType == RoutingType.Oscillator)
        {
            color = GUIUtility.OrangeTexture;

        }
        else if (
            routingType == RoutingType.AudioBypass ||
            routingType == RoutingType.AudioHigh ||
            routingType == RoutingType.AudioLow)
        {
            color = GUIUtility.GreenTexture;
        }

        ///Draw GUI
        var wholeWidth = sliderRect;
        sliderRect.width *= (value - min) / (max - min);
        GUI.DrawTexture(sliderRect, color);

        GUI.Label(wholeWidth, name + "\t" + value.ToString("0.##"));

        effect(value);
    }
    public override void SetFromFieldsString(string s)
    {
        var fields = s.Split(',');

        try
        {
            min = float.Parse(fields[0]);
            max = float.Parse(fields[1]);
            value = float.Parse(fields[2]);
            defaultValue = float.Parse(fields[3]);
            routingType = (RoutingType)int.Parse(fields[4]);
            oscillatorType = (OscillatorType)int.Parse(fields[5]);
            oscillatorFrequency = int.Parse(fields[6]);
            lerp = float.Parse(fields[7]);
            power = float.Parse(fields[8]);
            duty = float.Parse(fields[9]);
        } 
        catch(Exception e)
        {
            Debug.LogError(e);
            Debug.LogError("error parsing macro " + s);
        }
    }

    public override string SerializeData()
    {
        string s = string.Empty;
        
        s += min.ToString() + ",";
        s += max.ToString() + ",";
        s += value.ToString() + ",";
        s += defaultValue.ToString() + ",";
        s += ((int)routingType).ToString() + ",";
        s += ((int)oscillatorType).ToString() + ",";
        s += (oscillatorFrequency).ToString() + ",";
        s += (lerp).ToString() + ",";
        s += (power).ToString() + ",";
        s += (duty).ToString();

        return s;

    }
}