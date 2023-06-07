// Copyright (c) Fantasy Copilot. All rights reserved.

using System;
using System.Collections.ObjectModel;
using System.Threading;
using CommunityToolkit.Mvvm.ComponentModel;
using FantasyCopilot.Models.App.Image;
using FantasyCopilot.Services.Interfaces;
using FantasyCopilot.Toolkits.Interfaces;
using FantasyCopilot.ViewModels.Interfaces;
using Microsoft.Extensions.Logging;
using Microsoft.UI.Dispatching;

namespace FantasyCopilot.ViewModels;

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
    public ObservableCollection<Model> Models { get; set; }

    /// <inheritdoc/>
    public ObservableCollection<Sampler> Samplers { get; set; }

    /// <inheritdoc/>
    public ObservableCollection<string> Embeddings { get; set; }

    /// <inheritdoc/>
    public ObservableCollection<string> Loras { get; set; }
}
