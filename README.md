# Mocale
Localization framework for .NET Maui



## Installation

TODO: I will be building a Github Actions pipeline to create the packages



## Setup

Mocale is setup and configured in your `MauiProgram.cs`:

```csharp
var builder = MauiApp.CreateBuilder();
builder
    .UseMauiApp<App>()
    .UseMocale(mocale =>
    {
      ...
    }
```

There are multiple configuration points to use with Mocale:

- Global
- Internal Provider
- External Provider
- Caching Provider

You must call `WithConfiguration` :

```csha
var builder = MauiApp.CreateBuilder();
builder
    .UseMauiApp<App>()
    .UseMocale(mocale =>
    {
        mocale.WithConfiguration(config =>
            {
                ...
            })
```

This method will provide the basic configuration of the library, the following configuration points are available:

| Property              | Type          | Description                                                  | Default                   |
| --------------------- | ------------- | ------------------------------------------------------------ | ------------------------- |
| Default Culture       | `CultureInfo` | The default culture to load localizations for. This will be used on the first load of your app as the culture. | Current culture of thread |
| Show Missing Keys     | `bool`        | Whether missing localizations should have a value returned. If a key is missing and this is enabled, the key will be returned as the localization. This should serve as a reminder to update your resources. If disabled an empty string will be returned. | `true`                    |
| Not Found Symbol      | `string`      | The symbol to use to indicate a localization is actually a missing key. This symbol will be wrapped around the missing key string, to make it clear this is a missing value: `"$MyKey$"` | `$`                       |
| Use External Provider | `bool`        | Whether an external localization provider should be used, if you just want to use this library locally without the ability to update localizations on the fly this flag can be set to prevent the app from trying to update or cache localizations. | `true`                    |
| Save Culture Changed  | `bool`        | When a culture is changed, whether the new value should be saved for future loads. If the default language is English and a user updates their preference to French, when this value is true if the user kills the app and restarts it, it will load in French. | `true`                    |

If this method has not been called, an initialization exception will occur.

### Internal Provider

Mocale ships with 2 internal providers, one for `Resx` and one for `Json`. Currently both providers are supported but if using external providers it is recommended to use `Json` because i haven't fully implemented `Resx` with external providers or the source generators yet.

Internal providers provide the solid foundation for your localizations. These use files that are bundled with the app and can be load immediately

#### Embedded Resource Provider

This will use Json to load localizations that have been registered as an `EmbeddedResource`.

```
var builder = MauiApp.CreateBuilder();
builder
    .UseMauiApp<App>()
    .UseMocale(mocale =>
    {
        mocale.WithConfiguration(config =>
            {
                ...
            })
            .UseEmbeddedResources(config =>
            {
                config.ResourcesPath = "Locales";
                config.ResourcesAssembly = typeof(App).Assembly;
            });
    })
```

The following config is available:

| Property            | Type       | Description                                                  | Default                         |
| ------------------- | ---------- | ------------------------------------------------------------ | ------------------------------- |
| Resources Path      | `string`   | The directory name that contains the localisations. The files must exist in this directory as `EmbeddedResource` in order for the library to load them | `Locales`                       |
| Resource Assembly   | `Assembly` | The assembly containing the localisations. You can provide this value easily by referencing a type in your app and providing its assembly property: `typeof(App).Assembly` | Must be set during registration |
| Use Resource Folder | `bool`     | Whether Mocale should look for the locale folder in          | `true`                          |

The format of these files should be a keyvalue pair of strings in json format:

```json
{
  "CurrentLocaleName": "English",
  "LocalizationCurrentProviderIs": "The current localization provider is:",
  "LocalizationProviderName": "Json",
  "MocaleDescription": "Localization framework for .NET Maui",
  "MocaleTitle": "Mocale"
}
```



#### App Resources Provider

This will use local app resources (resx) to provide the internal localizations.

```csharp
var builder = MauiApp.CreateBuilder();
builder
    .UseMauiApp<App>()
    .UseMocale(mocale =>
    {
        mocale.WithConfiguration(config =>
            {
                ...
            })
            .UseAppResources(config =>
            {
               config.AppResourcesType = typeof(AppResources);
            });
    })
```

The following config is available:

| Property           | Type   | Description                                                  | Default                         |
| ------------------ | ------ | ------------------------------------------------------------ | ------------------------------- |
| App Resources Type | `Type` | The `Type` of the resources class generated by the compiler. At runtime this will be used to create an instance of `ResourceManager`, which will be able to query the values in the xml. | Must be set during registration |



### External Provider

External providers allow you to update localizations without recompiling your app. This is really powerful for multiple reasons:

- Fix mistakes made in QA for spelling
- Update text and resources based on business requirements

Currently only a single external provider can be used but it is a roadmap item to be able to use multiple providers where the need arises.

By design external providers are considered risky and they will only be considered a source of truth if they return values. The caching layer of the library will persist any data pulled from these providers and whilst the app is within the window of cache only the local data will be used.

There is currently only 1 implemented external provider and its implementation is not extremely robust (missing features such as auth) but I plan to develop these over time. It is also intended for you to create your own external providers based on your specific requirements. If the provider is generic enough I would implore you to contribute it back to this library, equally if it is business specific then you can manage it internally.

### Azure Blob Storage Provider

This provider will search for resource in Blob Storage.

```csharp
var builder = MauiApp.CreateBuilder();
builder
    .UseMauiApp<App>()
    .UseMocale(mocale =>
    {
        mocale.WithConfiguration(config =>
            {
                ...
            })
            .UseBlobStorage(config =>
            {
                config.BlobContainerUri = new Uri("https://azurestorage/mocale/");
                config.RequiresAuthentication = false;
                config.CheckForFile = true;
            });
    })
```

> Currently this provider only works for public facing blob files, I intend to allow authentication for privately hosted files.

The following configuration is available:

| Property                | Type                 | Description                                                  | Default                         |
| ----------------------- | -------------------- | ------------------------------------------------------------ | ------------------------------- |
| Blob Container Uri      | `Uri`                | The url for the blob container that will be queried. This is a base url so if the file in storage is `https://www.yourblobstorage.com/locales/en-GB.json`, this value would be `https://www.yourblobstorage.com/locales/` | Must be set during registration |
| Requires Authentication | `bool`               | Whether the requests to the container require authentication. This is not implemented and authenticated containers will fail when called. | `false`                         |
| Check For File          | `bool`               | Whether mocale should check for the blob file before trying to read it. When true, the file will be searched for before it is read. If false the library will assume the name of a file and blindly try and search for it. It is recommended to leave this value as `true`, however `false` will theortically yield better performance due to less api calls. | `true`                          |
| Version Prefix          | `string`             | The version prefix of the file. If you are versioning your blob storage files then this will automatically be appended between the url & file name.<br />Potential use cases are where you have files versioned for every major / minor version of your app. I intend to build upon this versioning idea in future builds of the library. | `null`                          |
| File Type               | `LocaleResourceType` | The file type of the resource, currently json is the only type supported | `Json`                          |



### Caching Provider

There is currently one caching provider `Mocale.Cache.SQLite`, this provides a cache layer for Mocale using a SQLite database.

Install `Mocale.Cache.SQLite` package into your project and in the `MocaleBuilder` register the provider:

```csharp
var builder = MauiApp.CreateBuilder();
builder
    .UseMauiApp<App>()
    .UseMocale(mocale =>
    {
        mocale.WithConfiguration(config =>
            {
                ...
            })
            .UseSqliteCache(config =>
            {
                config.UpdateInterval = TimeSpan.FromDays(7);
            })
    })
```

The following config is available:

| Property        | Type       | Description                                                  | Default |
| --------------- | ---------- | ------------------------------------------------------------ | ------- |
| Update Interval | `TimeSpan` | The interval in which the cache should be updated. If the last checked date was lower than the update interval, the library will attempt to use the external provider to update the localization resources. If the last checked date was greater than the update interval, the cache will be used and no external call will be made. | 1 day   |

The database name and location is not configurable, it will create its own database named `Mocale.db` under the `FileSystem.AppDataDirectory` path.

> Currently there is no in memory package and if you run Mocale without referencing this package it will not function because not all of the required classes for normal lib function have been implemented in the main assembly.
>
> This is a roadmap item to not have consumers depend on the SQLite package.

## Usage

Once registered using the host builder, the library will automatically initialise using the given config.

### Xaml

Reference mocale as an xml namespace:

```xml
xmlns:mocale="http://axemasta.com/schemas/2022/mocale"
```

Use the provided markup extension:

```xml
<Label Text="{mocale:Localize MocaleDescription}" />
```

Provide an `IValueConverter`:

```xml
<Label Text="{mocale:Localize Key=CurrentLocaleName, Converter={StaticResource LanguageEmojiConverter}}" />
```



Use the source generated key definitions ([see below for more details](# Source Generators)):

```xml
xmlns:keys="Acme.MauiApp"
```

```xml
<Label Text="{mocale:Localize {x:Static keys:TranslationKeys.MocaleDescription}}"/>
```



### Codebehind / ViewModel

#### Translations

> I will be updating this to provide a more testable way to retrieve translations in the viewmodel layer

In your class add `ITranslatorManager`:

```csharp
private readonly ITranslatorManager translatorManager;

public MyViewModel(ITranslatorManager translatorManager)
{
	this.translatorManager = translatorManager;
}
```

Use the methods provided to translate:

```csharp
private void DoSomething()
{
	var appTitle = translatorManager.Translate("AppTitle");
}
```



#### Change Culture

In your class add `ILocalizationManager`:

```csharp
private readonly ILocalizationManager localizationManager;

public MyViewModel(ILocalizationManager localizationManager)
{
	this.localizationManager = localizationManager;
}
```

Call `SetCultureAsync` to attempt to change culture:

```csharp
private async Task ChangeCultureToFrench()
{
	var french = new CultureInfo("fr-FR");
	
	var changed = await localizationManager.SetCultureAsync(french);
	
	Console.WriteLine($"Changed to french: {changed}");
}
```



## Source Generators

The `Mocale.SourceGenerators` package provides some extra useful features for develepment.

When installed this package will automatically generate a translations key constants file for you, this is essentially the same as the `Resx` designer file. This allows you to have strong typings for your translation keys which will be valuable for cases such as removing a resource key, normally this key would be orphaned in your code and wouldn't be an issue until runtime, now if you have a reference through code, removing it will cause your compile to fail.

> Source generators are only compatible with Json localizations, Resx already have designer files you can reference.

To get this source generator to work:

- Reference all your Locale Json files as `C# Analyzer Additional Files`:
  ```xml
  <ItemGroup>
    <AdditionalFiles Include="Locales\en-GB.json" />
    <AdditionalFiles Include="Locales\fr-FR.json" />
  </ItemGroup>
  ```

- Build solution (to run the generator)

- A new class named `TranslationKeys` will be generated under the namespace of the assembly referencing the generator.

The generated class will look similar to:

```csharp
// <auto-generated/>
namespace Acme.MauiApp;

public static class TranslationKeys
{
	/// <summary>
	/// Looks up a localized string using key KeyOne.
	/// </summary>
	public const string KeyOne = "KeyOne";


	/// <summary>
	/// Looks up a localized string using key KeyTwo.
	/// </summary>
	public const string KeyTwo = "KeyTwo";
}
```

The generated properties will be Pascal cased and have illegal c# characters sanitized. The values will be unaltered and will be able to use as keys for the localizations.

The namespace for the constants is equal to the assembly name referencing the generator:

- `Acme.MauiApp` references `Mocale.SourceGenerators`
- The following class will be created: `Acme.MauiApp.TranslationKeys`

You can use these classes in Xaml or C# to perform translations:

Xaml:

```xml
<Label Text="{mocale:Localize {x:Static keys:TranslationKeys.LabelTitle}}"/>
```

C#:

```csharp
var labelTitle = translationManager.Translate(TranslationKeys.LabelTitle);
```



## Feature Goals

These are features not yet currently in Mocale which I intend to add in the near future, mainly because I will be needing them. Feel free to raise a PR if I haven't got around to implementing them 😄

- String formatting
  - Stretch: Definable format criteria aka not just `"Replace {0} and {1}"` but for complicated strings like `"Replace {value} and {name}"`
- Support for different types of locale file (aka `resx`, `json`), currently json is assumed
- Support for multiple external providers
- Add a more MVVM friendly mechanism to resolve translations (such as a `ITranslationProvider` where you request a number of keys & get localizations back)
