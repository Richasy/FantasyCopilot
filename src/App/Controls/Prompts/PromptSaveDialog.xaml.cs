// Copyright (c) Fantasy Copilot. All rights reserved.

using FantasyCopilot.DI.Container;
using FantasyCopilot.Toolkits.Interfaces;

namespace FantasyCopilot.App.Controls
{
    /// <summary>
    /// Custom prompt save dialog.
    /// </summary>
    public sealed partial class PromptSaveDialog : ContentDialog
    {
        private SessionMetadata _data;

        /// <summary>
        /// Initializes a new instance of the <see cref="PromptSaveDialog"/> class.
        /// </summary>
        public PromptSaveDialog() => InitializeComponent();

        /// <summary>
        /// Initializes a new instance of the <see cref="PromptSaveDialog"/> class.
        /// </summary>
        public PromptSaveDialog(SessionMetadata data)
            : this()
        {
            _data = data;
            TitleBox.Text = data.Name;
            DescriptionBox.Text = data.Description;
            PromptBox.Text = data.SystemPrompt;
        }

        private async void OnPrimaryButtonClickAsync(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            args.Cancel = true;
            if (string.IsNullOrEmpty(TitleBox.Text) || string.IsNullOrEmpty(PromptBox.Text))
            {
                return;
            }

            IsPrimaryButtonEnabled = false;

            _data ??= new SessionMetadata
            {
                Id = Guid.NewGuid().ToString("N"),
            };

            _data.Name = TitleBox.Text;
            _data.Description = DescriptionBox.Text;
            _data.SystemPrompt = PromptBox.Text;
            await Locator.Current.GetService<ICacheToolkit>().AddOrUpdatePromptAsync(_data);
            Hide();
        }
    }
}
