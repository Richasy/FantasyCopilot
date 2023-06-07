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
