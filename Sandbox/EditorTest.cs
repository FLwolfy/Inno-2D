using InnoBase;
using InnoEditor.Core;
using InnoEngine.Core;
using InnoEngine.Core.Layer;
using InnoEngine.ECS;
using InnoEngine.ECS.Component;
using InnoInternal.Shell.Impl;

namespace Sandbox;

public class EditorTest
{
    public void Run()
    {
        TestEngineCore testCore = new TestEngineCore();
        testCore.Run();
    }
    
    private class TestEngineCore: EngineCore
    {
        private TestEditorLayer m_editorLayer = null!;
        
        protected override void Setup(IGameShell gameShell)
        {
            m_editorLayer = new TestEditorLayer();
            
            gameShell.SetWindowSize(1280, 720);
            gameShell.SetWindowResizable(true);
        }
        protected override void RegisterLayers(LayerStack layerStack)
        {
            layerStack.PushLayer(m_editorLayer);
        }
    }
    
    private class TestEditorLayer : EditorLayer
    {
        public override void OnAttach()
        {
            // Editor Initialization
            base.OnAttach();
            
            // TEST SCENE SETUP
            GameScene testScene = SceneManager.CreateScene("Test Scene");
            SceneManager.SetActiveScene(testScene);
        
            // Object 1
            GameObject testObject = new GameObject("Test Object 1");
            testObject.transform.worldPosition = new Vector3(320, 180, 0);
            testObject.transform.worldScale = new Vector3(1f, 2f, 1f);
            testObject.AddComponent<SpriteRenderer>();
        
            // Object 2 - 5
            for (int i = 2; i <= 5; i++)
            {
                GameObject to = new GameObject("Test Object" + i);
                to.transform.worldPosition = new Vector3(150 * (i - 2), 0, 5);
                to.transform.worldScale = new Vector3(10f, 10f, 1f);
                SpriteRenderer sr = to.AddComponent<SpriteRenderer>();
                sr.color = Color.BLACK;
            
                to.transform.SetParent(testObject.transform);
            }
        }
    }
}

