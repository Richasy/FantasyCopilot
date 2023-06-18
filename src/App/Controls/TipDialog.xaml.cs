// Copyright (c) Fantasy Copilot. All rights reserved.

namespace FantasyCopilot.App.Controls;

/// <summary>
/// Tip dialog.
/// </summary>
public sealed partial class TipDialog : ContentDialog
{
    /// <summary>
    /// Initializes a new instance of the <see cref="TipDialog"/> class.
    /// </summary>
    public TipDialog(string text)
    {
        InitializeComponent();
        TipBlock.Text = text;
    }
}
