using System.ComponentModel;
using System.Globalization;
using Mocale.Abstractions;
using Mocale.Enums;
using Mocale.Models;
using Mocale.Testing;

namespace Mocale.UnitTests.Testing;

public class TranslatorManagerProxyTests : FixtureBase<TranslatorManagerProxy>
{
    #region Setup

    public override TranslatorManagerProxy CreateSystemUnderTest()
    {
        return new TranslatorManagerProxy();
    }

    #endregion Setup

    #region Tests

    [Fact]
    public void VerifyDefaults()
    {
        Assert.True(Sut.GetType().IsAssignableTo(typeof(ITranslatorManager)));
        Assert.True(Sut.GetType().IsAssignableTo(typeof(ITranslationUpdater)));
        Assert.True(Sut.GetType().IsAssignableTo(typeof(INotifyPropertyChanged)));

        Assert.Null(Sut.CurrentCulture);
        Assert.Empty(Sut.PreferredLocalizations);
        Assert.Empty(Sut.BackupLocalizations);

        var translation1 = Sut.Translate("hello");
        Assert.Equal("$hello$", translation1);

        var translation2 = Sut.Translate("hello {0}", ["Gary"]);
        Assert.Equal("$hello Gary$", translation2);
    }

    [Fact]
    public void UpdateTranslations_WhenSourceIsExternalOrCache_ShouldUpdatePreferredTranslations()
    {
        // Arrange
        Assert.Empty(Sut.PreferredLocalizations);
        Assert.Empty(Sut.BackupLocalizations);

        var notifyInvocations = 0;

        Sut.PropertyChanged += (sender, args) =>
        {
            notifyInvocations++;
        };

        // Act
        Sut.UpdateTranslations(
            new Localization
            {
                CultureInfo = new CultureInfo("en-GB"),
                Translations = new Dictionary<string, string> { { "KeyOne", "Hello" }, { "KeyTwo", "World" } }
            }, TranslationSource.External);

        Sut.UpdateTranslations(
            new Localization
            {
                CultureInfo = new CultureInfo("en-GB"),
                Translations = new Dictionary<string, string> { { "KeyOne", "Hello" }, { "KeyTwo", "World" } }
            }, TranslationSource.WarmCache);

        Sut.UpdateTranslations(
            new Localization
            {
                CultureInfo = new CultureInfo("en-GB"),
                Translations = new Dictionary<string, string> { { "KeyOne", "Hello" }, { "KeyTwo", "World" } }
            }, TranslationSource.ColdCache);

        // Assert
        Assert.Equal(2, Sut.PreferredLocalizations.Count);

        Assert.Equivalent(new Dictionary<string, string> { { "KeyOne", "Hello" }, { "KeyTwo", "World" } },
            Sut.PreferredLocalizations);

        Assert.Empty(Sut.BackupLocalizations);
        Assert.Equal(3, notifyInvocations);
    }

    [Fact]
    public void UpdateTranslations_WhenSourceIsInternal_ShouldUpdateBackupTranslations()
    {
        // Arrange
        Assert.Empty(Sut.PreferredLocalizations);
        Assert.Empty(Sut.BackupLocalizations);

        var notifyInvocations = 0;

        Sut.PropertyChanged += (sender, args) =>
        {
            notifyInvocations++;
        };

        // Act
        Sut.UpdateTranslations(
            new Localization
            {
                CultureInfo = new CultureInfo("en-GB"),
                Translations = new Dictionary<string, string> { { "KeyOne", "Hello" }, { "KeyTwo", "World" } }
            }, TranslationSource.Internal);

        // Assert
        Assert.Equal(2, Sut.BackupLocalizations.Count);

        Assert.Equivalent(new Dictionary<string, string> { { "KeyOne", "Hello" }, { "KeyTwo", "World" } },
            Sut.BackupLocalizations);

        Assert.Empty(Sut.PreferredLocalizations);

        Assert.Equal(1, notifyInvocations);
    }

    [Fact]
    public void UpdateTranslations_ExternalSourceNoNotify_ShouldUpdateAndNotNotify()
    {
        // Arrange
        Assert.Empty(Sut.PreferredLocalizations);
        Assert.Empty(Sut.BackupLocalizations);

        var notifyInvocations = 0;

        Sut.PropertyChanged += (sender, args) =>
        {
            notifyInvocations++;
        };

        // Act
        Sut.UpdateTranslations(
            new Localization
            {
                CultureInfo = new CultureInfo("en-GB"),
                Translations = new Dictionary<string, string> { { "KeyOne", "Hello" }, { "KeyTwo", "World" } }
            }, TranslationSource.External, false);

        Sut.UpdateTranslations(
            new Localization
            {
                CultureInfo = new CultureInfo("en-GB"),
                Translations = new Dictionary<string, string> { { "KeyOne", "Hello" }, { "KeyTwo", "World" } }
            }, TranslationSource.WarmCache, false);

        Sut.UpdateTranslations(
            new Localization
            {
                CultureInfo = new CultureInfo("en-GB"),
                Translations = new Dictionary<string, string> { { "KeyOne", "Hello" }, { "KeyTwo", "World" } }
            }, TranslationSource.ColdCache, false);

        // Assert
        Assert.Equal(2, Sut.PreferredLocalizations.Count);

        Assert.Equivalent(new Dictionary<string, string> { { "KeyOne", "Hello" }, { "KeyTwo", "World" } },
            Sut.PreferredLocalizations);

        Assert.Empty(Sut.BackupLocalizations);
        Assert.Equal(0, notifyInvocations);
    }

    [Fact]
    public void UpdateTranslations_InternalSourceNoNotify_ShouldUpdateAndNotNotify()
    {
        // Arrange
        Assert.Empty(Sut.PreferredLocalizations);
        Assert.Empty(Sut.BackupLocalizations);

        var notifyInvocations = 0;

        Sut.PropertyChanged += (sender, args) =>
        {
            notifyInvocations++;
        };

        // Act
        Sut.UpdateTranslations(
            new Localization
            {
                CultureInfo = new CultureInfo("en-GB"),
                Translations = new Dictionary<string, string> { { "KeyOne", "Hello" }, { "KeyTwo", "World" } }
            }, TranslationSource.Internal, false);

        // Assert
        Assert.Equal(2, Sut.BackupLocalizations.Count);

        Assert.Equivalent(new Dictionary<string, string> { { "KeyOne", "Hello" }, { "KeyTwo", "World" } },
            Sut.BackupLocalizations);

        Assert.Empty(Sut.PreferredLocalizations);

        Assert.Equal(0, notifyInvocations);
    }

    #endregion Tests
}
