using InnoBase;
using InnoEngine.Core;
using InnoEngine.Core.Layer;
using InnoEngine.ECS;
using InnoEngine.ECS.Component;

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
        private readonly GameLayer m_gameLayer;

        public TestEngineCore()
        {
            m_gameLayer = new GameLayer();
        }
        
        protected override void Setup()
        {
            // TEST SCENE SETUP
            GameScene testScene = SceneManager.CreateScene("Test Scene");
            SceneManager.SetActiveScene(testScene);
        
            // Camera Setup
            GameObject cameraObject = new GameObject("Main Camera");
            OrthographicCamera camera = cameraObject.AddComponent<OrthographicCamera>();
            camera.isMainCamera = true;
            camera.aspectRatio = 16f / 9f;
            camera.size = 1440;
        
            // Object 1
            GameObject testObject = new GameObject("Test Object 1");
            testObject.transform.worldPosition = new Vector3(320, 180, 0f);
            testObject.transform.worldScale = new Vector3(10f, 20f, 1f);
            testObject.transform.localRotationZ = 45;
            testObject.AddComponent<SpriteRenderer>();
            
            var position4 = new Vector4(testObject.transform.worldPosition.x, testObject.transform.worldPosition.y, testObject.transform.worldPosition.z, 1f);
            var vp = camera.projectionMatrix * camera.viewMatrix;
            Console.WriteLine(vp * position4);
        
            // Object 2 - 5
            for (int i = 2; i <= 5; i++)
            {
                GameObject to = new GameObject("Test Object" + i);
                to.transform.worldPosition = new Vector3(150 * (i - 2), 0, 0);
                to.transform.worldScale = new Vector3(5f, 5f, 1f);
                SpriteRenderer sr = to.AddComponent<SpriteRenderer>();
                sr.color = Color.BLACK;
            
                to.transform.SetParent(testObject.transform);
            }
        }

        protected override void RegisterLayers(LayerStack layerStack)
        {
            layerStack.PushLayer(m_gameLayer);
        }
    }
}

