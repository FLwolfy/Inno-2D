using InnoBase;
using InnoInternal.ImGui.Impl;

namespace InnoEditor.GUI;

/// <summary>
/// EditorLayout wrapper based on IImGuiContext,
/// supports flexible layouts and common UI widgets
/// </summary>
public static class EditorGUILayout
{
    private static readonly Stack<LayoutType> LAYOUT_STACK = new();
    private static readonly Stack<LayoutAlign> ALIGN_STACK = new();

    private static IImGuiContext m_context = null!;

    internal static void Initialize(IImGuiContext context)
    {
        m_context = context;
    }

    /// <summary>
    /// Begin a layout group, with optional vertical/horizontal layout and alignment
    /// </summary>
    public static void Begin(LayoutType layout = LayoutType.Vertical, LayoutAlign align = LayoutAlign.Left)
    {
        if (m_context == null) throw new InvalidOperationException("EditorLayout.Context is not set");

        LAYOUT_STACK.Push(layout);
        ALIGN_STACK.Push(align);

        m_context.BeginGroup();
    }

    /// <summary>
    /// End the current layout group; must be paired with Begin
    /// </summary>
    public static void End()
    {
        if (LAYOUT_STACK.Count == 0 || ALIGN_STACK.Count == 0)
            throw new InvalidOperationException("EditorLayout.End called without matching Begin");

        LAYOUT_STACK.Pop();
        ALIGN_STACK.Pop();

        m_context.EndGroup();
    }

    /// <summary>
    /// Returns whether the current layout is horizontal
    /// </summary>
    private static bool IsHorizontalLayout() =>
        LAYOUT_STACK.Count > 0 && LAYOUT_STACK.Peek() == LayoutType.Horizontal;

    /// <summary>
    /// Align the next widget horizontally based on current alignment
    /// </summary>
    private static void AlignNextItem()
    {
        if (ALIGN_STACK.Count == 0) return;
        var align = ALIGN_STACK.Peek();

        var windowSize = m_context.GetWindowSize();
        float windowWidth = windowSize.x;
        float itemWidth = m_context.CalcItemWidth();

        if (align == LayoutAlign.Center)
        {
            float cursorX = (windowWidth - itemWidth) * 0.5f;
            m_context.SetCursorPosX(cursorX);
        }
        else if (align == LayoutAlign.Right)
        {
            float cursorX = windowWidth - itemWidth;
            m_context.SetCursorPosX(cursorX);
        }
        // No adjustment needed for Left alignment (default)
    }

    /// <summary>
    /// Render a text label
    /// </summary>
    public static void Label(string text)
    {
        AlignNextItem();
        if (IsHorizontalLayout()) m_context.SameLine();
        m_context.Text(text);
    }

    /// <summary>
    /// Render a button; returns true if clicked
    /// </summary>
    public static bool Button(string label)
    {
        AlignNextItem();
        if (IsHorizontalLayout()) m_context.SameLine();
        return m_context.Button(label);
    }

    /// <summary>
    /// Render and edit an integer field
    /// </summary>
    public static bool IntField(string label, ref int value)
    {
        AlignNextItem();
        if (IsHorizontalLayout()) m_context.SameLine();
        return m_context.InputInt(label, ref value);
    }

    /// <summary>
    /// Render and edit a float field
    /// </summary>
    public static bool FloatField(string label, ref float value)
    {
        AlignNextItem();
        if (IsHorizontalLayout()) m_context.SameLine();
        return m_context.InputFloat(label, ref value);
    }

    /// <summary>
    /// Render and edit a Vector2 field
    /// </summary>
    public static bool Vector2Field(string label, ref Vector2 value)
    {
        AlignNextItem();
        if (IsHorizontalLayout()) m_context.SameLine();
        return m_context.InputFloat2(label, ref value);
    }

    /// <summary>
    /// Render and edit a Vector3 field
    /// </summary>
    public static bool Vector3Field(string label, ref Vector3 value)
    {
        AlignNextItem();
        if (IsHorizontalLayout()) m_context.SameLine();
        return m_context.InputFloat3(label, ref value);
    }
    
    /// <summary>
    /// Render and edit a Quaternion field
    /// </summary>
    public static bool QuaternionField(string label, ref Quaternion value)
    {
        AlignNextItem();
        if (IsHorizontalLayout()) m_context.SameLine();
        return m_context.InputQuaternion(label, ref value);
    }

    /// <summary>
    /// Render and edit a text (string) field
    /// </summary>
    public static bool TextField(string label, ref string value, uint maxLength = 256)
    {
        AlignNextItem();
        if (IsHorizontalLayout()) m_context.SameLine();
        return m_context.InputText(label, ref value, maxLength);
    }

    /// <summary>
    /// Render and edit a boolean checkbox
    /// </summary>
    public static bool Checkbox(string label, ref bool value)
    {
        AlignNextItem();
        if (IsHorizontalLayout()) m_context.SameLine();
        m_context.Checkbox(label, ref value);
        // Checkbox may not return bool, use click detection instead
        return m_context.IsItemClicked(0);
    }
}
