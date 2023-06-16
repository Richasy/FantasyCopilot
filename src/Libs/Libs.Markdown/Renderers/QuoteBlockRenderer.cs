// Copyright (c) Fantasy Copilot. All rights reserved.

using System;
using Markdig.Syntax;
using Microsoft.UI;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;

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

        var border = new Border
        {
            Background = new SolidColorBrush(Colors.Black),
        };

        renderer.Add(border);
        var panel = new StackPanel();
        renderer.Add(panel);
        renderer.WriteChildren(obj);
        renderer.RedirectToChild();
        renderer.RedirectToChild();
    }
}
