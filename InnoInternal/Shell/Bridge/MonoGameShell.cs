using InnoInternal.Shell.Impl;
using Microsoft.Xna.Framework;

namespace InnoInternal.Shell.Bridge;

internal sealed class MonoGameShell : Game, IGameShell
{
    private readonly GraphicsDeviceManager m_graphics;

    private Action? m_onLoad;
    private Action? m_onSetUp;
    private Action<float, float>? m_onStep;
    private Action<float>? m_onDraw;
    private Action? m_onClose;

    internal MonoGameShell()
    {
        m_graphics = new GraphicsDeviceManager(this);
        IsMouseVisible = true;
    }

    protected override void LoadContent()
    {
        m_onLoad?.Invoke();
        m_onSetUp?.Invoke();
    }

    protected override void Update(GameTime gameTime)
    {
        float total = (float)gameTime.TotalGameTime.TotalSeconds;
        float delta = (float)gameTime.ElapsedGameTime.TotalSeconds;

        m_onStep?.Invoke(total, delta);

        base.Update(gameTime);
    }

    protected override void Draw(GameTime gameTime)
    {
        float delta = (float)gameTime.ElapsedGameTime.TotalSeconds;

        GraphicsDevice.Clear(Microsoft.Xna.Framework.Color.Gray);

        m_onDraw?.Invoke(delta);

        base.Draw(gameTime);
    }

    protected override void OnExiting(object sender, ExitingEventArgs args)
    {
        m_onClose?.Invoke();
        base.OnExiting(sender, args);
    }

    public void SetOnLoad(Action callback) => m_onLoad = callback;
    public void SetOnSetUp(Action callback) => m_onSetUp = callback;
    public void SetOnStep(Action<float, float> callback) => m_onStep = callback;
    public void SetOnDraw(Action<float> callback) => m_onDraw = callback;
    public void SetOnClose(Action callback) => m_onClose = callback;
    public object GetShellData()
    {
        return GraphicsDevice;
    }
}