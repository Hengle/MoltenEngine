﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Molten.IO;

namespace Molten.Graphics
{
    public class InstancedMesh<V, I> : Mesh<V>
        where V : unmanaged, IVertexType
        where I : unmanaged, IVertexInstanceType
    {
        IVertexBuffer _instanceBuffer;
        uint _instanceCount;

        /// <summary>
        /// Creates a new instance of <see cref="InstancedMesh{V, I}"/>, with 16-bit indices.
        /// </summary>
        /// <param name="renderer"></param>
        /// <param name="mode"></param>
        /// <param name="maxVertices"></param>
        /// <param name="maxIndices"></param>
        /// <param name="maxInstances"></param>
        internal InstancedMesh(RenderService renderer, GraphicsResourceFlags mode, ushort maxVertices, uint maxIndices, uint maxInstances,
            V[] initialVertices = null,
            ushort[] initialIndices = null) : 
            base(renderer, mode, maxVertices, maxIndices, initialVertices, initialIndices)
        {
            MaxInstances = maxInstances;
            _instanceBuffer = Renderer.Device.CreateVertexBuffer<I>(GraphicsResourceFlags.CpuWrite | GraphicsResourceFlags.GpuRead | GraphicsResourceFlags.Discard, maxIndices, null);
        }

        /// <summary>
        /// Creates a new instance of <see cref="InstancedMesh{V, I}"/>, with 32-bit indices.
        /// </summary>
        /// <param name="renderer"></param>
        /// <param name="mode"></param>
        /// <param name="maxVertices"></param>
        /// <param name="maxIndices"></param>
        /// <param name="maxInstances"></param>
        internal InstancedMesh(RenderService renderer, GraphicsResourceFlags mode, uint maxVertices, uint maxIndices, uint maxInstances,
            V[] initialVertices = null,
            uint[] initialIndices = null) :
            base(renderer, mode, maxVertices, maxIndices, initialVertices, initialIndices)
        {
            MaxInstances = maxInstances;
            _instanceBuffer = Renderer.Device.CreateVertexBuffer<I>(GraphicsResourceFlags.CpuWrite | GraphicsResourceFlags.GpuRead | GraphicsResourceFlags.Discard, maxInstances, null);
        }

        public void SetInstanceData(I[] data)
        {
            SetInstanceData(data, 0, (uint)data.Length);
        }

        public void SetInstanceData(I[] data, uint count)
        {
            SetInstanceData(data, 0, count);
        }

        public void SetInstanceData(I[] data, uint startIndex, uint count)
        {
            _instanceCount = count;
            _instanceBuffer.SetData(GraphicsPriority.Apply, data, startIndex, count, 0, Renderer.StagingBuffer); // Staging buffer will be ignored if the mesh is dynamic.
        }

        protected override void OnApply(GraphicsCommandQueue cmd)
        {
            base.OnApply(cmd);
            cmd.VertexBuffers[1].Value = _instanceBuffer;
        }

        protected override void OnPostDraw(GraphicsCommandQueue cmd)
        {
            base.OnPostDraw(cmd);
            cmd.VertexBuffers[1].Value = null;
        }

        protected override void OnDraw(GraphicsCommandQueue cmd)
        {
            if (MaxIndices > 0)
                cmd.DrawIndexedInstanced(Shader, IndexCount, _instanceCount);
            else
                cmd.DrawInstanced(Shader, VertexCount, _instanceCount, 0, 0);
        }

        protected override bool OnBatchRender(GraphicsCommandQueue cmd, RenderService renderer, RenderCamera camera, RenderDataBatch batch)
        {
            _instanceCount = (uint)batch.Data.Count;

            if (_instanceCount == 0 || Shader == null)
                return true;

            if (I.IsBatched)
            {
                // TODO Properly handle batches that are larger than the instance buffer.

                uint start = 0;
                uint byteOffset = 0;

                _instanceBuffer.GetStream(GraphicsPriority.Immediate,
                    (buffer, stream) =>
                    {
                        stream.Position += byteOffset;
                        for (int i = (int)start; i < batch.Data.Count; i++)
                            I.WriteBatchData(stream, batch.Data[i]);
                    },
                    Renderer.StagingBuffer);
            }

            Shader.Scene.ViewProjection.Value = camera.ViewProjection;

            OnApply(cmd);
            ApplyResources(Shader);
            OnDraw(cmd);
            OnPostDraw(cmd);
            
            return true;
        }

        public override void Dispose()
        {
            base.Dispose();
            _instanceBuffer.Dispose();
        }

        public uint MaxInstances { get; }

        public uint InstanceCount => _instanceCount;
    }
}
