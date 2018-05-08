﻿using SharpDX.Direct3D;
using SharpDX.Direct3D11;
using SharpDX.DXGI;
using Molten.Collections;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

namespace Molten.Graphics
{
    using Device = SharpDX.Direct3D11.Device;

    /// <summary>A Direct3D 11 graphics device.</summary>
    /// <seealso cref="Molten.Graphics.GraphicsPipe" />
    internal class GraphicsDeviceDX11 : GraphicsPipe
    {
        Device _d3d;
        GraphicsAdapterDX<Adapter1, AdapterDescription1, Output1> _adapter;
        List<SwapChainSurface> _swapChains;

        GraphicsPipe[] _pipes;
        int[] _freePipes;
        int _freePipeCount;
        int _pipeCount;

        Logger _log;
        VertexFormatBuilder _vertexBuilder;
        DisplayManagerDX11 _displayManager;
        GraphicsSettings _settings;
        long _allocatedVRAM;

        RasterizerStateBank _rasterizerBank;
        BlendStateBank _blendBank;
        DepthStateBank _depthBank;
        SamplerBank _samplerBank;

        ObjectPool<BufferSegment> _bufferSegmentPool;
        ThreadedQueue<PipelineObject> _objectsToDispose;

        /// <summary>The adapter to initially bind the graphics device to. Can be changed later.</summary>
        /// <param name="adapter">The adapter.</param>
        internal GraphicsDeviceDX11(Logger log, GraphicsSettings settings, RenderProfiler profiler, DisplayManagerDX11 manager, bool enableDebugLayer)
        {
            _log = log;
            _displayManager = manager;
            _adapter = _displayManager.SelectedAdapter as GraphicsAdapterDX<Adapter1, AdapterDescription1, Output1>;
            _pipes = new GraphicsPipe[0];
            _freePipes = new int[0];
            _swapChains = new List<SwapChainSurface>();
            _vertexBuilder = new VertexFormatBuilder();
            _settings = settings;
            _bufferSegmentPool = new ObjectPool<BufferSegment>(() => new BufferSegment(this));
            _objectsToDispose = new ThreadedQueue<PipelineObject>();

            DeviceCreationFlags flags = DeviceCreationFlags.BgraSupport;

            if (enableDebugLayer)
            {
                _log.WriteLine("Renderer debug layer enabled");
                flags |= DeviceCreationFlags.Debug;
            }

            using (var defaultDevice = new Device(_adapter.Adapter, flags, FeatureLevel.Level_11_0))
                _d3d = defaultDevice.QueryInterface<Device>();

            Features = new GraphicsDX11Features(_d3d);
            _rasterizerBank = new RasterizerStateBank(this);
            _blendBank = new BlendStateBank(this);
            _depthBank = new DepthStateBank(this);
            _samplerBank = new SamplerBank(this);

            Initialize(_log, this, _d3d.ImmediateContext, 0);
        }

        internal BufferSegment GetBufferSegment()
        {
            return _bufferSegmentPool.GetInstance();
        }

        internal void RecycleBufferSegment(BufferSegment segment)
        {
            _bufferSegmentPool.Recycle(segment);
        }

        internal void MarkForDisposal(PipelineObject pObject)
        {
            _objectsToDispose.Enqueue(pObject);
        }

        internal void DisposeMarkedObjects()
        {
            while (_objectsToDispose.TryDequeue(out PipelineObject obj))
                obj.PipelineDispose();
        }

        /// <summary>Track a VRAM allocation.</summary>
        /// <param name="bytes">The number of bytes that were allocated.</param>
        internal void AllocateVRAM(long bytes)
        {
            Interlocked.Add(ref _allocatedVRAM, bytes);
        }

        /// <summary>Track a VRAM deallocation.</summary>
        /// <param name="bytes">The number of bytes that were deallocated.</param>
        internal void DeallocateVRAM(long bytes)
        {
            Interlocked.Add(ref _allocatedVRAM, -bytes);
        }

        /// <summary>Gets a new deferred <see cref="GraphicsPipe"/>.</summary>
        /// <returns></returns>
        internal GraphicsPipe GetDeferredPipe()
        {
            int id = 0;
            if (_freePipeCount > 0)
                id = _freePipes[--_freePipeCount];
            else
            {
                id = _pipeCount++;
                Array.Resize(ref _pipes, _pipes.Length + 1);
            }

            GraphicsPipe pipe = new GraphicsPipe();
            pipe.Initialize(_log, this, new DeviceContext(_d3d), id);
            _pipes[id] = pipe;
            return pipe;
        }

        internal void RemoveDeferredPipe(GraphicsPipe pipe)
        {
            if(pipe == this)
                throw new GraphicsContextException("Cannot remove the graphics device from itself.");

            if (pipe.Device != this)
                throw new GraphicsContextException("Graphics pipe is owned by another device.");

            if (!pipe.IsDisposed)
                pipe.Dispose();

            int freeID = _freePipeCount++;
            if (_freePipeCount >= _freePipes.Length)
                Array.Resize(ref _freePipes, _freePipes.Length + 1);

            _freePipes[freeID] = pipe.ID;
            _pipes[pipe.ID] = null;
        }

        internal void SubmitContext(GraphicsPipe context)
        {
            if (context.Type != GraphicsContextType.Deferred)
                throw new Exception("Cannot submit immediate graphics contexts, only deferred.");

            // TODO take the underlying DX context from the GraphicsContext and give it a new/recycled one to work with.
            // TODO add the context's profiler stats to the device's main profiler.
        }

        /// <summary>Disposes of the <see cref="GraphicsDeviceDX11"/> and any deferred contexts and resources bound to it.</summary>
        protected override void OnDispose()
        {
            for (int i = 0; i < _pipes.Length; i++)
                _pipes[i]?.Dispose();

            // TODO dispose of all bound IGraphicsResource

            DisposeObject(ref _rasterizerBank);
            DisposeObject(ref _blendBank);
            DisposeObject(ref _depthBank);
            DisposeObject(ref _samplerBank);
            DisposeObject(ref _d3d);

            _bufferSegmentPool.Dispose();
            DisposeMarkedObjects();

            base.OnDispose();
        }

        /// <summary>Gets the underlying D3D device.</summary>
        internal Device D3d => _d3d;

        internal GraphicsPipe[] ActivePipes => _pipes;

        /// <summary>Gets an instance of <see cref="GraphicsDX11Features"/> which provides access to feature support details for the current graphics device.</summary>
        internal GraphicsDX11Features Features { get; private set; }

        internal DisplayManagerDX11 DisplayManager => _displayManager;

        internal GraphicsSettings Settings => _settings;

        /// <summary>Gets or sets the default render surface. This is the surface that <see cref="GraphicsPipe"/> instances revert to
        /// when a render surface is set to null.</summary>
        internal RenderSurfaceBase DefaultSurface { get; set; }

        internal VertexFormatBuilder VertexBuilder => _vertexBuilder;

        internal long AllocatedVRAM => _allocatedVRAM;

        /// <summary>
        /// Gets the device's blend state bank.
        /// </summary>
        public BlendStateBank BlendBank => _blendBank;

        /// <summary>
        /// Gets the device's rasterizer state bank.
        /// </summary>
        public RasterizerStateBank RasterizerBank => _rasterizerBank;

        /// <summary>
        /// Gets the device's depth-stencil state bank.
        /// </summary>
        public DepthStateBank DepthBank => _depthBank;

        /// <summary>
        /// Gets the device's texture sampler bank.
        /// </summary>
        public SamplerBank SamplerBank => _samplerBank;
    }
}
