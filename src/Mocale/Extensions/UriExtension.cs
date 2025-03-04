namespace Mocale.Extensions;

internal static class UriExtension
{
    public static Uri Append(this Uri uri, params string[] paths)
    {
        // https://stackoverflow.com/a/7993235/8828057
        return new Uri(paths.Aggregate(uri.AbsoluteUri, (current, path) => $"{current.TrimEnd('/')}/{path.TrimStart('/')}"));
    }

    public static bool TryAppend(this Uri uri, out Uri result, params string[] paths)
    {
        result = default!;

        var appendedUri = uri.Append(paths);

        var wellFormed = Uri.IsWellFormedUriString(appendedUri.ToString(), UriKind.Absolute);

        if (wellFormed)
        {
            result = appendedUri;
        }

        return wellFormed;
    }
}
