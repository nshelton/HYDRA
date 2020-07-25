using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BaseGUIModule : MonoBehaviour
{
    public static int ITEMHEIGHT = 35;
    public static int ITEMPADDING = 5;

    public List<GUIFloat> m_parameters = new List<GUIFloat>();
    public List<GUITrigger> m_triggers = new List<GUITrigger>();

    public abstract void Init();

    public int GetHeight()
    {
        var items = m_parameters.Count + Mathf.CeilToInt(m_triggers.Count/4)  + 1;
        return items * ITEMHEIGHT;
    }

    public  void DrawGUI(Rect area)
    {
        var itemRect = new Rect(
            area.x + ITEMPADDING,
            area.y + ITEMPADDING,
            area.width - ITEMPADDING * 2,
            ITEMHEIGHT - ITEMPADDING * 2);

        GUI.Label(itemRect, "-------------------");
        itemRect.y += ITEMHEIGHT;

        foreach (var slider in m_parameters)
        {
            slider.DrawGUI(itemRect);
            itemRect.y += ITEMHEIGHT;
        }

        itemRect.width /= 4;
        var width = itemRect.width;

        foreach (var button in m_triggers)
        {
            button.DrawGUI(itemRect);
            itemRect.x += width;
        }
    }

}
