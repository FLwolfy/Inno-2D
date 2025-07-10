using Engine.ECS;
using Engine.Extension;
using Microsoft.Xna.Framework;

namespace Sandbox;

public class TestComponent : GameBehavior
{
    public override ComponentTag orderTag => ComponentTag.Behavior;

    public override void Update()
    {
        float oldRotationZ = transform.worldRotation.ToEulerAnglesZYX().Z;
        transform.worldRotation = QuaternionExtension.FromEulerAnglesZYX(new Vector3(0, 0, oldRotationZ + 0.05f));
    }
}