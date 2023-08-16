using Microsoft.CodeAnalysis;
namespace Mocale.SourceGenerators;

internal static class Diagnostics
{
    public static class Errors
    {
        private static DiagnosticDescriptor Create(string id, string text)
        {
            return new DiagnosticDescriptor(id, text, text, "Mocale.SourceGenerators", DiagnosticSeverity.Error, true);
        }

        public static readonly DiagnosticDescriptor ParsingException = Create("MOCE001", "An exception occurred processing translations, Exception: {0}, Json {1}");
    }

    public static class Warnings
    {
        private static DiagnosticDescriptor Create(string id, string text)
        {
            return new DiagnosticDescriptor(id, text, text, "Mocale.SourceGenerators", DiagnosticSeverity.Warning, true);
        }

        public static readonly DiagnosticDescriptor NoLocalizationFilesDetected = Create("MOCW001", "No localizations files were found to process");

        public static readonly DiagnosticDescriptor FileNotJsonLocalization = Create("MOCW002", "The following file was not recognized as a valid localization json file: {0}");
    }

    public static class Information
    {
        private static DiagnosticDescriptor Create(string id, string text)
        {
            return new DiagnosticDescriptor(id, text, text, "Mocale.SourceGenerators", DiagnosticSeverity.Info, false);
        }

        public static readonly DiagnosticDescriptor ProcessingFiles = Create("MOCI001", "Processing translation files");
    }
}
