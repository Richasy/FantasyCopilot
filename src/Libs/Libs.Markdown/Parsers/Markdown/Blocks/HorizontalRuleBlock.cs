﻿// Copyright (c) Fantasy Copilot. All rights reserved.

using Microsoft.Toolkit.Parsers.Core;

namespace Microsoft.Toolkit.Parsers.Markdown.Blocks
{
    /// <summary>
    /// Represents a horizontal line.
    /// </summary>
    public class HorizontalRuleBlock : MarkdownBlock
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="HorizontalRuleBlock"/> class.
        /// </summary>
        public HorizontalRuleBlock()
            : base(MarkdownBlockType.HorizontalRule)
        {
        }

        /// <summary>
        /// Converts the object into it's textual representation.
        /// </summary>
        /// <returns> The textual representation of this object. </returns>
        public override string ToString()
        {
            return "---";
        }

        /// <summary>
        /// Parses a horizontal rule.
        /// </summary>
        /// <param name="markdown"> The markdown text. </param>
        /// <param name="start"> The location of the start of the line. </param>
        /// <param name="end"> The location of the end of the line. </param>
        /// <returns> A parsed horizontal rule block, or <c>null</c> if this is not a horizontal rule. </returns>
        internal static HorizontalRuleBlock Parse(string markdown, int start, int end)
        {
            // A horizontal rule is a line with at least 3 stars, optionally separated by spaces
            // OR a line with at least 3 dashes, optionally separated by spaces
            // OR a line with at least 3 underscores, optionally separated by spaces.
            var hrChar = '\0';
            var hrCharCount = 0;
            var pos = start;
            while (pos < end)
            {
                var c = markdown[pos++];
                if (c == '*' || c == '-' || c == '_')
                {
                    // All of the non-whitespace characters on the line must match.
                    if (hrCharCount > 0 && c != hrChar)
                    {
                        return null;
                    }

                    hrChar = c;
                    hrCharCount++;
                }
                else if (c == '\n')
                {
                    break;
                }
                else if (!ParseHelpers.IsMarkdownWhiteSpace(c))
                {
                    return null;
                }
            }

            // Hopefully there were at least 3 stars/dashes/underscores.
            return hrCharCount >= 3 ? new HorizontalRuleBlock() : null;
        }
    }
}
