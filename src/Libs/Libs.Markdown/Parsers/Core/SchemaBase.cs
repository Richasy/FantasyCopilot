// Copyright (c) Fantasy Copilot. All rights reserved.

namespace Microsoft.Toolkit.Parsers
{
    /// <summary>
    /// Strong typed schema base class.
    /// </summary>
    public abstract class SchemaBase
    {
        /// <summary>
        /// Gets or sets identifier for strong typed record.
        /// </summary>
        public string InternalID { get; set; }
    }
}
