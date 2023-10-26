// Copyright (c) Richasy Assistant. All rights reserved.

using RichasyAssistant.DI.Container;
using RichasyAssistant.Toolkits.Interfaces;
using RichasyAssistant.ViewModels.Interfaces;

namespace RichasyAssistant.App.Controls.Sessions;

/// <summary>
/// Session save dialog.
/// </summary>
public sealed partial class SessionSaveDialog : ContentDialog
{
    private readonly ISessionViewModel _viewModel;
    private SessionMetadata _metadata;

    /// <summary>
    /// Initializes a new instance of the <see cref="SessionSaveDialog"/> class.
    /// </summary>
    public SessionSaveDialog() => InitializeComponent();

    /// <summary>
    /// Initializes a new instance of the <see cref="SessionSaveDialog"/> class.
    /// </summary>
    public SessionSaveDialog(ISessionViewModel vm)
        : this() => _viewModel = vm;

    /// <summary>
    /// Initializes a new instance of the <see cref="SessionSaveDialog"/> class.
    /// </summary>
    public SessionSaveDialog(SessionMetadata data)
        : this()
    {
        _metadata = data;
        TitleBox.Text = data.Name;
        DescriptionBox.Text = data.Description;
    }

    private async void OnPrimaryButtonClickAsync(ContentDialog sender, ContentDialogButtonClickEventArgs args)
    {
        args.Cancel = true;
        if (string.IsNullOrEmpty(TitleBox.Text))
        {
            return;
        }

        IsPrimaryButtonEnabled = false;
        if (_viewModel != null)
        {
            var metadata = _viewModel.GetMetadata();
            var prompt = metadata?.SystemPrompt ?? string.Empty;
            _metadata = new SessionMetadata
            {
                Id = Guid.NewGuid().ToString("N"),
                Name = TitleBox.Text,
                Description = DescriptionBox.Text,
                SystemPrompt = prompt,
            };

            _viewModel.UseNewMetadata(_metadata);
            _viewModel.SaveCommand.Execute(default);
        }
        else
        {
            _metadata.Name = TitleBox.Text;
            _metadata.Description = DescriptionBox.Text;
        }

        var cacheToolkit = Locator.Current.GetService<ICacheToolkit>();
        await cacheToolkit.AddOrUpdateSessionMetadataAsync(_metadata);
        Hide();
    }
}
