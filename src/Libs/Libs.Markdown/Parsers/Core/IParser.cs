﻿// Copyright (c) Fantasy Copilot. All rights reserved.

using System.Collections.Generic;

namespace Microsoft.Toolkit.Parsers
{
    /// <summary>
    /// Parser interface.
    /// </summary>
    /// <typeparam name="T">Type to parse into.</typeparam>
    public interface IParser<out T>
        where T : SchemaBase
    {
        /// <summary>
        /// Parse method which all classes must implement.
        /// </summary>
        /// <param name="data">Data to parse.</param>
        /// <returns>Strong typed parsed data.</returns>
        IEnumerable<T> Parse(string data);
    }
}
