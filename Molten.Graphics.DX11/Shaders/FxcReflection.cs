﻿using Silk.NET.Direct3D11;

namespace Molten.Graphics.DX11;

public unsafe class FxcReflection : IDisposable
{
    internal ID3D11ShaderReflection* Ptr;

    internal ShaderDesc Desc;

    internal FxcReflection(ID3D11ShaderReflection* reflection)
    {
        Ptr = reflection;
        Ptr->GetDesc(ref Desc);
    }

    public void Dispose()
    {
        NativeUtil.ReleasePtr(ref Ptr);
    }
}
