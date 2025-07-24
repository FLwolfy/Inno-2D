namespace InnoEngine.Serialization
{
    [AttributeUsage(AttributeTargets.Property)]
    public class SerializablePropertyAttribute : Attribute
    {
        public PropertyVisibility propertyVisibility { get; }

        public SerializablePropertyAttribute(PropertyVisibility visibility = PropertyVisibility.Show)
        {
            propertyVisibility = visibility;
        }
    }
}