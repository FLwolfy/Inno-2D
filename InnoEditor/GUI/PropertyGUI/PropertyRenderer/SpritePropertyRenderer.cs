using InnoEngine.Graphics.RenderObject;

namespace InnoEditor.GUI.PropertyGUI.PropertyRenderer;

public class SpritePropertyRenderer : PropertyRenderer<Sprite>
{
    protected override void Bind(string name, Func<Sprite?> getter, Action<Sprite?> setter, bool enabled)
    {
        // TODO: Add Texture Load
    }
}