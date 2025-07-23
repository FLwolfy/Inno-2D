namespace InnoEditor.GUI.PropertyGUI.PropertyRenderer;

public class ListPropertyRenderer<T> : PropertyRenderer<List<T>>
{
    private IPropertyRenderer? m_renderer;
    
    protected override void Render(List<T> value)
    {
        if (m_renderer == null) { m_renderer = PropertyRendererRegistry.GetRenderer(typeof(T)); }
        foreach (var item in value)
        {
            m_renderer?.Render(item);
        }
    }
}