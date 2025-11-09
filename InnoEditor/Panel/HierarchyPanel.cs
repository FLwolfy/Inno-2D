using InnoBase;
using InnoEditor.Core;
using InnoEngine.ECS;
using InnoEngine.Graphics;
using InnoInternal.ImGui.Impl;
using InnoInternal.Render.Impl;

namespace InnoEditor.Panel;

public class HierarchyPanel : EditorPanel
{
    public override string title => "Hierarchy";

    private const string C_GAMEOBJECT_GUID_TYPE = "GameObjectGUID";
    private readonly Queue<Action> m_pendingGUIUpdateAction = new();
    
    internal HierarchyPanel() {}

    internal override void OnGUI(IImGuiContext imGuiContext, RenderContext renderContext)
    {
        // Draw Scene root
        DrawSceneObjectRoot(imGuiContext);

        // Draw root GameObjects
        foreach (var obj in SceneManager.GetActiveScene()!.GetAllRootGameObjects())
        {
            DrawRootGameObject(imGuiContext, obj);
        }
        
        // Handle Menu Events
        HandleMenu(imGuiContext);

        // Apply delayed actions
        while (m_pendingGUIUpdateAction.Count > 0)
        {
            m_pendingGUIUpdateAction.Dequeue().Invoke();
        }
    }

    private void HandleMenu(IImGuiContext context)
    {
        if (!context.IsAnyItemHovered() && context.IsWindowHovered() && context.IsMouseClicked((int)Input.MouseButton.Right))
        {
            context.OpenPopup("HierarchyContextMenu");
        }

        if (context.BeginPopup("HierarchyContextMenu"))
        {
            if (context.BeginMenu("Create"))
            {
                if (context.MenuItem("GameObject"))
                {
                    m_pendingGUIUpdateAction.Enqueue(() =>
                    {
                        var go = new GameObject("New GameObject");
                        EditorManager.selection.Select(go);
                    });
                }
                context.EndMenu();
            }
            context.EndPopup();
        }
    }

    private void DrawSceneObjectRoot(IImGuiContext context)
    {
        // Draw "Scene Root" as non-selectable, non-draggable
        context.Text("[ Scene Root ]");
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
    }

    private void DrawRootGameObject(IImGuiContext context, GameObject obj)
    {
        var selection = EditorManager.selection;
        bool isSelected = selection.IsSelected(obj);
        bool hasChildren = obj.transform.children.Count > 0;

        // TreeNodeFlags with Selected flag
        var flags = hasChildren ? IImGuiContext.TreeNodeFlags.DefaultOpen | IImGuiContext.TreeNodeFlags.OpenOnArrow | IImGuiContext.TreeNodeFlags.OpenOnDoubleClick : IImGuiContext.TreeNodeFlags.Leaf;
        if (isSelected) { flags |= IImGuiContext.TreeNodeFlags.Selected; }

        ////////////// Begin Tree Node //////////////
        bool isOpenTree = context.TreeNode($"{obj.name}###{obj.id}", flags);
        
        // Handle Right Click menu
        if (context.BeginPopupContextItem($"Popup_{obj.id}"))
        {
            if (context.MenuItem("Delete"))
            {
                m_pendingGUIUpdateAction.Enqueue(() =>
                {
                    obj.scene.UnregisterGameObject(obj);
                });
            }

            context.EndPopup();
        }
        
        // Handle click selection
        if (context.IsItemClicked((int)Input.MouseButton.Left))
        {
            selection.Select(obj);
        }

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
            foreach (var child in obj.transform.children) DrawRootGameObject(context, child.gameObject);
        }
        
        ////////////// End Tree Node //////////////
        if (isOpenTree) { context.TreePop(); }
    }
}
