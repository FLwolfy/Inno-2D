using InnoEditor.Core;
using InnoEngine.ECS;
using InnoInternal.ImGui.Impl;
using InnoInternal.Render.Impl;

namespace InnoEditor.Panel;

public class HierarchyPanel : EditorPanel
{
    public override string title => "Hierarchy";

    private const string C_GAMEOBJECT_GUID_TYPE = "GameObjectGUID";
    
    private readonly Queue<Action> m_pendingGUIUpdateAction = new Queue<Action>();

    internal override void OnGUI(IImGuiContext context, IRenderAPI renderAPI)
    {
        // Drag Root Target
        context.Selectable("[ Scene Root ]");
        if (context.BeginDragDropTarget())
        {
            var payload = context.AcceptDragDropPayload<Guid>(C_GAMEOBJECT_GUID_TYPE);
            if (payload != null)
            {
                var obj = SceneManager.GetActiveScene()!.FindGameObject(payload.Value);
                m_pendingGUIUpdateAction.Enqueue(() => obj?.transform.SetParent(null));
            }
            context.EndDragDropTarget();
        }
        
        // Recursion
        foreach (var obj in SceneManager.GetActiveScene()!.GetAllRootGameObjects())
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
        bool isOpenTree;

        // Draw Tree Nodes
        if (hasChildren)
            isOpenTree = context.TreeNode(obj.name, IImGuiContext.TreeNodeFlags.DefaultOpen);
        else
            isOpenTree = context.TreeNode(obj.name, IImGuiContext.TreeNodeFlags.Leaf);

        // Drag Source
        if (context.BeginDragDropSource())
        {
            context.SetDragDropPayload<Guid>(C_GAMEOBJECT_GUID_TYPE, obj.id);
            context.Text($"Dragging {obj.name}");
            context.EndDragDropSource();
        }
        
        // Drag Target
        if (context.BeginDragDropTarget())
        {
            var payload = context.AcceptDragDropPayload<Guid>(C_GAMEOBJECT_GUID_TYPE);
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
        }

        // Collapse Tree
        if (isOpenTree)
        {
            context.TreePop();
        }
    }
}