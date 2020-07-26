using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GUIUtility 
{

    public static Texture2D LightGrayTexture;
    public static Texture2D GrayTexture;

    public static void Init()
    {
        LightGrayTexture = new Texture2D(2, 2, UnityEngine.Experimental.Rendering.DefaultFormat.LDR, UnityEngine.Experimental.Rendering.TextureCreationFlags.None);
        GrayTexture = new Texture2D(2, 2, UnityEngine.Experimental.Rendering.DefaultFormat.LDR, UnityEngine.Experimental.Rendering.TextureCreationFlags.None);

        for (int i = 0; i < 2; i++)
        {
            for (int j = 0; j < 2; j++)
            {
                LightGrayTexture.SetPixel(i, j, new Color(1f, 1f, 1f, 0.5f));
                GrayTexture.SetPixel(i, j, new Color(0.5f, 0.5f, 0.5f, 0.5f));
            }
        }
        GrayTexture.Apply();
        LightGrayTexture.Apply();
    }

}
