namespace Engine.ECS
{
    /// <summary>
    /// Represents an entity in the scene. It holds an ID and allows component operations.
    /// </summary>
    public class GameObject
    {
        public readonly Guid Id = Guid.NewGuid();
        public string Name = "GameObject";
        
        private readonly GameScene m_scene;

        public GameObject()
        {
            m_scene = SceneManager.GetActiveScene();
            m_scene.RegisterGameObject(this);
        }
        
        public GameObject(string name)
        {
            m_scene = SceneManager.GetActiveScene();
            m_scene.RegisterGameObject(this);
            Name = name;
        }

        public GameObject(GameScene scene)
        {
            m_scene = scene;
            scene.RegisterGameObject(this);
        }

        public GameObject(GameScene scene, string name)
        {
            m_scene = scene;
            scene.RegisterGameObject(this);
            Name = name;
        }

        /// <summary>
        /// Adds a component of type T to this GameObject.
        /// </summary>
        public T AddComponent<T>() where T : GameComponent, new()
        {
            return m_scene.GetComponentManager().Add<T>(Id, this);
        }

        /// <summary>
        /// Gets a component of type T from this GameObject.
        /// </summary>
        public T? GetComponent<T>() where T : GameComponent
        {
            return m_scene.GetComponentManager().Get<T>(Id);
        }

        /// <summary>
        /// Checks whether the GameObject has a component of type T.
        /// </summary>
        public bool HasComponent<T>() where T : GameComponent
        {
            return m_scene.GetComponentManager().Has<T>(Id);
        }

        /// <summary>
        /// Removes a component of type T from this GameObject.
        /// </summary>
        public void RemoveComponent<T>() where T : GameComponent
        {
            m_scene.GetComponentManager().Remove<T>(Id);
        }
    }
}
