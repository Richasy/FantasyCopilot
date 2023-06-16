// Copyright (c) Fantasy Copilot. All rights reserved.

using System.Collections.Generic;
using System.Linq;
using FantasyCopilot.Libs.Markdown.Markdown.Render;
using FantasyCopilot.Libs.Markdown.Renderers.Inlines;
using Markdig.Helpers;
using Markdig.Renderers;
using Markdig.Syntax;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Documents;
using Microsoft.UI.Xaml.Media;
using Windows.UI.Text;

namespace FantasyCopilot.Libs.Markdown.Renderers;

internal class WinUIRenderer : RendererBase
{
    private readonly IList<UIElement> _elements = new List<UIElement>();
    private char[] _buffer;

    /// <summary>
    /// Initializes a new instance of the <see cref="WinUIRenderer"/> class.
    /// </summary>
    public WinUIRenderer() => _buffer = new char[1024];

    public RendererContext Context { get; set; }

    public override object Render(MarkdownObject markdownObject)
    {
        LoadRenderers();
        var rootPanel = new StackPanel
        {
            Background = Context.Background,
            BorderBrush = Context.BorderBrush,
            BorderThickness = Context.BorderThickness,
            Padding = Context.Padding,
        };

        Add(rootPanel);
        Write(markdownObject);
        return _elements.First();
    }

    public void WriteLeafInline(LeafBlock leafBlock)
    {
        if (leafBlock == null)
        {
            throw new System.ArgumentNullException(nameof(leafBlock));
        }

        if (leafBlock.Inline != null)
        {
            WriteChildren(leafBlock.Inline);
        }
    }

    public void WriteLeafRawLines(LeafBlock leafBlock)
    {
        if (leafBlock == null)
        {
            throw new System.ArgumentNullException(nameof(leafBlock));
        }

        if (leafBlock.Lines.Lines != null)
        {
            var lines = leafBlock.Lines;
            var slices = lines.Lines;
            for (var i = 0; i < lines.Count; i++)
            {
                if (i != 0)
                {
                    WriteInline(new LineBreak());
                }

                WriteText(ref slices[i].Slice);
            }
        }
    }

    public void Add(Microsoft.UI.Xaml.Documents.Block o)
    {
        var rtb = GetLastRichTextBlock();
        rtb.Blocks.Add(o);
    }

    public void Add(UIElement ele)
        => _elements.Add(ele);

    public void RedirectToChild()
    {
        if (_elements.Count == 1)
        {
            return;
        }

        var lastEle = _elements.LastOrDefault();
        if (lastEle == null)
        {
            return;
        }

        _elements.RemoveAt(_elements.Count - 1);
        var prevEle = _elements.LastOrDefault();
        if (prevEle is Panel panel)
        {
            panel.Children.Add(lastEle);
        }
        else if (prevEle is Border border)
        {
            border.Child = lastEle;
        }
    }

    public void WriteInline(Inline inline)
        => AddInline(GetLastParagraph(), inline);

    public void WriteText(ref StringSlice slice)
    {
        if (slice.Start > slice.End)
        {
            return;
        }

        WriteText(slice.Text, slice.Start, slice.Length);
    }

    public void WriteText(string text)
        => WriteInline(new Run { Text = text });

    public void WriteText(string text, int offset, int length)
    {
        if (text == null)
        {
            return;
        }

        if (offset == 0 && text.Length == length)
        {
            WriteText(text);
        }
        else
        {
            if (length > _buffer.Length)
            {
                _buffer = text.ToCharArray();
                WriteText(new string(_buffer, offset, length));
            }
            else
            {
                text.CopyTo(offset, _buffer, 0, length);
                WriteText(new string(_buffer, 0, length));
            }
        }
    }

    public TextBlock CreateDefaultTextBlock()
    {
        var result = new TextBlock
        {
            CharacterSpacing = Context.CharacterSpacing,
            FontFamily = Context.FontFamily,
            FontSize = Context.FontSize,
            FontStretch = Context.FontStretch,
            FontStyle = Context.FontStyle,
            FontWeight = Context.FontWeight,
            Foreground = Context.Foreground,
            IsTextSelectionEnabled = Context.IsTextSelectionEnabled,
            TextWrapping = Context.TextWrapping,
            FlowDirection = Context.FlowDirection,
        };
        return result;
    }

    protected virtual void LoadRenderers()
    {
        ObjectRenderers.Add(new ParagraphRenderer());
        ObjectRenderers.Add(new QuoteBlockRenderer());
        ObjectRenderers.Add(new HeadingRenderer());
        ObjectRenderers.Add(new HorizontalRuleRenderer());
        ObjectRenderers.Add(new ListRenderer());

        ObjectRenderers.Add(new LiteralInlineRenderer());
    }

    private static void AddInline(Microsoft.UI.Xaml.Documents.Block parent, Inline inline)
    {
        var para = parent as Paragraph;
        para.Inlines.Add(inline);
    }

    private RichTextBlock GetLastRichTextBlock()
    {
        var lastEle = _elements.LastOrDefault();
        if (lastEle is not RichTextBlock)
        {
            if (_elements.Count == 1)
            {
                var internalLast = (_elements.First() as Panel).Children.LastOrDefault();
                if (internalLast is RichTextBlock)
                {
                    lastEle = internalLast;
                }
            }

            if (lastEle is not RichTextBlock)
            {
                var rtb = new RichTextBlock
                {
                    CharacterSpacing = Context.CharacterSpacing,
                    FontFamily = Context.FontFamily,
                    FontSize = Context.FontSize,
                    FontStretch = Context.FontStretch,
                    FontStyle = Context.FontStyle,
                    FontWeight = Context.FontWeight,
                    Foreground = Context.Foreground,
                    IsTextSelectionEnabled = Context.IsTextSelectionEnabled,
                    TextWrapping = Context.TextWrapping,
                    FlowDirection = Context.FlowDirection,
                };
                _elements.Add(rtb);
                lastEle = rtb;
            }
        }

        var r = lastEle as RichTextBlock;
        return r;
    }

    private Paragraph GetLastParagraph()
    {
        var rtb = GetLastRichTextBlock();
        if (rtb.Blocks.Count == 0)
        {
            rtb.Blocks.Add(new Paragraph());
        }

        return rtb.Blocks.Last() as Paragraph;
    }
}
