// Copyright (c) Richasy Assistant. All rights reserved.

namespace RichasyAssistant.App.Controls.Settings;

/// <summary>
/// Azure translate setting section.
/// </summary>
public sealed partial class TranslateSettingSection : SettingSectionBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="TranslateSettingSection"/> class.
    /// </summary>
    public TranslateSettingSection()
    {
        InitializeComponent();
        Loaded += OnLoaded;
    }

    private void OnLoaded(object sender, RoutedEventArgs e)
        => TranslateSourceComboBox.SelectedIndex = (int)ViewModel.TranslateSource;

    private void OnTranslateSourceComboBoxSelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (!IsLoaded && TranslateSourceComboBox.SelectedIndex != -1)
        {
            return;
        }

        ViewModel.TranslateSource = (TranslateSource)TranslateSourceComboBox.SelectedIndex;
    }
}
