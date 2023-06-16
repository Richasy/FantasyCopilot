﻿// Copyright (c) Fantasy Copilot. All rights reserved.

using System;
using Markdig.Syntax;
using Microsoft.UI.Xaml.Documents;

namespace FantasyCopilot.Libs.Markdown.Renderers;

internal sealed class ParagraphRenderer : WinUIObjectRenderer<ParagraphBlock>
{
    protected override void Write(WinUIRenderer renderer, ParagraphBlock obj)
    {
        if (renderer == null)
        {
            throw new ArgumentNullException(nameof(renderer));
        }

        if (obj == null)
        {
            throw new ArgumentNullException(nameof(obj));
        }

        var paragraph = new Paragraph();
        renderer.Add(paragraph);
        renderer.WriteLeafInline(obj);
        renderer.RedirectToChild();
    }
}
