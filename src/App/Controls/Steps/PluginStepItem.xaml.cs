// Copyright (c) Fantasy Copilot. All rights reserved.

using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using FantasyCopilot.DI.Container;
using FantasyCopilot.Models.App.Plugins;
using FantasyCopilot.Toolkits.Interfaces;

namespace FantasyCopilot.App.Controls.Steps;

/// <summary>
/// Plugin step item.
/// </summary>
public sealed partial class PluginStepItem : WorkflowStepControlBase
{
    private readonly ICacheToolkit _cacheToolkit;
    private bool _isInitialized;
    private Dictionary<string, string> _inputDictionary;

    /// <summary>
    /// Initializes a new instance of the <see cref="PluginStepItem"/> class.
    /// </summary>
    public PluginStepItem()
    {
        InitializeComponent();
        _cacheToolkit = Locator.Current.GetService<ICacheToolkit>();
        _inputDictionary = new Dictionary<string, string>();
        Loaded += OnLoadedAsync;
    }

    internal override async void OnViewModelChanged(DependencyPropertyChangedEventArgs e)
        => await InitializeAsync(true);

    private async void OnLoadedAsync(object sender, RoutedEventArgs e)
        => await InitializeAsync();

    private async Task InitializeAsync(bool refresh = false)
    {
        if (_isInitialized && !refresh)
        {
            return;
        }

        var command = (await _cacheToolkit.GetPluginConfigsAsync())
            .SelectMany(p => p.Commands)
            .FirstOrDefault(p => p.Identity == ViewModel.Step.PluginCommandId);
        if (command == default)
        {
            return;
        }

        DescriptionBlock.Text = command.Description;
        StepContainer.StepName = command.Name;
        StepContainer.StepDescription = command.Description;

        if (!string.IsNullOrEmpty(ViewModel.Step.Detail))
        {
            _inputDictionary = JsonSerializer.Deserialize<Dictionary<string, string>>(ViewModel.Step.Detail);
        }

        if (ViewModel.State == WorkflowStepState.Configuring)
        {
            ControlContainer.Children.Clear();
            var hasConfig = command.ConfigSet?.Any() ?? false;
            var hasParameters = command.Parameters != null && command.Parameters.Where(p => !string.IsNullOrEmpty(p.Id)).Any();
            ControlContainer.Visibility = hasConfig ? Visibility.Visible : Visibility.Collapsed;
            ParameterContainer.Visibility = hasParameters ? Visibility.Visible : Visibility.Collapsed;
            if (hasConfig)
            {
                foreach (var config in command.ConfigSet)
                {
                    if (config.Type == WorkflowConstants.InputConfigName)
                    {
                        var tb = new TextBox
                        {
                            Tag = config.VariableName,
                            TextWrapping = TextWrapping.Wrap,
                            HorizontalAlignment = HorizontalAlignment.Stretch,
                            AcceptsReturn = true,
                            MaxHeight = 100,
                            Header = config.Title ?? string.Empty,
                            Text = config.DefaultValue ?? string.Empty,
                        };
                        tb.LostFocus += OnInputBoxLostFocus;

                        if (_inputDictionary.ContainsKey(config.VariableName))
                        {
                            tb.Text = _inputDictionary[config.VariableName];
                        }

                        ControlContainer.Children.Add(tb);
                        if (!_inputDictionary.ContainsKey(config.VariableName))
                        {
                            _inputDictionary.Add(config.VariableName, tb.Text);
                        }
                    }
                    else if (config.Type == WorkflowConstants.OptionConfigName)
                    {
                        var cb = new ComboBox
                        {
                            Tag = config.VariableName,
                            ItemsSource = config.Options,
                            ItemTemplate = OptionItemTemplate,
                            HorizontalAlignment = HorizontalAlignment.Stretch,
                            Header = config.Title ?? string.Empty,
                        };

                        if (!string.IsNullOrEmpty(config.DefaultValue))
                        {
                            var selectedItem = config.Options.FirstOrDefault(p => p.Id == config.DefaultValue);
                            if (selectedItem != default)
                            {
                                cb.SelectedItem = selectedItem;
                            }
                        }

                        cb.SelectionChanged += OnComboBoxSelectionChanged;

                        if (_inputDictionary.ContainsKey(config.VariableName))
                        {
                            var item = config.Options.FirstOrDefault(p => p.Id == _inputDictionary[config.VariableName]);
                            cb.SelectedItem = item;
                        }

                        ControlContainer.Children.Add(cb);
                        if (!_inputDictionary.ContainsKey(config.VariableName))
                        {
                            _inputDictionary.Add(config.VariableName, config.DefaultValue);
                        }
                    }
                }

                ViewModel.Step.Detail = JsonSerializer.Serialize(_inputDictionary);
            }

            if (hasParameters)
            {
                ParameterView.ItemsSource = command.Parameters.Where(p => !string.IsNullOrEmpty(p.Id)).ToList();
            }
        }

        _isInitialized = true;
    }

    private void OnComboBoxSelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        var cb = sender as ComboBox;
        var name = cb.Tag.ToString();
        var id = (cb.SelectedItem as InputOptionItem).Id;
        if (_inputDictionary.ContainsKey(name))
        {
            _inputDictionary[name] = id;
        }
        else
        {
            _inputDictionary.Add(name, id);
        }

        ViewModel.Step.Detail = JsonSerializer.Serialize(_inputDictionary);
    }

    private void OnInputBoxLostFocus(object sender, RoutedEventArgs e)
    {
        var tb = sender as TextBox;
        var name = tb.Tag.ToString();
        var text = tb.Text;
        if (_inputDictionary.ContainsKey(name))
        {
            _inputDictionary[name] = text;
        }
        else
        {
            _inputDictionary.Add(name, text);
        }

        ViewModel.Step.Detail = JsonSerializer.Serialize(_inputDictionary);
    }
}
