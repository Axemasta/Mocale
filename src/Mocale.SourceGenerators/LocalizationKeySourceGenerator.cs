using System.Collections.Immutable;
using System.Text;
using System.Text.RegularExpressions;
using Humanizer;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;
using Newtonsoft.Json;
namespace Mocale.SourceGenerators;

/// <summary>
/// Localization Key Source Generator
/// </summary>
[Generator]
public class LocalizationKeySourceGenerator : IIncrementalGenerator
{
    /// <inheritdoc />
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        var constants = context.AdditionalTextsProvider
            .Where(FileMatches)
            .Select((text, token) => text.GetText(token)?.ToString())
            .Where(text => text is not null)!
            .Collect<string>();

        context.RegisterSourceOutput(constants, GenerateCode);
    }

    private bool FileMatches(AdditionalText text)
    {
        var fileName = Path.GetFileNameWithoutExtension(text.Path);
        var extension = Path.GetExtension(text.Path);

        // TODO: Match all known cultures
        var match = Regex.IsMatch(fileName, "^[A-Za-z]{2,4}([_-][A-Za-z]{4})?([_-]([A-Za-z]{2}|[0-9]{3}))?$");

        return match && extension.Equals(".json", StringComparison.OrdinalIgnoreCase);
    }

    private static Dictionary<string, string> GetTranslationUniqueKeys(List<string> translationsJson, SourceProductionContext context)
    {
        var uniqueKeys = new Dictionary<string, string>();

        context.Report(translationsJson.Count < 1
            ? Diagnostics.Warnings.NoLocalizationFilesDetected
            : Diagnostics.Information.ProcessingFiles);

        foreach (var translationJson in translationsJson)
        {
            try
            {
                var translations = JsonConvert.DeserializeObject<Dictionary<string, string>>(translationJson);

                if (translations is null)
                {
                    context.Report(Diagnostics.Warnings.FileNotJsonLocalization, translationJson);
                    continue;
                }

                foreach (var translation in translations)
                {
                    var sanitizedKey = SanitizeKey(translation.Key);

                    if (!uniqueKeys.ContainsKey(sanitizedKey))
                    {
                        // Don't report dupes since we potentially will process multiple files with the same keys
                        uniqueKeys.Add(sanitizedKey, translation.Key);
                    }
                }
            }
            catch (Exception ex)
            {
                context.Report(Diagnostics.Errors.ParsingException, ex, translationJson);
            }
        }

        return uniqueKeys;
    }

    private static string SanitizeKey(string dirtyKey)
    {
        // https://stackoverflow.com/a/11396038
        var removeChars = new HashSet<char>(" ?&^$#@!()+-,:;<>’\'-*.");
        var result = new StringBuilder(dirtyKey.Length);

        foreach (var c in dirtyKey)
        {
            if (!removeChars.Contains(c)) // prevent dirty chars
            {
                result.Append(c);
            }
        }

        return result.ToString().Pascalize();
    }

    private static void GenerateCode(SourceProductionContext context, ImmutableArray<string> translations)
    {
        var translationKeys = GetTranslationUniqueKeys([.. translations], context);

        const string translationNamespace = "Mocale.Translations";

        var source = GenerateSource(translationNamespace, translationKeys);

        context.AddSource("MocaleTranslationKeys.g.cs", SourceText.From(source, Encoding.UTF8));
    }

    private static string GenerateSource(string generatedNamespace, Dictionary<string, string> keysToGenerate)
    {
        var namespaceDeclaration = SyntaxFactory.NamespaceDeclaration(SyntaxFactory.ParseName(generatedNamespace))
            .AddMembers(
                SyntaxFactory.ClassDeclaration("TranslationKeys")
                    .AddModifiers(SyntaxFactory.Token(SyntaxKind.PublicKeyword), SyntaxFactory.Token(SyntaxKind.StaticKeyword))
                    .AddMembers([.. keysToGenerate.Select(CreateConstantField)])
            );

        var compilationUnit = SyntaxFactory.CompilationUnit()
            .AddMembers(namespaceDeclaration)
            .NormalizeWhitespace("\t");

        // https://stackoverflow.com/a/79589669/8828057
        // Thanks to silkfire - we add newlines between members after normalization
        compilationUnit = compilationUnit.ReplaceNodes(
            compilationUnit.DescendantNodes().OfType<FieldDeclarationSyntax>()
                .ToArray()
                .Take(compilationUnit.DescendantNodes().OfType<FieldDeclarationSyntax>().Count() - 1),
            (_, node) => node.WithTrailingTrivia(
                SyntaxFactory.ElasticCarriageReturnLineFeed,
                SyntaxFactory.ElasticCarriageReturnLineFeed));

        return compilationUnit.ToFullString();
    }

    private static MemberDeclarationSyntax CreateConstantField(KeyValuePair<string, string> keyValuePair)
    {
        return SyntaxFactory.FieldDeclaration(
                SyntaxFactory.VariableDeclaration(SyntaxFactory.ParseTypeName("string"))
                    .AddVariables(SyntaxFactory.VariableDeclarator(keyValuePair.Key)
                        .WithInitializer(SyntaxFactory.EqualsValueClause(SyntaxFactory.LiteralExpression(
                            SyntaxKind.StringLiteralExpression, SyntaxFactory.Literal(keyValuePair.Value))))))
            .AddModifiers(SyntaxFactory.Token(SyntaxKind.PublicKeyword), SyntaxFactory.Token(SyntaxKind.ConstKeyword))
            .WithLeadingTrivia(SyntaxFactory.TriviaList(
                 SyntaxFactory.Comment("/// <summary>"),
                 SyntaxFactory.Comment($"/// Looks up a localized string using key {keyValuePair.Value}."),
                 SyntaxFactory.Comment("/// </summary>")
            ));
    }
}
