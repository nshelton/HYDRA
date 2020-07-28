using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.Design;
using UnityEngine;

public class GUIManager : MonoBehaviour
{

    public GUISkin m_guiSkin;
    public BaseGUIModule[] m_modules;

    public int MODULEWIDTH = 200;
    public int PADDING = 20;

    public bool m_displayGUI = true;

    public Texture2D m_buttonTexture;

    void Start()
    {
        GUIUtility.Init();
        GUIUtility.ButtonTexture = m_buttonTexture;

        foreach (var module in m_modules)
        {
            module.Init();
        }
    }

    private void OnGUI()
    {

        if (!m_displayGUI)
            return;

        GUI.skin = m_guiSkin;
        Rect activeRect = new Rect(PADDING, PADDING, MODULEWIDTH, Screen.height - 2 * PADDING);

        foreach (var module in m_modules)
        {
            if (activeRect.y + module.GetHeight() > Screen.height)
            {
                activeRect.x += MODULEWIDTH;
                activeRect.y = PADDING;
            }

            module.DrawGUI(activeRect);
            activeRect.y += module.GetHeight();
           
        }
    }

    void Update()
    {
        if ( Input.GetKeyDown(KeyCode.Escape))
        {
            m_displayGUI = !m_displayGUI;
        }
    }
}
