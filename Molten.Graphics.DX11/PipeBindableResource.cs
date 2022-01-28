﻿using Silk.NET.Direct3D11;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Molten.Graphics
{
    public unsafe abstract class PipeBindableResource : PipeBindable<ID3D11Resource>
    {
        /// <summary>Gets or sets the <see cref="ID3D11UnorderedAccessView"/> attached to the object.</summary>
        internal protected UAView UAV { get; }

        /// <summary>Gets the <see cref="ID3D11ShaderResourceView"/> attached to the object.</summary>
        internal protected SRView SRV { get; }

        internal PipeBindableResource(Device device) : 
            base(device)
        {
            SRV = new SRView(device);
            UAV = new UAView(device);
        }

        internal override void PipelineDispose()
        {
            UAV.Dispose();
            SRV.Dispose();
        }

        #region Implicit cast operators
        public static implicit operator ID3D11UnorderedAccessView*(PipeBindableResource resource)
        {
            return resource.UAV;
        }

        public static implicit operator ID3D11ShaderResourceView*(PipeBindableResource resource)
        {
            return resource.SRV;
        }
        #endregion
    }

    internal unsafe abstract class PipeBindableResource<T> : PipeBindableResource
        where T : unmanaged
    {
        internal PipeBindableResource(Device device) : 
            base(device)
        {
        }

        /// <summary>
        /// Gets the underlying resource pointer. This should be the same address as <see cref="PipeBindable{ID3D11Resource}.NativePtr"/>
        /// </summary>
        internal abstract T* ResourcePtr { get; }

        public static implicit operator T*(PipeBindableResource<T> resource)
        {
            return resource.ResourcePtr;
        }
    }
}
