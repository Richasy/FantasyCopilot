// Copyright (c) Richasy Assistant. All rights reserved.

using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.Logging;
using Microsoft.UI.Dispatching;
using RichasyAssistant.DI.Container;
using RichasyAssistant.Models.App;
using RichasyAssistant.Models.App.Image;
using RichasyAssistant.Models.Constants;
using RichasyAssistant.Services.Interfaces;
using RichasyAssistant.Toolkits.Interfaces;
using RichasyAssistant.ViewModels.Interfaces;
using Windows.Storage;

namespace RichasyAssistant.ViewModels;

/// <summary>
/// Text to image module view model.
/// </summary>
public sealed partial class TextToImageModuleViewModel : ViewModelBase, ITextToImageModuleViewModel
{
    /// <summary>
    /// Initializes a new instance of the <see cref="TextToImageModuleViewModel"/> class.
    /// </summary>
    public TextToImageModuleViewModel(
        IResourceToolkit resourceToolkit,
        IFileToolkit fileToolkit,
        IAppViewModel appViewModel,
        IImageService imageService,
        ILogger<TextToImageModuleViewModel> logger)
    {
        _imageService = imageService;
        _resourceToolkit = resourceToolkit;
        _fileToolkit = fileToolkit;
        _appViewModel = appViewModel;
        _logger = logger;
        _dispatcherQueue = DispatcherQueue.GetForCurrentThread();

        Models = new SynchronizedObservableCollection<Model>();
        Samplers = new SynchronizedObservableCollection<Sampler>();
        Embeddings = new SynchronizedObservableCollection<string>();
        Loras = new SynchronizedObservableCollection<string>();

        AttachIsRunningToAsyncCommand(p => IsInitializing = p, InitializeCommand);
        AttachIsRunningToAsyncCommand(p => IsGenerating = p, GenerateCommand);
    }

    /// <inheritdoc/>
    public void InjectData(string prompt, string negativePrompt, GenerateOptions options)
    {
        Prompt = prompt;
        NegativePrompt = negativePrompt;
        _tempOptions = options;
    }

    /// <inheritdoc/>
    public byte[] GetTempImageData()
        => _imageBytes;

    [RelayCommand]
    private static async Task OpenInBrowserAsync()
    {
        var settingsToolkit = Locator.Current.GetService<ISettingsToolkit>();
        var uri = settingsToolkit.ReadLocalSetting(SettingNames.StableDiffusionUrl, string.Empty);
        if (!Uri.IsWellFormedUriString(uri, UriKind.RelativeOrAbsolute))
        {
            return;
        }

        await Windows.System.Launcher.LaunchUriAsync(new Uri(uri));
    }

    [RelayCommand]
    private async Task InitializeAsync()
    {
        if (!_isInitialized)
        {
            var isConnected = await _imageService.PingAsync();
            IsDisconnected = !isConnected;
            if (IsDisconnected)
            {
                return;
            }

            var modelTask = Task.Run(async () =>
            {
                try
                {
                    var models = await _imageService.GetModelsAsync();
                    _dispatcherQueue.TryEnqueue(() =>
                    {
                        foreach (var item in models)
                        {
                            Models.Add(item);
                        }
                    });
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "There was an error loading the image models");
                }
            });

            var samplerTask = Task.Run(async () =>
            {
                try
                {
                    var samplers = await _imageService.GetSamplersAsync();
                    _dispatcherQueue.TryEnqueue(() =>
                    {
                        foreach (var item in samplers)
                        {
                            Samplers.Add(item);
                        }
                    });
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "There was an error loading the samplers");
                }
            });

            var extraTask = Task.Run(async () =>
            {
                try
                {
                    var modelPack = await _imageService.GetExtraModelsAsync();
                    _dispatcherQueue.TryEnqueue(() =>
                    {
                        foreach (var item in modelPack.Embeddings)
                        {
                            Embeddings.Add(item);
                        }

                        foreach (var item in modelPack.Loras)
                        {
                            Loras.Add(item);
                        }
                    });
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "There was an error loading the extra model");
                }
            });

            await Task.WhenAll(modelTask, samplerTask, extraTask);

            _isInitialized = Samplers.Count > 0;
        }

        if (_tempOptions != null)
        {
            var modelName = string.Empty;
            var samplerName = string.Empty;

            if (!string.IsNullOrEmpty(_tempOptions.Model))
            {
                var originalModel = Models.FirstOrDefault(p => p.Name.Contains(_tempOptions.Model, StringComparison.InvariantCultureIgnoreCase));
                if (originalModel != null)
                {
                    modelName = originalModel.Name;
                }
                else if (_tempOptions.Model.Length > 5)
                {
                    var modelFirst5Characters = _tempOptions.Model.Substring(0, 5);
                    var similarModel = Models.FirstOrDefault(p => p.Name.Contains(modelFirst5Characters, StringComparison.InvariantCultureIgnoreCase));
                    if (similarModel != null)
                    {
                        modelName = similarModel.Name;
                    }
                }
            }

            if (string.IsNullOrEmpty(modelName))
            {
                modelName = Models.FirstOrDefault()?.Name;
            }

            if (!string.IsNullOrEmpty(_tempOptions.Sampler))
            {
                var originSampler = Samplers.FirstOrDefault(p => p.Name.Contains(_tempOptions.Sampler, StringComparison.OrdinalIgnoreCase));
                if (originSampler != null)
                {
                    samplerName = originSampler.Name;
                }
            }

            if (string.IsNullOrEmpty(samplerName))
            {
                samplerName = Samplers.FirstOrDefault()?.Name;
            }

            _tempOptions.Model = modelName;
            _tempOptions.Sampler = samplerName;
            var vm = Locator.Current.GetService<IImageGenerateOptionsViewModel>();
            vm.Initialize(_tempOptions);
            Options = vm;
        }

        if (Options == null)
        {
            var vm = Locator.Current.GetService<IImageGenerateOptionsViewModel>();
            vm.Initialize();
            vm.Model = Models.FirstOrDefault()?.Name;
            vm.Sampler = Samplers.FirstOrDefault()?.Name;
            Options = vm;
        }
    }

    [RelayCommand]
    private async Task GenerateAsync()
    {
        if (string.IsNullOrEmpty(Options.Model)
            || string.IsNullOrEmpty(Options.Sampler)
            || string.IsNullOrEmpty(Prompt))
        {
            _appViewModel.ShowTip(_resourceToolkit.GetLocalizedString(StringNames.NeedFillRequiredFields), InfoType.Error);
            return;
        }

        Cancel();
        _cancellationTokenSource = new CancellationTokenSource();
        var options = Options.GetOptions();
        ImageMetadata = string.Empty;
        IsFailed = false;
        _imageBytes = default;
        try
        {
            var (imageData, context) = await _imageService.GenerateImageAsync(Prompt, NegativePrompt, options, _cancellationTokenSource.Token);
            if (imageData != default)
            {
                _imageBytes = imageData;
                ImageGenerated?.Invoke(this, imageData);
                ImageMetadata = context;
            }
            else
            {
                IsFailed = true;
            }
        }
        catch (Exception)
        {
            IsFailed = true;
        }

        _cancellationTokenSource = default;
    }

    [RelayCommand]
    private async Task SaveImageAsync()
    {
        if (IsFailed || _imageBytes == default)
        {
            return;
        }

        var fileObj = await _fileToolkit.SaveFileAsync(Guid.NewGuid().ToString("N") + ".jpeg", _appViewModel.MainWindow);
        if (fileObj is StorageFile file)
        {
            await FileIO.WriteBytesAsync(file, _imageBytes);
            _appViewModel.ShowTip(_resourceToolkit.GetLocalizedString(StringNames.FileSaved), InfoType.Success);
        }
    }

    [RelayCommand]
    private async Task RefreshModelAsync()
    {
        await _imageService.InitializeModelsIfNotReadyAsync(true);
        TryClear(Models);
        var models = await _imageService.GetModelsAsync();
        foreach (var item in models)
        {
            Models.Add(item);
        }
    }

    [RelayCommand]
    private async Task RefreshExtraModelAsync()
    {
        await _imageService.InitializeExtraModelsIfNotReadyAsync(true);
        TryClear(Embeddings);
        TryClear(Loras);
        var modelPack = await _imageService.GetExtraModelsAsync();
        foreach (var item in modelPack.Embeddings)
        {
            Embeddings.Add(item);
        }

        foreach (var item in modelPack.Loras)
        {
            Loras.Add(item);
        }
    }

    [RelayCommand]
    private void Cancel()
    {
        if (_cancellationTokenSource != null && _cancellationTokenSource.Token.CanBeCanceled)
        {
            _cancellationTokenSource?.Cancel();
            _cancellationTokenSource = default;
        }
    }

    partial void OnIsInSettingsChanged(bool value)
        => _appViewModel.IsBackButtonShown = value;
}
