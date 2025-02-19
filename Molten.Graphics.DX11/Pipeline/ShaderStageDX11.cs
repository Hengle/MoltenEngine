﻿using Silk.NET.Direct3D11;

namespace Molten.Graphics.DX11;

internal unsafe abstract class ShaderStageDX11
{
    GraphicsStateValueGroup<ConstantBufferDX11> _constantBuffers;
    GraphicsStateValueGroup<GraphicsResource> _resources;
    GraphicsStateBasicValueGroup<SamplerDX11> _samplers;
    GraphicsStateBasicValue<ShaderPassStage> _shader;

    internal ShaderStageDX11(GraphicsQueueDX11 queue, ShaderStageType type)
    {
        Cmd = queue;
        Type = type;

        GraphicsCapabilities cap = Cmd.Device.Capabilities;
        ShaderStageCapabilities shaderCap = cap[type];

        _samplers = new GraphicsStateBasicValueGroup<SamplerDX11>(cap.MaxShaderSamplers);
        _resources = new GraphicsStateValueGroup<GraphicsResource>(shaderCap.MaxInResources);
        _constantBuffers = new GraphicsStateValueGroup<ConstantBufferDX11>(cap.ConstantBuffers.MaxSlots);
        _shader = new GraphicsStateBasicValue<ShaderPassStage>();
    }

    internal bool Bind(ShaderPassStage passStage)
    {
        _shader.Value = passStage;
        bool shaderChanged = _shader.Bind();
        passStage = _shader.BoundValue;

        if (shaderChanged)
        {
            if (passStage != null)
                SetShader(passStage.PtrShader, null, 0);
            else
                SetShader(null, null, 0);
        }

        // Clear current bindings
        _samplers.Reset();
        _resources.Reset();
        _constantBuffers.Reset();

        if (passStage != null)
        {
            ShaderBind<ShaderSamplerVariable>[] samplers = passStage.Bindings.Samplers;
            ShaderBind<ShaderResourceVariable>[] resources = passStage.Bindings[ShaderBindType.Resource];
            ShaderBind<ShaderResourceVariable>[] constantBuffers = passStage.Bindings[ShaderBindType.ConstantBuffer];

            // Apply pass samplers to slots
            for(int i = 0; i < samplers.Length; i++)
            {
                ref ShaderBind<ShaderSamplerVariable> bind = ref samplers[i];
                _samplers[bind.Info.BindPoint] = bind.Object.Value as SamplerDX11;
            }

            // Apply pass resources to slots
            for(int i = 0; i < resources.Length; i++)
            {
                ref ShaderBind<ShaderResourceVariable> bind = ref resources[i];
                _resources[bind.Info.BindPoint] = bind.Object.Resource;
            }

            // Apply pass constant buffers to slots
            for(int i = 0; i < constantBuffers.Length; i++)
            {
                ref ShaderBind<ShaderResourceVariable> bind = ref constantBuffers[i];
                _constantBuffers[bind.Info.BindPoint] = bind.Object.Resource as ConstantBufferDX11;
            }
        }

        BindSamplers();
        BindResources();
        BindConstantBuffers();
        OnBind(passStage, shaderChanged);

        return shaderChanged;
    }

    protected virtual void OnBind(ShaderPassStage c, bool shaderChanged) { }

    private void BindSamplers()
    {
        if (!_samplers.Bind(Cmd))
            return;

        ID3D11SamplerState** samplers = stackalloc ID3D11SamplerState*[_samplers.Length];

        for (int i = 0; i < _samplers.Length; i++)
        {
            if (_samplers.BoundValues[i] != null)
                samplers[i] = _samplers.BoundValues[i].NativePtr;
            else
                samplers[i] = null;
        }

        SetSamplers((uint)_samplers.Length, samplers);
    }

    private void BindResources()
    {
        if(!_resources.Bind(Cmd))
            return;

        ID3D11ShaderResourceView1** res = stackalloc ID3D11ShaderResourceView1*[_resources.Length];
        for (int i = 0; i < _resources.Length; i++)
        {
            if (_resources.BoundValues[i] != null)
                res[i] = ((ResourceHandleDX11)_resources.BoundValues[i].Handle).SRV;
            else
                res[i] = null;
        }

        SetResources((uint)_resources.Length, res);
    }

    private void BindConstantBuffers()
    {
        if (!_constantBuffers.Bind(Cmd))
            return;

        int count = _constantBuffers.Length;
        ID3D11Buffer** cBuffers = stackalloc ID3D11Buffer*[count];
        uint* cFirstConstants = stackalloc uint[count];
        uint* cNumConstants = stackalloc uint[count];
        ConstantBufferDX11 cb = null;

        for (int i = 0; i < count; i++)
        {
            cb = _constantBuffers.BoundValues[i];

            if (cb != null)
            {
                cBuffers[i] = (ID3D11Buffer*)cb.Handle;
                cFirstConstants[i] = 0; // TODO implement this using BufferSegment
                cNumConstants[i] = (uint)cb.Variables.Length;
            }
            else
            {
                cBuffers[i] = null;
                cFirstConstants[i] = 0;
                cNumConstants[i] = 0;
            }
        }

        SetConstantBuffers((uint)count, cBuffers);
    }

    internal abstract void SetSamplers(uint numSamplers, ID3D11SamplerState** states);

    internal abstract void SetResources(uint numViews, ID3D11ShaderResourceView1** views);

    internal abstract void SetConstantBuffers(uint numBuffers, ID3D11Buffer** buffers);

    internal abstract void SetShader(void* shader, ID3D11ClassInstance** classInstances, uint numClassInstances);

    internal GraphicsQueueDX11 Cmd { get; }

    internal ShaderStageType Type { get; }
}
