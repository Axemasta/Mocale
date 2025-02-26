using System.ComponentModel;
using Mocale.Extensions;

namespace Mocale.UnitTests.Extensions;

public class EnumExtensionsTests
{
    private enum TestEnum
    {
        [Description("Test Description")] ValueWithAttribute,
        ValueWithoutAttribute
    }

    [Fact]
    public void GetAttributeValue_WhenTypeIsNotAttribute_ShouldThrow()
    {
        // Arrange
        var enumValue = TestEnum.ValueWithAttribute;
        var invalidType = typeof(string);

        // Act & Assert
        var exception = Assert.Throws<ArgumentException>(() =>
            enumValue.GetAttributeValue(invalidType, "SomeProperty"));

        Assert.Contains("is not an attribute", exception.Message);
    }

    [Fact]
    public void GetAttributeValue_WhenTargetDoesNotHaveAttribute_ShouldReturnNull()
    {
        // Arrange
        var enumValue = TestEnum.ValueWithoutAttribute;
        var attributeType = typeof(DescriptionAttribute);

        // Act
        var result = enumValue.GetAttributeValue(attributeType, "Description");

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public void GetAttributeValue_WhenTargetPropertyDoesNotExist_ShouldReturnNull()
    {
        // Arrange
        var enumValue = TestEnum.ValueWithAttribute;
        var attributeType = typeof(DescriptionAttribute);
        var nonExistentProperty = "NonExistentProperty";

        // Act
        var result = enumValue.GetAttributeValue(attributeType, nonExistentProperty);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public void GetAttributeValue_WhenAttributeAndTargetPropertyExist_ShouldGetValue()
    {
        // Arrange
        var enumValue = TestEnum.ValueWithAttribute;
        var attributeType = typeof(DescriptionAttribute);
        var propertyName = "Description";

        // Act
        var result = enumValue.GetAttributeValue(attributeType, propertyName);

        // Assert
        Assert.Equal("Test Description", result);
    }
}
