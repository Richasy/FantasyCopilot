// Copyright (c) Fantasy Copilot. All rights reserved.

using System.ComponentModel;
using FantasyCopilot.ViewModels.Interfaces;

namespace FantasyCopilot.App.Pages;

/// <summary>
/// Image generate service page.
/// </summary>
public sealed partial class ImagePage : ImagePageBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ImagePage"/> class.
    /// </summary>
    public ImagePage() => InitializeComponent();

    /// <inheritdoc/>
    protected override void OnPageLoaded()
    {
        CoreViewModel.CheckImageServiceCommand.Execute(default);
        ModulePicker.SelectedIndex = (int)ViewModel.SelectionType;
        ViewModel.PropertyChanged += OnViewModelPropertyChanged;
    }

    /// <inheritdoc/>
    protected override void OnPageUnloaded()
        => ViewModel.PropertyChanged += OnViewModelPropertyChanged;

    private void OnViewModelPropertyChanged(object sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(ViewModel.SelectionType))
        {
            var index = (int)ViewModel.SelectionType;
            if (index != ModulePicker.SelectedIndex)
            {
                ModulePicker.SelectedIndex = (int)ViewModel.SelectionType;
            }
        }
    }

    private void OnModulePickerSelectionChanged(object sender, SelectionChangedEventArgs e)
        => ViewModel.SelectionType = (ImageModuleType)ModulePicker.SelectedIndex;
}

/// <summary>
/// Base for <see cref="ImagePage"/>.
/// </summary>
public class ImagePageBase : PageBase<IImagePageViewModel>
{
}
