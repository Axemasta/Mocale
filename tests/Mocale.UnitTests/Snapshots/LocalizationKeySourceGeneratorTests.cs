using Humanizer;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Mocale.SourceGenerators;
using Newtonsoft.Json;

namespace Mocale.UnitTests.Snapshots;

public class TranslationKeySourceGeneratorSnapshotTests
{
    [Fact]
    public Task GeneratesTranslationKeys_WhenSingleFile_ShouldGenerateCorrectly()
    {
        var enGbJson =
            """
            {
             "KeyOne": "Value One",
             "Key_Two": "Value Two",
             "Key@!_/Three": "Value Three",
             "Key.Four": "Value Four"
            }
            """;

        var enGbAdditionalText = new TestAdditionalText("en-GB.json", enGbJson);

        return Verify([enGbAdditionalText]);
    }

    [Fact]
    public Task GeneratesTranslationKeys_WhenMultipleFilesButAllTheSameKeys_ShouldContainOnlyUniqueKeys()
    {
        var enGbJson =
            """
            {
                "KeyOne": "Value One",
                "Key_Two": "Value Two",
                "Key@!_/Three": "Value Three",
                "Key.Four": "Value Four"
            }
            """;

        var frFrJson =
            """
            {
                "KeyOne": "Valeur Un",
                "Key_Two": "Valeur Deux",
                "Key@!_/Three": "Valeur Trois",
                "Key.Four": "Valeur Quatre"
            }
            """;

        var enGbAdditionalText = new TestAdditionalText("en-GB.json", enGbJson);
        var frFrAdditionalText = new TestAdditionalText("fr-FR.json", frFrJson);

        return Verify([enGbAdditionalText, frFrAdditionalText]);
    }

    [Fact]
    public Task GeneratesTranslationKeys_WhenMultipleFilesButDifferentKeys_ShouldContainOnlyUniqueKeys()
    {
        var enGbJson =
            """
            {
                "KeyOne": "Value One",
                "Key_Two": "Value Two",
                "Key@!_/Three": "Value Three",
                "Key.Four": "Value Four"
            }
            """;

        var frFrJson =
            """
            {
                "KeyOne": "Valeur Un",
                "Key_Two": "Valeur Deux",
                "Key.Four": "Valeur Quatre",
                "KeyFive": "Valeur Cinq",
            }
            """;

        var enGbAdditionalText = new TestAdditionalText("en-GB.json", enGbJson);
        var frFrAdditionalText = new TestAdditionalText("fr-FR.json", frFrJson);

        return Verify([enGbAdditionalText, frFrAdditionalText]);
    }

    [Fact]
    public Task GeneratesTranslationKeys_WhenNoAdditionalFiles_ShouldGenerateNoKeys()
    {
        return Verify([]);
    }

    [Fact]
    public Task GeneratesTranslationKeys_WhenKeysContainIllegalCharacters_ShouldGenerateSanitizedFields()
    {
        var enGbJson =
            """
            {
              "Key?Zero" : "Value for ?",
              "Key&One" : "Value for &",
              "Key^Two" : "Value for ^",
              "Key$Three" : "Value for $",
              "Key#Four" : "Value for #",
              "Key@Five" : "Value for @",
              "Key!Six" : "Value for !",
              "Key(Seven" : "Value for (",
              "Key)Eight" : "Value for )",
              "Key+Nine" : "Value for +",
              "Key-Ten" : "Value for -",
              "Key,Eleven" : "Value for ,",
              "Key:Twelve" : "Value for :",
              "Key;Thirteen" : "Value for ;",
              "Key<Fourteen" : "Value for <",
              "Key>Fifteen" : "Value for >",
              "Key’Sixteen" : "Value for ’",
              "Key'Seventeen" : "Value for '",
              "Key\\Eighteen" : "Value for \\",
              "Key*Nineteen" : "Value for *",
              "Key.Twenty" : "Value for .",
              "Key/Twenty-one" : "Value for /"
            }
            """;

        var enGbAdditionalText = new TestAdditionalText("en-GB.json", enGbJson);

        return Verify([enGbAdditionalText]);
    }

    public static Task Verify(List<AdditionalText> additionalTexts)
    {
        // Create a Roslyn compilation for the syntax tree.
        var compilation = CSharpCompilation.Create(
            assemblyName: "Tests");

        // Create an instance of our LocalizationKeySourceGenerator incremental source generator
        var generator = new LocalizationKeySourceGenerator();

        // The GeneratorDriver is used to run our generator against a compilation
        GeneratorDriver driver;

        if (additionalTexts.Count != 0)
        {
            driver = CSharpGeneratorDriver.Create(generator)
                .AddAdditionalTexts([..additionalTexts]);
        }
        else
        {
            driver = CSharpGeneratorDriver.Create(generator);
        }

        // Run the source generator!
        driver = driver.RunGenerators(compilation);

        // Use verify to snapshot test the source generator output!
        return Verifier.Verify(driver);
    }
}
