// Copyright (c) Fantasy Copilot. All rights reserved.

using ColorCode.Styling;
using Microsoft.UI.Xaml.Media;

namespace FantasyCopilot.Libs.Markdown.Renderers;

/// <summary>
/// Rendering configuration context.
/// </summary>
public sealed class RendererConfig
{
    /// <summary>
    /// Gets or sets the Default Code Styling for Code Blocks.
    /// </summary>
    public StyleDictionary CodeStyling { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether to use Syntax Highlighting on Code.
    /// </summary>
    public bool UseSyntaxHighlighting { get; set; }

    /// <summary>
    /// Gets or sets the Default Font Family for the Markdown Document.
    /// </summary>
    public string FontFamily { get; set; }

    /// <summary>
    /// Gets or sets a brush that provides the background of the control.
    /// </summary>
    public string Background { get; set; }

    /// <summary>
    /// Gets or sets a brush that describes the foreground color.
    /// </summary>
    public string Foreground { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether to Wrap the Code Block or use a Horizontal Scroll.
    /// </summary>
    public bool CodeBlockWrapping { get; set; }

    /// <summary>
    /// Gets or sets the background brush for inline code.
    /// </summary>
    public string InlineCodeBackground { get; set; }

    /// <summary>
    /// Gets or sets the foreground brush for inline code.
    /// </summary>
    public string InlineCodeForeground { get; set; }

    /// <summary>
    /// Gets or sets the border brush for inline code.
    /// </summary>
    public string InlineCodeBorderBrush { get; set; }

    /// <summary>
    /// Gets or sets the font used to display code.  If this is <c>null</c>, then
    /// <see cref="FontFamily"/> is used.
    /// </summary>
    public string InlineCodeFontFamily { get; set; }

    /// <summary>
    /// Gets or sets the top space between the code border and the text.
    /// </summary>
    public double InlineCodePaddingTop { get; set; }

    /// <summary>
    /// Gets or sets the right space between the code border and the text.
    /// </summary>
    public double InlineCodePaddingRight { get; set; }

    /// <summary>
    /// Gets or sets the bottom space between the code border and the text.
    /// </summary>
    public double InlineCodePaddingBottom { get; set; }

    /// <summary>
    /// Gets or sets the left space between the code border and the text.
    /// </summary>
    public double InlineCodePaddingLeft { get; set; }

    /// <summary>
    /// Gets or sets the top margin for inline code.
    /// </summary>
    public double InlineCodeMarginTop { get; set; }

    /// <summary>
    /// Gets or sets the right margin for inline code.
    /// </summary>
    public double InlineCodeMarginRight { get; set; }

    /// <summary>
    /// Gets or sets the bottom margin for inline code.
    /// </summary>
    public double InlineCodeMarginBottom { get; set; }

    /// <summary>
    /// Gets or sets the left margin for inline code.
    /// </summary>
    public double InlineCodeMarginLeft { get; set; }

    /// <summary>
    /// Gets or sets the thickness of the border for inline code.
    /// </summary>
    public double InlineCodeBorderThickness { get; set; }

    /// <summary>
    /// Gets or sets the stretch used for images.
    /// </summary>
    public Stretch ImageStretch { get; set; }

    /// <summary>
    /// Gets or sets the brush used to render links.  If this is
    /// <c>null</c>, then Foreground is used.
    /// </summary>
    public string LinkForeground { get; set; }

    /// <summary>
    /// Gets or sets the brush used to fill the background of a code block.
    /// </summary>
    public string CodeBlockBackground { get; set; }

    /// <summary>
    /// Gets or sets the brush used to render the border fill of a code block.
    /// </summary>
    public string CodeBlockBorderBrush { get; set; }

    /// <summary>
    /// Gets or sets the brush used to render the text inside a code block.  If this is
    /// <c>null</c>, then Foreground is used.
    /// </summary>
    public string CodeBlockForeground { get; set; }

    /// <summary>
    /// Gets or sets the font used to display code.  If this is <c>null</c>, then
    /// <see cref="FontFamily"/> is used.
    /// </summary>
    public string CodeBlockFontFamily { get; set; }

    /// <summary>
    /// Gets or sets top space between the code border and the text.
    /// </summary>
    public double CodeBlockPaddingTop { get; set; }

    /// <summary>
    /// Gets or sets right space between the code border and the text.
    /// </summary>
    public double CodeBlockPaddingRight { get; set; }

    /// <summary>
    /// Gets or sets bottom space between the code border and the text.
    /// </summary>
    public double CodeBlockPaddingBottom { get; set; }

    /// <summary>
    /// Gets or sets left space between the code border and the text.
    /// </summary>
    public double CodeBlockPaddingLeft { get; set; }

    /// <summary>
    /// Gets or sets the top space between the code border and the text.
    /// </summary>
    public double CodeBlockMarginTop { get; set; }

    /// <summary>
    /// Gets or sets the right space between the code border and the text.
    /// </summary>
    public double CodeBlockMarginRight { get; set; }

    /// <summary>
    /// Gets or sets the bottom space between the code border and the text.
    /// </summary>
    public double CodeBlockMarginBottom { get; set; }

    /// <summary>
    /// Gets or sets the left space between the code border and the text.
    /// </summary>
    public double CodeBlockMarginLeft { get; set; }

    /// <summary>
    /// Gets or sets the thickness of the border around code blocks.
    /// </summary>
    public double CodeBlockBorderThickness { get; set; }

    /// <summary>
    /// Gets or sets the font used to display emojis.  If this is <c>null</c>, then
    /// Segoe UI Emoji font is used.
    /// </summary>
    public string EmojiFontFamily { get; set; }

    /// <summary>
    /// Gets or sets the font weight to use for level 1 headers.
    /// </summary>
    public ushort Header1FontWeight { get; set; }

    /// <summary>
    /// Gets or sets the foreground brush for level 1 headers.
    /// </summary>
    public string Header1Foreground { get; set; }

    /// <summary>
    /// Gets or sets the font size for level 1 headers.
    /// </summary>
    public double Header1FontSize { get; set; }

    /// <summary>
    /// Gets or sets the font weight to use for level 2 headers.
    /// </summary>
    public ushort Header2FontWeight { get; set; }

    /// <summary>
    /// Gets or sets the foreground brush for level 2 headers.
    /// </summary>
    public string Header2Foreground { get; set; }

    /// <summary>
    /// Gets or sets the font size for level 2 headers.
    /// </summary>
    public double Header2FontSize { get; set; }

    /// <summary>
    /// Gets or sets the font weight to use for level 3 headers.
    /// </summary>
    public ushort Header3FontWeight { get; set; }

    /// <summary>
    /// Gets or sets the foreground brush for level 3 headers.
    /// </summary>
    public string Header3Foreground { get; set; }

    /// <summary>
    /// Gets or sets the font size for level 3 headers.
    /// </summary>
    public double Header3FontSize { get; set; }

    /// <summary>
    /// Gets or sets the font weight to use for level 4 headers.
    /// </summary>
    public ushort Header4FontWeight { get; set; }

    /// <summary>
    /// Gets or sets the foreground brush for level 4 headers.
    /// </summary>
    public string Header4Foreground { get; set; }

    /// <summary>
    /// Gets or sets the font size for level 4 headers.
    /// </summary>
    public double Header4FontSize { get; set; }

    /// <summary>
    /// Gets or sets the font weight to use for level 5 headers.
    /// </summary>
    public ushort Header5FontWeight { get; set; }

    /// <summary>
    /// Gets or sets the foreground brush for level 5 headers.
    /// </summary>
    public string Header5Foreground { get; set; }

    /// <summary>
    /// Gets or sets the font size for level 5 headers.
    /// </summary>
    public double Header5FontSize { get; set; }

    /// <summary>
    /// Gets or sets the font weight to use for level 6 headers.
    /// </summary>
    public ushort Header6FontWeight { get; set; }

    /// <summary>
    /// Gets or sets the foreground brush for level 6 headers.
    /// </summary>
    public string Header6Foreground { get; set; }

    /// <summary>
    /// Gets or sets the font size for level 6 headers.
    /// </summary>
    public double Header6FontSize { get; set; }

    /// <summary>
    /// Gets or sets the brush used to render a horizontal rule.  If this is <c>null</c>, then
    /// <see cref="Foreground"/> is used.
    /// </summary>
    public string HorizontalRuleBrush { get; set; }

    /// <summary>
    /// Gets or sets the top margin used for horizontal rules.
    /// </summary>
    public double HorizontalRuleMarginTop { get; set; }

    /// <summary>
    /// Gets or sets the right margin used for horizontal rules.
    /// </summary>
    public double HorizontalRuleMarginRight { get; set; }

    /// <summary>
    /// Gets or sets the bottom margin used for horizontal rules.
    /// </summary>
    public double HorizontalRuleMarginBottom { get; set; }

    /// <summary>
    /// Gets or sets the left margin used for horizontal rules.
    /// </summary>
    public double HorizontalRuleMarginLeft { get; set; }

    /// <summary>
    /// Gets or sets the vertical thickness of the horizontal rule.
    /// </summary>
    public double HorizontalRuleThickness { get; set; }

    /// <summary>
    /// Gets or sets the brush used to fill the background of a quote block.
    /// </summary>
    public string QuoteBackground { get; set; }

    /// <summary>
    /// Gets or sets the brush used to render a quote border.
    /// </summary>
    public string QuoteBorderBrush { get; set; }

    /// <summary>
    /// Gets or sets the brush used to render the text inside a quote block.  If this is
    /// <c>null</c>, then Foreground is used.
    /// </summary>
    public string QuoteForeground { get; set; }

    /// <summary>
    /// Gets or sets the brush used to render table borders.
    /// </summary>
    public string TableBorderBrush { get; set; }

    /// <summary>
    /// Gets or sets the top margin used by lists.
    /// </summary>
    public double ListMarginTop { get; set; }

    /// <summary>
    /// Gets or sets the right margin used by lists.
    /// </summary>
    public double ListMarginRight { get; set; }

    /// <summary>
    /// Gets or sets the bottom margin used by lists.
    /// </summary>
    public double ListMarginBottom { get; set; }

    /// <summary>
    /// Gets or sets the left margin used by lists.
    /// </summary>
    public double ListMarginLeft { get; set; }

    /// <summary>
    /// Gets or sets the width of the space used by list item bullets/numbers.
    /// </summary>
    public double ListGutterWidth { get; set; }

    /// <summary>
    /// Gets or sets the space between the list item bullets/numbers and the list item content.
    /// </summary>
    public double ListBulletSpacing { get; set; }

    /// <summary>
    /// Gets or sets the top margin used for paragraphs.
    /// </summary>
    public double ParagraphMarginTop { get; set; }

    /// <summary>
    /// Gets or sets the right margin used for paragraphs.
    /// </summary>
    public double ParagraphMarginRight { get; set; }

    /// <summary>
    /// Gets or sets the bottom margin used for paragraphs.
    /// </summary>
    public double ParagraphMarginBottom { get; set; }

    /// <summary>
    /// Gets or sets the left margin used for paragraphs.
    /// </summary>
    public double ParagraphMarginLeft { get; set; }

    /// <summary>
    /// Gets or sets the line height used for paragraphs.
    /// </summary>
    public int ParagraphLineHeight { get; set; }

    /// <summary>
    /// Gets or sets the top thickness of quote borders.
    /// </summary>
    public double QuoteBorderThicknessTop { get; set; }

    /// <summary>
    /// Gets or sets the right thickness of quote borders.
    /// </summary>
    public double QuoteBorderThicknessRight { get; set; }

    /// <summary>
    /// Gets or sets the bottom thickness of quote borders.
    /// </summary>
    public double QuoteBorderThicknessBottom { get; set; }

    /// <summary>
    /// Gets or sets the left thickness of quote borders.
    /// </summary>
    public double QuoteBorderThicknessLeft { get; set; }

    /// <summary>
    /// Gets or sets the top space outside of quote borders.
    /// </summary>
    public double QuoteMarginTop { get; set; }

    /// <summary>
    /// Gets or sets the right space outside of quote borders.
    /// </summary>
    public double QuoteMarginRight { get; set; }

    /// <summary>
    /// Gets or sets the bottom space outside of quote borders.
    /// </summary>
    public double QuoteMarginBottom { get; set; }

    /// <summary>
    /// Gets or sets the left space outside of quote borders.
    /// </summary>
    public double QuoteMarginLeft { get; set; }

    /// <summary>
    /// Gets or sets the top space between the quote border and the text.
    /// </summary>
    public double QuotePaddingTop { get; set; }

    /// <summary>
    /// Gets or sets the right space between the quote border and the text.
    /// </summary>
    public double QuotePaddingRight { get; set; }

    /// <summary>
    /// Gets or sets the bottom space between the quote border and the text.
    /// </summary>
    public double QuotePaddingBottom { get; set; }

    /// <summary>
    /// Gets or sets the left space between the quote border and the text.
    /// </summary>
    public double QuotePaddingLeft { get; set; }

    /// <summary>
    /// Gets or sets the thickness of any table borders.
    /// </summary>
    public double TableBorderThickness { get; set; }

    /// <summary>
    /// Gets or sets the top padding inside each cell.
    /// </summary>
    public double TableCellPaddingTop { get; set; }

    /// <summary>
    /// Gets or sets the right padding inside each cell.
    /// </summary>
    public double TableCellPaddingRight { get; set; }

    /// <summary>
    /// Gets or sets the bottom padding inside each cell.
    /// </summary>
    public double TableCellPaddingBottom { get; set; }

    /// <summary>
    /// Gets or sets the left padding inside each cell.
    /// </summary>
    public double TableCellPaddingLeft { get; set; }

    /// <summary>
    /// Gets or sets the top margin used by tables.
    /// </summary>
    public double TableMarginTop { get; set; }

    /// <summary>
    /// Gets or sets the right margin used by tables.
    /// </summary>
    public double TableMarginRight { get; set; }

    /// <summary>
    /// Gets or sets the bottom margin used by tables.
    /// </summary>
    public double TableMarginBottom { get; set; }

    /// <summary>
    /// Gets or sets the left margin used by tables.
    /// </summary>
    public double TableMarginLeft { get; set; }

    /// <summary>
    /// Gets or sets the MaxWidth for images.
    /// </summary>
    public double ImageMaxWidth { get; set; }

    /// <summary>
    /// Gets or sets the MaxHeight for images.
    /// </summary>
    public double ImageMaxHeight { get; set; }
}
