
using InnoInternal.ImGui.Impl;
using InnoInternal.Shell.Impl;

namespace Sandbox;

internal class ImGuiTest
{
    public void Run()
    {
        var shell = IGameShell.CreateShell(IGameShell.ShellType.Veldrid);
        shell.SetWindowSize(1280, 720);
        shell.SetWindowResizable(true);
        
        var imguiRenderer = IImGuiRenderer.CreateRenderer(IImGuiRenderer.ImGuiRendererType.Veldrid);
        
        shell.SetOnResize((width, height) =>
        {
            imguiRenderer.OnWindowResize(width, height);
        });

        shell.SetOnLoad(() =>
        {
            imguiRenderer.Initialize(shell.GetGraphicsDevice(), shell.GetWindowHolder());
        });
        
        shell.SetOnDraw(deltaTime =>
        {
            imguiRenderer.BeginLayout(deltaTime);
        
            // Demo GUI
            ImGuiNET.ImGui.ShowDemoWindow();
            
            imguiRenderer.EndLayout();
        });

        shell.Run();
    }
}