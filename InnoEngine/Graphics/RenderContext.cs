using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace InnoEngine.Graphics;

/// <summary>
/// Shared data between render passes.
/// </summary>
public class RenderContext
{
    private readonly Dictionary<string, RenderTarget2D> m_renderTargets = new();
    
    public GraphicsDevice graphicsDevice = null!;
    public SpriteBatch spriteBatch = null!;
    public GameTime gameTime = null!;
    // TODO: Add Camera to RenderContext for culling
    // public Camera Camera;
    
    /// <summary>
    /// Reset the render context for new rendering.
    /// </summary>
    public void Reset(GraphicsDevice gd, SpriteBatch sb, GameTime time)
    {
        graphicsDevice = gd;
        spriteBatch = sb;
        gameTime = time;
        
        m_renderTargets.Clear();
    }
    
    /// <summary>
    /// Set a render target for the current render context.
    /// </summary>
    public void SetTarget(string name, RenderTarget2D rt) => m_renderTargets[name] = rt;
    
    /// <summary>
    /// Get the render target through a string name.
    /// </summary>
    public RenderTarget2D? GetTarget(string name) => m_renderTargets.GetValueOrDefault(name);
}