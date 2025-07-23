using InnoEditor.PropertyRenderer;

public static class PropertyRendererRegistry
{
    private static readonly Dictionary<Type, IPropertyRenderer> RENDERERS = new();
    private static readonly Dictionary<Type, Type> OPEN_GENERIC_RENDERERS = new();

    internal static void Initialize()
    {
        var assemblies = AppDomain.CurrentDomain.GetAssemblies();

        foreach (var assembly in assemblies)
        {
            foreach (var type in assembly.GetTypes())
            {
                if (type.IsAbstract || type.IsInterface)
                    continue;

                var baseType = type.BaseType;
                while (baseType != null)
                {
                    if (baseType.IsGenericType && baseType.GetGenericTypeDefinition() == typeof(PropertyRenderer<>))
                    {
                        if (type.IsGenericTypeDefinition)
                        {
                            var genericArg = baseType.GetGenericArguments()[0];
                            var genericParam = genericArg.IsGenericType ? genericArg.GetGenericTypeDefinition() : genericArg;
                            OPEN_GENERIC_RENDERERS[genericParam] = type;
                        }
                        else
                        {
                            var instance = Activator.CreateInstance(type) as IPropertyRenderer;
                            if (instance != null)
                            {
                                var genericArg = baseType.GetGenericArguments()[0];
                                RENDERERS[genericArg] = instance;
                            }
                        }
                        break;
                    }
                    baseType = baseType.BaseType;
                }
            }
        }
    }

    public static IPropertyRenderer? GetRenderer(Type type)
    {
        if (RENDERERS.TryGetValue(type, out var renderer))
            return renderer;

        if (type.IsGenericType)
        {
            var genericDef = type.GetGenericTypeDefinition();

            if (OPEN_GENERIC_RENDERERS.TryGetValue(genericDef, out var openGenericRendererType))
            {
                var genericArgs = type.GetGenericArguments();
                
                var closedRendererType = openGenericRendererType.MakeGenericType(genericArgs);
                var instance = (IPropertyRenderer)Activator.CreateInstance(closedRendererType);
                if (instance == null)
                    return null;

                RENDERERS[type] = instance;
                return instance;
            }
        }

        return null;
    }
}
