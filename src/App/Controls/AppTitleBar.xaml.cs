// Copyright (c) Fantasy Copilot. All rights reserved.

using FantasyCopilot.DI.Container;
using FantasyCopilot.ViewModels.Interfaces;

namespace FantasyCopilot.App.Controls
{
    /// <summary>
    /// App title bar.
    /// </summary>
    public sealed partial class AppTitleBar : UserControl
    {
        private readonly IAppViewModel _appViewModel;

        /// <summary>
        /// Initializes a new instance of the <see cref="AppTitleBar"/> class.
        /// </summary>
        public AppTitleBar()
        {
            InitializeComponent();
            _appViewModel = Locator.Current.GetService<IAppViewModel>();
            Loaded += OnLoaded;
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
            => System.Diagnostics.Debug.WriteLine($"App title bar loaded: {DateTime.Now.Ticks}");

        private void OnBackButtonClick(object sender, RoutedEventArgs e)
            => _appViewModel.BackCommand.Execute(default);
    }
}
