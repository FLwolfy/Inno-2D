using InnoBase.Graphics;
using InnoBase.Math;
using InnoInternal.ImGui.Impl;

namespace Inno.Editor.GUI;

/// <summary>
/// EditorLayout wrapper based on IImGuiContext,
/// supports flexible layouts and common UI widgets
/// </summary>
public static class EditorGUILayout
{
    public enum LayoutAlign { Front, Center, Back }

    private static readonly Stack<int> SCOPE_STACK = new();
    private static readonly Stack<LayoutAlign> ALIGN_STACK = new();
    private static readonly Stack<bool> COLUMN_DIRTY_STACK = new();
    private static readonly Dictionary<int, int> COLUMN_COUNT_MAP = new();
    private static readonly Dictionary<int, float> COLUMN_TOTAL_WEIGHT_MAP = new();
    private static readonly Dictionary<int, List<float>> COLUMN_WEIGHT_MAP = new();
    
    private static int m_autoID = 0;
    private static int m_autoMeasureID = 0;
    private static int m_columnDepth = 0;
    private static bool m_frameBegin = false;
    
    private static IImGuiContext m_context = null!;

    #region Lifecycles

    /// <summary>
    /// Reset auto ID.
    /// </summary>
    public static void BeginFrame(IImGuiContext context)
    {
        if (m_frameBegin)
        {
            throw new InvalidOperationException("BeginFrame() can only be called once.");
        }
        
        m_context = context;

        m_autoID = 0;
        m_autoMeasureID = 0;
        m_frameBegin = true;
    }
    
    /// <summary>
    /// Check the end condition.
    /// </summary>
    public static void EndFrame()
    {
        if (ALIGN_STACK.Count != 0 || SCOPE_STACK.Count != 0 || !m_frameBegin)
        {
            throw new InvalidOperationException("EndFrame() is called improperly.");
        }
        
        m_frameBegin = false;
    }

    /// <summary>
    /// Begin a scope for the following GUI render.
    /// </summary>
    public static void BeginScope(int id)
    {
        m_context.PushID(id);
        SCOPE_STACK.Push(id);
    }

    /// <summary>
    /// End the current GUI scope.
    /// </summary>
    public static void EndScope()
    {
        m_context.PopID();
        SCOPE_STACK.Pop();
    }
    
    #endregion
    
    #region Layouts

    /// <summary>
    /// Begins a column layout.
    /// </summary>
    public static void BeginColumns(float firstColumnWeight = 1.0f, bool bordered = false)
    {
        var flags = IImGuiContext.TableFlags.SizingStretchProp;
        
        if (bordered)
        {
            flags |= IImGuiContext.TableFlags.BordersInner | IImGuiContext.TableFlags.BordersOuter;
        }
        
        m_columnDepth++;
        COLUMN_DIRTY_STACK.Push(!COLUMN_COUNT_MAP.ContainsKey(m_columnDepth));

        if (!COLUMN_DIRTY_STACK.Peek())
        {
            var columnCount = COLUMN_COUNT_MAP[m_columnDepth];
            m_context.BeginTable("EditorLayout", columnCount, flags);

            for (var i = 0; i < columnCount; i++)
            {
                m_context.TableSetupColumn($"Column {i}", COLUMN_WEIGHT_MAP[m_columnDepth][i]);
            }
            
            m_context.TableNextRow();
            m_context.TableSetColumnIndex(0);
        }
        else
        {
            COLUMN_COUNT_MAP[m_columnDepth] = 1;
            COLUMN_TOTAL_WEIGHT_MAP[m_columnDepth] = firstColumnWeight;
            COLUMN_WEIGHT_MAP[m_columnDepth] = [firstColumnWeight];
        }
        
    }

    /// <summary>
    /// Ends the current column layout.
    /// </summary>
    public static void EndColumns()
    {
        if (!COLUMN_DIRTY_STACK.Peek())
        {
            m_context.EndTable();
        }
        else
        {
            var totalWeight = COLUMN_TOTAL_WEIGHT_MAP[m_columnDepth];
            if (totalWeight != 0)
            {
                for (var i = 0; i < COLUMN_COUNT_MAP[m_columnDepth]; i++)
                {
                
                    COLUMN_WEIGHT_MAP[m_columnDepth][i] /= totalWeight;
                }
            }
        }
        
        m_columnDepth--;
    }

    /// <summary>
    /// Split columns in the current column layout.
    /// </summary>
    public static void SplitColumns(float nextColumnWeight = 1.0f)
    {
        if (!COLUMN_DIRTY_STACK.Peek())
        {
            m_context.TableNextColumn();
        }
        else
        {
            COLUMN_COUNT_MAP[m_columnDepth]++;
            COLUMN_TOTAL_WEIGHT_MAP[m_columnDepth] += nextColumnWeight;
            COLUMN_WEIGHT_MAP[m_columnDepth].Add(nextColumnWeight);
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
    /// Inserts horizontal indentation of given width (default 8px)
    /// </summary>
    public static void Indent(float pixels = 8f)
    {
        m_context.Dummy(new Vector2(pixels, 1));
        m_context.SameLine();
    }
    
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

        using (new DrawScope(enabled))
        {
            float textHeight = m_context.GetTextLineHeight(true);
            float frameHeight = m_context.GetFrameHeight(false);
            float verticalOffset = (frameHeight - textHeight) * 0.5f;
            var cursorPos = m_context.GetCursorPos();
            m_context.SetCursorPosY(cursorPos.y + verticalOffset);
            m_context.Text(text);
        }
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
    public static bool CollapsingHeader(string label, Action? onClose = null, bool defaultOpen = true, bool enabled = true)
    {
        bool visibility = true;
        var openFlag = defaultOpen ? IImGuiContext.TreeNodeFlags.DefaultOpen : IImGuiContext.TreeNodeFlags.None;

        bool result;
        if (onClose == null)
        {
            result = m_context.CollapsingHeader(label, openFlag);
        }
        else
        {
            using (new DrawScope(enabled)) { result = m_context.CollapsingHeader(label, ref visibility, openFlag); }
            if (!visibility) { onClose.Invoke(); }
        }
        
        return result;
    }

    /// <summary>
    /// Render a combo box for selecting objects.
    /// </summary>
    public static bool Combo(string label, string[] list, ref int selectedIndex, bool enabled = true)
    {
        var demmySelected = selectedIndex;
        float width = MeasureWidth(() => m_context.Combo(label, ref demmySelected, list));
        AlignNextItem(width);
        
        using (new DrawScope(enabled)) { return m_context.Combo(label, ref selectedIndex, list); }
    }

    /// <summary>
    /// Render a popup menu for selecting items.
    /// </summary>
    public static bool PopupMenu(string label, string emptyMsg, string[] itemNameList, out int? selectedIndex, bool enabled = true)
    {
        bool changed = false;
        selectedIndex = null;

        using (new DrawScope(enabled))
        {
            AlignNextItem(MeasureWidth(() => m_context.Button(label)));
            if (m_context.Button(label))
            {
                m_context.OpenPopup(label);
            }

            if (m_context.BeginPopup(label))
            {
                if (itemNameList.Length == 0)
                {
                    m_context.Text(emptyMsg);
                }
                
                for (int i = 0; i < itemNameList.Length; i++)
                {
                    if (m_context.MenuItem(itemNameList[i]))
                    {
                        selectedIndex = i;
                        changed = true;
                        m_context.CloseCurrentPopup();
                    }
                }

                m_context.EndPopup();
            }
        }

        return changed;
    }

    
    #endregion
}
