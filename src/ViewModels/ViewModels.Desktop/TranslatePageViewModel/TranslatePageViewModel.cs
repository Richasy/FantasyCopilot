// Copyright (c) Fantasy Copilot. All rights reserved.

using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.Input;
using FantasyCopilot.Models.App;
using FantasyCopilot.Models.Constants;
using FantasyCopilot.Services.Interfaces;
using FantasyCopilot.Toolkits.Interfaces;
using FantasyCopilot.ViewModels.Interfaces;
using Microsoft.UI.Dispatching;

namespace FantasyCopilot.ViewModels;

/// <summary>
/// Translate page view model.
/// </summary>
public sealed partial class TranslatePageViewModel : ViewModelBase, ITranslatePageViewModel
{
    /// <summary>
    /// Initializes a new instance of the <see cref="TranslatePageViewModel"/> class.
    /// </summary>
    public TranslatePageViewModel(
        ISettingsToolkit settingsToolkit,
        IResourceToolkit resourceToolkit,
        ITranslateService translateService,
        IAppViewModel appViewModel)
    {
        _settingsToolkit = settingsToolkit;
        _resourceToolkit = resourceToolkit;
        _translateService = translateService;
        _appViewModel = appViewModel;
        _dispatcherQueue = DispatcherQueue.GetForCurrentThread();
        SourceLanguages = new ObservableCollection<LocaleInfo>();
        TargetLanguages = new ObservableCollection<LocaleInfo>();

        AttachIsRunningToAsyncCommand(p => p = IsInitializing = p, InitializeCommand);
        AttachIsRunningToAsyncCommand(p => IsTranslating = p, TranslateCommand);
    }

    [RelayCommand]
    private async Task InitializeAsync()
    {
        if (SourceLanguages.Count > 0 || IsInitializing)
        {
            return;
        }

        var languages = await _translateService.GetSupportLanguagesAsync();
        foreach (var language in languages.OrderBy(p => p.Name))
        {
            SourceLanguages.Add(language);
            TargetLanguages.Add(language);
        }

        var autoDetect = new LocaleInfo(string.Empty, _resourceToolkit.GetLocalizedString(StringNames.AutoDetect));
        SourceLanguages.Insert(0, autoDetect);

        var localLocale = languages.FirstOrDefault(p => p.Id == CultureInfo.CurrentUICulture.TwoLetterISOLanguageName);
        var localSourceLanguage = _settingsToolkit.ReadLocalSetting(SettingNames.TranslateSourceLanguage, string.Empty);
        var localTargetLanguage = _settingsToolkit.ReadLocalSetting(SettingNames.TranslateTargetLanguage, localLocale?.Id ?? string.Empty);
        SelectedSourceLanguage = SourceLanguages.FirstOrDefault(p => p.Id == localSourceLanguage) ?? autoDetect;
        SelectedTargetLanguage = TargetLanguages.FirstOrDefault(p => p.Id == localTargetLanguage) ?? TargetLanguages.FirstOrDefault(p => p.Id == "en");
    }

    [RelayCommand]
    private async Task TranslateAsync()
    {
        OutputText = string.Empty;
        if (string.IsNullOrEmpty(SourceText)
            || SelectedSourceLanguage == default
            || SelectedTargetLanguage == default)
        {
            return;
        }

        try
        {
            IsError = false;
            if (_cancellationTokenSource != null
                && _cancellationTokenSource.Token.CanBeCanceled)
            {
                _cancellationTokenSource.Cancel();
            }

            _cancellationTokenSource = new CancellationTokenSource();
            var content = await _translateService.TranslateTextAsync(
                SourceText,
                SelectedSourceLanguage.Id,
                SelectedTargetLanguage.Id,
                _cancellationTokenSource.Token);
            _cancellationTokenSource = null;
            OutputText = content;
        }
        catch (TaskCanceledException)
        {
            // do nothing.
        }
        catch
        {
            IsError = true;
        }
    }

    partial void OnSelectedSourceLanguageChanged(LocaleInfo value)
        => _settingsToolkit.WriteLocalSetting(SettingNames.TranslateSourceLanguage, value.Id);

    partial void OnSelectedTargetLanguageChanged(LocaleInfo value)
    {
        _settingsToolkit.WriteLocalSetting(SettingNames.TranslateTargetLanguage, value.Id);
        if (!string.IsNullOrEmpty(SourceText))
        {
            TranslateCommand.Execute(default);
        }
    }
}
