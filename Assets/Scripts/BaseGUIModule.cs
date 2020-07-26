using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BaseGUIModule : MonoBehaviour
{
    public static int ITEMHEIGHT = 20;
    public static int ITEMPADDING = 2;

    public bool m_hidden = false;

    public List<GUIFloat> m_parameters = new List<GUIFloat>();
    public List<GUITrigger> m_triggers = new List<GUITrigger>();

    public abstract void Init();
    public abstract string Name();

    public virtual int GetHeight()
    {
        int items =  1;

        if (!m_hidden)
        {
            items += m_parameters.Count + Mathf.CeilToInt(m_triggers.Count / 4);
        }
        return items * ITEMHEIGHT;
    }

    public virtual void Update()
    {
        foreach (var slider in m_parameters)
        {
            slider.Update();
        }
        foreach (var button in m_triggers)
        {
            button.Update();
        }
    }

    public virtual void DrawGUI(Rect area)
    {
        var itemRect = new Rect(
            area.x + ITEMPADDING,
            area.y + ITEMPADDING,
            area.width - ITEMPADDING * 2,
            ITEMHEIGHT - ITEMPADDING * 2);

        var oldBG = GUI.backgroundColor;
        GUI.backgroundColor = Color.clear;
        if (GUI.Button(itemRect, Name()))
        {
            m_hidden = !m_hidden;
        }

        itemRect.y += ITEMHEIGHT;
        GUI.backgroundColor = oldBG;

        if (m_hidden)
            return ;

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
