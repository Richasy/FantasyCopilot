// Copyright (c) Richasy Assistant. All rights reserved.

using Markdig.Extensions.Emoji;
using Microsoft.UI.Xaml.Documents;

namespace RichasyAssistant.Libs.Markdown.Renderers.Inlines;

internal sealed class EmojiInlineRenderer : WinUIObjectRenderer<EmojiInline>
{
    protected override void Write(WinUIRenderer renderer, EmojiInline obj)
    {
        var run = new Run
        {
            Text = obj.Match,
            FontFamily = renderer.Context.EmojiFontFamily,
        };

        renderer.Add(run);
    }
}
