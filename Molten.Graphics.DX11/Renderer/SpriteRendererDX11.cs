﻿namespace Molten.Graphics
{
    public class SpriteRendererDX11 : Renderable, ISpriteRenderer
    {
        internal SpriteRendererDX11(Device device, Action<SpriteBatcher> callback) : base(device)
        {
            Callback = callback;
        }

        public Action<SpriteBatcher> Callback { get; set; }

        private protected override void OnRender(DeviceContext pipe, RendererDX11 renderer, RenderCamera camera, ObjectRenderData data)
        {
            SpriteStyle curStyle = renderer.SpriteBatcher.GetStyle();
            Callback?.Invoke(renderer.SpriteBatcher);
            renderer.SpriteBatcher.Flush(pipe, camera, data);
        }
    }
}
