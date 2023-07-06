// Copyright (c) Fantasy Copilot. All rights reserved.

using System.ComponentModel;
using FantasyCopilot.ViewModels.Interfaces;

namespace FantasyCopilot.App.Controls.Settings;

/// <summary>
/// Azure OpenAI setting section.
/// </summary>
public sealed partial class AISettingSection : SettingSectionBase
{
    /// <summary>
    /// Dependency property for <see cref="IsCustomEnabled"/>.
    /// </summary>
    public static readonly DependencyProperty IsCustomEnabledProperty =
        DependencyProperty.Register(nameof(IsCustomEnabled), typeof(bool), typeof(AISettingSection), new PropertyMetadata(true));

    /// <summary>
    /// Initializes a new instance of the <see cref="AISettingSection"/> class.
    /// </summary>
    public AISettingSection()
    {
        InitializeComponent();
        Loaded += OnLoaded;
        Unloaded += OnUnloaded;
    }

    /// <summary>
    /// Whether to support custom models.
    /// </summary>
    public bool IsCustomEnabled
    {
        get => (bool)GetValue(IsCustomEnabledProperty);
        set => SetValue(IsCustomEnabledProperty, value);
    }

    private void OnLoaded(object sender, RoutedEventArgs e)
    {
        AISourceComboBox.SelectedIndex = (int)ViewModel.AiSource;
        ViewModel.LoadModelsCommand.Execute(false);
        ViewModel.PropertyChanged += OnViewModelPropertyChangedAsync;
    }

    private void OnUnloaded(object sender, RoutedEventArgs e)
        => ViewModel.PropertyChanged -= OnViewModelPropertyChangedAsync;

    private async void OnViewModelPropertyChangedAsync(object sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(ViewModel.SelectedChatConnector))
        {
            await Task.Delay(100);
            ChatModelComboBox.SelectedItem = ViewModel.SelectedChatConnector;
        }
    }

    private void OnAISourceComboBoxSelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (!IsLoaded && AISourceComboBox.SelectedIndex != -1)
        {
            return;
        }

        ViewModel.AiSource = (AISource)AISourceComboBox.SelectedIndex;
    }

    private void OnAIKeyBoxLostFocus(object sender, RoutedEventArgs e)
        => ViewModel.LoadModelsCommand.Execute(true);

    private async void OnChatModelComboBoxLoadedAsync(object sender, RoutedEventArgs e)
    {
        await Task.Delay(100);
        ChatModelComboBox.SelectedItem = ViewModel.SelectedChatConnector;
    }

    private void OnChatModelComboBoxSelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        var item = ChatModelComboBox.SelectedItem as IConnectorConfigViewModel;
        ViewModel.SelectedChatConnector = item;
    }
}
