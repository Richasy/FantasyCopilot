// Copyright (c) Fantasy Copilot. All rights reserved.

namespace FantasyCopilot.App.Controls.Settings;

/// <summary>
/// Azure OpenAI setting section.
/// </summary>
public sealed partial class AISettingSection : SettingSectionBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="AISettingSection"/> class.
    /// </summary>
    public AISettingSection()
    {
        InitializeComponent();
        Loaded += OnLoaded;
    }

    private void OnLoaded(object sender, RoutedEventArgs e)
        => AISourceComboBox.SelectedIndex = (int)ViewModel.AiSource;

    private void OnAISourceComboBoxSelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (!IsLoaded && AISourceComboBox.SelectedIndex != -1)
        {
            return;
        }

        ViewModel.AiSource = (AISource)AISourceComboBox.SelectedIndex;
    }
}
