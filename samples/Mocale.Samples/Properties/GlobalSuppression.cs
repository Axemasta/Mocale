#pragma warning disable IDE0076
/*
 * Platform Suppressions
 *
 * The dotnet analyzers don't work properly when suppressions apply only to files not currently beign targetted.
 * ie a suppression rule for iOS / macOS won't apply when targetting windows / android and the compiler with
 * erroniously assume the suppression is invalid.
 */
using System.Diagnostics.CodeAnalysis;

[assembly: SuppressMessage("Naming", "CA1711:Identifiers should not have incorrect suffix", Justification = "iOS / macOS Platform Name", Scope = "type", Target = "~T:Mocale.Samples.AppDelegate")]
#pragma warning restore IDE0076
