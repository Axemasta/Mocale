using System.Globalization;
using System.Reflection;

namespace Mocale.Resx.Abstractions;

public interface IAppResourcesConfig
{
    Assembly ResourceAssembly { get; }
}
