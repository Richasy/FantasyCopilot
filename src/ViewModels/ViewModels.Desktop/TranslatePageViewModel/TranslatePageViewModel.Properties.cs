// Copyright (c) Fantasy Copilot. All rights reserved.

using System.Threading;
using CommunityToolkit.Mvvm.ComponentModel;
using FantasyCopilot.Models.App;
using FantasyCopilot.Services.Interfaces;
using FantasyCopilot.Toolkits.Interfaces;
using FantasyCopilot.ViewModels.Interfaces;
using Microsoft.UI.Dispatching;

namespace FantasyCopilot.ViewModels;

/// <summary>
/// Translate page view model.
/// </summary>
public sealed partial class TranslatePageViewModel
{
    private readonly ISettingsToolkit _settingsToolkit;
    private readonly IResourceToolkit _resourceToolkit;
    private readonly ITranslateService _translateService;
    private readonly IAppViewModel _appViewModel;
    private readonly DispatcherQueue _dispatcherQueue;
    private CancellationTokenSource _cancellationTokenSource;

    [ObservableProperty]
    private LocaleInfo _selectedSourceLanguage;

    [ObservableProperty]
    private LocaleInfo _selectedTargetLanguage;

    [ObservableProperty]
    private string _sourceText;

    [ObservableProperty]
    private string _outputText;

    [ObservableProperty]
    private bool _isInitializing;

    [ObservableProperty]
    private bool _isTranslating;

    [ObservableProperty]
    private bool _isError;

    /// <inheritdoc/>
    public SynchronizedObservableCollection<LocaleInfo> SourceLanguages { get; }

    /// <inheritdoc/>
    public SynchronizedObservableCollection<LocaleInfo> TargetLanguages { get; }
}
