using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

public class PostFXController : MonoBehaviour
{

    PostProcessVolume m_volume;

    Chroma m_chroma;
    Mirror m_mirror;

    private void Start()
    {
     /*   var settings = m_volume.sharedProfile.settings;
        foreach(var s in settings)
        {
            if ((Chroma)s != null)
                m_chroma = (Chroma)s;
            if ((Mirror)s != null)
                m_mirror = (Mirror)s;
        }*/
    }

    public float Mirror
    {
        set { m_mirror.amount.value = value; }
    }

    public float Chroma
    {
        set { m_chroma.amount.value = value; }
    }

}
