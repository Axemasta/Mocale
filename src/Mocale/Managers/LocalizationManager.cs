using System.Globalization;
using Ardalis.GuardClauses;

namespace Mocale.Managers;

internal partial class LocalizationManager : ILocalizationManager
{
	private readonly ICurrentCultureManager currentCultureManager;
	private readonly ILogger logger;
	private readonly IMocaleConfiguration mocaleConfiguration;
	private readonly ITranslationResolver translationResolver;
	private readonly IInternalTranslatorManager translatorManager;

	public CultureInfo CurrentCulture { get; private set; }

	public LocalizationManager(
		ICurrentCultureManager currentCultureManager,
		IConfigurationManager<IMocaleConfiguration> configurationManager,
		ILogger<LocalizationManager> logger,
		ITranslationResolver translationResolver,
		IInternalTranslatorManager translatorManager)
	{
		this.currentCultureManager = Guard.Against.Null(currentCultureManager);
		this.logger = Guard.Against.Null(logger);
		this.translationResolver = Guard.Against.Null(translationResolver);
		this.translatorManager = Guard.Against.Null(translatorManager);

		configurationManager = Guard.Against.Null(configurationManager);
		mocaleConfiguration = configurationManager.Configuration;

		CurrentCulture = currentCultureManager.GetActiveCulture();
	}

	public async Task<bool> SetCultureAsync(CultureInfo culture)
	{
		try
		{
			bool loaded;

			if (mocaleConfiguration.UseExternalProvider)
			{
				loaded = await TryLoadInternalAndExternalTranslations(culture);
			}
			else
			{
				loaded = TryLoadInternalTranslations(culture);
			}

			if (!loaded)
			{
				LogUnableToLoadCultureCultureNameNoLocalizationsFound(culture.Name);
				return false;
			}

			translatorManager.RaisePropertyChanged();

			CurrentCulture = culture;

			UpdateThreadCulture(culture);

			currentCultureManager.SetActiveCulture(culture);

			LogUpdatedLocalizationCultureToCultureName(culture.Name);

			return true;
		}
		catch (Exception ex)
		{
			LogAnExceptionOccurredLoadingCultureCultureName(ex, culture.Name);

			return false;
		}
	}

	public async Task<bool> Initialize()
	{
		try
		{
			return await InitializeInternal();
		}
		catch (Exception ex)
		{
			LogAnExceptionOccurredInitializingLocalizationManager(ex);
			return false;
		}
	}

	private Task<bool> InitializeInternal()
	{
		var localTranslations = translationResolver.LoadLocalTranslations(CurrentCulture);

		if (!localTranslations.Loaded)
		{
			LogUnableToLoadTranslationsForCultureCultureName(CurrentCulture.Name);
			return Task.FromResult(false);
		}

		translatorManager.UpdateTranslations(localTranslations.Localization, localTranslations.Source);

		LogLoadedLocalTranslationsFromSourceTranslationSource(localTranslations.Source);

		if (localTranslations.Source is TranslationSource.Internal or TranslationSource.ColdCache && mocaleConfiguration.UseExternalProvider)
		{
			LogExternalTranslationsCanBeUpdatedCheckingForNewerCopy();

			// Check cache and go get up to date translations
			Task.Run(() => CheckForTranslationUpdates(CurrentCulture))
				.Forget();
		}

		UpdateThreadCulture(CurrentCulture);

		return Task.FromResult(true);
	}

	private async Task CheckForTranslationUpdates(CultureInfo cultureInfo)
	{
		var external = await translationResolver.LoadTranslations(cultureInfo);

		if (!external.Loaded)
		{
			LogUnableToLoadExternalTranslationsForCultureCultureInfo(cultureInfo);
			return;
		}

		translatorManager.UpdateTranslations(external.Localization, TranslationSource.External);
	}

	private bool TryLoadInternalTranslations(CultureInfo culture)
	{
		var localTranslations = translationResolver.LoadLocalTranslations(culture);

		if (localTranslations.Loaded)
		{
			translatorManager.UpdateTranslations(localTranslations.Localization, localTranslations.Source, false);
			return true;
		}
		else
		{
			LogNoInternalTranslationsFoundForCultureCultureNameConsiderAddingThemAsABackup(culture.Name);
			return false;
		}
	}

	private async Task<bool> TryLoadInternalAndExternalTranslations(CultureInfo culture)
	{
		var result = await translationResolver.LoadTranslations(culture);

		if (result.Loaded)
		{
			translatorManager.UpdateTranslations(result.Localization, result.Source, false);
		}
		else
		{
			LogUnableToLoadCultureCultureNameFromExternalProvider(culture.Name);
		}

		var localTranslations = translationResolver.LoadLocalTranslations(culture);

		if (localTranslations.Loaded)
		{
			translatorManager.UpdateTranslations(localTranslations.Localization, localTranslations.Source, false);
		}
		else
		{
			LogNoInternalTranslationsFoundForCultureCultureNameConsiderAddingThemAsABackup(culture.Name);
		}

		return result.Loaded || localTranslations.Loaded;
	}

	private static void UpdateThreadCulture(CultureInfo cultureInfo)
	{
		Thread.CurrentThread.CurrentCulture = cultureInfo;
		Thread.CurrentThread.CurrentUICulture = cultureInfo;
		CultureInfo.CurrentCulture = cultureInfo;
		CultureInfo.CurrentUICulture = cultureInfo;
		CultureInfo.DefaultThreadCurrentCulture = cultureInfo;
		CultureInfo.DefaultThreadCurrentUICulture = cultureInfo;
	}

	#region Logging

	[LoggerMessage(LogLevel.Debug, "Updated localization culture to {CultureName}")]
	partial void LogUpdatedLocalizationCultureToCultureName(string cultureName);

	[LoggerMessage(LogLevel.Trace, "Loaded local translations from source: {TranslationSource}")]
	partial void LogLoadedLocalTranslationsFromSourceTranslationSource(TranslationSource translationSource);

	[LoggerMessage(LogLevel.Information, "No internal translations found for culture: {CultureName}, consider adding them as a backup")]
	partial void LogNoInternalTranslationsFoundForCultureCultureNameConsiderAddingThemAsABackup(string cultureName);

	[LoggerMessage(LogLevel.Warning, "Unable to load culture {CultureName}, no localizations found")]
	partial void LogUnableToLoadCultureCultureNameNoLocalizationsFound(string cultureName);

	[LoggerMessage(LogLevel.Error, "An exception occurred loading culture: {CultureName}")]
	partial void LogAnExceptionOccurredLoadingCultureCultureName(Exception exception, string cultureName);

	[LoggerMessage(LogLevel.Error, "An exception occurred initializing LocalizationManager")]
	partial void LogAnExceptionOccurredInitializingLocalizationManager(Exception exception);

	[LoggerMessage(LogLevel.Warning, "Unable to load translations for culture: {CultureName}")]
	partial void LogUnableToLoadTranslationsForCultureCultureName(string cultureName);

	[LoggerMessage(LogLevel.Information, "External translations can be updated, checking for newer copy...")]
	partial void LogExternalTranslationsCanBeUpdatedCheckingForNewerCopy();

	[LoggerMessage(LogLevel.Warning, "Unable to load external translations for culture: {CultureInfo}")]
	partial void LogUnableToLoadExternalTranslationsForCultureCultureInfo(CultureInfo cultureInfo);

	[LoggerMessage(LogLevel.Warning, "Unable to load culture {CultureName} from external provider")]
	partial void LogUnableToLoadCultureCultureNameFromExternalProvider(string cultureName);

	#endregion Logging
}
