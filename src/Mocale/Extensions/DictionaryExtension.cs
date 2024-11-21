namespace Mocale.Extensions;

internal static class DictionaryExtension
{
    public static void AddOrUpdateValues(this Dictionary<string, string> currentValues, Dictionary<string, string> updatedValues)
    {
        var newValues = updatedValues.Where(uv => !currentValues.ContainsKey(uv.Key))
            .ToList();

        if (newValues.Count > 0)
        {
            foreach (var newValue in newValues)
            {
                currentValues.Add(newValue.Key, newValue.Value);
            }
        }

        var modifiedValues = updatedValues.Where(
            uv => currentValues.ContainsKey(uv.Key) &&
                  !currentValues[uv.Key].Equals(uv.Value, StringComparison.Ordinal))
            .ToList();

        if (modifiedValues.Count > 0)
        {
            foreach (var modifiedValue in modifiedValues)
            {
                currentValues[modifiedValue.Key] = modifiedValue.Value;
            }
        }
    }
}
