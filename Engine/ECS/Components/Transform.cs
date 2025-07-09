using System.Numerics;

namespace Engine.ECS.Components
{
    /// <summary>
    /// Transform component manages position, rotation, scale and parent-child hierarchy.
    /// Supports 3D transforms (even if your engine is 2D only, this matches Unity design).
    /// </summary>
    public class Transform : GameComponent
    {
        // Local transform relative to parent
        private Vector3 m_localPosition = Vector3.Zero;
        private Quaternion m_localRotation = Quaternion.Identity;
        private Vector3 m_localScale = Vector3.One;

        // Children transforms
        private readonly List<Transform> m_children = [];

        // Cached world transform, updated lazily
        private Vector3 m_worldPosition = Vector3.Zero;
        private Quaternion m_worldRotation = Quaternion.Identity;
        private Vector3 m_worldScale = Vector3.One;

        // Dirty flag to mark transform changes needing update
        private bool m_isDirty = true;
        
        public override ComponentTag OrderTag => ComponentTag.Transform;

        #region Local Properties

        /// <summary>
        /// Local position relative to parent transform.
        /// </summary>
        public Vector3 LocalPosition
        {
            get => m_localPosition;
            set { m_localPosition = value; MarkDirty(); }
        }

        /// <summary>
        /// Local rotation relative to parent transform.
        /// </summary>
        public Quaternion LocalRotation
        {
            get => m_localRotation;
            set { m_localRotation = value; MarkDirty(); }
        }

        /// <summary>
        /// Local scale relative to parent transform.
        /// </summary>
        public Vector3 LocalScale
        {
            get => m_localScale;
            set { m_localScale = value; MarkDirty(); }
        }

        #endregion

        #region World Properties

        /// <summary>
        /// World position in global space.
        /// </summary>
        public Vector3 WorldPosition
        {
            get
            {
                UpdateIfDirty();
                return m_worldPosition;
            }
        }

        /// <summary>
        /// World rotation in global space.
        /// </summary>
        public Quaternion WorldRotation
        {
            get
            {
                UpdateIfDirty();
                return m_worldRotation;
            }
        }

        /// <summary>
        /// World scale in global space.
        /// </summary>
        public Vector3 WorldScale
        {
            get
            {
                UpdateIfDirty();
                return m_worldScale;
            }
        }

        #endregion

        /// <summary>
        /// Parent transform. Null if root.
        /// </summary>
        public Transform? Parent { get; private set; }

        /// <summary>
        /// Read-only list of children transforms.
        /// </summary>
        public IReadOnlyList<Transform> Children => m_children;

        /// <summary>
        /// Sets the parent transform.
        /// If worldPositionStays is true, keeps the world transform unchanged after reparenting.
        /// </summary>
        public void SetParent(Transform? newParent, bool worldPositionStays = true)
        {
            if (Parent == newParent)
                return;

            UpdateIfDirty(); // Ensure current world transform is up to date

            if (worldPositionStays)
            {
                // Calculate new local transform to keep world transform same after reparent
                var currentWorldPos = WorldPosition;
                var currentWorldRot = WorldRotation;
                var currentWorldScale = WorldScale;

                // Remove from old parent
                Parent?.m_children.Remove(this);

                Parent = newParent;
                Parent?.m_children.Add(this);

                // Compute new local transform from world transform and new parent
                if (Parent == null)
                {
                    LocalPosition = currentWorldPos;
                    LocalRotation = currentWorldRot;
                    LocalScale = currentWorldScale;
                }
                else
                {
                    var invParentRot = Quaternion.Inverse(Parent.WorldRotation);
                    var parentScale = Parent.WorldScale;
                    
                    var delta = (currentWorldPos - Parent.WorldPosition);
                    var scaled = new Vector3(delta.X / parentScale.X, delta.Y / parentScale.Y, delta.Z / parentScale.Z);
                    
                    LocalPosition = Vector3.Transform(scaled, invParentRot);
                    LocalRotation = invParentRot * currentWorldRot;
                    LocalScale = new Vector3(
                        currentWorldScale.X / parentScale.X,
                        currentWorldScale.Y / parentScale.Y,
                        currentWorldScale.Z / parentScale.Z
                    );
                }
            }
            else
            {
                // Just reparent, local transform remains same
                Parent?.m_children.Remove(this);
                Parent = newParent;
                Parent?.m_children.Add(this);
            }

            MarkDirty();
        }
        
        private void MarkDirty()
        {
            m_isDirty = true;
            foreach (var child in m_children)
                child.MarkDirty();
        }
        
        private void UpdateIfDirty()
        {
            if (!m_isDirty)
                return;

            if (Parent == null)
            {
                m_worldPosition = m_localPosition;
                m_worldRotation = m_localRotation;
                m_worldScale = m_localScale;
            }
            else
            {
                // Compose world transform: scale, rotate, translate
                m_worldScale = new Vector3(
                    m_localScale.X * Parent.WorldScale.X,
                    m_localScale.Y * Parent.WorldScale.Y,
                    m_localScale.Z * Parent.WorldScale.Z
                );

                m_worldRotation = Parent.WorldRotation * m_localRotation;
                
                var scaled = new Vector3(m_localPosition.X * Parent.WorldScale.X, m_localPosition.Y * Parent.WorldScale.Y, m_localPosition.Z * Parent.WorldScale.Z);
                var rotated = Vector3.Transform(scaled, Parent.WorldRotation);

                m_worldPosition = Parent.WorldPosition + rotated;
            }

            m_isDirty = false;
        }

        /// <summary>
        /// Updates this transform (called each frame by ECS).
        /// </summary>
        public override void Update()
        {
            UpdateIfDirty();
        }

        /// <summary>
        /// Called when the component is detached, cleans up parent and children references.
        /// </summary>
        public override void OnDetach()
        {
            SetParent(null);
            foreach (var child in m_children.ToArray())
            {
                child.SetParent(null);
            }
            m_children.Clear();
        }
    }
}
