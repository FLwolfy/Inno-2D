using Inno.Editor.Utility;
using Inno.Graphics;
using Inno.Platform.ImGui;

namespace Inno.Editor.Core;

public static class EditorManager
{
    private static readonly Dictionary<string, EditorPanel> PANELS = new();
    private static readonly EditorSelection SELECTION = new();
    
    // Manager Properties
    public static EditorSelection selection => SELECTION; // TODO: Make this more robust later

    public static void RegisterPanel(EditorPanel panel)
    {
        if (!PANELS.TryAdd(panel.title, panel))
            throw new Exception("Panel already registered");
    }
    
    internal static void DrawPanels(IImGuiContext imGuiContext, RenderContext renderContext)
    {
        foreach (var panel in PANELS.Values)
        {
            if (!panel.isOpen) continue;

            imGuiContext.BeginWindow(panel.title);
            panel.OnGUI(imGuiContext, renderContext);
            imGuiContext.EndWindow();
        }
    }
}