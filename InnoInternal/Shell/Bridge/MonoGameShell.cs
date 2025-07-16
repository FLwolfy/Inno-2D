using InnoInternal.Shell.Impl;
using Microsoft.Xna.Framework;

namespace InnoInternal.Shell.Bridge;

internal sealed class MonoGameShell : Game, IGameShell
{
    private readonly GraphicsDeviceManager m_graphics;

    private Action? m_onLoad;
    private Action? m_onSetup;
    private Action<float, float>? m_onStep;
    private Action<float>? m_onDraw;
    private Action? m_onClose;
    private Action<int, int>? m_onWindowSizeChanged;

    internal MonoGameShell()
    {
        m_graphics = new GraphicsDeviceManager(this);
        IsMouseVisible = true;
    }
    
    protected override void Initialize()
    {
        base.Initialize();

        Window.ClientSizeChanged += OnClientSizeChanged;
    }
    
    private void OnClientSizeChanged(object? sender, EventArgs e)
    {
        int width = Window.ClientBounds.Width;
        int height = Window.ClientBounds.Height;

        m_graphics.PreferredBackBufferWidth = width;
        m_graphics.PreferredBackBufferHeight = height;
        m_graphics.ApplyChanges();

        m_onWindowSizeChanged?.Invoke(width, height);
    }

    protected override void LoadContent()
    {
        m_onLoad?.Invoke();
        m_onSetup?.Invoke();
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
    public void SetOnSetup(Action callback) => m_onSetup = callback;
    public void SetOnStep(Action<float, float> callback) => m_onStep = callback;
    public void SetOnDraw(Action<float> callback) => m_onDraw = callback;
    public void SetOnClose(Action callback) => m_onClose = callback;
    public void SetWindowResizable(bool enable) => Window.AllowUserResizing = enable;
    public void SetOnWindowSizeChanged(Action<int, int> callback) => m_onWindowSizeChanged = callback;

    public object GetGraphicsDevice()
    {
        return GraphicsDevice;
    }

    public object GetWindowHolder()
    {
        return this;
    }
}