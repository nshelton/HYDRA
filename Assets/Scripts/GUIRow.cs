using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GUIRow 
{
    public List<GUIBase> Items = new List<GUIBase>();

    public void Update()
    {
        for(int i = 0; i < Items.Count; i++)
        {
            Items[i].Update();
        }    
    }
    public void UIUpdate()
    {
        for (int i = 0; i < Items.Count; i++)
        {
            Items[i].UIUpdate();
        }
    }

    public float Height = GUIUtility.BaseHeight;
    public Color GUIColor = Color.white;
    
    public void DrawGUI(Rect rowRect)
    {
        var prevGUIColor = GUI.color;

        GUI.color = Color.white * GUIUtility.Opacity * GUIColor;

        float width = rowRect.width / Items.Count;

        Rect itemRect = new Rect(
            rowRect.x + GUIUtility.ItemPadding,
            rowRect.y + GUIUtility.ItemPadding,
            width - GUIUtility.ItemPadding * 2,
            rowRect.height - GUIUtility.ItemPadding * 2);

        for(int i = 0; i < Items.Count; i++)
        {
            Items[i].DrawGUI(itemRect);
            itemRect.x += width;
        }

        GUI.color = prevGUIColor;
    }
}

