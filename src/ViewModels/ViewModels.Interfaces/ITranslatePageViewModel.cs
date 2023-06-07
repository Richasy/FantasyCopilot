// Copyright (c) Fantasy Copilot. All rights reserved.

using System.Collections.ObjectModel;
using System.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FantasyCopilot.Models.App;

namespace FantasyCopilot.ViewModels.Interfaces;

/// <summary>
/// Interface definition for translate page view model.
/// </summary>
public interface ITranslatePageViewModel : INotifyPropertyChanged
{
    /// <summary>
    /// Optional list of currently input languages.
    /// </summary>
    ObservableCollection<LocaleInfo> SourceLanguages { get; }

    /// <summary>
    /// Optional list of languages for current output.
    /// </summary>
    ObservableCollection<LocaleInfo> TargetLanguages { get; }

    /// <summary>
    /// Selected source language.
    /// </summary>
    LocaleInfo SelectedSourceLanguage { get; set; }

    /// <summary>
    /// Selected target language.
    /// </summary>
    LocaleInfo SelectedTargetLanguage { get; set; }

    /// <summary>
    /// Source text.
    /// </summary>
    string SourceText { get; set; }

    /// <summary>
    /// Translated text.
    /// </summary>
    string OutputText { get; set; }

    /// <summary>
    /// Is it loading.
    /// </summary>
    bool IsInitializing { get; }

    /// <summary>
    /// Is it translating.
    /// </summary>
    bool IsTranslating { get; }

    /// <summary>
    /// Translation failed.
    /// </summary>
    bool IsError { get; }

    /// <summary>
    /// Translate text.
    /// </summary>
    IAsyncRelayCommand TranslateCommand { get; }

    /// <summary>
    /// Initialize page.
    /// </summary>
    IAsyncRelayCommand InitializeCommand { get; }
}
