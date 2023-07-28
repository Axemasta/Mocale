using System.Globalization;
using Microsoft.Extensions.Logging;
namespace Mocale.Cache.SQLite.Repositories;

public class TranslationsRepository : RepositoryBase, ITranslationsRepository
{
    #region Constructors

    public TranslationsRepository(
        IDatabaseConnectionProvider databaseConnectionProvider,
        ILogger<TranslationsRepository> logger)
        : base(
            databaseConnectionProvider,
            logger)
    {
        Connection.CreateTable<TranslationItem>();
    }

    #endregion Constructors

    #region Methods

    private static List<TranslationItem> CreateEntities(CultureInfo cultureInfo, Dictionary<string, string> translations)
    {
        var entities = new List<TranslationItem>();

        foreach (var translation in translations)
        {
            entities.Add(new TranslationItem
            {
                CultureName = cultureInfo.Name,
                Key = translation.Key,
                Value = translation.Value,
            });
        }

        return entities;
    }

    #endregion Methods

    #region Interface Implementations

    /// <inheritdoc />
    public Dictionary<string, string>? GetTranslations(CultureInfo cultureInfo)
    {
        try
        {
            var entities = Connection.Table<TranslationItem>()
                .Where(t => t.CultureName == cultureInfo.Name)
                .ToList();

            if (entities is null || !entities.Any())
            {
                logger.LogTrace("No cached translations found for culture: {CultureName}", cultureInfo.Name);
                return null;
            }

            return entities.ToDictionary(t => t.Key, t => t.Value);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An exception occurred retrieving translations for culture: {CultureName}", cultureInfo);
            return null;
        }
    }

    /// <inheritdoc />
    public bool AddTranslations(CultureInfo cultureInfo, Dictionary<string, string> translations)
    {
        try
        {
            // Check existing
            var currentEntities = Connection.Table<TranslationItem>()
                .Where(t => t.CultureName == cultureInfo.Name)
                .ToList();

            var transactionHistory = new List<bool>();

            var newValues = translations.Where(t => currentEntities.All(c => c.Key != t.Key))
                .ToList();

            if (newValues.Any())
            {
                var newTranslations = translations.Where(t => currentEntities.All(c => c.Key != t.Key))
                    .ToDictionary(d => d.Key, d => d.Value);

                var entities = CreateEntities(cultureInfo, newTranslations);

                var rows = Connection.InsertAll(entities);

                transactionHistory.Add(rows == entities.Count);
            }

            var deletedValues = currentEntities.Where(t => translations.All(c => c.Key != t.Key))
                .ToList();

            if (deletedValues.Any())
            {
                var rows = 0;

                foreach (var valueToDelete in deletedValues)
                {
                    rows += Connection.Delete(valueToDelete);
                }

                transactionHistory.Add(rows == deletedValues.Count);
            }

            var updatedValues = currentEntities.Where(t => translations.Any(c => c.Key == t.Key && c.Value != t.Value))
                .ToList();

            if (updatedValues.Any())
            {
                foreach (var translation in updatedValues)
                {
                    var newValue = translations.FirstOrDefault(t => t.Key == translation.Key);

                    translation.Value = newValue.Value;
                }

                var rows = Connection.UpdateAll(updatedValues);

                transactionHistory.Add(rows == updatedValues.Count);
            }

            return transactionHistory.All(t => t);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An exception occurred adding translations to the database for culture: {CultureName}", cultureInfo.Name);
            return false;
        }
    }


    /// <inheritdoc />
    public bool DeleteTranslations(CultureInfo cultureInfo)
    {
        try
        {
            var entities = Connection.Table<TranslationItem>()
                .Where(t => t.CultureName == cultureInfo.Name)
                .ToList();

            if (entities is null || !entities.Any())
            {
                logger.LogWarning("No translations to delete for culture: {CultureName}", cultureInfo.Name);
                return false;
            }

            var expectedCount = entities.Count;

            var rows = Connection.Delete(entities);

            return expectedCount == rows;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An exception occurred deleting translations for culture: {CultureName}", cultureInfo);
            return false;
        }
    }

    #endregion Interface Implementations
}
