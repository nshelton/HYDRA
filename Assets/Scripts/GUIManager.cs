using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GUIManager : MonoBehaviour
{

    public GUISkin m_guiSkin;
    public BaseGUIModule[] m_modules;

    public int MODULEWIDTH = 200;
    public int PADDING = 20;

    void Start()
    {
        foreach(var module in m_modules)
        {
            module.Init();
        }
    }

    private void OnGUI()
    {
        GUI.skin = m_guiSkin;
        GUI.backgroundColor = Color.white * 0.5f;

        Rect activeRect = new Rect(PADDING, PADDING, MODULEWIDTH, Screen.height - 2 * PADDING);

        foreach (var module in m_modules)
        {
            module.DrawGUI(activeRect);

            activeRect.y += module.GetHeight();
            if (activeRect.y + module.GetHeight() > Screen.height)
            {
                activeRect.x += MODULEWIDTH;
                activeRect.y = 0;

            }
        }
    }

    void Update()
    {
        
    }
}
