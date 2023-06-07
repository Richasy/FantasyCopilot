// Copyright (c) Fantasy Copilot. All rights reserved.

using System.ComponentModel;
using System.IO;
using FantasyCopilot.DI.Container;
using FantasyCopilot.Toolkits.Interfaces;
using FantasyCopilot.ViewModels.Interfaces;
using Windows.Media.Playback;
using Windows.Storage;
using Windows.System;

namespace FantasyCopilot.App.Controls.Steps;

/// <summary>
/// Output voice step item.
/// </summary>
public sealed partial class OutputVoiceStepItem : WorkflowStepControlBase
{
    /// <summary>
    /// Dependency property of <see cref="IsPlaying"/>.
    /// </summary>
    public static readonly DependencyProperty IsPlayingProperty =
        DependencyProperty.Register(nameof(IsPlaying), typeof(bool), typeof(OutputVoiceStepItem), new PropertyMetadata(false));

    private MediaPlayer _player;

    /// <summary>
    /// Initializes a new instance of the <see cref="OutputVoiceStepItem"/> class.
    /// </summary>
    public OutputVoiceStepItem()
        => InitializeComponent();

    /// <summary>
    /// Is audio playing.
    /// </summary>
    public bool IsPlaying
    {
        get => (bool)GetValue(IsPlayingProperty);
        set => SetValue(IsPlayingProperty, value);
    }

    internal override void OnViewModelChanged(DependencyPropertyChangedEventArgs e)
    {
        if (e.OldValue is IWorkflowStepViewModel oldVM)
        {
            oldVM.PropertyChanged -= OnViewModelPropertyChangedAsync;
        }

        if (e.NewValue is IWorkflowStepViewModel newVM)
        {
            newVM.PropertyChanged += OnViewModelPropertyChangedAsync;
        }

        _player?.Dispose();
        _player = null;
    }

    private async void OnViewModelPropertyChangedAsync(object sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(ViewModel.State))
        {
            if (ViewModel.State == WorkflowStepState.Output)
            {
                HideError();
                var resToolkit = Locator.Current.GetService<IResourceToolkit>();
                try
                {
                    var path = ViewModel.Step.Detail;
                    if (File.Exists(path))
                    {
                        var file = await StorageFile.GetFileFromPathAsync(path);
                        _player = new MediaPlayer();
                        _player.CurrentStateChanged += OnPlayerStateChanged;
                        _player.MediaFailed += OnPlayerFailed;
                        _player.MediaEnded += OnPlayerEnded;
                        _player.MediaOpened += OnPlayerOpened;
                        _player.SetFileSource(file);
                    }
                    else
                    {
                        ShowError(resToolkit.GetLocalizedString(StringNames.InvalidFilePath));
                    }
                }
                catch (Exception ex)
                {
                    ShowError(ex.Message);
                }
            }
        }
    }

    private void OnPlayerOpened(MediaPlayer sender, object args)
    {
        DispatcherQueue.TryEnqueue(() =>
        {
            Progress.Maximum = sender.PlaybackSession.NaturalDuration.TotalMilliseconds;
            Progress.Value = sender.PlaybackSession.Position.TotalMilliseconds;
            _player.PlaybackSession.PositionChanged -= OnPlayerPositionChanged;
            _player.PlaybackSession.PositionChanged += OnPlayerPositionChanged;
            _player.Play();
        });
    }

    private void OnPlayerPositionChanged(MediaPlaybackSession sender, object args)
    {
        DispatcherQueue.TryEnqueue(() =>
        {
            Progress.Maximum = sender.NaturalDuration.TotalMilliseconds;
            Progress.Value = sender.Position.TotalMilliseconds;
        });
    }

    private void OnPlayerEnded(MediaPlayer sender, object args)
    {
        DispatcherQueue.TryEnqueue(() =>
        {
            _player.PlaybackSession.Position = TimeSpan.Zero;
        });
    }

    private void OnPlayerFailed(MediaPlayer sender, MediaPlayerFailedEventArgs args)
    {
        DispatcherQueue.TryEnqueue(() =>
        {
            ShowError(args.ErrorMessage);
        });
    }

    private void OnPlayerStateChanged(MediaPlayer sender, object args)
    {
        DispatcherQueue.TryEnqueue(() =>
        {
            if (sender.PlaybackSession == null)
            {
                IsPlaying = false;
                return;
            }

            IsPlaying = sender.PlaybackSession.PlaybackState == MediaPlaybackState.Playing;
        });
    }

    private void OnPlayPauseButtonClick(object sender, RoutedEventArgs e)
    {
        if (_player == null)
        {
            return;
        }

        if (IsPlaying)
        {
            _player.Pause();
        }
        else
        {
            _player.Play();
        }
    }

    private void OnResetButtonClick(object sender, RoutedEventArgs e)
    {
        if (_player == null)
        {
            return;
        }

        _player.Position = TimeSpan.Zero;
    }

    private async void OnSaveButtonClickAsync(object sender, RoutedEventArgs e)
    {
        if (_player == null)
        {
            return;
        }

        var sourceFilePath = ViewModel.Step.Detail;
        var fileName = Path.GetFileName(sourceFilePath);
        var fileToolkit = Locator.Current.GetService<IFileToolkit>();
        var file = await fileToolkit.SaveFileAsync(fileName, MainWindow.Instance);
        if (file is StorageFile sf)
        {
            var appVM = Locator.Current.GetService<IAppViewModel>();
            var resToolkit = Locator.Current.GetService<IResourceToolkit>();
            var sourceFile = await StorageFile.GetFileFromPathAsync(sourceFilePath);
            await sourceFile.CopyAndReplaceAsync(sf);
            appVM.ShowTip(resToolkit.GetLocalizedString(StringNames.FileSaved), InfoType.Success);
        }
    }

    private void ShowError(string msg)
    {
        ErrorBlock.Visibility = Visibility.Visible;
        PlayContainer.Visibility = Visibility.Collapsed;
        ErrorBlock.Text = string.IsNullOrEmpty(msg) ? "Unknown error" : msg;
        _player?.Dispose();
        _player = null;
    }

    private void HideError()
    {
        ErrorBlock.Visibility = Visibility.Collapsed;
        PlayContainer.Visibility = Visibility.Visible;
    }
}
