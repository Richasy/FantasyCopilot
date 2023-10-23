// Copyright (c) Richasy Assistant. All rights reserved.

using RichasyAssistant.ViewModels.Interfaces;
using Windows.Globalization.NumberFormatting;

namespace RichasyAssistant.App.Controls.Sessions;

/// <summary>
/// Session options panel.
/// </summary>
public sealed partial class SessionOptionsPanel : SessionOptionsPanelBase
{
    /// <summary>
    /// Dependency property for <see cref="IsSemanticOptions"/>.
    /// </summary>
    public static readonly DependencyProperty IsSemanticOptionsProperty =
        DependencyProperty.Register(nameof(IsSemanticOptions), typeof(bool), typeof(SessionOptionsPanel), new PropertyMetadata(false));

    /// <summary>
    /// Initializes a new instance of the <see cref="SessionOptionsPanel"/> class.
    /// </summary>
    public SessionOptionsPanel()
    {
        InitializeComponent();
        var rounder = new IncrementNumberRounder
        {
            Increment = 1,
            RoundingAlgorithm = RoundingAlgorithm.RoundUp,
        };

        var formatter = new DecimalFormatter
        {
            IntegerDigits = 1,
            FractionDigits = 0,
            NumberRounder = rounder,
        };
        TokenNumberBox.NumberFormatter = formatter;
    }

    /// <summary>
    /// Gets or sets the visibility of the stream output toggle.
    /// </summary>
    public bool IsSemanticOptions
    {
        get => (bool)GetValue(IsSemanticOptionsProperty);
        set => SetValue(IsSemanticOptionsProperty, value);
    }

    private void OnTokenNumberValueChanged(NumberBox sender, NumberBoxValueChangedEventArgs args)
    {
        if (ViewModel != null)
        {
            var tokenNum = args.NewValue;
            ViewModel.MaxResponseTokens = Convert.ToInt32(tokenNum);
        }
    }
}

/// <summary>
/// Base of <see cref="SessionOptionsPanel"/>.
/// </summary>
public class SessionOptionsPanelBase : ReactiveUserControl<ISessionOptionsViewModel>
{
}
