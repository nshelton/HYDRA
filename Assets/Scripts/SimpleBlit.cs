using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class SimpleBlit : MonoBehaviour
{
    [SerializeField] RenderTexture m_texture;

    void OnRenderImage(RenderTexture src, RenderTexture dest)
    {
        Graphics.Blit(m_texture, dest);
    }
}
