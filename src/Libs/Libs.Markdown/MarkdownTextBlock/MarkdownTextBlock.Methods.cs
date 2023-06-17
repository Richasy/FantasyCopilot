// Copyright (c) Fantasy Copilot. All rights reserved.

using System;
using System.Diagnostics;
using FantasyCopilot.Libs.Markdown.Renderers;
using Markdig;
using Microsoft.UI.Xaml;

namespace FantasyCopilot.Libs.Markdown;

/// <summary>
/// An efficient and extensible control that can parse and render markdown.
/// </summary>
public partial class MarkdownTextBlock
{
    /// <summary>
    /// Called to preform a render of the current Markdown.
    /// </summary>
    private void RenderMarkdown()
    {
        // Leave if we don't have our root yet.
        if (_rootElement == null)
        {
            return;
        }

        // Clear everything that exists.
        _listeningHyperlinks.Clear();

        // Make sure we have something to parse.
        if (string.IsNullOrWhiteSpace(Text))
        {
            _rootElement.Child = null;
        }
        else
        {
            try
            {
                var theme = _themeListener.CurrentTheme == ApplicationTheme.Dark ? ElementTheme.Dark : ElementTheme.Light;
                var context = new RendererContext();
                context.CurrentTheme = theme;
                context.CornerRadius = CornerRadius;
                context.UseSyntaxHighlighting = UseSyntaxHighlighting;
                context.Background = Background;
                context.BorderBrush = BorderBrush;
                context.BorderThickness = BorderThickness;
                context.CharacterSpacing = CharacterSpacing;
                context.FontFamily = FontFamily;
                context.FontSize = FontSize;
                context.FontStretch = FontStretch;
                context.FontStyle = FontStyle;
                context.FontWeight = FontWeight;
                context.Foreground = Foreground;
                context.IsTextSelectionEnabled = IsTextSelectionEnabled;
                context.Padding = Padding;
                context.CodeStyling = CodeStyling;
                context.CodeBackground = CodeBackground;
                context.CodeBorderBrush = CodeBorderBrush;
                context.CodeBorderThickness = CodeBorderThickness;
                context.InlineCodeBorderThickness = InlineCodeBorderThickness;
                context.InlineCodeBackground = InlineCodeBackground;
                context.InlineCodeBorderBrush = InlineCodeBorderBrush;
                context.InlineCodePadding = InlineCodePadding;
                context.InlineCodeFontFamily = InlineCodeFontFamily;
                context.InlineCodeForeground = InlineCodeForeground;
                context.CodeForeground = CodeForeground;
                context.CodeFontFamily = CodeFontFamily;
                context.CodePadding = CodePadding;
                context.CodeMargin = CodeMargin;
                context.EmojiFontFamily = EmojiFontFamily;
                context.Header1FontSize = Header1FontSize;
                context.Header1FontWeight = Header1FontWeight;
                context.Header1Margin = Header1Margin;
                context.Header1Foreground = Header1Foreground;
                context.Header2FontSize = Header2FontSize;
                context.Header2FontWeight = Header2FontWeight;
                context.Header2Margin = Header2Margin;
                context.Header2Foreground = Header2Foreground;
                context.Header3FontSize = Header3FontSize;
                context.Header3FontWeight = Header3FontWeight;
                context.Header3Margin = Header3Margin;
                context.Header3Foreground = Header3Foreground;
                context.Header4FontSize = Header4FontSize;
                context.Header4FontWeight = Header4FontWeight;
                context.Header4Margin = Header4Margin;
                context.Header4Foreground = Header4Foreground;
                context.Header5FontSize = Header5FontSize;
                context.Header5FontWeight = Header5FontWeight;
                context.Header5Margin = Header5Margin;
                context.Header5Foreground = Header5Foreground;
                context.Header6FontSize = Header6FontSize;
                context.Header6FontWeight = Header6FontWeight;
                context.Header6Margin = Header6Margin;
                context.Header6Foreground = Header6Foreground;
                context.HorizontalRuleBrush = HorizontalRuleBrush;
                context.HorizontalRuleMargin = HorizontalRuleMargin;
                context.HorizontalRuleThickness = HorizontalRuleThickness;
                context.ListMargin = ListMargin;
                context.ListGutterWidth = ListGutterWidth;
                context.ListBulletSpacing = ListBulletSpacing;
                context.ParagraphMargin = ParagraphMargin;
                context.ParagraphLineHeight = ParagraphLineHeight;
                context.QuoteBackground = QuoteBackground;
                context.QuoteBorderBrush = QuoteBorderBrush;
                context.QuoteBorderThickness = QuoteBorderThickness;
                context.QuoteForeground = QuoteForeground;
                context.QuoteMargin = QuoteMargin;
                context.QuotePadding = QuotePadding;
                context.TableBorderBrush = TableBorderBrush;
                context.TableBorderThickness = TableBorderThickness;
                context.TableCellPadding = TableCellPadding;
                context.TableMargin = TableMargin;
                context.TextWrapping = TextWrapping;
                context.LinkForeground = LinkForeground;
                context.ImageStretch = ImageStretch;
                context.ImageMaxHeight = ImageMaxHeight;
                context.ImageMaxWidth = ImageMaxWidth;
                context.WrapCodeBlock = WrapCodeBlock;
                context.FlowDirection = FlowDirection;

                var pipeline = new MarkdownPipelineBuilder()
                    .UseAdvancedExtensions()
                    .Build();
                var renderer = new WinUIRenderer();
                renderer.Context = context;
                pipeline.Setup(renderer);
                var doc = Markdig.Markdown.Parse(Text, pipeline);
                _rootElement.Child = renderer.Render(doc) as UIElement;
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Error while parsing and rendering: " + ex.Message);
                if (Debugger.IsAttached)
                {
                    Debugger.Break();
                }
            }
        }
    }
}
