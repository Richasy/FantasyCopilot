﻿// Copyright (c) Richasy Assistant. All rights reserved.

using Markdig.Syntax.Inlines;

namespace RichasyAssistant.Libs.Markdown.Renderers.Inlines;

internal sealed class DelimiterInlineRenderer : WinUIObjectRenderer<DelimiterInline>
{
    protected override void Write(WinUIRenderer renderer, DelimiterInline obj)
    {
        renderer.WriteText(obj.ToLiteral());
        renderer.WriteChildren(obj);
    }
}
