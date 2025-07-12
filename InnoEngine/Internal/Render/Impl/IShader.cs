using System.Runtime.InteropServices;

namespace InnoEngine.Internal.Render.Impl;

internal interface IShader
{
    void SetTexture(string name, ITexture2D texture);
    void Apply();
}