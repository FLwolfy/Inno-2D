using InnoBase;
using InnoInternal.ImGui.Impl;

namespace InnoEditor.Gizmo;

public class GridGizmo : EditorGizmo
{
    public Vector2 startPos = Vector2.ZERO;
    public Vector2 size = Vector2.ZERO;
    public Vector2 offset = Vector2.ZERO;
    public Color color = Color.GRAY;
    public float spacing = 100;
    public float lineThickness = 1f;
    
    public bool showNumber = true;
    public Vector2 topLeftNumber = Vector2.ZERO;
    public Vector2 numberIncrement = Vector2.ONE;
    
    internal override void Draw(IImGuiContext context)
    {
        if (!isVisible || spacing <= 0f) return;
        
        Vector2 windowPos = context.GetWindowPosition();
        Vector2 screenPos = context.GetCursorStartPos();
        
        Vector2 bottomRightBounds = startPos + size; // Bottom-right bounds
        Vector2 axisTopLeft = startPos + offset;     // Top-left
        
        for (float i = axisTopLeft.x; i < bottomRightBounds.x; i += spacing)
        {
            context.DrawLine(
                new Vector2(i, startPos.y),
                new Vector2(i, bottomRightBounds.y),
                color,
                lineThickness);
        }
        
        for (float j = axisTopLeft.y; j < bottomRightBounds.y; j += spacing)
        {
            context.DrawLine(
                new Vector2(startPos.x, j),
                new Vector2(bottomRightBounds.x, j),
                color,
                lineThickness);
        }
        
    }
}