using System.Collections.Generic;
using System.Linq;

namespace Engine.Graphics
{
    /// <summary>
    /// Manages registered sorting layers and their priority order.
    /// </summary>
    public static class SortingLayerManager
    {
        private static readonly List<SortingLayer> layers = new();

        /// <summary>
        /// Registers a new sorting layer. Lower Order means drawn earlier (behind).
        /// </summary>
        public static void RegisterLayer(string name, int order)
        {
            if (layers.Any(l => l.Name == name))
                return; // Already exists

            layers.Add(new SortingLayer(name, order));
            layers.Sort((a, b) => a.Order.CompareTo(b.Order));
        }

        /// <summary>
        /// Returns the order of a layer by name. If not found, returns int.MaxValue (draw last).
        /// </summary>
        public static int GetLayerOrder(string name)
        {
            var layer = layers.FirstOrDefault(l => l.Name == name);
            return layer != null ? layer.Order : int.MaxValue;
        }

        /// <summary>
        /// Get all registered layers in order.
        /// </summary>
        public static IReadOnlyList<SortingLayer> GetAllLayers() => layers.AsReadOnly();
    }
}