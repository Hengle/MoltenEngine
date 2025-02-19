﻿using Silk.NET.Direct3D11;

namespace Molten.Graphics.DX11;

internal class ShaderCSStage : ShaderStageDX11
{
    GraphicsStateValueGroup<GraphicsResource> _uavs;

    public ShaderCSStage(GraphicsQueueDX11 queue) : base(queue, ShaderStageType.Compute)
    {
        uint uavSlots = queue.Device.Capabilities.Compute.MaxUnorderedAccessSlots;
        _uavs = new GraphicsStateValueGroup<GraphicsResource>(uavSlots);
    }

    protected unsafe override void OnBind(ShaderPassStage passStage, bool shaderChanged)
    {
        _uavs.Reset();

        // Apply unordered acces views to slots
        if (passStage != null)
        {
            ref ShaderBind<ShaderResourceVariable>[] uavs = ref passStage.Bindings[ShaderBindType.UnorderedAccess];
            for (int j = 0; j < uavs.Length; j++)
            {
                ref ShaderBind<ShaderResourceVariable> bind = ref uavs[j];
                _uavs[bind.Info.BindPoint] = bind.Object.Resource;
            }

            if (_uavs.Bind(Cmd))
            {
                // Set unordered access resources
                int count = _uavs.Length;
                ID3D11UnorderedAccessView1** pUavs = stackalloc ID3D11UnorderedAccessView1*[count];
                uint* pInitialCounts = stackalloc uint[count];

                for (int i = 0; i < count; i++)
                {
                    if (_uavs.BoundValues[i] != null)
                        pUavs[i] = ((ResourceHandleDX11)_uavs.BoundValues[i].Handle).UAV;
                    else
                        pUavs[i] = null;

                    pInitialCounts[i] = 0; // TODO set initial counts. Research this more.
                }

                SetUnorderedAccessViews((uint)count, pUavs, pInitialCounts);
            }
        }
    }

    internal override unsafe void SetConstantBuffers(uint numBuffers, ID3D11Buffer** buffers)
    {
        Cmd.Ptr->CSSetConstantBuffers(0, numBuffers, buffers);
    }

    internal override unsafe void SetResources(uint numViews, ID3D11ShaderResourceView1** views)
    {
        Cmd.Ptr->CSSetShaderResources(0, numViews, (ID3D11ShaderResourceView**)views);
    }

    internal override unsafe void SetSamplers(uint numSamplers, ID3D11SamplerState** states)
    {
        Cmd.Ptr->CSSetSamplers(0, numSamplers, states);
    }

    internal override unsafe void SetShader(void* shader, ID3D11ClassInstance** classInstances, uint numClassInstances)
    {
        Cmd.Ptr->CSSetShader((ID3D11ComputeShader*)shader, classInstances, numClassInstances);
    }

    internal unsafe void SetUnorderedAccessViews(uint numUAVs, ID3D11UnorderedAccessView1** ppUnorderedAccessViews, uint* pUAVInitialCounts)
    {
        Cmd.Ptr->CSSetUnorderedAccessViews(0, numUAVs, (ID3D11UnorderedAccessView**)ppUnorderedAccessViews, pUAVInitialCounts);
    }

}
