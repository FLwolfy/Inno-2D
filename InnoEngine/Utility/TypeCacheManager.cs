using System.Reflection;

namespace InnoEngine.Utility;

public static class TypeCacheManager
{
    private const string C_ASSEMBLYCOMPANY_NAME = "Inno";
    
    private static readonly Dictionary<Type, List<Type>> SUBCLASS_CACHE = new();
    private static readonly Dictionary<Type, List<Type>> INTERFACE_CACHE = new();
    private static readonly Dictionary<Type, List<Type>> ATTRIBUTE_CACHE = new();

    private static bool m_isDirty = true;
    
    public static event Action? OnRefreshed;

    internal static void Initialize()
    {
        AppDomain.CurrentDomain.AssemblyLoad += (_, __) =>
        {
            m_isDirty = true;
        };
        
        Refresh();
    }

    private static void Refresh()
    {
        var assemblies = AppDomain.CurrentDomain.GetAssemblies()
            .Where(a =>
            {
                if (a.IsDynamic) return false;
                var attr = a.GetCustomAttribute<AssemblyCompanyAttribute>();
                return attr != null && attr.Company == C_ASSEMBLYCOMPANY_NAME;
            });

        var allTypes = assemblies
            .SelectMany(a =>
            {
                try { return a.GetTypes(); }
                catch { return Type.EmptyTypes; }
            })
            .ToArray();

        SUBCLASS_CACHE.Clear();
        INTERFACE_CACHE.Clear();
        ATTRIBUTE_CACHE.Clear();

        foreach (var type in allTypes)
        {
            if (type.IsAbstract) continue;

            // Index by base type
            var baseType = type.BaseType;
            while (baseType != null && baseType != typeof(object))
            {
                if (!SUBCLASS_CACHE.TryGetValue(baseType, out var list))
                    SUBCLASS_CACHE[baseType] = list = new();
                list.Add(type);
                baseType = baseType.BaseType;
            }

            // Index by interfaces
            foreach (var iface in type.GetInterfaces())
            {
                if (!INTERFACE_CACHE.TryGetValue(iface, out var list))
                    INTERFACE_CACHE[iface] = list = new();
                list.Add(type);
            }

            // Index by attributes
            foreach (var attr in type.GetCustomAttributes(inherit: true))
            {
                var attrType = attr.GetType();
                if (!ATTRIBUTE_CACHE.TryGetValue(attrType, out var list))
                    ATTRIBUTE_CACHE[attrType] = list = new();
                list.Add(type);
            }
        }

        m_isDirty = false;
        OnRefreshed?.Invoke();
    }

    public static IReadOnlyList<Type> GetSubTypesOf<T>()
    {
        if (m_isDirty) Refresh();
        if (SUBCLASS_CACHE.TryGetValue(typeof(T), out var list)) return list;
        return [];
    }

    public static IReadOnlyList<Type> GetTypesImplementing<TInterface>()
    {
        if (m_isDirty) Refresh();
        if (INTERFACE_CACHE.TryGetValue(typeof(TInterface), out var list)) return list;
        return [];
    }

    public static IReadOnlyList<Type> GetTypesWithAttribute<TAttr>() where TAttr : Attribute
    {
        if (m_isDirty) Refresh();
        if (ATTRIBUTE_CACHE.TryGetValue(typeof(TAttr), out var list)) return list;
        return [];
    }
}
