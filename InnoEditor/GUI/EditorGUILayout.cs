using InnoBase;
using InnoInternal.ImGui.Impl;

namespace InnoEditor.GUI;

/// <summary>
/// EditorLayout wrapper based on IImGuiContext,
/// supports flexible layouts and common UI widgets
/// </summary>
public static class EditorGUILayout
{
    public enum LayoutType { Vertical, Horizontal }
    public enum LayoutAlign { Front, Center, Back }
    
    private static readonly Stack<LayoutAlign> ALIGN_STACK = new();

    private static int m_autoID = 0;
    private static int m_autoMeasureID = 0;
    private static IImGuiContext m_context = null!;

    internal static void Initialize(IImGuiContext context)
    {
        m_context = context;
    }

    #region Lifecycles

    /// <summary>
    /// Reset auto ID.
    /// </summary>
    public static void BeginFrame()
    {
        m_autoID = 0;
        m_autoMeasureID = 0;
    }
    
    /// <summary>
    /// Reset the layout stacks at the end of a frame.
    /// </summary>
    public static void EndFrame()
    {
        if (ALIGN_STACK.Count != 0)
        {
            throw new InvalidOperationException("EditorLayout.EndFrame called without matching EndAlignment");
        }
    }
    
    #endregion
    
    #region Layouts
    
    /// <summary>
    /// Begin a new layout with specified type and alignment.
    /// </summary>
    public static void BeginAlignment(LayoutAlign align)
    {
        ALIGN_STACK.Push(align);
        m_context.BeginGroup();
    }

    /// <summary>
    /// End the current alignment layout.
    /// </summary>
    public static void EndAlignment()
    {
        if (ALIGN_STACK.Count == 0) {throw new InvalidOperationException("EditorLayout.End called without matching Begin");}
        ALIGN_STACK.Pop();
        m_context.EndGroup();
    }
    
    private readonly struct DrawScope : IDisposable
    {
        private readonly bool m_enabled = true;

        public DrawScope(bool enabled)
        {
            m_context.PushID(m_autoID++);
            if (!enabled)
            {
                m_enabled = enabled;
                m_context.BeginDisabled();
            }
        }

        public void Dispose()
        {
            if (!m_enabled)
            {
                m_context.EndDisabled();
            }
            m_context.PopID();
        }
    }


    private static void AlignNextItem(float itemWidth)
    {
        if (ALIGN_STACK.Count == 0) return;

        var align = ALIGN_STACK.Peek();
        var cursorPos = m_context.GetCursorPos();
        var regionAvail = m_context.GetContentRegionAvail();

        var offsetX = 0f;
        switch (align)
        {
            case LayoutAlign.Center:
                offsetX = (regionAvail.x - itemWidth) * 0.5f;
                break;
            case LayoutAlign.Back:
                offsetX = regionAvail.x - itemWidth;
                break;
            case LayoutAlign.Front:
            default:
                offsetX = 0f;
                break;
        }

        m_context.SetCursorPosX(cursorPos.x + offsetX);
    }

    private static float MeasureWidth(Action onMeasure)
    {
        m_context.BeginInvisible();
        m_context.PushID(m_autoMeasureID++);
        onMeasure.Invoke();
        m_context.PopID();
        m_context.EndInvisible();

        return m_context.GetInvisibleItemRectSize().x;
    }
    
    #endregion

    #region Widgets
    /// <summary>
    /// Render a text label
    /// </summary>
    public static void Label(string text, bool enabled = true)
    {
        float width = MeasureWidth(() => m_context.Text(text));
        AlignNextItem(width);
        
        using (new DrawScope(enabled)) { m_context.Text(text); }
    }

        /// <summary>
    /// Render a button; returns true if clicked
    /// </summary>
    public static bool Button(string label, bool enabled = true)
    {
        float width = MeasureWidth(() => m_context.Button(label));
        AlignNextItem(width);

        using (new DrawScope(enabled)) { return m_context.Button(label); }
    }

    /// <summary>
    /// Render and edit an integer field
    /// </summary>
    public static bool IntField(string label, ref int value, bool enabled = true)
    {
        var dummyValue = value;
        float width = MeasureWidth(() => m_context.InputInt(label, ref dummyValue));
        AlignNextItem(width);
        
        using (new DrawScope(enabled)) { return m_context.InputInt(label, ref value); }
    }

    /// <summary>
    /// Render and edit a float field
    /// </summary>
    public static bool FloatField(string label, ref float value, bool enabled = true)
    {
        var dummyValue = value;
        float width = MeasureWidth(() => m_context.InputFloat(label, ref dummyValue));
        AlignNextItem(width);
        
        using (new DrawScope(enabled)) { return m_context.InputFloat(label, ref value); }
    }

    /// <summary>
    /// Render and edit a Vector2 field
    /// </summary>
    public static bool Vector2Field(string label, ref Vector2 value, bool enabled = true)
    {
        var dummyValue = value;
        float width = MeasureWidth(() => m_context.InputFloat2(label, ref dummyValue));
        AlignNextItem(width);
        
        using (new DrawScope(enabled)) { return m_context.InputFloat2(label, ref value); }
    }

    /// <summary>
    /// Render and edit a Vector3 field
    /// </summary>
    public static bool Vector3Field(string label, ref Vector3 value, bool enabled = true)
    {
        var dummyValue = value;
        float width = MeasureWidth(() => m_context.InputFloat3(label, ref dummyValue));
        AlignNextItem(width);
        
        using (new DrawScope(enabled)) { return m_context.InputFloat3(label, ref value); }
    }

    /// <summary>
    /// Render and edit a Quaternion field
    /// </summary>
    public static bool QuaternionField(string label, ref Quaternion value, bool enabled = true)
    {
        var dummyValue = value;
        float width = MeasureWidth(() => m_context.InputQuaternion(label, ref dummyValue));
        AlignNextItem(width);
        
        using (new DrawScope(enabled)) { return m_context.InputQuaternion(label, ref value); }
    }

    /// <summary>
    /// Render and edit a text (string) field
    /// </summary>
    public static bool TextField(string label, ref string value, uint maxLength = 256, bool enabled = true)
    {
        var dummyValue = value;
        float width = MeasureWidth(() => m_context.InputText(label, ref dummyValue, maxLength));
        AlignNextItem(width);
        
        using (new DrawScope(enabled)) { return m_context.InputText(label, ref value, maxLength); }
    }

    /// <summary>
    /// Render and edit a boolean checkbox
    /// </summary>
    public static bool Checkbox(string label, ref bool value, bool enabled = true)
    {
        var dummyValue = value;
        float width = MeasureWidth(() => m_context.Checkbox(label, ref dummyValue));
        AlignNextItem(width);
        
        using (new DrawScope(enabled)) { return m_context.Checkbox(label, ref value); }
    }
    
    /// <summary>
    /// Render and edit a Color field
    /// </summary>
    public static bool ColorField(string label, in Color input, out Color output, bool enabled = true)
    {
        var dummyValue = input;
        float width = MeasureWidth(() => m_context.ColorEdit4(label, in dummyValue, out _));
        AlignNextItem(width);
        
        using (new DrawScope(enabled)) { return m_context.ColorEdit4(label, in input, out output); }
    }
    
    /// <summary>
    /// Render a Collapsable Header with a label and an action to call when closed.
    /// </summary>
    public static bool CollapsingHeader(string label, Action onClose, bool defaultOpen = true, bool enabled = true)
    {
        bool visibility = true;
        var openFlag = defaultOpen ? IImGuiContext.TreeNodeFlags.DefaultOpen : IImGuiContext.TreeNodeFlags.None;
        
        bool result;
        using (new DrawScope(enabled)) { result = m_context.CollapsingHeader(label, ref visibility, openFlag); }
        if (!visibility) { onClose.Invoke(); }
        return result;
    }
    
    /// <summary>
    /// Inserts vertical spacing of given height (default 8px)
    /// </summary>
    public static void Space(float pixels = 8f)
    {
        m_context.Dummy(new Vector2(1, pixels));
    }

    /// <summary>
    /// Inserts a horizontal separator line
    /// </summary>
    public static void Separator()
    {
        m_context.Separator();
    }
    
    #endregion
}
