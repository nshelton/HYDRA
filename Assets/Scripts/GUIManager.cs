using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.Design;
using UnityEngine;

public class GUIManager : MonoBehaviour
{

    public GUISkin m_guiSkin;
    public BaseGUIModule[] m_modules;

    public int ItemHeight = 25;
    public int ItemPadding = 2;

    public int MODULEWIDTH = 200;
    public int PADDING = 20;

    public bool m_displayGUI = true;

    void Start()
    {
        GUIUtility.Init();
        RoutingModal.Init();

        foreach (var module in m_modules)
        {
            module.Init();
        }
    }

    private void OnGUI()
    {
        GUIUtility.BaseHeight = ItemHeight;
        GUIUtility.ItemPadding = ItemPadding;

        GUI.color = Color.white;

        GUI.contentColor = Color.white;
        GUI.backgroundColor = Color.clear;

        if (!m_displayGUI)
            return;

        GUI.skin = m_guiSkin;
        Rect activeRect = new Rect(PADDING, PADDING, MODULEWIDTH, Screen.height - 2 * PADDING);

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

        if ( Input.GetKeyDown(KeyCode.Escape))
        {
            m_displayGUI = !m_displayGUI;
        }

    }
}
