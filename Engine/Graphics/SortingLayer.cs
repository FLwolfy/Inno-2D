namespace Engine.Graphics
{
    /// <summary>
    /// Represents a sorting layer with a unique name and priority order.
    /// </summary>
    public class SortingLayer
    {
        public string Name { get; }
        public int Order { get; }

        public SortingLayer(string name, int order)
        {
            Name = name;
            Order = order;
        }
    }
}