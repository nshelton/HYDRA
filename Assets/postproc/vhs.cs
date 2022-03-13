using System;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

[Serializable]
[PostProcess(typeof(vhsRenderer), PostProcessEvent.BeforeStack, "Custom/vhs")]
public sealed class vhs : PostProcessEffectSettings
{
    [Range(0f, 1f), Tooltip("vhs effect intensity.")]
    public FloatParameter RGBFuzz = new FloatParameter { value = 0.5f };
    public FloatParameter RGBScroll = new FloatParameter { value = 0.5f };
    public FloatParameter Scanline = new FloatParameter { value = 0.5f };
    public FloatParameter Jump = new FloatParameter { value = 0.5f };
    public FloatParameter Jitter = new FloatParameter { value = 0.5f };
}

public sealed class vhsRenderer : PostProcessEffectRenderer<vhs>
{
    private float _jumpTime = 0;
    private float _prevTime = 0;
    private float _AccumRGBScroll = 0;

    public override void Render(PostProcessRenderContext context)
    {
        var time = Time.time;
        var delta = time - _prevTime;

        _prevTime = time;

        // Jitter parameters (threshold, displacement)
        var jv = settings.Jitter;
        var vjitter = new Vector3(
            Mathf.Max(0, 1.001f - jv * 1.2f),
           jv
        );

        _jumpTime += delta * settings.Jump * 11.3f;

        var vjump = new Vector2(_jumpTime, settings.Jump);
        _AccumRGBScroll += delta * settings.RGBScroll;

        var sheet = context.propertySheets.Get(Shader.Find("Hidden/Custom/vhs"));
        sheet.properties.SetFloat("_RGBFuzz", settings.RGBFuzz);
        sheet.properties.SetFloat("_RGBScroll", _AccumRGBScroll);
        sheet.properties.SetFloat("_Scanline", settings.Scanline);
        
        sheet.properties.SetVector("_Jitter", vjitter);
        sheet.properties.SetVector("_Jump", vjump);

        sheet.properties.SetFloat("_Height", context.height);
        sheet.properties.SetFloat("_Width", context.width);

        context.command.BlitFullscreenTriangle(context.source, context.destination, sheet, 0);
    }
}