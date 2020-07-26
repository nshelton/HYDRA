using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneModule : BaseGUIModule
{
    public GameObject[] m_scenes;

    public override string Name() { return "scenes"; }

    public override int GetHeight()
    {
        return (m_triggers.Count / 2 + 1) * ITEMHEIGHT;
    }


    public override void Init()
    {
        foreach(var scene in m_scenes)
        {
            m_triggers.Add(new GUITrigger()
            {
                name = scene.name,
                effect = delegate { scene.SetActive(!scene.activeSelf); },
            });
        }
    }


    public override void DrawGUI(Rect area)
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

        GUI.backgroundColor = oldBG;

        if (m_hidden)
            return;

        itemRect.width /= 2;
        var width = itemRect.width;

        for (int i = 0; i < m_triggers.Count; i ++)
        {
            if (i % 2 == 0)
            {
                itemRect.y += ITEMHEIGHT;
                itemRect.x = area.x + ITEMPADDING;
            }
            else
            {
                itemRect.x += itemRect.width;

            }

            GUI.backgroundColor = m_scenes[i].activeSelf ? oldBG : Color.clear;
            m_triggers[i].DrawGUI(itemRect);
           

        }

        GUI.backgroundColor = oldBG;

    }

}
