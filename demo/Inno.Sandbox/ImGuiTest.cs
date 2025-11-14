using System.Numerics;
using ImGuiNET;
using InnoInternal.ImGui.Impl;
using InnoInternal.Shell.Impl;

namespace Inno.Sandbox;

internal class ImGuiTest
{
    private Vector3 testVect3;
    
    public void Run()
    {
        var shell = IGameShell.CreateShell(IGameShell.ShellType.Veldrid);
        shell.SetWindowSize(1280, 720);
        shell.SetWindowResizable(true);
        
        var imGuiRenderer = IImGuiRenderer.CreateRenderer(IImGuiRenderer.ImGuiRendererType.Veldrid);

        shell.SetOnLoad(() =>
        {
            imGuiRenderer.Initialize(shell.GetGraphicsDevice(), shell.GetWindow());
        });
        
        shell.SetOnDraw(deltaTime =>
        {
            imGuiRenderer.BeginLayout(deltaTime, shell.GetGraphicsDevice().swapchainFrameBuffer);
            
            // Simple Window (Cannot be docked onto the main window)
            {
                ImGui.Text("Simple Window");
            }
            
            // Normal Window (Can be docked onto the main window)
            ImGui.Begin("Normal Window");
            ImGui.InputFloat3("test Vec3", ref testVect3);
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