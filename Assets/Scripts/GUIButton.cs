using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

 
public class GUITrigger : GUIBase
{
    protected CustomButton Button;

    public GUITrigger(string name, Action effectL, Action effectR)
    {
        Button = new CustomButton(name, effectL, effectR);
        this.name = name;
    }

    public GUITrigger(string name, Action effectL)
    {
        Button = new CustomButton(name, effectL, null);
        this.name = name;
    }

    public override void DrawGUI(Rect buttonRect)
    {
        Button.DrawGUI(buttonRect, false);
    }
    public override void Update()
    {
        base.Update();
    }

    public override void UIUpdate()
    {
        Button.Update();
        base.UIUpdate();
    }
}
 
public class GUIToggle : GUIBase
{
    public bool Enabled;
    private CustomButton Button;
    public GUIToggle(string name, Action<bool> effect)
    {
        this.Enabled = false;
        this.name = name;

        Button = new CustomButton(this.name, delegate
        {
            this.Enabled = !this.Enabled;
            effect(Enabled);

        }, null);
    }

    public override void DrawGUI(Rect buttonRect)
    {
        Button.DrawGUI(buttonRect, Enabled);
    }

    public override void Update()
    {
        base.Update();
    }

    public override void UIUpdate()
    {
        Button.Update();
        base.UIUpdate();
    }
}

public class CustomButton 
{

    Action actionLeft;
    Action actionRight;

    string name;
    Rect currentRect = new Rect();

    public CustomButton(string name, Action actionLeft, Action actionRight)
    {
        this.actionLeft = actionLeft;
        this.actionRight = actionRight;
        this.name = name;
    }

    public void Update()
    {
        Vector2 mouse = Input.mousePosition;
        mouse.y = Screen.height - mouse.y;

        if (currentRect.Contains(mouse))
        {
            if (Input.GetMouseButtonDown(0))
            {
                actionLeft();
            }
            if (Input.GetMouseButtonDown(1) && actionRight != null)
            {
                actionRight();
            }
        }
    }

    public void DrawGUI(Rect area, bool enabled)
    {
        currentRect = area;
        GUI.DrawTexture(area, enabled ? GUIUtility.GreenTexture : GUIUtility.WhiteTexture );
        GUI.Label(area, name );
    }

    public void DrawGUI(Rect area, Texture2D texture)
    {
        currentRect = area;
        GUI.DrawTexture(area, texture);
        GUI.Label(area, name);
    }
}


public class GUIPresetTrigger : GUITrigger
{
    public bool Assigned = false;
    public bool Active = false;

    public GUIPresetTrigger(string name, Action effectL, Action effectR) :
        base(name, effectL, effectR)
    {
    }

    public override void DrawGUI(Rect buttonRect)
    {
        var texture = GUIUtility.GrayTexture;

        if (Active)
        {
            texture = GUIUtility.GreenTexture;
        }
        else if (Assigned)
        {
            texture = GUIUtility.WhiteTexture;
        }

        Button.DrawGUI(buttonRect, texture);
    }
}