﻿// Copyright (c) Fantasy Copilot. All rights reserved.

using System;

namespace FantasyCopilot.Libs.Markdown
{
    /// <summary>
    /// Arguments for the OnMarkdownRendered event which indicates when the markdown has been
    /// rendered.
    /// </summary>
    public class MarkdownRenderedEventArgs : EventArgs
    {
        internal MarkdownRenderedEventArgs(Exception ex) => Exception = ex;

        /// <summary>
        /// Gets the exception if there was one. If the exception is null there was no error.
        /// </summary>
        public Exception Exception { get; }
    }
}
