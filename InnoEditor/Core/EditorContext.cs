using InnoEngine.ECS;

namespace InnoEditor.Core;
public class EditorContext
{
    public static EditorContext current { get; } = new();

    public GameObject? selectedGameObject { get; set; }
    public EditorMode mode { get; set; } = EditorMode.Edit;

    // Hovered 组件等也可加入
}