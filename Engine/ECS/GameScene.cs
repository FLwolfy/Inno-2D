namespace Engine.ECS
{
    /// <summary>
    /// Represents a scene that contains and manages GameObjects.
    /// </summary>
    public class GameScene
    {
        public readonly Guid Id = Guid.NewGuid();
        public string Name = "GameScene";
        
        private readonly List<GameObject> m_gameObjects = [];
        private readonly List<GameObject> m_pendingGameObjectRemoves = [];
        private readonly ComponentManager m_componentManager = new();

        internal GameScene(){}
        
        internal GameScene(string name)
        {
            Name = name;
        }
        
        internal ComponentManager GetComponentManager() => m_componentManager;

        /// <summary>
        /// Registers a GameObject to this scene.
        /// </summary>
        public void RegisterGameObject(GameObject obj)
        {
            if (m_gameObjects.Contains(obj)) { return; }
            m_gameObjects.Add(obj);
        }

        /// <summary>
        /// Unregisters a GameObject from this scene.
        /// </summary>
        public void UnregisterGameObject(GameObject obj)
        {
            var components = m_componentManager.GetAll(obj.Id);
            foreach (var comp in components)
            {
                m_componentManager.Remove(obj.Id, comp);
            }
            
            m_pendingGameObjectRemoves.Add(obj);
        }

        /// <summary>
        /// Get a gameobject with its uuid.
        /// </summary>
        public GameObject? FindGameObject(Guid id)
        {
            return m_gameObjects.Find(obj => obj.Id == id);
        }
        
        /// <summary>
        /// Get a gameobject with its name.
        /// </summary>
        public GameObject? FindGameObject(string name)
        {
            return m_gameObjects.Find(obj => obj.Name == name);
        }

        /// <summary>
        /// Gets all GameObjects in this scene.
        /// </summary>
        public IReadOnlyList<GameObject> GetAllGameObjects()
        {
            return m_gameObjects;
        }

        /// <summary>
        /// Called at the very first of the very first frame.
        /// </summary>
        public void Start()
        {
            m_componentManager.BeginRuntime();
            m_componentManager.WakeAll();
        }

        /// <summary>
        /// Update the GameScene and its gameObjects and components.
        /// </summary>
        public void Update()
        {
            m_componentManager.UpdateAll();
            
            foreach (var gameObject in m_pendingGameObjectRemoves) { m_gameObjects.Remove(gameObject); }
            m_pendingGameObjectRemoves.Clear();
        }
    }
}