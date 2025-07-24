namespace InnoEditor.GUI.PropertyGUI.PropertyRenderer;

public class ListPropertyRenderer<T> : PropertyRenderer<List<T>>
{
    private IPropertyRenderer? m_renderer;
    
    protected override void Bind(string name, Func<List<T>?> getter, Action<List<T>> setter)
    {
        if (m_renderer == null) { m_renderer = PropertyRendererRegistry.GetRenderer(typeof(T)); }
        
        // TODO
        
    }
}