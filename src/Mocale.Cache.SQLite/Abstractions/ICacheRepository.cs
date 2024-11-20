using System.Globalization;

namespace Mocale.Cache.SQLite.Abstractions;

/// <summary>
/// Cache Repository
/// </summary>
internal interface ICacheRepository
{
    /// <summary>
    /// Add Or Update Item
    /// </summary>
    /// <param name="cultureInfo">The culture to add / update</param>
    /// <param name="lastUpdated">The last time this culture was updated</param>
    /// <returns>Success</returns>
    bool AddOrUpdateItem(CultureInfo cultureInfo, DateTime lastUpdated);

    /// <summary>
    /// Get update item
    /// </summary>
    /// <param name="cultureInfo">The culture to check for</param>
    /// <returns>Update record if it exists</returns>
    UpdateHistoryItem? GetItem(CultureInfo cultureInfo);

    /// <summary>
    /// Delete all records
    /// </summary>
    /// <returns>Success</returns>
    bool DeleteAll();

    /// <summary>
    /// Delete the given culture
    /// </summary>
    /// <param name="cultureInfo">The culture to delete</param>
    /// <returns>Success</returns>
    bool DeleteItem(CultureInfo cultureInfo);
}
