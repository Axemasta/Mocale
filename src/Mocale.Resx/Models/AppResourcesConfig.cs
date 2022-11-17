using System.Reflection;
using Mocale.Resx.Abstractions;

namespace Mocale.Resx.Models;

public class AppResourcesConfig : IAppResourcesConfig
{
    public Assembly ResourceAssembly { get; set; }
}
