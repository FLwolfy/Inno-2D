using InnoEngine.ECS;
using InnoEngine.Internal.Base;

namespace Sandbox;

public class TestComponent : GameBehavior
{
    public override ComponentTag orderTag => ComponentTag.Behavior;

    public override void Update()
    {
        float oldRotationZ = transform.worldRotation.ToEulerAnglesZYX().z;
        transform.worldRotation = Quaternion.FromEulerAnglesZYX(new Vector3(0, 0, oldRotationZ + 0.05f));
    }
}