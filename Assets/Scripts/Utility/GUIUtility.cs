using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GUIUtility 
{

    public static float BaseHeight = 25;
    public static float ItemPadding = 2;

    public static GUIBase ActiveControl;

    public static Texture2D ClearTexture;
    public static Texture2D WhiteTexture;
    public static Texture2D GrayTexture;

    public static Texture2D GreenTexture;
    public static Texture2D OrangeTexture;
    public static Texture2D BlackTexture;

    public static float OverlayAlpha = 0.7f;
    public static Color OscillatorRoutingColor = new Color(1.0f, 0.5f, 0.0f);
    public static Color AudioRoutingColor = new Color(1.0f, 0.5f, 0.0f);

    public static GUIBase ControlModal;

    public static void Init()
    {
        ClearTexture = new Texture2D(2, 2, UnityEngine.Experimental.Rendering.DefaultFormat.LDR, UnityEngine.Experimental.Rendering.TextureCreationFlags.None);
        WhiteTexture = new Texture2D(2, 2, UnityEngine.Experimental.Rendering.DefaultFormat.LDR, UnityEngine.Experimental.Rendering.TextureCreationFlags.None);
        GrayTexture = new Texture2D(2, 2, UnityEngine.Experimental.Rendering.DefaultFormat.LDR, UnityEngine.Experimental.Rendering.TextureCreationFlags.None);
        GreenTexture = new Texture2D(2, 2, UnityEngine.Experimental.Rendering.DefaultFormat.LDR, UnityEngine.Experimental.Rendering.TextureCreationFlags.None);
        OrangeTexture = new Texture2D(2, 2, UnityEngine.Experimental.Rendering.DefaultFormat.LDR, UnityEngine.Experimental.Rendering.TextureCreationFlags.None);
        BlackTexture = new Texture2D(2, 2, UnityEngine.Experimental.Rendering.DefaultFormat.LDR, UnityEngine.Experimental.Rendering.TextureCreationFlags.None);

        for (int i = 0; i < 2; i++)
        {
            for (int j = 0; j < 2; j++)
            {
                ClearTexture.SetPixel(i, j, new Color(0,0,0,0));
                WhiteTexture.SetPixel(i, j, new Color(1f, 1f, 1f, 1f));
                GrayTexture.SetPixel(i, j, new Color(0.5f, 0.5f, 0.5f, 1f));
                GreenTexture.SetPixel(i, j, new Color(0.0f, 1.0f, 0.0f, 1f));
                OrangeTexture.SetPixel(i, j, new Color(1.0f, 0.5f, 0.5f, 1f));
                BlackTexture.SetPixel(i, j, new Color(0,0,0,1f));
            }
        }

        ClearTexture.Apply();
        GrayTexture.Apply();
        WhiteTexture.Apply();
        GreenTexture.Apply();
        OrangeTexture.Apply();
        BlackTexture.Apply();
    }

}
