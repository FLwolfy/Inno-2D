namespace InnoEngine.Serialization
{
    [AttributeUsage(AttributeTargets.Property)]
    public class SerializablePropertyAttribute : Attribute
    {
        /// <summary>
        /// The visibility of the property.
        /// </summary>
        public SerializedProperty.PropertyVisibility propertyVisibility { get; }

        /// <summary>
        /// Creates a new SerializablePropertyAttribute with the specified visibility.
        /// Default visibility is Show.
        /// </summary>
        public SerializablePropertyAttribute(SerializedProperty.PropertyVisibility visibility = SerializedProperty.PropertyVisibility.Show)
        {
            propertyVisibility = visibility;
        }
    }
}