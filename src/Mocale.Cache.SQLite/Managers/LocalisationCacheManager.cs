using System.Globalization;
using Microsoft.Extensions.Logging;
using Mocale.Abstractions;

namespace Mocale.Cache.SQLite.Managers;

internal partial class LocalisationCacheManager(
	ICacheUpdateManager cacheUpdateManager,
	ILogger<LocalisationCacheManager> logger,
	ITranslationsRepository translationsRepository)
	: ILocalisationCacheManager
{
	#region Fields

	private readonly ICacheUpdateManager cacheUpdateManager = Guard.Against.Null(cacheUpdateManager);
	private readonly ILogger logger = Guard.Against.Null(logger);
	private readonly ITranslationsRepository translationsRepository = Guard.Against.Null(translationsRepository);

	#endregion Fields

	#region Interface Implementations

	public Dictionary<string, string>? GetCachedLocalizations(CultureInfo cultureInfo)
	{
		return translationsRepository.GetTranslations(cultureInfo);
	}

	public bool SaveCachedLocalizations(CultureInfo cultureInfo, Dictionary<string, string> localizations)
	{
		var saved = translationsRepository.AddTranslations(cultureInfo, localizations);

		if (!saved)
		{
			LogFailedToAddTranslationsForCultureCultureName(cultureInfo.Name);
			return false;
		}

		var cacheUpdated = cacheUpdateManager.SetCacheUpdated(cultureInfo);

		if (!cacheUpdated)
		{
			LogTranslationsForCultureCultureNameWereSavedToTheCacheDatabaseButTheCacheHistoryWasNotUpdated(cultureInfo.Name);
		}

		return cacheUpdated;
	}

	#endregion Interface Implementations

	#region Logging

	[LoggerMessage(LogLevel.Warning, "Failed to add translations for culture: {CultureName}")]
	partial void LogFailedToAddTranslationsForCultureCultureName(string cultureName);

	[LoggerMessage(LogLevel.Warning, "Translations for culture: {CultureName} were saved to the cache database but the cache history was not updated")]
	partial void LogTranslationsForCultureCultureNameWereSavedToTheCacheDatabaseButTheCacheHistoryWasNotUpdated(string cultureName);

	#endregion Logging
}
