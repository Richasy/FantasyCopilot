// Copyright (c) Richasy Assistant. All rights reserved.

using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.Logging;
using RichasyAssistant.DI.Container;
using RichasyAssistant.Models.App.Web;
using RichasyAssistant.Toolkits.Interfaces;
using RichasyAssistant.ViewModels.Interfaces;
using Windows.Storage;
using Windows.System;

namespace RichasyAssistant.ViewModels;

/// <summary>
/// Civitai image view model.
/// </summary>
public sealed partial class CivitaiImageViewModel : ViewModelBase, ICivitaiImageViewModel
{
    /// <summary>
    /// Initializes a new instance of the <see cref="CivitaiImageViewModel"/> class.
    /// </summary>
    public CivitaiImageViewModel(
        IFileToolkit fileToolkit,
        IResourceToolkit resourceToolkit,
        IAppViewModel appViewModel,
        ILogger<CivitaiImageViewModel> logger)
    {
        _fileToolkit = fileToolkit;
        _resourceToolkit = resourceToolkit;
        _appViewModel = appViewModel;
        _logger = logger;
        AttachIsRunningToAsyncCommand(p => _isSaving = p, SaveImageCommand);
    }

    /// <inheritdoc/>
    public void InjectData(CivitaiImage image)
    {
        _imageId = image.Id.ToString();
        _source = image;
        ImagePath = image.Url;
        UserName = image.UserName;
        HasMetadata = image.Meta != null;
        CreateTime = image.CreatedAt.ToLocalTime().ToString("yyyy-MM-dd HH:mm");
        if (HasMetadata)
        {
            Prompt = image.Meta.Prompt;
            NegativePrompt = image.Meta.NegativePrompt;
            Sampler = image.Meta.Sampler;
            Model = image.Meta.Model;
            CfgScale = image.Meta.CfgScale;
            Steps = image.Meta.Steps;
            Seed = image.Meta.Seed;
            ClipSkip = Convert.ToInt32(image.Meta.ClipSkip);
        }
    }

    [RelayCommand]
    private void NavigateToText2Image()
    {
        var imagePageVM = Locator.Current.GetService<IImagePageViewModel>();
        var txt2imgVM = Locator.Current.GetService<ITextToImageModuleViewModel>();
        txt2imgVM.InjectData(Prompt, NegativePrompt, new Models.App.Image.GenerateOptions
        {
            CfgScale = CfgScale,
            ClipSkip = ClipSkip,
            Model = Model,
            Sampler = Sampler,
            Seed = Seed,
            Steps = Steps,
            Width = _source.Width,
            Height = _source.Height,
        });

        imagePageVM.SelectionType = Models.Constants.ImageModuleType.TextToImage;
    }

    [RelayCommand]
    private async Task SaveImageAsync()
    {
        if (_isSaving)
        {
            return;
        }

        var extension = Path.GetExtension(ImagePath);
        var fileObj = await _fileToolkit.SaveFileAsync($"{_imageId}{extension}", _appViewModel.MainWindow);
        if (fileObj is not StorageFile file)
        {
            return;
        }

        try
        {
            using var client = new HttpClient();
            var bytes = await client.GetByteArrayAsync(ImagePath);
            await FileIO.WriteBytesAsync(file, bytes);
            _appViewModel.ShowTip(_resourceToolkit.GetLocalizedString(Models.Constants.StringNames.FileSaved), Models.Constants.InfoType.Success);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Failed when trying to save the image, image URL: {ImagePath}");
            _appViewModel.ShowTip(_resourceToolkit.GetLocalizedString(Models.Constants.StringNames.FileSaveFailed), Models.Constants.InfoType.Error);
        }
    }

    [RelayCommand]
    private async Task OpenInBrowserAsync()
        => await Launcher.LaunchUriAsync(new Uri($"https://civitai.com/images/{_imageId}"));

    [RelayCommand]
    private async Task SearchModelAsync()
    {
        var uri = new Uri($"https://www.bing.com/search?q={Uri.EscapeDataString(Model)}+site%3Acivitai.com");
        await Launcher.LaunchUriAsync(uri);
    }
}
