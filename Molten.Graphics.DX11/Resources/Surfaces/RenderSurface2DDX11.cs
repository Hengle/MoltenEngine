﻿using Silk.NET.Direct3D11;

namespace Molten.Graphics.DX11;

public unsafe class RenderSurface2DDX11 : Texture2DDX11, IRenderSurface2D
{
    internal RenderSurface2DDX11(
        DeviceDX11 device,
        uint width,
        uint height,
        GraphicsResourceFlags flags = GraphicsResourceFlags.None,
        GraphicsFormat format = GraphicsFormat.R8G8B8A8_SNorm,
        uint mipCount = 1,
        uint arraySize = 1,
        AntiAliasLevel aaLevel = AntiAliasLevel.None,
        MSAAQuality msaa = MSAAQuality.Default, 
        string name = null)
        : base(device, width, height, flags, format, mipCount, arraySize, aaLevel, msaa, name)
    {
        Viewport = new ViewportF(0, 0, width, height);
        Name = $"Surface_{name ?? GetType().Name}";
    }

    protected override ResourceHandleDX11<ID3D11Resource> CreateTexture(DeviceDX11 device)
    {
        ID3D11Texture2D1* ptrTex = null;
        fixed (Texture2DDesc1* pDesc = &Desc)
            Device.Handle->CreateTexture2D1(pDesc, null, ref ptrTex);

        SurfaceHandleDX11 handle = new SurfaceHandleDX11(this)
        {
            NativePtr = (ID3D11Resource*)ptrTex,
        };

        ref RenderTargetViewDesc1 desc = ref handle.RTV.Desc;
        desc.Format = DxgiFormat;

        SetRTVDescription(ref desc);

        if (Desc.SampleDesc.Count > 1)
        {
            desc.ViewDimension = RtvDimension.Texture2Dmsarray;
            desc.Texture2DMSArray = new Tex2DmsArrayRtv
            {
                ArraySize = Desc.ArraySize,
                FirstArraySlice = 0,
            };
        }
        else
        {
            desc.ViewDimension = RtvDimension.Texture2Darray;
            desc.Texture2DArray = new Tex2DArrayRtv1()
            {
                ArraySize = Desc.ArraySize,
                MipSlice = 0,
                FirstArraySlice = 0,
                PlaneSlice = 0,
            };
        }

        handle.RTV.Create();
        return handle;
    }

    protected virtual void SetRTVDescription(ref RenderTargetViewDesc1 desc) { }

    protected override void UpdateDescription(TextureDimensions dimensions, GraphicsFormat newFormat)
    {
        base.UpdateDescription(dimensions, newFormat);

        Desc.MipLevels = 1; // NOTE: Do we set this on render targets?
        Viewport = new ViewportF(Viewport.X, Viewport.Y, dimensions.Width, dimensions.Height);
    }

    internal virtual void OnClear(GraphicsQueueDX11 cmd, Color color)
    {
        SurfaceHandleDX11 rsHandle = Handle as SurfaceHandleDX11;

        if (rsHandle.RTV.Ptr != null)
        {
            Color4 c4 = color;
            cmd.Ptr->ClearRenderTargetView(rsHandle.RTV, (float*)&c4);
        }
    }

    public void Clear(GraphicsPriority priority, Color color)
    {
        Surface2DClearTask task = Device.Tasks.Get<Surface2DClearTask>();
        task.Color = color;
        Device.Tasks.Push(priority, this, task);
    }

    /// <summary>Gets the viewport that defines the default renderable area of the render target.</summary>
    public ViewportF Viewport { get; protected set; }
}
