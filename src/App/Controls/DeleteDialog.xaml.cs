// Copyright (c) Richasy Assistant. All rights reserved.

using RichasyAssistant.DI.Container;
using RichasyAssistant.Toolkits.Interfaces;

namespace RichasyAssistant.App.Controls
{
    /// <summary>
    /// Delete reminder dialog.
    /// </summary>
    public sealed partial class DeleteDialog : ContentDialog
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DeleteDialog"/> class.
        /// </summary>
        public DeleteDialog(StringNames title, StringNames text)
        {
            InitializeComponent();
            var toolkit = Locator.Current.GetService<IResourceToolkit>();
            Title = toolkit.GetLocalizedString(title);
            TipBlock.Text = toolkit.GetLocalizedString(text);
        }
    }
}
