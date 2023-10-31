// Copyright (c) Richasy Assistant. All rights reserved.

using System;
using System.Threading;
using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.Extensions.Logging;
using Microsoft.UI.Dispatching;
using RichasyAssistant.Models.App;
using RichasyAssistant.Models.App.Image;
using RichasyAssistant.Libs.Services.Interfaces;
using RichasyAssistant.Toolkits.Interfaces;
using RichasyAssistant.ViewModels.Interfaces;

namespace RichasyAssistant.ViewModels;

/// <summary>
/// Text to image module view model.
/// </summary>
public sealed partial class TextToImageModuleViewModel
{
    private readonly IImageService _imageService;
    private readonly IResourceToolkit _resourceToolkit;
    private readonly IFileToolkit _fileToolkit;
    private readonly IAppViewModel _appViewModel;
    private readonly ILogger<TextToImageModuleViewModel> _logger;
    private readonly DispatcherQueue _dispatcherQueue;

    private bool _isInitialized;
    private GenerateOptions _tempOptions;
    private CancellationTokenSource _cancellationTokenSource;
    private byte[] _imageBytes;

    [ObservableProperty]
    private string _prompt;

    [ObservableProperty]
    private string _negativePrompt;

    [ObservableProperty]
    private bool _isGenerating;

    [ObservableProperty]
    private bool _isFailed;

    [ObservableProperty]
    private bool _isDisconnected;

    [ObservableProperty]
    private bool _isInitializing;

    [ObservableProperty]
    private string _imageMetadata;

    [ObservableProperty]
    private bool _isInSettings;

    [ObservableProperty]
    private IImageGenerateOptionsViewModel _options;

    /// <inheritdoc/>
    public event EventHandler<byte[]> ImageGenerated;

    /// <inheritdoc/>
    public SynchronizedObservableCollection<Model> Models { get; set; }

    /// <inheritdoc/>
    public SynchronizedObservableCollection<Sampler> Samplers { get; set; }

    /// <inheritdoc/>
    public SynchronizedObservableCollection<string> Embeddings { get; set; }

    /// <inheritdoc/>
    public SynchronizedObservableCollection<string> Loras { get; set; }
}
