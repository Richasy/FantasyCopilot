// Copyright (c) Fantasy Copilot. All rights reserved.

namespace Microsoft.Toolkit.Parsers.Core
{
    /// <summary>
    /// This class offers helpers for Parsing.
    /// </summary>
    public static class ParseHelpers
    {
        /// <summary>
        /// Determines if a Markdown string is blank or comprised entirely of whitespace characters.
        /// </summary>
        /// <returns>true if blank or white space.</returns>
        public static bool IsMarkdownBlankOrWhiteSpace(string str)
        {
            for (var i = 0; i < str.Length; i++)
            {
                if (!IsMarkdownWhiteSpace(str[i]))
                {
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Determines if a character is a Markdown whitespace character.
        /// </summary>
        /// <returns>true if is white space.</returns>
        public static bool IsMarkdownWhiteSpace(char c)
        {
            return c == ' ' || c == '\t' || c == '\r' || c == '\n';
        }
    }
}
