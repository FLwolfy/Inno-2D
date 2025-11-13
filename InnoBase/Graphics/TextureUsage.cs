namespace InnoBase.Graphics;

[Flags]
public enum TextureUsage : byte
{
    // NOTE: This is copied from Veldrid.TextureUsage.
    //       Values must match exactly.
    Sampled = 1 << 0,
    Storage = 1 << 1,
    RenderTarget = 1 << 2,
    DepthStencil = 1 << 3,
    Cubemap = 1 << 4,
    Staging = 1 << 5,
    GenerateMipmaps = 1 << 6,
}