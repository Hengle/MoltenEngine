﻿using Silk.NET.Core.Native;
using Silk.NET.Direct3D12;

namespace Molten.Graphics.DX12;

public unsafe class ShaderIOLayoutDX12 : ShaderIOLayout, IEquatable<ShaderIOLayoutDX12>
{
    internal InputElementDesc[] VertexElements { get; private set; }

    public ShaderIOLayoutDX12(uint elementCount) : base(elementCount) { }

    public ShaderIOLayoutDX12(ShaderCodeResult result, ShaderType sType, ShaderIOLayoutType type) :
        base(result, sType, type)
    { }

    protected override void Initialize(uint numVertexElements)
    {
        if (numVertexElements > 0)
            VertexElements = new InputElementDesc[numVertexElements];
    }

    protected override void BuildVertexElement(ShaderCodeResult result, ShaderIOLayoutType type, ShaderParameterInfo pInfo, GraphicsFormat format, int index)
    {
        // Elements is null if the IO is not for a vertex shader input.
        if (VertexElements == null)
            return;

        VertexElements[index] = new InputElementDesc()
        {
            SemanticName = (byte*)pInfo.SemanticNamePtr,
            SemanticIndex = pInfo.SemanticIndex,
            InputSlot = 0, // This does not need to be set. A shader has a single layout, 
            InstanceDataStepRate = 0, // This does not need to be set. The data is set via Context.DrawInstanced + vertex data/layout.
            AlignedByteOffset = 16 * pInfo.Register,
            InputSlotClass = InputClassification.PerVertexData,
            Format = format.ToApi(),
        };
    }

    public override bool Equals(object obj)
    {
        if(obj is ShaderIOLayoutDX12 layout)
            return Equals(layout);

        return false;
    }

    public bool Equals(ShaderIOLayoutDX12 other)
    {
        // If we're comparing the object to itself, return true.
        if(EOID == other.EOID)
            return true;

        if(VertexElements.Length != other.VertexElements.Length)
            return false;

        for (int i = 0; i < VertexElements.Length; i++)
        {
            ref InputElementDesc element = ref VertexElements[i];
            ref InputElementDesc otherElement = ref other.VertexElements[i];

            if (Metadata[i].Name != other.Metadata[i].Name)
                return false;

            if (element.SemanticIndex != otherElement.SemanticIndex
            || element.InputSlot != otherElement.InputSlot
            || element.InstanceDataStepRate != otherElement.InstanceDataStepRate
            || element.AlignedByteOffset != otherElement.AlignedByteOffset
            || element.InputSlotClass != otherElement.InputSlotClass
            || element.Format != otherElement.Format)
                return false;
        }

        return true;
    }

    protected override void OnDispose()
    {
        // Dispose of element string pointers, since they were statically-allocated by Silk.NET
        if (VertexElements != null)
        {
            for (uint i = 0; i < VertexElements.Length; i++)
                SilkMarshal.Free((nint)VertexElements[i].SemanticName);
        }
    }
}
