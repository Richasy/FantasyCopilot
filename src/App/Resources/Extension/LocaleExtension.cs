// Copyright (c) Richasy Assistant. All rights reserved.

using Microsoft.UI.Xaml.Markup;
using RichasyAssistant.DI.Container;
using RichasyAssistant.Toolkits.Interfaces;

namespace RichasyAssistant.App.Resources.Extension;

/// <summary>
/// Localized text extension.
/// </summary>
[MarkupExtensionReturnType(ReturnType = typeof(string))]
public sealed class LocaleExtension : MarkupExtension
{
    /// <summary>
    /// Language name.
    /// </summary>
    public StringNames Name { get; set; }

    /// <inheritdoc/>
    protected override object ProvideValue()
        => Locator.Current.GetService<IResourceToolkit>().GetLocalizedString(Name);
}
