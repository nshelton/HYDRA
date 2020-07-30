using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GUITexture : GUIBase
{
    Texture2D Texture;

    public GUITexture(Texture2D tex)
    {
        Texture = tex;
    }

    public override void DrawGUI(Rect buttonRect)
    {
        GUI.DrawTexture(buttonRect, Texture);
    }
    public override void Update()
    {
    }

}
