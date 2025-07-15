using System;
using InnoInternal.ImGui.Impl;
using InnoInternal.ImGui.Bridge;
using InnoEngine.Core;
using ImGuiNET;

namespace InnoEditor;

public class MyEditorApp : EditorCore
{
    private IImGuiRenderer _imguiRenderer;
    private IImGuiContext _imguiContext;

    private bool _showDemoWindow = true;
    private bool _showAnotherWindow = false;
    private bool _checkboxValue = false;

    public override void SetUp()
    {
        _imguiRenderer = new ImGuiNETMonoGameRenderer();
        _imguiContext = _imguiRenderer.context;
        
        _imguiRenderer.Initialize();
    }

    public override void OnEditorUpdate(float totalTime, float deltaTime)
    {
        // 这里写编辑器每帧逻辑，比如输入处理等
    }

    public override void OnEditorGUI(float deltaTime)
    {
        _imguiRenderer.BeginFrame();

        // 1. DockSpace，支持拖拽停靠
        ImGui.DockSpaceOverViewport(ImGui.GetMainViewport().ID);

        // 2. 显示官方Demo窗口，方便调试和测试
        if (_showDemoWindow)
        {
            ImGui.ShowDemoWindow(ref _showDemoWindow);
        }

        // 3. 自定义窗口
        _imguiContext.BeginWindow("My Editor Window");
        _imguiContext.Text("Welcome to your editor!");
        if (_imguiContext.Button("Click me"))
        {
            Console.WriteLine("Button clicked!");
            _showAnotherWindow = !_showAnotherWindow;
        }
        _imguiContext.Checkbox("Toggle Demo Window", ref _showDemoWindow);
        _imguiContext.Checkbox("Toggle Another Window", ref _showAnotherWindow);
        _imguiContext.EndWindow();

        // 4. 另一个窗口，根据按钮切换显示
        if (_showAnotherWindow)
        {
            _imguiContext.BeginWindow("Another Window");
            _imguiContext.Text("Hello from another window!");
            _imguiContext.Checkbox("Check me", ref _checkboxValue);
            _imguiContext.EndWindow();
        }

        _imguiRenderer.EndFrame();

        // 渲染 ImGui 绘制数据
        _imguiRenderer.RenderDrawData(ImGui.GetDrawData());
    }
}
