using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class CustomSlider 
{
    public float min;
    public float max;
    public float defaultValue;
    public float value;
    string name;

    bool isActive = false;
    public GUIFloat parent;

    public CustomSlider(float min, float max, float defaultValue, string name, GUIFloat parent)
    {
        this.min = min;
        this.max = max;
        this.defaultValue = defaultValue;
        this.name = name;
        this.value = defaultValue;
        this.parent = parent;

    }

    public float Draw(Rect area)
    {
        ///Handle selection
        if (isActive)
        {
            if (Event.current.type == EventType.MouseDrag)
            {
                float percent = (Event.current.mousePosition.x - area.min.x) / (area.max.x - area.min.x);
                value = min + (max - min) * Mathf.Clamp01(percent);
            }
            else if(Event.current.type == EventType.MouseUp)
            {
                isActive = false;
                GUIUtility.ActiveControl = null;
            }
        }

        if (area.Contains(Event.current.mousePosition)) {

            if (Event.current.type == EventType.MouseDown)
            {
                if (Event.current.button == 0)
                {
                    float percent = (Event.current.mousePosition.x - area.min.x)  / (area.max.x - area.min.x);
                    value = min + (max - min) * percent;
                    isActive = true;
                    GUIUtility.ActiveControl = this;
                }
                else if (Event.current.button == 1)
                {
                    parent.ShowRoutingModal = true;
                }
            }
        }

        ///Draw GUI
        GUI.backgroundColor = Color.clear;
        GUI.Label(area, name + "\t" + value.ToString("0.##"));

        GUI.backgroundColor = Color.white * 0.5f;
        area.width *= (value - min) / (max - min);
        GUI.DrawTexture(area, GUIUtility.GrayTexture);

        return value;

    }

}
