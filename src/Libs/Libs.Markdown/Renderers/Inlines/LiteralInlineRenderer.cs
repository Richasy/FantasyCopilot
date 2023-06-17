// Copyright (c) Fantasy Copilot. All rights reserved.

using Markdig.Syntax.Inlines;

namespace FantasyCopilot.Libs.Markdown.Renderers.Inlines;

internal sealed class LiteralInlineRenderer : WinUIObjectRenderer<LiteralInline>
{
    protected override void Write(WinUIRenderer renderer, LiteralInline obj)
    {
        if (obj.Content.IsEmpty)
        {
            return;
        }

        renderer.WriteText(ref obj.Content);
    }
}
