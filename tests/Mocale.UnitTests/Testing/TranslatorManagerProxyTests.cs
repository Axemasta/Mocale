using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mocale.Abstractions;
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

        var translation2 = Sut.Translate("hello {0}", [ "Gary"]);
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
        Sut.UpdateTranslations(new Localization()
        {
            CultureInfo = new System.Globalization.CultureInfo("en-GB"),
            Translations = new Dictionary<string, string>()
            {
                { "KeyOne", "Hello" },
                { "KeyTwo", "World" },
            },
        }, Enums.TranslationSource.External, true);

        Sut.UpdateTranslations(new Localization()
        {
            CultureInfo = new System.Globalization.CultureInfo("en-GB"),
            Translations = new Dictionary<string, string>()
            {
                { "KeyOne", "Hello" },
                { "KeyTwo", "World" },
            },
        }, Enums.TranslationSource.WarmCache, true);

        Sut.UpdateTranslations(new Localization()
        {
            CultureInfo = new System.Globalization.CultureInfo("en-GB"),
            Translations = new Dictionary<string, string>()
            {
                { "KeyOne", "Hello" },
                { "KeyTwo", "World" },
            },
        }, Enums.TranslationSource.ColdCache, true);

        // Assert
        Assert.Equal(2, Sut.PreferredLocalizations.Count);

        Assert.Equivalent(new Dictionary<string, string>()
            {
                { "KeyOne", "Hello" },
                { "KeyTwo", "World" },
            }, Sut.PreferredLocalizations);

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
        Sut.UpdateTranslations(new Localization()
        {
            CultureInfo = new System.Globalization.CultureInfo("en-GB"),
            Translations = new Dictionary<string, string>()
            {
                { "KeyOne", "Hello" },
                { "KeyTwo", "World" },
            },
        }, Enums.TranslationSource.Internal, true);

        // Assert
        Assert.Equal(2, Sut.BackupLocalizations.Count);

        Assert.Equivalent(new Dictionary<string, string>()
            {
                { "KeyOne", "Hello" },
                { "KeyTwo", "World" },
            }, Sut.BackupLocalizations);

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
        Sut.UpdateTranslations(new Localization()
        {
            CultureInfo = new System.Globalization.CultureInfo("en-GB"),
            Translations = new Dictionary<string, string>()
            {
                { "KeyOne", "Hello" },
                { "KeyTwo", "World" },
            },
        }, Enums.TranslationSource.External, false);

        Sut.UpdateTranslations(new Localization()
        {
            CultureInfo = new System.Globalization.CultureInfo("en-GB"),
            Translations = new Dictionary<string, string>()
            {
                { "KeyOne", "Hello" },
                { "KeyTwo", "World" },
            },
        }, Enums.TranslationSource.WarmCache, false);

        Sut.UpdateTranslations(new Localization()
        {
            CultureInfo = new System.Globalization.CultureInfo("en-GB"),
            Translations = new Dictionary<string, string>()
            {
                { "KeyOne", "Hello" },
                { "KeyTwo", "World" },
            },
        }, Enums.TranslationSource.ColdCache, false);

        // Assert
        Assert.Equal(2, Sut.PreferredLocalizations.Count);

        Assert.Equivalent(new Dictionary<string, string>()
            {
                { "KeyOne", "Hello" },
                { "KeyTwo", "World" },
            }, Sut.PreferredLocalizations);

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
        Sut.UpdateTranslations(new Localization()
        {
            CultureInfo = new System.Globalization.CultureInfo("en-GB"),
            Translations = new Dictionary<string, string>()
            {
                { "KeyOne", "Hello" },
                { "KeyTwo", "World" },
            },
        }, Enums.TranslationSource.Internal, false);

        // Assert
        Assert.Equal(2, Sut.BackupLocalizations.Count);

        Assert.Equivalent(new Dictionary<string, string>()
            {
                { "KeyOne", "Hello" },
                { "KeyTwo", "World" },
            }, Sut.BackupLocalizations);

        Assert.Empty(Sut.PreferredLocalizations);

        Assert.Equal(0, notifyInvocations);
    }

    #endregion Tests
}
