using Microsoft.CodeAnalysis;

namespace Mocale.SourceGenerators;

internal static class SourceProductionContextExtensions
{
    public static void Report(this SourceProductionContext context, DiagnosticDescriptor diagnosticDescriptor, params object?[]? args) =>
        context.ReportDiagnostic(Diagnostic.Create(diagnosticDescriptor, null, args));
}

