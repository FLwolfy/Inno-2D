namespace InnoEngine.Graphics;

/// <summary>
/// 用于创建、缓存、回收 RenderTarget
/// </summary>
public static class RenderTargetManager
{
    private static readonly Dictionary<(int, int), Stack<MonoGameRenderTarget>> _pool = new();

    public static IRenderTarget GetTemporary(int width, int height)
    {
        var key = (width, height);
        if (_pool.TryGetValue(key, out var stack) && stack.Count > 0)
            return stack.Pop();

        return new MonoGameRenderTarget(width, height);
    }

    public static void ReleaseTemporary(IRenderTarget target)
    {
        if (target is not MonoGameRenderTarget rt) return;

        var key = (rt.RawTarget.Width, rt.RawTarget.Height);
        if (!_pool.ContainsKey(key))
            _pool[key] = new Stack<MonoGameRenderTarget>();

        _pool[key].Push(rt);
    }
}