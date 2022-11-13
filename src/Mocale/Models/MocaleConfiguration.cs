using Mocale.Abstractions;
using Mocale.Enums;

namespace Mocale.Models
{
    public class MocaleConfiguration : IMocaleConfiguration
    {
        public LocalResourceType ResourceType { get; set; }
    }
}
