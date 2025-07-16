namespace InnoEditor.Core;

public abstract class EditorMenuItem
{
    public string name { get; }
    
    public abstract void Action();
}