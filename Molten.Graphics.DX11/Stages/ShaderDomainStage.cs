﻿using Silk.NET.Direct3D11;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Molten.Graphics
{
    internal unsafe class ShaderDomainStage : PipeShaderStage<ID3D11DomainShader>
    {
        public ShaderDomainStage(DeviceContext pipe) :
            base(pipe, ShaderType.DomainShader)
        {

        }

        protected override void OnUnbindConstBuffer(PipeSlot<ShaderConstantBuffer> slot)
        {
            Pipe.Native->DSSetConstantBuffers(slot.Index, 1, null);
        }

        protected override void OnUnbindResource(PipeSlot<PipeBindableResource> slot)
        {
            Pipe.Native->DSSetShaderResources(slot.Index, 1, null);
        }

        protected override void OnUnbindSampler(PipeSlot<ShaderSampler> slot)
        {
            Pipe.Native->DSSetSamplers(slot.Index, 1, null);
        }

        protected override void OnUnbindShaderComposition(PipeSlot<ShaderComposition<ID3D11DomainShader>> slot)
        {
            Pipe.Native->DSSetShader(null, null, 0);
        }

        protected override unsafe void OnBindConstants(PipeSlotGroup<ShaderConstantBuffer> grp,
            ID3D11Buffer** buffers)
        {
            Pipe.Native->DSSetConstantBuffers(grp.FirstChanged, grp.NumSlotsChanged, buffers);
        }

        protected override unsafe void OnBindResources(PipeSlotGroup<PipeBindableResource> grp,
            ID3D11ShaderResourceView** srvs)
        {
            Pipe.Native->DSSetShaderResources(grp.FirstChanged, grp.NumSlotsChanged, srvs);
        }

        protected override unsafe void OnBindSamplers(PipeSlotGroup<ShaderSampler> grp, ID3D11SamplerState** resources)
        {
            Pipe.Native->DSSetSamplers(grp.FirstChanged, grp.NumSlotsChanged, resources);
        }

        protected override unsafe void OnBindShader(PipeSlot<ShaderComposition<ID3D11DomainShader>> slot)
        {
            if (slot.BoundValue != null)
                Pipe.Native->DSSetShader(slot.BoundValue.PtrShader, null, 0);
            else
                Pipe.Native->DSSetShader(null, null, 0);
        }
    }
}
