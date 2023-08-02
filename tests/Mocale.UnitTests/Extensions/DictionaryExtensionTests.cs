using Mocale.Extensions;
namespace Mocale.UnitTests.Extensions;

public class DictionaryExtensionTests
{
    [Fact]
    public void AddOrUpdateValues_WhenCurrentValueIsEmpty_ShouldAddNewValues()
    {
        // Arrange
        var current = new Dictionary<string, string>();
        var updated = new Dictionary<string, string>()
        {
            { "Key One", "Value One" },
            { "Key Two", "Value Two" },
        };

        // Act
        current.AddOrUpdateValues(updated);

        // Assert
        Assert.True(current.Count == 2);
        current.Should().BeEquivalentTo(new Dictionary<string, string>()
        {
            { "Key One", "Value One" },
            { "Key Two", "Value Two" },
        });
    }

    [Fact]
    public void AddOrUpdateValues_WhenCurrentValueHasEntriesAndNewValuesAreAllDistinct_ShouldAddNewValues()
    {
        // Arrange
        var current = new Dictionary<string, string>()
        {
            { "Key One", "Value One" },
            { "Key Two", "Value Two" },
        };

        var updated = new Dictionary<string, string>()
        {
            { "Key Three", "Value Three" },
            { "Key Four", "Value Four" },
        };

        // Act
        current.AddOrUpdateValues(updated);

        // Assert
        Assert.True(current.Count == 4);
        current.Should().BeEquivalentTo(new Dictionary<string, string>()
        {
            { "Key One", "Value One" },
            { "Key Two", "Value Two" },
            { "Key Three", "Value Three" },
            { "Key Four", "Value Four" },
        });
    }

    [Fact]
    public void AddOrUpdateValues_WhenCurrentValueHasEntriesAndAllValuesAreSame_ShouldAddNoValues()
    {
        // Arrange
        var current = new Dictionary<string, string>()
        {
            { "Key One", "Value One" },
            { "Key Two", "Value Two" },
        };

        var updated = new Dictionary<string, string>()
        {
            { "Key One", "Value One" },
            { "Key Two", "Value Two" },
        };

        // Act
        current.AddOrUpdateValues(updated);

        // Assert
        Assert.True(current.Count == 2);
        current.Should().BeEquivalentTo(new Dictionary<string, string>()
        {
            { "Key One", "Value One" },
            { "Key Two", "Value Two" },
        });
    }

    [Fact]
    public void AddOrUpdateValues_WhenCurrentValueHasEntriesAndNewValuesAreDifferent_ShouldUpdateExistingValues()
    {
        // Arrange
        var current = new Dictionary<string, string>()
        {
            { "Key One", "Value One" },
            { "Key Two", "Value Two" },
        };

        var updated = new Dictionary<string, string>()
        {
            { "Key Two", "Value 2" },
        };

        // Act
        current.AddOrUpdateValues(updated);

        // Assert
        Assert.True(current.Count == 2);
        current.Should().BeEquivalentTo(new Dictionary<string, string>()
        {
            { "Key One", "Value One" },
            { "Key Two", "Value 2" },
        });
    }

    [Fact]
    public void AddOrUpdateValues_WhenSomeNewValuesAndSomeUpdatedValues_ShouldAddAndUpdateValues()
    {
        // Arrange
        var current = new Dictionary<string, string>()
        {
            { "Key One", "Value One" },
            { "Key Two", "Value Two" },
        };

        var updated = new Dictionary<string, string>()
        {
            { "Key Two", "Value 2" },
            { "Key Three", "Value Three" },
        };

        // Act
        current.AddOrUpdateValues(updated);

        // Assert
        Assert.True(current.Count == 3);
        current.Should().BeEquivalentTo(new Dictionary<string, string>()
        {
            { "Key One", "Value One" },
            { "Key Two", "Value 2" },
            { "Key Three", "Value Three" },
        });
    }
}
