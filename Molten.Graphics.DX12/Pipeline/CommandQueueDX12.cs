﻿using Silk.NET.Core.Native;
using Silk.NET.Direct3D12;

namespace Molten.Graphics.DX12;

public unsafe class CommandQueueDX12 : GraphicsQueue<DeviceDX12>
{
    CommandQueueDesc _desc;
    ID3D12CommandQueue* _ptr;
    CommandAllocatorDX12 _cmdAllocator;
    PipelineInputLayoutDX12 _inputLayout;
    List<PipelineInputLayoutDX12> _cachedLayouts = new List<PipelineInputLayoutDX12>();

    internal CommandQueueDX12(Logger log, DeviceDX12 device, DeviceBuilderDX12 builder, ref CommandQueueDesc desc) : 
        base(device)
    {
        _desc = desc;
        Log = log;
        Device = device;

        Initialize(builder);
    }

    private void Initialize(DeviceBuilderDX12 builder)
    {
        Guid cmdGuid = ID3D12CommandQueue.Guid;
        void* cmdQueue = null;

        DeviceDX12 device = Device; 
        HResult r = device.Ptr->CreateCommandQueue(_desc, &cmdGuid, &cmdQueue);
        if (!device.Log.CheckResult(r))
        {
            Log.Error($"Failed to initialize '{_desc.Type}' command queue");
            return;
        }
        else
        {
            Log.WriteLine($"Initialized '{_desc.Type}' command queue");
        }

        _ptr = (ID3D12CommandQueue*)cmdQueue;
    }

    protected override void GenerateMipMaps(GraphicsResource texture)
    {
        throw new NotImplementedException();
    }

    protected override void OnResetState()
    {
        throw new NotImplementedException();
    }

    public override void Execute(GraphicsCommandList list)
    {
        throw new NotImplementedException();
    }

    public override void Sync(GraphicsCommandListFlags flags)
    {
        throw new NotImplementedException();
    }

    public override void Begin(GraphicsCommandListFlags flags = GraphicsCommandListFlags.None)
    {
        base.Begin(flags);
    }

    public override GraphicsCommandList End()
    {
        return base.End();
    }

    public override GraphicsBindResult Draw(HlslShader shader, uint vertexCount, uint vertexStartIndex = 0)
    {
        throw new NotImplementedException();
    }

    public override GraphicsBindResult DrawInstanced(HlslShader shader, uint vertexCountPerInstance, uint instanceCount, uint vertexStartIndex = 0, uint instanceStartIndex = 0)
    {
        throw new NotImplementedException();
    }

    public override GraphicsBindResult DrawIndexed(HlslShader shader, uint indexCount, int vertexIndexOffset = 0, uint startIndex = 0)
    {
        throw new NotImplementedException();
    }

    public override GraphicsBindResult DrawIndexedInstanced(HlslShader shader, uint indexCountPerInstance, uint instanceCount, uint startIndex = 0, int vertexIndexOffset = 0, uint instanceStartIndex = 0)
    {
        throw new NotImplementedException();
    }

    public override void BeginEvent(string label)
    {
        throw new NotImplementedException();
    }

    public override void EndEvent()
    {
        throw new NotImplementedException();
    }

    public override void SetMarker(string label)
    {
        throw new NotImplementedException();
    }

    protected override GraphicsBindResult DoRenderPass(HlslPass pass, QueueValidationMode mode, Action callback)
    {
        throw new NotImplementedException();
    }

    protected override GraphicsBindResult DoComputePass(HlslPass pass)
    {
        throw new NotImplementedException();
    }

    protected override ResourceMap GetResourcePtr(GraphicsResource resource, uint subresource, GraphicsMapType mapType)
    {
        throw new NotImplementedException();
    }

    protected override void OnUnmapResource(GraphicsResource resource, uint subresource)
    {
        throw new NotImplementedException();
    }

    protected override unsafe void UpdateResource(GraphicsResource resource, uint subresource, ResourceRegion? region, void* ptrData, uint rowPitch, uint slicePitch)
    {
        throw new NotImplementedException();
    }

    protected override void CopyResource(GraphicsResource src, GraphicsResource dest)
    {
        throw new NotImplementedException();
    }

    public override unsafe void CopyResourceRegion(GraphicsResource source, uint srcSubresource, ResourceRegion? sourceRegion, GraphicsResource dest, uint destSubresource, Vector3UI destStart)
    {
        throw new NotImplementedException();
    }

    protected override void OnDispose(bool immediate)
    {
        NativeUtil.ReleasePtr(ref _ptr);
    }

    public override GraphicsBindResult Dispatch(HlslShader shader, Vector3UI groups)
    {
        throw new NotImplementedException();
    }

    protected override GraphicsBindResult CheckInstancing()
    {
        if (_inputLayout != null && _inputLayout.IsInstanced)
            return GraphicsBindResult.Successful;
        else
            return GraphicsBindResult.NonInstancedVertexLayout;
    }

    /// <summary>Retrieves or creates a usable input layout for the provided vertex buffers and sub-effect.</summary>
    /// <returns>An instance of InputLayout.</returns>
    private PipelineInputLayoutDX12 GetInputLayout(ShaderPassDX12 pass)
    {
        // Retrieve layout list or create new one if needed.
        foreach (PipelineInputLayoutDX12 l in _cachedLayouts)
        {
            if (l.IsMatch(Device.Log, State.VertexBuffers))
                return l;
        }

        PipelineInputLayoutDX12 input = new PipelineInputLayoutDX12(Device, State.VertexBuffers, pass);
        _cachedLayouts.Add(input);

        return input;
    }

    internal ID3D12CommandQueue* Ptr => _ptr;

    internal Logger Log { get; }

    protected override GraphicsCommandList Cmd
    {
        get => throw new NotImplementedException();
        set => throw new NotImplementedException();
    }

    internal new DeviceDX12 Device { get; }
}
