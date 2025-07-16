using System;
using InnoInternal.ImGui.Impl;
using InnoInternal.ImGui.Bridge;
using InnoEngine.Core;
using ImGuiNET;

namespace InnoEditor;

public class MyEditorApp : EditorCore
{
    private readonly IImGuiRenderer m_imGuiRenderer = new ImGuiNETMonoGameRenderer();
    private IImGuiContext imGuiContext => m_imGuiRenderer.context;

    private bool m_showDemoWindow = true;
    private bool m_showAnotherWindow = false;
    private bool m_checkboxValue = false;

    protected override void Setup()
    {
        m_imGuiRenderer.Initialize(GetWindowHolder());
    }

    protected override void OnEditorUpdate(float totalTime, float deltaTime)
    {
        // 这里写编辑器每帧逻辑，比如输入处理等
    }

    protected override void OnEditorGUI(float deltaTime)
    {
        m_imGuiRenderer.BeginLayout(deltaTime);

        // 1. DockSpace，支持拖拽停靠
        ImGui.DockSpaceOverViewport(ImGui.GetMainViewport().ID);

        // 2. 显示官方Demo窗口，方便调试和测试
        if (m_showDemoWindow)
        {
            ImGui.ShowDemoWindow(ref m_showDemoWindow);
        }

        // 3. 自定义窗口
        imGuiContext.BeginWindow("My Editor Window");
        imGuiContext.Text("Welcome to your editor!");
        if (imGuiContext.Button("Click me"))
        {
            Console.WriteLine("Button clicked!");
            m_showAnotherWindow = !m_showAnotherWindow;
        }
        imGuiContext.Checkbox("Toggle Demo Window", ref m_showDemoWindow);
        imGuiContext.Checkbox("Toggle Another Window", ref m_showAnotherWindow);
        imGuiContext.EndWindow();

        // 4. 另一个窗口，根据按钮切换显示
        if (m_showAnotherWindow)
        {
            imGuiContext.BeginWindow("Another Window");
            imGuiContext.Text("Hello from another window!");
            imGuiContext.Checkbox("Check me", ref m_checkboxValue);
            imGuiContext.EndWindow();
        }

        m_imGuiRenderer.EndLayout();
    }
}
