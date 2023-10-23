// Copyright (c) Richasy Assistant. All rights reserved.

using Markdig.Renderers;
using Markdig.Syntax;

namespace RichasyAssistant.Libs.Markdown.Renderers;

/// <summary>
/// A base class for WinUI rendering <see cref="Block"/> and <see cref="Markdig.Syntax.Inlines.Inline"/> Markdown objects.
/// </summary>
/// <typeparam name="TObject">The type of the object.</typeparam>
/// <seealso cref="IMarkdownObjectRenderer" />
internal abstract class WinUIObjectRenderer<TObject> : MarkdownObjectRenderer<WinUIRenderer, TObject>
    where TObject : MarkdownObject
{
}
