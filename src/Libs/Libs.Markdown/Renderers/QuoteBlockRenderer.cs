// Copyright (c) Fantasy Copilot. All rights reserved.

using System;
using Markdig.Syntax;
using Microsoft.UI.Xaml.Controls;

namespace FantasyCopilot.Libs.Markdown.Renderers;

internal sealed class QuoteBlockRenderer : WinUIObjectRenderer<QuoteBlock>
{
    protected override void Write(WinUIRenderer renderer, QuoteBlock obj)
    {
        if (renderer == null)
        {
            throw new ArgumentNullException(nameof(renderer));
        }

        if (obj == null)
        {
            throw new ArgumentNullException(nameof(obj));
        }

        var context = renderer.Context;
        var border = new Border
        {
            Margin = context.QuoteMargin,
            Background = context.QuoteBackground,
            BorderBrush = context.QuoteBorderBrush,
            BorderThickness = context.QuoteBorderThickness,
            Padding = context.QuotePadding,
        };

        renderer.Add(border);
        var panel = new StackPanel();
        renderer.Add(panel);
        renderer.WriteChildren(obj);
        renderer.RedirectToChild();
        renderer.RedirectToChild();
    }
}
