// Copyright (c) Fantasy Copilot. All rights reserved.

using CommunityToolkit.Mvvm.ComponentModel;
using FantasyCopilot.DI.Container;
using FantasyCopilot.Models.Constants;
using FantasyCopilot.Toolkits.Interfaces;
using FantasyCopilot.ViewModels.Interfaces;

namespace FantasyCopilot.ViewModels;

/// <summary>
/// Image page view model.
/// </summary>
public sealed partial class ImagePageViewModel : ViewModelBase, IImagePageViewModel
{
    private readonly ISettingsToolkit _settingsToolkit;
    private readonly IAppViewModel _appViewModel;

    [ObservableProperty]
    private ImageModuleType _selectionType;

    [ObservableProperty]
    private bool _isTextToImageSelected;

    [ObservableProperty]
    private bool _isGallerySelected;

    /// <summary>
    /// Initializes a new instance of the <see cref="ImagePageViewModel"/> class.
    /// </summary>
    public ImagePageViewModel(
        ISettingsToolkit settingsToolkit,
        IAppViewModel appViewModel)
    {
        _settingsToolkit = settingsToolkit;
        _appViewModel = appViewModel;
        SelectionType = _settingsToolkit.ReadLocalSetting(SettingNames.LastImageModuleType, ImageModuleType.TextToImage);
        CheckCurrentSelectionType();
    }

    partial void OnSelectionTypeChanged(ImageModuleType value)
    {
        CheckCurrentSelectionType();
        _settingsToolkit.WriteLocalSetting(SettingNames.LastImageModuleType, value);
    }

    private void CheckCurrentSelectionType()
    {
        IsTextToImageSelected = SelectionType == ImageModuleType.TextToImage;
        IsGallerySelected = SelectionType == ImageModuleType.Gallery;
        _appViewModel.IsBackButtonShown =
            SelectionType == ImageModuleType.TextToImage
            ? Locator.Current.GetService<ITextToImageModuleViewModel>().IsInSettings
            : false;
    }
}
