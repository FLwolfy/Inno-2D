namespace Engine.ECS
{
    /// <summary>
    /// Manages all scenes and tracks the active scene.
    /// </summary>
    public static class SceneManager
    {
        private static readonly Dictionary<Guid, GameScene> SCENES = [];
        private static GameScene? m_activeScene;

        /// <summary>
        /// Creates and registers a new scene.
        /// </summary>
        public static GameScene CreateScene()
        {
            var scene = new GameScene();
            SCENES[scene.Id] = scene;
            return scene;
        }
        
        /// <summary>
        /// Creates and registers a new scene with the given name.
        /// </summary>
        public static GameScene CreateScene(string name)
        {
            var scene = new GameScene(name);
            SCENES[scene.Id] = scene;
            return scene;
        }

        /// <summary>
        /// Sets the active scene.
        /// </summary>
        public static void SetActiveScene(GameScene scene)
        {
            m_activeScene = scene;
        }

        /// <summary>
        /// Gets the currently active scene. Throws if not set.
        /// </summary>
        public static GameScene GetActiveScene()
        {
            if (m_activeScene == null) {throw new InvalidOperationException("No active scene is set.");}
            return m_activeScene;
        }

        /// <summary>
        /// Gets a scene by name, or null if not found.
        /// </summary>
        public static GameScene? GetScene(string name)
        {
            return SCENES.Values.FirstOrDefault(scene => scene.Name == name);
        }

        /// <summary>
        /// Gets a scene by uuid, or null if not found.
        /// </summary>
        public static GameScene? GetScene(Guid id)
        {
            return SCENES.GetValueOrDefault(id);
        }
        
        /// <summary>
        /// Gets all the scenes loaded.
        /// </summary>
        public static IReadOnlyList<GameScene> GetAllScenes() => SCENES.Values.ToList();
    }
}