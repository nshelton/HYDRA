using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.GlobalIllumination;

public class LightingController : MonoBehaviour
{
    [SerializeField] public Light m_dirLight;

    public float AmbientLevel
    {
        set { UnityEngine.RenderSettings.ambientLight = Color.white * value; }
    }

    public float TimeScale
    {
        set { UnityEngine.Time.timeScale = value; }
    }

    public float DirectionalLevel
    {
        set { m_dirLight.intensity = 3f * value; }
    }

}
