using ImGuiNET;
using InnoInternal.ImGui.Impl;
using InnoInternal.Render.Impl;
using InnoInternal.Shell.Impl;

namespace Sandbox;

internal class ImGuiTest
{
    public void Run()
    {
        var shell = IGameShell.CreateShell(IGameShell.ShellType.Veldrid);
        shell.SetWindowSize(1280, 720);
        shell.SetWindowResizable(true);
        
        var imGuiRenderer = IImGuiRenderer.CreateRenderer(IImGuiRenderer.ImGuiRendererType.Veldrid);
        
        shell.SetOnResize((width, height) =>
        {
            imGuiRenderer.OnWindowResize(width, height);
        });

        shell.SetOnLoad(() =>
        {
            imGuiRenderer.Initialize(shell.GetGraphicsDevice(), shell.GetWindowHolder());
        });
        
        shell.SetOnDraw(deltaTime =>
        {
            imGuiRenderer.BeginLayout(deltaTime);
            
            // Simple Window (Cannot be docked onto the main window)
            {
                ImGui.Text("Simple Window");
            }
            
            // Normal Window (Can be docked onto the main window)
            ImGui.Begin("Normal Window");
            ImGui.Text("Hello from another window!");
            ImGui.End();
            
            // Demo Window
            ImGui.ShowDemoWindow();
            
            imGuiRenderer.EndLayout();
            
            // Swap Buffers
            shell.GetGraphicsDevice().SwapBuffers();
            imGuiRenderer.SwapExtraImGuiWindows();
        });

        shell.Run();
    }
}