using System;
using Microsoft.Maui.Controls;
using Mocale.Json.Abstractions;
using Mocale.Json.Models;

namespace Mocale.Json.Managers
{
    public class JsonConfigurationManager : IJsonConfigurationManager
    {
        private IJsonResourcesConfig configuration;

        public IJsonResourcesConfig GetConfiguration()
        {
            return configuration ?? new JsonResourcesConfig();
        }

        public void SetConfiguration(IJsonResourcesConfig configuration)
        {
            this.configuration = configuration;
        }
    }
}

