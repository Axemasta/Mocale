# Mocale
Localization framework for .NET Maui



## Feature Goals

- Xaml localization
- Mvvm friendly resource provider
- String formatting
  - Stretch: Definable format criteria aka not just `"Replace {0} and {1}"` but for complicated strings like `"Replace {value} and {name}"`
- Support for different types of locale file (aka `resx`, `json`)
- Support for providing files from internet (aka azure blob storage)
