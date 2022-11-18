using System;
using System.Reflection;
using Mocale.Json.Abstractions;

namespace Mocale.Json.Models
{
    public class JsonResourcesConfig : IJsonResourcesConfig
    {
        public string ResourcesPath { get; set; } = "Locales";

        public Assembly ResourcesAssembly { get; set; }

        public bool UseResourceFolder { get; set; } = true;
    }
}

