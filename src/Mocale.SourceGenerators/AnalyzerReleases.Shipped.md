; Shipped analyzer releases
; https://github.com/dotnet/roslyn-analyzers/blob/main/src/Microsoft.CodeAnalysis.Analyzers/ReleaseTrackingAnalyzers.Help.md

## Release 1.0

### New Rules

| Rule ID | Category | Severity    | Notes                                                                                             |
|---------|----------|-------------|---------------------------------------------------------------------------------------------------|
| MOCE001 | Usage    | Error       | MOCE001_ParsingException, Raised if an exception is thrown during translation file parsing        |
| MOCW001 | Usage    | Warning     | MOCW001_NoLocalizationFilesDetected, Raised if no translation files are discovered for parsing    |
| MOCW002 | Usage    | Warning     | MOCW002_FileNotJsonLocalization, Raised when a file processed by the generator is not appropriate |
| MOCI001 | Usage    | Information | MOCI001_ProcessingFiles, Raised when the generator starts processing files                        |
