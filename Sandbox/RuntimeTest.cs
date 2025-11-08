using InnoBase;
using InnoEngine.Core;
using InnoEngine.Core.Layer;
using InnoEngine.ECS;
using InnoEngine.ECS.Component;
using InnoEngine.Utility;

namespace Sandbox;

public class RuntimeTest
{
    public void Run()
    {
        TestEngineCore testCore = new TestEngineCore();
        testCore.Run();
    }
    
    private class TestEngineCore: EngineCore
    {
        private TestGameLayer m_gameLayer = null!;
        
        protected override void Setup()
        {
            m_gameLayer = new TestGameLayer();
        }
        protected override void RegisterLayers(LayerStack layerStack)
        {
            layerStack.PushLayer(m_gameLayer);
        }
    }
    
    private class TestGameLayer : GameLayer
    {
        private GameObject m_testParentObject = null!;
        
        public override void OnUpdate()
        {
            m_testParentObject.transform.localRotationZ += Time.deltaTime * 100f;
            
            base.OnUpdate();
        }

        public override void OnAttach()
        {
            // TEST SCENE SETUP
            GameScene testScene = SceneManager.CreateScene("Test Scene");
            SceneManager.SetActiveScene(testScene);
        
            // Camera Setup
            GameObject cameraObject = new GameObject("Main Camera");
            OrthographicCamera camera = cameraObject.AddComponent<OrthographicCamera>();
            camera.isMainCamera = true;
            camera.aspectRatio = 16f / 9f;
            camera.size = 720f;
        
            // Object 1
            m_testParentObject = new GameObject("Test Parent Object 1");
            m_testParentObject.transform.worldPosition = new Vector3(100, 100, 1f);
            m_testParentObject.transform.worldScale = new Vector3(1f, 1f, 1f);
            m_testParentObject.transform.localRotationZ = 45f;
            m_testParentObject.AddComponent<SpriteRenderer>();
        
            // Object 2 - 5
            for (int i = 2; i <= 5; i++)
            {
                GameObject to = new GameObject("Test Object" + i);
                to.transform.worldPosition = new Vector3(150 * (i - 2), 0, 0f);
                to.transform.worldScale = new Vector3(5f, 5f, 1f);
                SpriteRenderer sr = to.AddComponent<SpriteRenderer>();
                sr.color = Color.BLACK;
            
                to.transform.SetParent(m_testParentObject.transform);
            }
            
            // Scene Begin Runtime
            base.OnAttach();
        }
    }
}

