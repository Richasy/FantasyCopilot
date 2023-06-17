// Copyright (c) Fantasy Copilot. All rights reserved.

using Markdig.Syntax;
using Microsoft.UI.Xaml.Documents;

namespace FantasyCopilot.Libs.Markdown.Renderers.Blocks;

internal sealed class ParagraphRenderer : WinUIObjectRenderer<ParagraphBlock>
{
    protected override void Write(WinUIRenderer renderer, ParagraphBlock obj)
    {
        var paragraph = new Paragraph();
        renderer.Add(paragraph);
        renderer.WriteLeafInline(obj);
        renderer.ExtractLastElementAsChild();
    }
}
