﻿using Molten.Collections;
using Molten.Graphics.Dxgi;
using Molten.Windows32;
using Silk.NET.Core.Native;
using Silk.NET.Direct3D11;
using Silk.NET.DXGI;

namespace Molten.Graphics.DX11;

/// <summary>A render target that is created from, and outputs to, a device's swap chain.</summary>
public unsafe abstract class SwapChainSurfaceDX11 : RenderSurface2DDX11, ISwapChainSurface
{
    protected internal IDXGISwapChain4* SwapChainHandle;

    PresentParameters* _presentParams;
    SwapChainDesc1 _swapDesc;

    ThreadedQueue<Action> _dispatchQueue;
    uint _vsync;

    internal SwapChainSurfaceDX11(DeviceDX11 device, uint width, uint height, uint mipCount, GraphicsFormat format = GraphicsFormat.B8G8R8A8_UNorm)
        : base(device, width, height, 
              GraphicsResourceFlags.DenyShaderAccess | GraphicsResourceFlags.None | GraphicsResourceFlags.GpuWrite,
              format, mipCount, 1, AntiAliasLevel.None, MSAAQuality.Default)
    {
        _dispatchQueue = new ThreadedQueue<Action>();
        _presentParams = EngineUtil.Alloc<PresentParameters>();
        _presentParams[0] = new PresentParameters();
    }

    protected override ResourceHandleDX11<ID3D11Resource> CreateTexture(DeviceDX11 device)
    {
        // Resize the swap chain if needed.
        if (SwapChainHandle != null)
        {
            WinHResult result = SwapChainHandle->ResizeBuffers(Device.FrameBufferSize, Width, Height, GraphicsFormat.Unknown.ToApi(), 0U);
            SwapChainHandle->GetDesc1(ref _swapDesc);
        }
        else
        {
            NativeUtil.ReleasePtr(ref SwapChainHandle);
            OnCreateSwapchain(ref Desc);
            SwapChainHandle->GetDesc1(ref _swapDesc);

            _vsync = Device.Settings.VSync ? 1U : 0;
            Device.Settings.VSync.OnChanged += VSync_OnChanged;
        }

        /* NOTE:
         *  Discard Mode = Only image index 0 can be accessed
         *  Sequential/FlipS-Sequential Modes = Only image index 0 can be accesed for writing. The rest can only be accesed for reading.
         *  
         *  This means we only need 1 handle for the swap chain, as the next image is always at index 0.
         */
        void* ppSurface = null;
        Guid riid = ID3D11Texture2D1.Guid;
        WinHResult hr = SwapChainHandle->GetBuffer(0, &riid, &ppSurface);
        DxgiError err = hr.ToEnum<DxgiError>();

        if (err == DxgiError.Ok)
        {
            SurfaceHandleDX11 handle = new SurfaceHandleDX11(this)
            {
                NativePtr = (ID3D11Resource*)ppSurface,
            };

            handle.RTV.Desc = new RenderTargetViewDesc1()
            {
                Format = _swapDesc.Format,
                ViewDimension = RtvDimension.Texture2D,
            };

            handle.RTV.Create();
            Viewport = new ViewportF(0, 0, Width, Height);
            return handle;
        }
        else
        {
            Device.Log.Error($"Error creating resource for SwapChainSurface '{Name}': {err}");
        }

        return null;
    }

    protected abstract void OnCreateSwapchain(ref Texture2DDesc1 desc);

    protected void CreateSwapChain(DisplayModeDXGI mode, bool windowed, IntPtr controlHandle)
    {
        IUnknown* deviceHandle = (IUnknown*)Device.Handle;
        GraphicsManagerDXGI dxgiManager = Device.Manager as GraphicsManagerDXGI;

        DxgiError result = dxgiManager.CreateSwapChain(mode, SwapEffect.FlipDiscard, Device.FrameBufferSize, Device.Log, deviceHandle, controlHandle, out SwapChainHandle);
        if (result == DxgiError.DeviceRemoved)
        {
            WinHResult hr = Device.Handle->GetDeviceRemovedReason();
            DxgiError dxgiReason = hr.ToEnum<DxgiError>();
            Device.Log.Error($"Device removed reason: {dxgiReason}");
        }
    }

    private void VSync_OnChanged(bool oldValue, bool newValue)
    {
        _vsync = newValue ? 1U : 0;
    }

    DxgiError _lastError;
    internal void Present()
    {
        Apply(Device.Queue);

        if (OnPresent() && SwapChainHandle != null)
        {
            // TODO implement partial-present - Partial Presentation (using scroll or dirty rects)
            // is not valid until first submitting a regular Present without scroll or dirty rects.
            // Otherwise, the preserved back-buffer data would be uninitialized.

            // See for flags: https://learn.microsoft.com/en-us/windows/win32/direct3ddxgi/dxgi-present
            WinHResult r = SwapChainHandle->Present1(_vsync, 0U, _presentParams);
            DxgiError de = r.ToEnum<DxgiError>();

            if (de != DxgiError.Ok)
            {
                if (_lastError != de)
                {
                    Device.Log.Error($"Creation of swapchain failed with result: {de}");
                    _lastError = de;
                }
            }
        }

        if (!IsDisposed)
        {
            while (_dispatchQueue.TryDequeue(out Action action))
                action();
        }
    }

    public void Dispatch(Action action)
    {
        _dispatchQueue.Enqueue(action);
    }

    protected override void OnGraphicsRelease()
    {
        NativeUtil.ReleasePtr(ref SwapChainHandle);
        EngineUtil.Free(ref _presentParams);
        base.OnGraphicsRelease();
    }

    protected abstract bool OnPresent();

    /// <inheritdoc/>
    public bool IsEnabled { get; set; }
}
