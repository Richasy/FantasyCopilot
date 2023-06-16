// Copyright (c) Fantasy Copilot. All rights reserved.

using System.Collections.Generic;

namespace Microsoft.Toolkit.Parsers.Markdown.Blocks
{
    /// <summary>
    /// This specifies the Content of the List element.
    /// </summary>
    public class ListItemBlock
    {
        internal ListItemBlock()
        {
        }

        /// <summary>
        /// Gets or sets the contents of the list item.
        /// </summary>
        public IList<MarkdownBlock> Blocks { get; set; }
    }
}
