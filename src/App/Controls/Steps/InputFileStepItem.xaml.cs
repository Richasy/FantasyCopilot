﻿// Copyright (c) Fantasy Copilot. All rights reserved.

using FantasyCopilot.DI.Container;
using FantasyCopilot.Toolkits.Interfaces;
using FantasyCopilot.ViewModels.Interfaces;
using Windows.Storage;

namespace FantasyCopilot.App.Controls.Steps
{
    /// <summary>
    /// Input file step item.
    /// </summary>
    public sealed partial class InputFileStepItem : WorkflowStepControlBase
    {
        private readonly IFileToolkit _fileToolkit;

        /// <summary>
        /// Initializes a new instance of the <see cref="InputFileStepItem"/> class.
        /// </summary>
        public InputFileStepItem()
        {
            InitializeComponent();
            _fileToolkit = Locator.Current.GetService<IFileToolkit>();
        }

        private void OnStartButtonClick(object sender, RoutedEventArgs e)
        {
            var text = PathBox.Text;
            var runnerVM = Locator.Current.GetService<IWorkflowRunnerViewModel>();
            runnerVM.ExecuteCommand.Execute(text);
        }

        private async void OnPickFileButtonClickAsync(object sender, RoutedEventArgs e)
        {
            var fileObj = await _fileToolkit.PickFileAsync("*", MainWindow.Instance);
            if (fileObj is not StorageFile file)
            {
                return;
            }

            PathBox.Text = file.Path;
        }

        private void OnInputBoxTextChanged(object sender, TextChangedEventArgs e)
            => StartButton.IsEnabled = !string.IsNullOrEmpty(PathBox.Text);
    }
}
