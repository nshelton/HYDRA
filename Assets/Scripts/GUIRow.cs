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

    public float GetHeight()
    {
        if (Items.Count == 0)
        {
            return GUIUtility.BaseHeight * 0.5f;
        }
        else if (Items[0] is GUIFloat)
        {
            return GUIUtility.BaseHeight;
        }
        else if (Items[0] is GUIToggle || Items[0] is GUITrigger)
        {
            return GUIUtility.BaseHeight * 1f;
        }
        return 0;
    }

    public void DrawGUI(Rect rowRect)
    {
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
    }
}
