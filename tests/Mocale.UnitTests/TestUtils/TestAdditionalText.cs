using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;

namespace Mocale.UnitTests.TestUtils;

public class TestAdditionalText(string path, string content) : AdditionalText
{
    public override string Path => path;

    public override SourceText GetText(CancellationToken cancellationToken = default)
    {
        return SourceText.From(content);
    }
}
