using System.Globalization;
using Ardalis.GuardClauses;

namespace Mocale.Managers;

internal class CurrentCultureManager : ICurrentCultureManager
{
    #region Fields

    private readonly IMocaleConfiguration mocaleConfiguration;
    private readonly ILogger logger;
    private readonly IPreferences preferences;

    #endregion Fields

    #region Constructors

    public CurrentCultureManager(
        IConfigurationManager<IMocaleConfiguration> mocaleConfigurationManager,
        ILogger<CurrentCultureManager> logger,
        IPreferences preferences)
    {
        mocaleConfigurationManager = Guard.Against.Null(mocaleConfigurationManager, nameof(mocaleConfigurationManager));

        mocaleConfiguration = mocaleConfigurationManager.Configuration;
        this.logger = Guard.Against.Null(logger, nameof(logger));
        this.preferences = Guard.Against.Null(preferences, nameof(preferences));
    }

    #endregion Constructors

    #region Interface Implementations

    public CultureInfo GetActiveCulture()
    {
        var defaultCulture = mocaleConfiguration.DefaultCulture;

        if (!mocaleConfiguration.SaveCultureChanged)
        {
            return defaultCulture;
        }

        // Load Here
        var lastUsedCulture = preferences.Get(Constants.LastUsedCultureKey, string.Empty);

        if (string.IsNullOrEmpty(lastUsedCulture))
        {
            logger.LogTrace("Setting Last Used Culture as: {DefaultCulture}", defaultCulture);
            SetActiveCulture(defaultCulture);
            return defaultCulture;
        }

        if (lastUsedCulture.TryParseCultureInfo(out var cultureInfo))
        {
            return cultureInfo;
        }

        // TODO: Wipe preferences if this happens?

        logger.LogWarning("Unable to parse culture from preferences: {LastUsedCulture}", lastUsedCulture);
        return defaultCulture;
    }

    public void SetActiveCulture(CultureInfo cultureInfo)
    {
        if (!mocaleConfiguration.SaveCultureChanged)
        {
            return;
        }

        var cultureString = cultureInfo.ToString();

        preferences.Set(Constants.LastUsedCultureKey, cultureString);
    }

    #endregion Interface Implementations
}

