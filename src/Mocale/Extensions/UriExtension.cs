namespace Mocale.Extensions;

internal static class UriExtension
{
    public static Uri Append(this Uri uri, params string[] paths)
    {
        // https://stackoverflow.com/a/7993235/8828057
        return new Uri(paths.Aggregate(uri.AbsoluteUri, (current, path) => string.Format("{0}/{1}", current.TrimEnd('/'), path.TrimStart('/'))));
    }

    public static bool TryAppend(this Uri uri, out Uri result, params string[] paths)
    {
        result = default!;

        try
        {
            result = uri.Append(paths);

            return true;
        }
        catch
        {
            return false;
        }
    }
}
