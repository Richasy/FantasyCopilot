// Copyright (c) Richasy Assistant. All rights reserved.

using System;
using System.Collections.Specialized;
using System.Threading;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.Logging;
using RichasyAssistant.DI.Container;
using RichasyAssistant.Models.App;
using RichasyAssistant.Models.Constants;
using RichasyAssistant.Libs.Services.Interfaces;
using RichasyAssistant.Toolkits.Interfaces;
using RichasyAssistant.ViewModels.Interfaces;

namespace RichasyAssistant.ViewModels;

/// <summary>
/// Image gallery module view model.
/// </summary>
public sealed partial class ImageGalleryModuleViewModel : ViewModelBase, IImageGalleryModuleViewModel
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ImageGalleryModuleViewModel"/> class.
    /// </summary>
    public ImageGalleryModuleViewModel(
        ISettingsToolkit settingsToolkit,
        IResourceToolkit resourceToolkit,
        ICivitaiService civitaiService,
        IAppViewModel appViewModel,
        ILogger<ImageGalleryModuleViewModel> logger)
    {
        _settingsToolkit = settingsToolkit;
        _resourceToolkit = resourceToolkit;
        _civitaiService = civitaiService;
        _appViewModel = appViewModel;
        _logger = logger;
        _currentPage = 1;

        Images = new SynchronizedObservableCollection<ICivitaiImageViewModel>();
        IsEmpty = true;
        HasNext = true;
        Images.CollectionChanged += OnImagesCollectionChanged;

        CurrentSortType = _settingsToolkit.ReadLocalSetting(SettingNames.LastCivitaiSortType, CivitaiSortType.MostReactions);
        CurrentPeriodType = _settingsToolkit.ReadLocalSetting(SettingNames.LastCivitaiPeriodType, CivitaiPeriodType.Day);

        AttachIsRunningToAsyncCommand(p => IsInitializing = p, RefreshCommand);
    }

    [RelayCommand]
    private async Task RefreshAsync()
    {
        TryClear(Images);
        _currentPage = 1;
        HasNext = true;
        IsEmpty = false;
        await RequestInternalAsync();
    }

    [RelayCommand]
    private async Task RequestAsync()
    {
        if (IsInitializing || IsIncrementalLoading)
        {
            return;
        }

        IsIncrementalLoading = true;
        await RequestInternalAsync();
        IsIncrementalLoading = false;
    }

    private async Task RequestInternalAsync()
    {
        if (!HasNext)
        {
            return;
        }

        if (_cancellationTokenSource != null && _cancellationTokenSource.Token.CanBeCanceled)
        {
            _cancellationTokenSource?.Cancel();
            _cancellationTokenSource = default;
        }

        try
        {
            _cancellationTokenSource = new CancellationTokenSource(TimeSpan.FromMinutes(1.5));
            var data = await _civitaiService.RequestImagesAsync(
                CurrentSortType,
                CurrentPeriodType,
                _currentPage,
                _cancellationTokenSource.Token);

            if (data == null
                || data.Items == null)
            {
                HasNext = false;
                return;
            }

            foreach (var item in data.Items)
            {
                var vm = Locator.Current.GetService<ICivitaiImageViewModel>();
                vm.InjectData(item);
                Images.Add(vm);
            }

            _currentPage++;
            HasNext = data.Metadata.TotalPages >= _currentPage;
        }
        catch (TaskCanceledException)
        {
            // do nothing.
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Requesting image list from Civitai fails");
            _appViewModel.ShowTip(_resourceToolkit.GetLocalizedString(StringNames.RequestImageFailed), InfoType.Error);
        }

        IsEmpty = Images.Count == 0;
    }

    private void OnImagesCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        => IsEmpty = Images.Count == 0;

    partial void OnCurrentPeriodTypeChanged(CivitaiPeriodType value)
        => _settingsToolkit.WriteLocalSetting(SettingNames.LastCivitaiPeriodType, value);

    partial void OnCurrentSortTypeChanged(CivitaiSortType value)
        => _settingsToolkit.WriteLocalSetting(SettingNames.LastCivitaiSortType, value);
}
