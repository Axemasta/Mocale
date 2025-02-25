// This file is used by Code Analysis to maintain SuppressMessage
// attributes that are applied to this project.
// Project-level suppressions either have no target or are given
// a specific target and scoped to a namespace, type, member, etc.

using System.Diagnostics.CodeAnalysis;

[assembly: SuppressMessage("Naming", "CA1707:Identifiers should not contain underscores", Justification = "Test naming convention uses underscores")]
[assembly: SuppressMessage("Performance", "CA1848:Use the LoggerMessage delegates", Justification = "This is a unit test project")]
[assembly: SuppressMessage("Usage", "CA2201:Do not raise reserved exception types", Justification = "Unit tests will raise bogus exceptions for test cases")]
[assembly: SuppressMessage("Globalization", "CA1304:Specify CultureInfo", Justification = "I'm explicitly testing this method...", Scope = "member", Target = "~M:Mocale.UnitTests.Cache.InMemoryCacheManagerTests.ClearCache_CallsClearMethod")]
