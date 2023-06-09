// Copyright (c) Fantasy Copilot. All rights reserved.

using FantasyCopilot.ViewModels.Interfaces;
using Windows.Globalization.NumberFormatting;

namespace FantasyCopilot.App.Controls.Sessions;

/// <summary>
/// Session options panel.
/// </summary>
public sealed partial class SessionOptionsPanel : SessionOptionsPanelBase
{
    /// <summary>
    /// Dependency property for <see cref="StreamOutputVisibility"/>.
    /// </summary>
    public static readonly DependencyProperty StreamOutputVisibilityProperty =
        DependencyProperty.Register(nameof(StreamOutputVisibility), typeof(Visibility), typeof(SessionOptionsPanel), new PropertyMetadata(Visibility.Visible));

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
    public Visibility StreamOutputVisibility
    {
        get => (Visibility)GetValue(StreamOutputVisibilityProperty);
        set => SetValue(StreamOutputVisibilityProperty, value);
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
