using System.Reflection;
namespace Mocale.Models;

public class EmbeddedResourcesConfig : IEmbeddedResourcesConfig
{
    public string ResourcesPath { get; set; } = "Locales";

    public Assembly ResourcesAssembly { get; set; }

    public bool UseResourceFolder { get; set; } = true;
}
