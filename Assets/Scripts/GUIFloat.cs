using System.Collections;
using System;
using UnityEngine;

[System.Serializable]
public class GUIFloat : GUIBase
{
    public Action<float> effect;
    public float min = 0;
    public float max = 1;

    // these are always in the range 0-1
    public float uiMin = 0;
    public float uiMax = 1;

    public float lerpedValue;
    public float value;
    public float defaultValue;

    private bool isSliding = false;
    private bool isSlidingUIMin = false;
    private bool isSlidingUIMax = false;

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
        uiMin = 0;
        uiMax = 1;
    }

    private float RangeGrabPosition(float uiVal)
    {
        return currentRect.x + currentRect.width * uiVal;
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
        newVal = Mathf.Pow(newVal, power);
        newVal = Mathf.Lerp(uiMin, uiMax, newVal);
        newVal = min + newVal * (max - min);

        if (newVal > value)
        {
            value = newVal;

        }
        else
        {
            value = Mathf.Lerp(value, newVal, lerp);
        }
    }

    public override void UIUpdate()
    {
        ///Handle inputs
        Vector2 mouse = Input.mousePosition;
        mouse.y = Screen.height - mouse.y;

        if (currentRect.Contains(mouse))
        {
            if (Input.GetMouseButtonDown(0))
            {
                if (routingType != RoutingType.None)
                {
                    if (mouse.x < RangeGrabPosition(uiMin) + 10)
                    {
                        isSlidingUIMin = true;
                    }
                    else if (mouse.x > RangeGrabPosition(uiMax) - 10)
                    {
                        isSlidingUIMax = true;
                    }
                    else
                    {
                        isSliding = true;
                    }
                }
                else
                {
                    isSliding = true;
                }

                GUIUtility.ActiveControl = this;
            }
            if (Input.GetMouseButtonDown(1))
            {
                RoutingModal.SetTarget(this);
            }
        }


        if (isSliding)
        {
            float percent = (mouse.x - currentRect.min.x) / (currentRect.max.x - currentRect.min.x);
            value = min + (max - min) * Mathf.Clamp01(percent);

            if (Input.GetMouseButtonUp(0))
            {
                isSliding = false;
                GUIUtility.ActiveControl = null;
            }
        }
        else if (routingType != RoutingType.None)
        {
            if (isSlidingUIMin)
            {
                float percent = (mouse.x - currentRect.min.x) / (currentRect.max.x - currentRect.min.x);
                uiMin = Mathf.Clamp01(percent);

                if (Input.GetMouseButtonUp(0))
                {
                    isSlidingUIMin = false;
                    GUIUtility.ActiveControl = null;
                }
            }
            else if (isSlidingUIMax)
            {
                float percent = (mouse.x - currentRect.min.x) / (currentRect.max.x - currentRect.min.x);
                uiMax = Mathf.Clamp01(percent);

                if (Input.GetMouseButtonUp(0))
                {
                    isSlidingUIMax = false;
                    GUIUtility.ActiveControl = null;
                }
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

        if ( routingType != RoutingType.None)
        {
            float newWidth = wholeWidth.width * (uiMax - uiMin);
            sliderRect.y += sliderRect.height - 4;
            sliderRect.height = 4;
            sliderRect.x += wholeWidth.width * uiMin;
            sliderRect.width = newWidth;
            GUI.DrawTexture(sliderRect, GUIUtility.WhiteTexture);
        }
     

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
            uiMin = float.Parse(fields[10]);
            uiMax = float.Parse(fields[11]);
        } 
        catch(Exception e)
        {
            Debug.LogError(e);
            Debug.LogError("error parsing preset " + s);
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
        s += oscillatorFrequency.ToString() + ",";
        s += lerp.ToString() + ",";
        s += power.ToString() + ",";
        s += duty.ToString() + ",";
        s += uiMin.ToString() + ",";
        s += uiMax.ToString();
        return s;
    }
}