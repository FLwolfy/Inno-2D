using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace Inno.Assets;

/// <summary>
/// Interface for asset loaders
/// </summary>
internal interface IAssetLoader
{
    protected static readonly IDeserializer DESERIALIZER = new DeserializerBuilder()
        .WithNamingConvention(CamelCaseNamingConvention.Instance)
        .IgnoreUnmatchedProperties()
        .Build();

    protected static readonly ISerializer SERIALIZER = new SerializerBuilder()
        .WithNamingConvention(CamelCaseNamingConvention.Instance)
        .Build();
    
    /// <summary>
    /// Load the asset from disk / raw file
    /// </summary>
    InnoAsset? Load(string path);
}