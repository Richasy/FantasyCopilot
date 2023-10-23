// Copyright (c) Richasy Assistant. All rights reserved.

using Markdig.Syntax;

namespace RichasyAssistant.Libs.Markdown.Renderers.Blocks;

internal sealed class HtmlBlockRenderer : WinUIObjectRenderer<HtmlBlock>
{
    protected override void Write(WinUIRenderer renderer, HtmlBlock obj)
    {
        renderer.WriteLeafRawLines(obj);
    }
}
