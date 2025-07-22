using InnoEditor.Core;
using InnoEngine.ECS;
using InnoInternal.ImGui.Impl;
using InnoInternal.Render.Impl;

namespace InnoEditor.Panel;

public class HierarchyPanel : EditorPanel
{
    public override string title => "Hierarchy";
    
    private readonly Queue<Action> m_pendingGUIUpdateAction = new Queue<Action>();
    
    private bool m_openContextMenu = false;

    internal override void OnGUI(IImGuiContext context, IRenderAPI renderAPI)
    {
        var rootObjects = SceneManager.GetActiveScene()!.GetAllRootGameObjects();
        foreach (var obj in rootObjects)
        {
            DrawGameObjectTree(context, obj);
        }
        
        // Delay Update
        while (m_pendingGUIUpdateAction.Count > 0)
        {
            m_pendingGUIUpdateAction.Dequeue().Invoke();
        }
    }

    private void DrawGameObjectTree(IImGuiContext context, GameObject obj)
    {
        bool hasChildren = obj.transform.children.Count > 0;
        bool isOpenTree = false;

        // Draw Tree Nodes
        if (hasChildren)
            isOpenTree = context.TreeNode(obj.name);
        else
            context.Selectable(obj.name);

        // Drag Source
        if (context.BeginDragDropSource())
        {
            context.SetDragDropPayload<Guid>("GameObjectGUID", obj.id);
            context.Text($"Dragging {obj.name}");
            context.EndDragDropSource();
        }
        
        // Drag Target
        if (context.BeginDragDropTarget())
        {
            var payload = context.AcceptDragDropPayload<Guid>("GameObjectGUID");
            if (payload != null && payload != obj.id)
            {
                var payloadObj = SceneManager.GetActiveScene()!.FindGameObject(payload.Value);
                m_pendingGUIUpdateAction.Enqueue(() => payloadObj?.transform.SetParent(obj.transform));
            }
            context.EndDragDropTarget();
        }

        // Draw Children
        if (hasChildren && isOpenTree)
        {
            foreach (var child in obj.transform.children)
                DrawGameObjectTree(context, child.gameObject);

            context.TreePop();
        }
    }
}