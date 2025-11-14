using InnoBase;
using InnoBase.Event;
using InnoBase.Graphics;
using InnoBase.Math;
using InnoEngine.Core;
using InnoEngine.Core.Layer;
using InnoEngine.ECS;
using InnoEngine.ECS.Component;
using InnoEngine.Utility;
using InnoInternal.Shell.Impl;

namespace Inno.Sandbox;

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
        
        protected override void Setup(IGameShell gameShell)
        {
            m_gameLayer = new TestGameLayer();
            
            gameShell.SetWindowSize(1280, 720);
            gameShell.SetWindowResizable(true);
        }
        protected override void RegisterLayers(LayerStack layerStack)
        {
            layerStack.PushLayer(m_gameLayer);
        }
    }
    
    private class TestGameLayer : GameLayer
    {
        private GameObject m_mainTestObj = null!;
        
        private bool m_shouldRotate = false;
        
        public override void OnUpdate()
        {
            if (m_shouldRotate)
            {
                m_mainTestObj.transform.localRotationZ += Time.deltaTime * 100f;
            }
            
            base.OnUpdate();
        }

        public override void OnEvent(Event e)
        {
            base.OnEvent(e);
            
            if (e.type == EventType.KeyPressed)
            {
                var keyEvent = (e as KeyPressedEvent);

                if (keyEvent!.key == Input.KeyCode.R && !keyEvent!.repeat)
                {
                    m_shouldRotate = true;
                }
            }
            else if (e.type == EventType.KeyReleased)
            {
                var keyEvent = e as KeyReleasedEvent;

                if (keyEvent!.key == Input.KeyCode.R)
                {
                    m_shouldRotate = false;
                }
            }
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
            m_mainTestObj = new GameObject("Test Object 1");
            m_mainTestObj.transform.worldPosition = new Vector3(0, 0, 1);
            m_mainTestObj.transform.worldScale = new Vector3(100f, 100f, 1f);
            m_mainTestObj.transform.localRotationZ = 45;
            var mainSR = m_mainTestObj.AddComponent<SpriteRenderer>();
            mainSR.layerDepth = 1;

            // TODO: DEBUG: Why Depth test not applied on Metal
            
            // Object 2
            GameObject to = new GameObject("Test Object" + 2);
            to.transform.worldPosition = new Vector3(0, 0, 0f);
            to.transform.worldScale = new Vector3(50f, 50f, 1f);
            SpriteRenderer sr = to.AddComponent<SpriteRenderer>();
            sr.color = Color.BLACK;
            
            // Scene Begin Runtime
            base.OnAttach();
        }
    }
}

