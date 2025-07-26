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

    private static int m_autoID = 0;
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
    }
    
    /// <summary>
    /// Reset the layout stacks at the end of a frame.
    /// </summary>
    public static void EndFrame()
    {
        // Reset Layout and Align stacks
        LAYOUT_STACK.Clear();
        ALIGN_STACK.Clear();
    }
    
    #endregion
    
    #region Layouts
    
    /// <summary>
    /// Begin a new layout with specified type and alignment.
    /// </summary>
    public static void Layout(LayoutType layout, LayoutAlign align, Action guiDrawCall)
    {
        // Invisible Buffer to get size
        m_context.BeginInvisible();
        guiDrawCall.Invoke();
        m_context.EndInvisible();
        
        // Align Start
        LAYOUT_STACK.Push(layout);
        ALIGN_STACK.Push(align);
        m_context.BeginGroup();
        
        // Draw
        guiDrawCall.Invoke();
        
        // Align End
        if (LAYOUT_STACK.Count == 0 || ALIGN_STACK.Count == 0) {throw new InvalidOperationException("EditorLayout.End called without matching Begin");}
        LAYOUT_STACK.Pop();
        ALIGN_STACK.Pop();
        m_context.EndGroup();
    }
    
    /// <summary>
    /// Creates a visual background block for grouping content. Use in a using statement.
    /// </summary>
    public static IDisposable Box(string name, bool bordered = false, bool enabled = true)
    {
        return new BoxScope(name, bordered, enabled);
    }
    
    /// <summary>
    /// The BoxScope is used to create a visual box with optional border and auto-resize.
    /// It should be used in a using statement to ensure proper disposal.
    /// </summary>
    private readonly struct BoxScope : IDisposable
    {
        public BoxScope(string boxName, bool bordered, bool enabled)
        {
            m_drawScope = new DrawScope(enabled, false);

            var flags = IImGuiContext.ChildFlags.AlwaysAutoResize | IImGuiContext.ChildFlags.AutoResizeY;
            if (bordered)
                flags |= IImGuiContext.ChildFlags.Borders;

            m_context.BeginChild(boxName, new Vector2(0, 0), flags);
        }

        public void Dispose()
        {
            m_context.EndChild();
            m_drawScope.Dispose();
        }

        private readonly DrawScope m_drawScope;
    }

    /// <summary>
    /// Align the next widget horizontally based on current alignment
    /// </summary>
    private static void AlignNextItem()
    {
        if (ALIGN_STACK.Count == 0) return;

        var align = ALIGN_STACK.Peek();
        var cursorPos = m_context.GetCursorPos();
        var regionAvail = m_context.GetContentRegionAvail();
        var itemSize = m_context.GetItemRectSize();

        var offsetY = 0f;
        var offsetX = 0f;
        if (LAYOUT_STACK.Count > 0 && LAYOUT_STACK.Peek() == LayoutType.Horizontal)
        {
            switch (align)
            {
                case LayoutAlign.Center:
                    offsetX = (regionAvail.x - itemSize.x) * 0.5f;
                    break;
                case LayoutAlign.Back:
                    offsetX = regionAvail.x - itemSize.x;
                    break;
                case LayoutAlign.Front:
                default:
                    offsetX = 0f;
                    break;
            }

            m_context.SetCursorPosX(cursorPos.x + offsetX);
        }
        else // Vertical layout
        {
            switch (align)
            {
                case LayoutAlign.Center:
                    offsetY = (regionAvail.y - itemSize.y) * 0.5f;
                    break;
                case LayoutAlign.Back: // “Back” interpreted as Bottom here
                    offsetY = regionAvail.y - itemSize.y;
                    break;
                case LayoutAlign.Front: // “Front” interpreted as Top
                default:
                    offsetY = 0f;
                    break;
            }

            m_context.SetCursorPosY(cursorPos.y + offsetY);
        }
    }


    /// <summary>
    /// All Widgets calls should be within this scope.
    /// </summary>
    private readonly struct DrawScope : IDisposable
    {
        private readonly bool m_enabled;
        public DrawScope(bool enabled = true, bool applyAlign = true)
        {
            // Align if needed
            if (applyAlign)
            {
                AlignNextItem();
            }
            
            // Check Horizontal
            // TODO: CHECK HERE, THIS IS BUGGED
            // if (LAYOUT_STACK.Count > 0 && LAYOUT_STACK.Peek() == LayoutType.Horizontal)
            // {
            //     m_context.SameLine();
            // }
            
            // ID
            m_context.PushID(m_autoID++);
            
            // Disable Check
            m_enabled = enabled;
            if (!enabled)
            {
                m_context.BeginDisabled();
            }
        }
        
        public void Dispose()
        {
            if (!m_enabled) m_context.EndDisabled();
            m_context.PopID();
        }
    }
    
    #endregion

    #region Widgets
    /// <summary>
    /// Render a text label
    /// </summary>
    public static void Label(string text, bool enabled = true)
    {
        using (new DrawScope(enabled))
            m_context.Text(text);
    }

        /// <summary>
    /// Render a button; returns true if clicked
    /// </summary>
    public static bool Button(string label, bool enabled = true)
    {
        using (new DrawScope(enabled))
            return m_context.Button(label);
    }

    /// <summary>
    /// Render and edit an integer field
    /// </summary>
    public static bool IntField(string label, ref int value, bool enabled = true)
    {
        using (new DrawScope(enabled)) 
            return m_context.InputInt(label, ref value);
    }

    /// <summary>
    /// Render and edit a float field
    /// </summary>
    public static bool FloatField(string label, ref float value, bool enabled = true)
    {
        using (new DrawScope(enabled))
            return m_context.InputFloat(label, ref value);
    }

    /// <summary>
    /// Render and edit a Vector2 field
    /// </summary>
    public static bool Vector2Field(string label, ref Vector2 value, bool enabled = true)
    {
        using (new DrawScope(enabled))
            return m_context.InputFloat2(label, ref value);
    }

    /// <summary>
    /// Render and edit a Vector3 field
    /// </summary>
    public static bool Vector3Field(string label, ref Vector3 value, bool enabled = true)
    {
        using (new DrawScope(enabled))
            return m_context.InputFloat3(label, ref value);
    }

    /// <summary>
    /// Render and edit a Quaternion field
    /// </summary>
    public static bool QuaternionField(string label, ref Quaternion value, bool enabled = true)
    {
        using (new DrawScope(enabled))
            return m_context.InputQuaternion(label, ref value);
    }

    /// <summary>
    /// Render and edit a text (string) field
    /// </summary>
    public static bool TextField(string label, ref string value, uint maxLength = 256, bool enabled = true)
    {
        using (new DrawScope(enabled))
            return m_context.InputText(label, ref value, maxLength);
    }

    /// <summary>
    /// Render and edit a boolean checkbox
    /// </summary>
    public static bool Checkbox(string label, ref bool value, bool enabled = true)
    {
        using (new DrawScope(enabled))
            return m_context.Checkbox(label, ref value);
    }
    
    /// <summary>
    /// Render and edit a Color field
    /// </summary>
    public static bool ColorField(string label, in Color input, out Color output, bool enabled = true)
    {
        using (new DrawScope(enabled))
            return m_context.ColorEdit4(label, in input, out output);
    }
    
    /// <summary>
    /// Render a Collapsable Header with a label and an action to call when closed.
    /// </summary>
    public static bool CollapsingHeader(string label, Action onClose, bool defaultOpen = true)
    {
        using (new DrawScope(true, false))
        {
            bool visibility = true;
            var openFlag = defaultOpen ? IImGuiContext.TreeNodeFlags.DefaultOpen : IImGuiContext.TreeNodeFlags.None;
            bool result = m_context.CollapsingHeader(label, ref visibility, openFlag);
            if (!visibility) { onClose.Invoke(); }
            return result;
        }
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
