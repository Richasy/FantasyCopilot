// Copyright (c) Richasy Assistant. All rights reserved.

using System.ComponentModel;
using RichasyAssistant.DI.Container;
using RichasyAssistant.ViewModels.Interfaces;

namespace RichasyAssistant.App.Controls.Steps
{
    /// <summary>
    /// Input voice step item.
    /// </summary>
    public sealed partial class InputVoiceStepItem : WorkflowStepControlBase
    {
        /// <summary>
        /// Dependency property for <see cref="IsRecordingContainerShown"/>.
        /// </summary>
        public static readonly DependencyProperty IsRecordingContainerShownProperty =
            DependencyProperty.Register(nameof(IsRecordingContainerShown), typeof(bool), typeof(InputVoiceStepItem), new PropertyMetadata(default));

        private readonly ISpeechRecognizeModuleViewModel _speechRecognizeVM;
        private readonly IAppViewModel _appViewModel;

        /// <summary>
        /// Initializes a new instance of the <see cref="InputVoiceStepItem"/> class.
        /// </summary>
        public InputVoiceStepItem()
        {
            InitializeComponent();
            _speechRecognizeVM = Locator.Current.GetService<ISpeechRecognizeModuleViewModel>();
            _appViewModel = Locator.Current.GetService<IAppViewModel>();
            Loaded += OnLoaded;
            Unloaded += OnUnloaded;
        }

        /// <summary>
        /// Is recording container shown.
        /// </summary>
        public bool IsRecordingContainerShown
        {
            get => (bool)GetValue(IsRecordingContainerShownProperty);
            set => SetValue(IsRecordingContainerShownProperty, value);
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            if (_appViewModel.IsVoiceAvailable)
            {
                _speechRecognizeVM.InitializeCommand.Execute(default);
                _speechRecognizeVM.PropertyChanged += OnSpeechViewModelPropertyChanged;
            }
        }

        private void OnUnloaded(object sender, RoutedEventArgs e)
            => _speechRecognizeVM.PropertyChanged -= OnSpeechViewModelPropertyChanged;

        private void OnSpeechViewModelPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(_speechRecognizeVM.IsRecording) && _speechRecognizeVM.IsRecording)
            {
                IsRecordingContainerShown = true;
            }
        }

        private void OnInputBoxTextChanged(object sender, TextChangedEventArgs e)
            => StartWorkflowButton.IsEnabled = InputBox.Text.Length > 0;

        private void OnStartButtonClick(object sender, RoutedEventArgs e)
        {
            var text = InputBox.Text;
            _speechRecognizeVM.Text = string.Empty;
            if (string.IsNullOrEmpty(text))
            {
                return;
            }

            var runnerVM = Locator.Current.GetService<IWorkflowRunnerViewModel>();
            runnerVM.ExecuteCommand.Execute(text);
            IsRecordingContainerShown = false;
        }

        private void OnCancelButtonClick(object sender, RoutedEventArgs e)
        {
            _speechRecognizeVM.Text = string.Empty;
            IsRecordingContainerShown = false;
        }
    }
}
