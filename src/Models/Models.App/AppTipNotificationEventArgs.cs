// Copyright (c) Fantasy Copilot. All rights reserved.

using FantasyCopilot.Models.Constants;

namespace FantasyCopilot.Models.App
{
    /// <summary>
    /// App tip notification event args.
    /// </summary>
    public class AppTipNotificationEventArgs : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AppTipNotificationEventArgs"/> class.
        /// </summary>
        public AppTipNotificationEventArgs(string msg, InfoType type = InfoType.Information)
        {
            Message = msg;
            Type = type;
        }

        /// <summary>
        /// Message content.
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// Message type.
        /// </summary>
        public InfoType Type { get; set; }
    }
}
