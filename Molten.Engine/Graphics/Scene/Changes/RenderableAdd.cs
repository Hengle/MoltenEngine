﻿namespace Molten.Graphics;

/// <summary>A <see cref="GraphicsTask"/> for adding a <see cref="Renderable"/> to the root of a scene.</summary>
internal class RenderableAdd : GraphicsTask
{
    public Renderable Renderable;

    public ObjectRenderData Data;

    public LayerRenderData LayerData;

    public override void ClearForPool()
    {
        Renderable = default;
        Data = null;
        LayerData = null;
    }

    public override bool Validate() => true;

    protected override bool OnProcess(RenderService renderer, GraphicsQueue queue)
    {
        RenderDataBatch batch;
        if (!LayerData.Renderables.TryGetValue(Renderable, out batch))
        {
            batch = new RenderDataBatch();
            LayerData.Renderables.Add(Renderable, batch);
        }

        batch.Add(Data);
        return true;
    }
}
