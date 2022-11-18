using System;
using System.Reflection;

namespace Mocale.Json.Abstractions
{
    public interface IJsonResourcesConfig
    {
        string ResourcesPath { get; }

        Assembly ResourcesAssembly { get; }
    }
}

