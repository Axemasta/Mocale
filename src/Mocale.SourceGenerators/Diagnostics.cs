using Microsoft.CodeAnalysis;
namespace Mocale.SourceGenerators;

internal static class Diagnostics
{
    /*
     * I am using diagnostics to perform logging, after some research it seemed like this was one of the easier approaches...
     *
     * If what i am doing here is sacrilege, please raise an issue and let me know of a better way to add diagnostics to the generators 😇
     */

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

        public static readonly DiagnosticDescriptor DuplicateKey = Create("MOCW003", "Duplicate key detected: Sanitized Key '{0}' conflicts with Original Key '{1}'");
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
