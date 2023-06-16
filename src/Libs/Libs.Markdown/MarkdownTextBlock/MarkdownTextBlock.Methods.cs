﻿// Copyright (c) Fantasy Copilot. All rights reserved.

using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using ColorCode;
using FantasyCopilot.Libs.Markdown.Markdown.Render;
using Microsoft.Toolkit.Parsers.Markdown;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Documents;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Media.Imaging;
using Windows.System;

namespace FantasyCopilot.Libs.Markdown
{
    /// <summary>
    /// An efficient and extensible control that can parse and render markdown.
    /// </summary>
    public partial class MarkdownTextBlock
    {
        /// <summary>
        /// Sets the Markdown Renderer for Rendering the UI.
        /// </summary>
        /// <typeparam name="T">The Inherited Markdown Render.</typeparam>
        public void SetRenderer<T>()
            where T : MarkdownRenderer
            => _rendererType = typeof(T);

        /// <summary>
        /// Called when the render has a link we need to listen to.
        /// </summary>
        public void RegisterNewHyperLink(Hyperlink newHyperlink, string linkUrl)
        {
            // Setup a listener for clicks.
            newHyperlink.Click += Hyperlink_Click;

            // Associate the URL with the hyperlink.
            newHyperlink.SetValue(HyperlinkUrlProperty, linkUrl);

            // Add it to our list
            _listeningHyperlinks.Add(newHyperlink);
        }

        /// <summary>
        /// Called when the render has a link we need to listen to.
        /// </summary>
        public void RegisterNewHyperLink(Image newImageLink, string linkUrl, bool isHyperLink)
        {
            // Setup a listener for clicks.
            newImageLink.Tapped += NewImageLink_Tapped;

            // Associate the URL with the hyperlink.
            newImageLink.SetValue(HyperlinkUrlProperty, linkUrl);

            // Set if the Image is HyperLink or not
            newImageLink.SetValue(IsHyperlinkProperty, isHyperLink);

            // Add it to our list
            _listeningHyperlinks.Add(newImageLink);
        }

        /// <summary>
        /// Called when the renderer needs to display a image.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        public async Task<ImageSource> ResolveImageAsync(string url, string tooltip)
        {
            if (!Uri.TryCreate(url, UriKind.Absolute, out var uri))
            {
                if (!string.IsNullOrEmpty(UriPrefix))
                {
                    url = string.Format("{0}{1}", UriPrefix, url);
                }
            }

            var eventArgs = new ImageResolvingEventArgs(url, tooltip);
            ImageResolving?.Invoke(this, eventArgs);

            await eventArgs.WaitForDeferrals();

            try
            {
                return eventArgs.Handled
                    ? eventArgs.Image
                    : GetImageSource(new Uri(url));
            }
            catch (Exception)
            {
                return null;
            }

            static ImageSource GetImageSource(Uri imageUrl)
            {
                return Path.GetExtension(imageUrl.AbsolutePath)?.ToLowerInvariant() == ".svg"
                    ? new SvgImageSource(imageUrl)
                    : new BitmapImage(imageUrl);
            }
        }

        /// <summary>
        /// Called when a Code Block is being rendered.
        /// </summary>
        /// <returns>Parsing was handled Successfully.</returns>
        public bool ParseSyntax(InlineCollection inlineCollection, string text, string codeLanguage)
        {
            var eventArgs = new CodeBlockResolvingEventArgs(inlineCollection, text, codeLanguage);
            CodeBlockResolving?.Invoke(this, eventArgs);

            try
            {
                var result = eventArgs.Handled;
                if (UseSyntaxHighlighting && !result && codeLanguage != null)
                {
                    var language = Languages.FindById(codeLanguage);
                    if (language != null)
                    {
                        RichTextBlockFormatter formatter;
                        if (CodeStyling != null)
                        {
                            formatter = new RichTextBlockFormatter(CodeStyling);
                        }
                        else
                        {
                            var theme = _themeListener.CurrentTheme == ApplicationTheme.Dark ? ElementTheme.Dark : ElementTheme.Light;
                            if (RequestedTheme != ElementTheme.Default)
                            {
                                theme = RequestedTheme;
                            }

                            formatter = new RichTextBlockFormatter(theme);
                        }

                        formatter.FormatInlines(text, language, inlineCollection);
                        return true;
                    }
                }

                return result;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Called when a link needs to be handled.
        /// </summary>
        internal void LinkHandled(string url, bool isHyperlink)
        {
            // Links that are nested within superscript elements cause the Click event to fire multiple times.
            // e.g. this markdown "[^bot](http://www.reddit.com/r/youtubefactsbot/wiki/index)"
            // Therefore we detect and ignore multiple clicks.
            if (_multiClickDetectionTriggered)
            {
                return;
            }

            _multiClickDetectionTriggered = true;
            DispatcherQueue.TryEnqueue(() => _multiClickDetectionTriggered = false);

            // Get the hyperlink URL.
            if (url == null)
            {
                return;
            }

            // Fire off the event.
            var eventArgs = new LinkClickedEventArgs(url);
            if (isHyperlink)
            {
                LinkClicked?.Invoke(this, eventArgs);
            }
            else
            {
                ImageClicked?.Invoke(this, eventArgs);
            }
        }

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

            // Disconnect from OnClick handlers.
            UnhookListeners();

            // Clear everything that exists.
            _listeningHyperlinks.Clear();

            var markdownRenderedArgs = new MarkdownRenderedEventArgs(null);

            // Make sure we have something to parse.
            if (string.IsNullOrWhiteSpace(Text))
            {
                _rootElement.Child = null;
            }
            else
            {
                try
                {
                    // Try to parse the markdown.
#pragma warning disable CS0618 // Type or member is obsolete
                    var markdown = new MarkdownDocument();
                    foreach (var str in SchemeList.Split(',').ToList())
                    {
                        if (!string.IsNullOrEmpty(str))
                        {
                            MarkdownDocument.KnownSchemes.Add(str);
                        }
                    }
#pragma warning restore CS0618 // Type or member is obsolete

                    markdown.Parse(Text);

                    // Now try to display it
                    var renderer = Activator.CreateInstance(_rendererType, markdown, this, this, this) as MarkdownRenderer ?? throw new Exception("Markdown Renderer was not of the correct type.");

                    renderer.Background = Background;
                    renderer.BorderBrush = BorderBrush;
                    renderer.BorderThickness = BorderThickness;
                    renderer.CharacterSpacing = CharacterSpacing;
                    renderer.FontFamily = FontFamily;
                    renderer.FontSize = FontSize;
                    renderer.FontStretch = FontStretch;
                    renderer.FontStyle = FontStyle;
                    renderer.FontWeight = FontWeight;
                    renderer.Foreground = Foreground;
                    renderer.IsTextSelectionEnabled = IsTextSelectionEnabled;
                    renderer.Padding = Padding;
                    renderer.CodeBackground = CodeBackground;
                    renderer.CodeBorderBrush = CodeBorderBrush;
                    renderer.CodeBorderThickness = CodeBorderThickness;
                    renderer.InlineCodeBorderThickness = InlineCodeBorderThickness;
                    renderer.InlineCodeBackground = InlineCodeBackground;
                    renderer.InlineCodeBorderBrush = InlineCodeBorderBrush;
                    renderer.InlineCodePadding = InlineCodePadding;
                    renderer.InlineCodeFontFamily = InlineCodeFontFamily;
                    renderer.InlineCodeForeground = InlineCodeForeground;
                    renderer.CodeForeground = CodeForeground;
                    renderer.CodeFontFamily = CodeFontFamily;
                    renderer.CodePadding = CodePadding;
                    renderer.CodeMargin = CodeMargin;
                    renderer.EmojiFontFamily = EmojiFontFamily;
                    renderer.Header1FontSize = Header1FontSize;
                    renderer.Header1FontWeight = Header1FontWeight;
                    renderer.Header1Margin = Header1Margin;
                    renderer.Header1Foreground = Header1Foreground;
                    renderer.Header2FontSize = Header2FontSize;
                    renderer.Header2FontWeight = Header2FontWeight;
                    renderer.Header2Margin = Header2Margin;
                    renderer.Header2Foreground = Header2Foreground;
                    renderer.Header3FontSize = Header3FontSize;
                    renderer.Header3FontWeight = Header3FontWeight;
                    renderer.Header3Margin = Header3Margin;
                    renderer.Header3Foreground = Header3Foreground;
                    renderer.Header4FontSize = Header4FontSize;
                    renderer.Header4FontWeight = Header4FontWeight;
                    renderer.Header4Margin = Header4Margin;
                    renderer.Header4Foreground = Header4Foreground;
                    renderer.Header5FontSize = Header5FontSize;
                    renderer.Header5FontWeight = Header5FontWeight;
                    renderer.Header5Margin = Header5Margin;
                    renderer.Header5Foreground = Header5Foreground;
                    renderer.Header6FontSize = Header6FontSize;
                    renderer.Header6FontWeight = Header6FontWeight;
                    renderer.Header6Margin = Header6Margin;
                    renderer.Header6Foreground = Header6Foreground;
                    renderer.HorizontalRuleBrush = HorizontalRuleBrush;
                    renderer.HorizontalRuleMargin = HorizontalRuleMargin;
                    renderer.HorizontalRuleThickness = HorizontalRuleThickness;
                    renderer.ListMargin = ListMargin;
                    renderer.ListGutterWidth = ListGutterWidth;
                    renderer.ListBulletSpacing = ListBulletSpacing;
                    renderer.ParagraphMargin = ParagraphMargin;
                    renderer.ParagraphLineHeight = ParagraphLineHeight;
                    renderer.QuoteBackground = QuoteBackground;
                    renderer.QuoteBorderBrush = QuoteBorderBrush;
                    renderer.QuoteBorderThickness = QuoteBorderThickness;
                    renderer.QuoteForeground = QuoteForeground;
                    renderer.QuoteMargin = QuoteMargin;
                    renderer.QuotePadding = QuotePadding;
                    renderer.TableBorderBrush = TableBorderBrush;
                    renderer.TableBorderThickness = TableBorderThickness;
                    renderer.YamlBorderBrush = YamlBorderBrush;
                    renderer.YamlBorderThickness = YamlBorderThickness;
                    renderer.TableCellPadding = TableCellPadding;
                    renderer.TableMargin = TableMargin;
                    renderer.TextWrapping = TextWrapping;
                    renderer.LinkForeground = LinkForeground;
                    renderer.ImageStretch = ImageStretch;
                    renderer.ImageMaxHeight = ImageMaxHeight;
                    renderer.ImageMaxWidth = ImageMaxWidth;
                    renderer.WrapCodeBlock = WrapCodeBlock;
                    renderer.FlowDirection = FlowDirection;

                    _rootElement.Child = renderer.Render();
                }
                catch (Exception ex)
                {
                    Debug.WriteLine("Error while parsing and rendering: " + ex.Message);
                    if (Debugger.IsAttached)
                    {
                        Debugger.Break();
                    }

                    markdownRenderedArgs = new MarkdownRenderedEventArgs(ex);
                }
            }

            // Indicate that the parse is done.
            MarkdownRendered?.Invoke(this, markdownRenderedArgs);
        }

        private void HookListeners()
        {
            // Re-hook all hyper link events we currently have
            foreach (var link in _listeningHyperlinks)
            {
                if (link is Hyperlink hyperlink)
                {
                    hyperlink.Click -= Hyperlink_Click;
                    hyperlink.Click += Hyperlink_Click;
                }
                else if (link is Image image)
                {
                    image.Tapped -= NewImageLink_Tapped;
                    image.Tapped += NewImageLink_Tapped;
                }
            }
        }

        private void UnhookListeners()
        {
            // Unhook any hyper link events if we have any
            foreach (var link in _listeningHyperlinks)
            {
                if (link is Hyperlink hyperlink)
                {
                    hyperlink.Click -= Hyperlink_Click;
                }
                else if (link is Image image)
                {
                    image.Tapped -= NewImageLink_Tapped;
                }
            }
        }
    }
}
