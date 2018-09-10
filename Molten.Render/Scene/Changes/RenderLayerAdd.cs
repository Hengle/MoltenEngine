﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Molten.Graphics
{
    /// <summary>A <see cref="RenderLayerAdd"/> for adding <see cref="SceneLayerData"/> to the a<see cref="SceneRenderData"/> instance.</summary>
    internal class RenderLayerAdd : RenderSceneChange<RenderLayerAdd> 
    {
        public SceneRenderData SceneData;

        public SceneLayerData LayerData;

        public override void Clear()
        {
            SceneData = null;
            LayerData = null;
        }

        public override void Process()
        {
            SceneData.Layers.Add(LayerData);
            Recycle(this);
        }
    }
}
