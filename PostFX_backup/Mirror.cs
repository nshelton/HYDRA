using System;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

[Serializable]
[PostProcess(typeof(MirrorRenderer), PostProcessEvent.AfterStack, "Custom/Mirror")]
public sealed class Mirror : PostProcessEffectSettings
{
    [Range(0f, 1f), Tooltip("Mirror effect intensity.")]
    public FloatParameter amount = new FloatParameter { value = 0.5f };
}

public sealed class MirrorRenderer : PostProcessEffectRenderer<Mirror>
{
    public override void Render(PostProcessRenderContext context)
    {
        var sheet = context.propertySheets.Get(Shader.Find("Hidden/Custom/Mirror"));
        sheet.properties.SetFloat("_Amount", settings.amount);
        sheet.properties.SetFloat("_Height", context.height);

        context.command.BlitFullscreenTriangle(context.source, context.destination, sheet, 0);
    }
}
