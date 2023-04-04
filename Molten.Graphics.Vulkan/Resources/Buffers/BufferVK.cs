﻿using Silk.NET.Vulkan;
using Buffer = Silk.NET.Vulkan.Buffer;

namespace Molten.Graphics
{
    internal unsafe abstract class BufferVK : GraphicsBuffer
    {
        BufferCreateInfo _desc;
        ResourceHandleVK* _handle;
        MemoryAllocationVK _memory;

        protected BufferVK(GraphicsDevice device,
            GraphicsBufferType type,
            GraphicsResourceFlags flags,
            BufferUsageFlags usageFlags,
            uint stride,
            uint numElements,
            void* initialData = null) :
            base(device, stride, numElements, flags, type)
        {
            _handle = EngineUtil.Alloc<ResourceHandleVK>();

            MemoryPropertyFlags memFlags = BuildDescription(usageFlags);
            InitializeBuffer(memFlags, initialData);
        }

        private MemoryPropertyFlags BuildDescription(BufferUsageFlags usage)
        {
            MemoryPropertyFlags memFlags = MemoryPropertyFlags.None;

            // Does the memory need to be host-visible?
            if(Flags.Has(GraphicsResourceFlags.CpuRead) || Flags.Has(GraphicsResourceFlags.CpuWrite))
                memFlags |= MemoryPropertyFlags.HostCoherentBit | MemoryPropertyFlags.HostVisibleBit;
            else
                memFlags |= MemoryPropertyFlags.DeviceLocalBit;

            if (Flags.Has(GraphicsResourceFlags.None))
                usage |= BufferUsageFlags.TransferSrcBit;

            if (Flags.Has(GraphicsResourceFlags.GpuWrite))
                usage |= BufferUsageFlags.TransferDstBit;

            _desc.SType = StructureType.BufferCreateInfo;
            _desc.Usage = usage;
            _desc.SharingMode = SharingMode.Exclusive;
            _desc.Flags = BufferCreateFlags.None;
            _desc.Size = Stride * ElementCount;
            _desc.PQueueFamilyIndices = EngineUtil.AllocArray<uint>(1);
            _desc.PQueueFamilyIndices[0] = (Device.Cmd as CommandQueueVK).Index;
            _desc.QueueFamilyIndexCount = 1;

            return memFlags;
        }

        private void InitializeBuffer(MemoryPropertyFlags memFlags, void* initialData)
        {
            _handle->Ptr = EngineUtil.Alloc<Buffer>();

            DeviceVK device = Device as DeviceVK;
            Result r = device.VK.CreateBuffer(device, in _desc, null, (Buffer*)_handle->Ptr);
            if (!r.Check(device))
                return;

            MemoryRequirements memRequirements;
            device.VK.GetBufferMemoryRequirements(device, *(Buffer*)_handle->Ptr, &memRequirements);
           _memory = device.Memory.Allocate(ref memRequirements, memFlags);
            if (_memory != null)
            {
                _handle->Memory = _memory;
                r = device.VK.BindBufferMemory(device, *(Buffer*)_handle->Ptr, _handle->Memory, 0);
                if (!r.Check(device))
                    return;
            }
            else
            {
                device.Log.Error($"Unable to allocate memory for buffer of size {_desc.Size} bytes.");
                return;
            }
        }

        public override void GraphicsRelease()
        {
            DeviceVK device = Device as DeviceVK;

            if (_handle->Ptr != null)
            {
                device.VK.DestroyBuffer(device, *(Buffer*)_handle->Ptr, null);
                device.VK.FreeMemory(device, _handle->Memory, null);

                EngineUtil.Free(ref _handle->Ptr);
                EngineUtil.Free(ref _handle);
                EngineUtil.Free(ref _desc.PQueueFamilyIndices);
            }
        }

        internal bool HasFlags(BufferUsageFlags flags)
        {
            return (_desc.Usage & flags) == flags;
        }

        protected override void OnApply(GraphicsCommandQueue cmd) { }

        public override unsafe void* Handle => _handle;

        public override unsafe void* UAV => throw new NotImplementedException();

        public override unsafe void* SRV => throw new NotImplementedException();

        public override GraphicsFormat ResourceFormat { get => throw new NotImplementedException(); protected set => throw new NotImplementedException(); }
    }
}
