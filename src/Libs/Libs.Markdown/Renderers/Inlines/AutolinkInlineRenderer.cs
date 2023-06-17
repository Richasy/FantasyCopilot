// Copyright (c) Fantasy Copilot. All rights reserved.

using System;
using Markdig.Syntax.Inlines;
using Microsoft.UI.Xaml.Documents;

namespace FantasyCopilot.Libs.Markdown.Renderers.Inlines;

internal sealed class AutolinkInlineRenderer : WinUIObjectRenderer<AutolinkInline>
{
    protected override void Write(WinUIRenderer renderer, AutolinkInline obj)
    {
        var context = renderer.Context;
        try
        {
            var link = new Hyperlink
            {
                NavigateUri = new Uri(obj.Url),
                UnderlineStyle = UnderlineStyle.None,
                Foreground = context.LinkForeground ?? context.Foreground,
            };

            link.Inlines.Add(new Run { Text = obj.Url });
            renderer.Add(link);
            renderer.ExtractSpanAsChild();
        }
        catch (Exception)
        {
            renderer.WriteText(obj.Url);
        }
    }
}
