using InnoBase;
using InnoInternal.ImGui.Impl;

namespace InnoEditor.Gizmo;

public class GridGizmo : EditorGizmo
{
    public float width;
    public float height;
    public Vector2 offset = Vector2.ZERO;
    public int spacing = 100;
    public Color color = Color.GRAY;
    public float lineThickness = 1f;
    
    public bool showNumber = true;
    public Vector2 topLeftNumber = Vector2.ZERO;
    public Vector2 numberIncrement = Vector2.ONE;
    
    internal override void Draw(IImGuiContext context)
    {
        if (!isVisible || spacing <= 0f) return;
        
        Vector2 windowPos = context.GetWindowPosition();
        Vector2 screenPos = context.GetCursorStartPos();

        Vector2 topLeftBounds = windowPos + screenPos;
        Vector2 bottomRightBounds = topLeftBounds + new Vector2(width, height); // Bottom-right bounds
        Vector2 axisTopLeft = topLeftBounds + offset;                           // Top-left
        
        for (float i = axisTopLeft.x; i < bottomRightBounds.x; i += spacing)
        {
            context.DrawLine(
                new Vector2(i, topLeftBounds.y),
                new Vector2(i, bottomRightBounds.y),
                color,
                lineThickness);
        }
        
        for (float j = axisTopLeft.y; j < bottomRightBounds.y; j += spacing)
        {
            context.DrawLine(
                new Vector2(topLeftBounds.x, j),
                new Vector2(bottomRightBounds.x, j),
                color,
                lineThickness);
        }
        
    }
}