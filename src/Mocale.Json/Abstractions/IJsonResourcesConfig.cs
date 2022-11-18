using System;
using System.Reflection;

namespace Mocale.Json.Abstractions
{
    /// <summary>
    /// Configuration for mocale json resources
    /// </summary>
    public interface IJsonResourcesConfig
    {
        /// <summary>
        /// Path to the resources directory
        /// </summary>
        string ResourcesPath { get; }

        /// <summary>
        /// Assembly that contains the resources
        /// </summary>
        Assembly ResourcesAssembly { get; }

        /// <summary>
        /// Whether the resources live inside the Maui Resources folder or relative to the assembly
        /// </summary>
        bool UseResourceFolder { get; }
    }
}

