﻿using Silk.NET.Direct3D11;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Molten.Graphics
{
    internal unsafe class ShaderHullStage : PipeShaderStage<ID3D11HullShader>
    {
        public ShaderHullStage(DeviceContext pipe) :
            base(pipe, ShaderType.HullShader)
        {

        }

        protected override void OnUnbindConstBuffer(PipeSlot<ShaderConstantBuffer> slot)
        {
            Pipe.NativeContext->HSSetConstantBuffers(slot.Index, 1, null);
        }

        protected override void OnUnbindResource(PipeSlot<PipeBindableResource> slot)
        {
            Pipe.NativeContext->HSSetShaderResources(slot.Index, 1, null);
        }

        protected override void OnUnbindSampler(PipeSlot<ShaderSampler> slot)
        {
            Pipe.NativeContext->HSSetSamplers(slot.Index, 1, null);
        }

        protected override void OnUnbindShaderComposition(PipeSlot<ShaderComposition<ID3D11HullShader>> slot)
        {
            Pipe.NativeContext->HSSetShader(null, null, 0);
        }

        protected override unsafe void OnBindConstants(PipeSlotGroup<ShaderConstantBuffer> grp,
            ID3D11Buffer** buffers)
        {
            Pipe.NativeContext->HSSetConstantBuffers(grp.FirstChanged, grp.NumSlotsChanged, buffers);
        }

        protected override unsafe void OnBindResources(PipeSlotGroup<PipeBindableResource> grp,
            ID3D11ShaderResourceView** srvs)
        {
            Pipe.NativeContext->HSSetShaderResources(grp.FirstChanged, grp.NumSlotsChanged, srvs);
        }

        protected override unsafe void OnBindSamplers(PipeSlotGroup<ShaderSampler> grp, ID3D11SamplerState** resources)
        {
            Pipe.NativeContext->HSSetSamplers(grp.FirstChanged, grp.NumSlotsChanged, resources);
        }

        protected override unsafe void OnBindShader(PipeSlot<ShaderComposition<ID3D11HullShader>> slot)
        {
            if (slot.BoundValue != null)
                Pipe.NativeContext->HSSetShader(slot.BoundValue.PtrShader, null, 0);
            else
                Pipe.NativeContext->HSSetShader(null, null, 0);
        }
    }
}
