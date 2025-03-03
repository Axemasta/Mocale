using Mocale.Extensions;

namespace Mocale.UnitTests.Extensions;

public class UriExtensionTests
{
    [Fact]
    public void Append_ShouldAppendPathsCorrectly()
    {
        // Arrange
        var baseUri = new Uri("https://example.com/api");
        var paths = new[] { "v1", "users" };

        // Act
        var result = baseUri.Append(paths);

        // Assert
        Assert.Equal("https://example.com/api/v1/users", result.ToString());
    }

    [Fact]
    public void Append_ShouldHandleTrailingAndLeadingSlashes()
    {
        // Arrange
        var baseUri = new Uri("https://example.com/api/");
        var paths = new[] { "/v1/", "/users/" };

        // Act
        var result = baseUri.Append(paths);

        // Assert
        Assert.Equal("https://example.com/api/v1/users/", result.ToString());
    }

    [Fact]
    public void TryAppend_ShouldReturnTrueAndAppendPaths()
    {
        // Arrange
        var baseUri = new Uri("https://example.com/api");
        var paths = new[] { "v1", "users" };

        // Act
        var success = baseUri.TryAppend(out var result, paths);

        // Assert
        Assert.True(success);
        Assert.Equal("https://example.com/api/v1/users", result.ToString());
    }

    [Fact]
    public void TryAppend_ShouldReturnFalseOnInvalidUri()
    {
        // Arrange
        var baseUri = new Uri("https://example.com/api");
        var paths = new[] { "\0invalid\turi<<{{>}_!!##" };

        // Act
        var success = baseUri.TryAppend(out var result, paths);

        // Assert
        Assert.False(success);
        Assert.Null(result);
    }
}
