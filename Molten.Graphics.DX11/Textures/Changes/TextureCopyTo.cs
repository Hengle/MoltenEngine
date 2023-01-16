﻿namespace Molten.Graphics
{
    internal unsafe struct TextureCopyTo : ITextureTask
    {
        public TextureBase Destination;

        public bool Process(CommandQueueDX11 pipe, TextureBase texture)
        {
            if (Destination.HasFlags(TextureFlags.Dynamic))
                throw new TextureCopyException(texture, Destination, "Cannot copy to a dynamic texture via GPU. GPU cannot write to dynamic textures.");

            // Validate dimensions.
            if (Destination.Width != texture.Width ||
                Destination.Height != texture.Height ||
                Destination.Depth != texture.Depth)
                throw new TextureCopyException(texture, Destination, "The source and destination textures must have the same dimensions.");

            pipe.Native->CopyResource(Destination.NativePtr, texture.NativePtr);

            return Destination == texture;
        }
    }
}