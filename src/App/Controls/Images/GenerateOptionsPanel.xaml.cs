// Copyright (c) Richasy Assistant. All rights reserved.

using System.ComponentModel;
using System.Linq;
using RichasyAssistant.DI.Container;
using RichasyAssistant.Models.App.Image;
using RichasyAssistant.ViewModels.Interfaces;
using Windows.Globalization.NumberFormatting;

namespace RichasyAssistant.App.Controls.Images;

/// <summary>
/// Image generate options panel.
/// </summary>
public sealed partial class GenerateOptionsPanel : GenerateOptionsPanelBase
{
    private readonly ITextToImageModuleViewModel _txt2imgViewModel;

    /// <summary>
    /// Initializes a new instance of the <see cref="GenerateOptionsPanel"/> class.
    /// </summary>
    public GenerateOptionsPanel()
    {
        InitializeComponent();
        _txt2imgViewModel = Locator.Current.GetService<ITextToImageModuleViewModel>();
        var rounder = new IncrementNumberRounder();
        rounder.RoundingAlgorithm = RoundingAlgorithm.RoundUp;

        var formatter = new DecimalFormatter
        {
            IntegerDigits = 1,
            FractionDigits = 0,
            NumberRounder = rounder,
        };

        WidthBox.NumberFormatter = formatter;
        HeightBox.NumberFormatter = formatter;
        ClipSkipBox.NumberFormatter = formatter;
        StepsBox.NumberFormatter = formatter;
        SeedBox.NumberFormatter = formatter;

        Loaded += OnLoaded;
    }

    internal override void OnViewModelChanged(DependencyPropertyChangedEventArgs e)
    {
        if (e.OldValue is IImageGenerateOptionsViewModel oldViewModel)
        {
            oldViewModel.PropertyChanged -= OnViewModelPropertyChanged;
        }

        if (e.NewValue is IImageGenerateOptionsViewModel newViewModel)
        {
            newViewModel.PropertyChanged += OnViewModelPropertyChanged;
            Initialize();
        }
    }

    private void OnLoaded(object sender, RoutedEventArgs e)
        => Initialize();

    private void OnModelComboBoxSelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (!IsLoaded)
        {
            return;
        }

        if (ModelComboBox.SelectedItem is Model model)
        {
            ViewModel.Model = model.Name;
        }
    }

    private void OnSamplerComboBoxSelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (!IsLoaded)
        {
            return;
        }

        if (SamplerComboBox.SelectedItem is Sampler sampler)
        {
            ViewModel.Sampler = sampler.Name;
        }
    }

    private void OnViewModelPropertyChanged(object sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(IImageGenerateOptionsViewModel.Model))
        {
            LoadModel();
        }
        else if (e.PropertyName == nameof(IImageGenerateOptionsViewModel.Sampler))
        {
            LoadSampler();
        }
    }

    private void Initialize()
    {
        if (ViewModel == null)
        {
            return;
        }

        LoadModel();
        LoadSampler();
    }

    private void LoadModel()
    {
        var model = ViewModel.Model;
        var sourceModel = _txt2imgViewModel.Models.FirstOrDefault(p => p.Name == model);

        if (ModelComboBox.SelectedItem is Model m && m == sourceModel)
        {
            return;
        }

        ModelComboBox.SelectedItem = sourceModel;
    }

    private void LoadSampler()
    {
        var sampler = ViewModel.Sampler;
        var sourceSampler = _txt2imgViewModel.Samplers.FirstOrDefault(p => p.Name == sampler);

        if (SamplerComboBox.SelectedItem is Sampler s && s == sourceSampler)
        {
            return;
        }

        SamplerComboBox.SelectedItem = sourceSampler;
    }

    private async void OnModelRefreshButtonClickAsync(object sender, RoutedEventArgs e)
    {
        ModelRefreshButton.IsEnabled = false;
        await _txt2imgViewModel.RefreshModelCommand.ExecuteAsync(default);
        LoadModel();
        ModelRefreshButton.IsEnabled = true;
    }
}

/// <summary>
/// Base for <see cref="GenerateOptionsPanel"/>.
/// </summary>
public class GenerateOptionsPanelBase : ReactiveUserControl<IImageGenerateOptionsViewModel>
{
}
