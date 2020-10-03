using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.Design;
using UnityEngine;

public class GUIManager : MonoBehaviour
{

    public GUISkin m_guiSkin;
    public BaseGUIModule[] m_modules;
    public MacroModule m_macroModule;

    [Range(0,1)]
    public float m_opacity = 0.5f;

    public int ItemHeight = 25;
    public int ItemPadding = 2;

    public static int MODULEWIDTH = 250;
    public int PADDING = 20;

    public bool m_displayGUI = true;
    public bool m_collapseAll = false;

    void Start()
    {
        GUIUtility.Init();
        RoutingModal.Init();

        foreach (var module in m_modules)
        {
            module.Init();
        }

        m_macroModule.Init();

    }

    private void OnGUI()
    {
        GUIUtility.Opacity = m_opacity;

        GUIUtility.BaseHeight = ItemHeight;
        GUIUtility.ItemPadding = ItemPadding;

        // Text color
        GUI.contentColor = Color.black;
        GUI.backgroundColor = Color.clear;

        if (!m_displayGUI)
            return;

        GUI.skin = m_guiSkin;
        Rect activeRect = new Rect(0, 0, MODULEWIDTH, Screen.height - 2 * PADDING);

        foreach (var module in m_modules)
        {
            if (module.gameObject.activeInHierarchy)
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

        m_macroModule.DrawGUI(new Rect(
            Screen.width - MODULEWIDTH, 
            0,
            MODULEWIDTH, Screen.height));

        GUI.color = Color.white * 0.8f;

        if (GUIUtility.ControlModal != null)
        {
            RoutingModal.DrawGUI();
        }
    }

    void Update()
    {
        if (GUIUtility.ControlModal != null)
        {
            RoutingModal.Update();
            RoutingModal.UIUpdate();
        }

        foreach (var module in m_modules)
        {
            module.ManagerUpdate();

            if ( GUIUtility.ControlModal == null)
            {
                module.UIUpdate();
            }
        }

        m_macroModule.UIUpdate();

        if ( Input.GetKeyDown(KeyCode.Escape))
        {
            m_opacity = 0f ;
        }
         

        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            m_opacity = 0.4f;
        }


        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            m_opacity = 0.8f;
        }


        if (Input.GetKeyDown(KeyCode.Tab))
        {
            m_collapseAll = !m_collapseAll;
            foreach (var module in m_modules)
            {
                module.m_hidden = m_collapseAll;
            }
        }

    }
}
