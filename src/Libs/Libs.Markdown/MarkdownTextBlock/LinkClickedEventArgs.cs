// Copyright (c) Fantasy Copilot. All rights reserved.

using System;

namespace FantasyCopilot.Libs.Markdown
{
    /// <summary>
    /// Arguments for the OnLinkClicked event which is fired then the user presses a link.
    /// </summary>
    public class LinkClickedEventArgs : EventArgs
    {
        internal LinkClickedEventArgs(string link)
            => Link = link;

        /// <summary>
        /// Gets the link that was tapped.
        /// </summary>
        public string Link { get; }
    }
}
