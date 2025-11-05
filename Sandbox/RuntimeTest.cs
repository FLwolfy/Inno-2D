using InnoEngine.Core;
using InnoEngine.Core.Layer;

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
        private GameLayer m_gameLayer = null!;
        
        protected override void Setup()
        {
            m_gameLayer = new GameLayer();
        }

        protected override void RegisterLayers(LayerStack layerStack)
        {
            layerStack.PushLayer(m_gameLayer);
        }
    }
}

