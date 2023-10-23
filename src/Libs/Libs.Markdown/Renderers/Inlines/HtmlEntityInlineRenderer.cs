// Copyright (c) Richasy Assistant. All rights reserved.

using Markdig.Syntax.Inlines;

namespace RichasyAssistant.Libs.Markdown.Renderers.Inlines;

internal sealed class HtmlEntityInlineRenderer : WinUIObjectRenderer<HtmlEntityInline>
{
    protected override void Write(WinUIRenderer renderer, HtmlEntityInline obj)
    {
        var transcoded = obj.Transcoded;
        renderer.WriteText(ref transcoded);
    }
}
