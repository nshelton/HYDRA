

using System;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

[Serializable]
[PostProcess(typeof(ChromaRenderer), PostProcessEvent.AfterStack, "Custom/Chroma")]
public sealed class Chroma : PostProcessEffectSettings
{
    [Range(0f, 100f), Tooltip("Chroma effect intensity.")]
    public FloatParameter amount = new FloatParameter { value = 0.5f };
}

public sealed class ChromaRenderer : PostProcessEffectRenderer<Chroma>
{
    public override void Render(PostProcessRenderContext context)
    {
        var sheet = context.propertySheets.Get(Shader.Find("Hidden/Custom/Chroma"));
        sheet.properties.SetFloat("_Amount", settings.amount /context.width);

        context.command.BlitFullscreenTriangle(context.source, context.destination, sheet, 0);
    }
}