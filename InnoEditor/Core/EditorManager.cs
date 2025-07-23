using InnoEditor.Utility;
using InnoInternal.ImGui.Impl;
using InnoInternal.Render.Impl;

namespace InnoEditor.Core;

public static class EditorManager
{
    private static readonly Dictionary<string, EditorPanel> WINDOWS = new();
    private static readonly EditorSelection SELECTION = new();
    
    // Manager Properties
    public static EditorSelection selection => SELECTION;

    public static void RegisterWindow(EditorPanel panel)
    {
        if (!WINDOWS.ContainsKey(panel.title))
            WINDOWS.Add(panel.title, panel);
        else throw new Exception("Window already registered");
    }
    
    internal static void DrawPanels(IImGuiContext context, IRenderAPI renderAPI)
    {
        foreach (var window in WINDOWS.Values)
        {
            if (!window.isOpen) continue;

            context.BeginWindow(window.title);
            window.OnGUI(context, renderAPI);
            context.EndWindow();
        }
    }
}