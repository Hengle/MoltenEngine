﻿namespace Molten.Graphics;

/// <summary>A <see cref="RenderLayerAdd"/> for adding <see cref="LayerRenderData"/> to the a<see cref="SceneRenderData"/> instance.</summary>
internal class RenderLayerRemove : GraphicsTask
{
    public SceneRenderData SceneData;

    public LayerRenderData LayerData;

    public override void ClearForPool()
    {
        SceneData = null;
        LayerData = null;
    }

    public override bool Validate() => true;

    protected override bool OnProcess(RenderService renderer, GraphicsQueue queue)
    {
        SceneData.Layers.Remove(LayerData);
        return true;
    }
}
