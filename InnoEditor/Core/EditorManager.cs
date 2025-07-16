using InnoInternal.ImGui.Impl;
using InnoInternal.Render.Impl;

namespace InnoEditor.Core;

public static class EditorManager
{
    public delegate void MenuItemAction();

    private static readonly Dictionary<string, EditorWindow> WINDOWS = new();
    private static readonly Dictionary<string, MenuItemAction> MENU_ITEMS = new();

    public static void RegisterWindow(EditorWindow window)
    {
        if (!WINDOWS.ContainsKey(window.Title))
            WINDOWS.Add(window.Title, window);
    }
    
    public static void RegisterMenuItem(string key, EditorMenuItem menuItem)
    {
        if (!MENU_ITEMS.ContainsKey(key))
            MENU_ITEMS.Add(key, menuItem.Action);
    }
    
    internal static void DrawWindow(IImGuiContext context, IRenderAPI renderAPI)
    {
        foreach (var window in WINDOWS.Values)
        {
            if (!window.IsOpen) continue;

            context.BeginWindow(window.Title);
            window.OnGUI(context, renderAPI);
            context.EndWindow();
        }
    }

    internal static void DrawMenuBar(IImGuiContext context)
    {
        if (context.BeginMainMenuBar())
        {
            foreach (var kvp in MENU_ITEMS)
            {
                if (context.BeginMenu(kvp.Key))
                {
                    kvp.Value?.Invoke();
                    context.EndMenu();
                }
            }
            context.EndMainMenuBar();
        }
    }
}