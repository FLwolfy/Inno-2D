using InnoEngine.ECS;

namespace InnoEditor.Utility;

public class EditorSelection
{
    public delegate void SelectionChangedHandler(GameObject? oldObj, GameObject? newObj);
    public event SelectionChangedHandler? OnSelectionChanged;
    
    private GameObject? m_selectedObject;
    public GameObject? selectedObject => m_selectedObject;

    public void Select(GameObject obj)
    {
        if (m_selectedObject != obj)
        {
            var old = m_selectedObject;
            m_selectedObject = obj;
            OnSelectionChanged?.Invoke(old, obj);
        }
        
        // TODO: REMOVE DEBUG
        foreach (var prop in obj.transform.GetSerializedProperties())
        {
            Console.WriteLine(prop.typeDescriptor);
        }
    }

    public void Deselect()
    {
        if (m_selectedObject != null)
        {
            var old = m_selectedObject;
            m_selectedObject = null;
            OnSelectionChanged?.Invoke(old, null);
        }
    }

    public bool IsSelected(GameObject obj)
    {
        return m_selectedObject == obj;
    }
}

