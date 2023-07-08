// Copyright (c) Fantasy Copilot. All rights reserved.

using System.ComponentModel;
using FantasyCopilot.App.Controls;
using FantasyCopilot.App.Pages;
using FantasyCopilot.DI.Container;
using FantasyCopilot.Models.App;
using FantasyCopilot.ViewModels.Interfaces;
using Windows.Graphics;
using WinRT.Interop;

namespace FantasyCopilot.App;

/// <summary>
/// An empty window that can be used on its own or navigated to within a Frame.
/// </summary>
public sealed partial class MainWindow : Window
{
    private readonly IAppViewModel _appViewModel;

    /// <summary>
    /// Initializes a new instance of the <see cref="MainWindow"/> class.
    /// </summary>
    public MainWindow()
    {
        InitializeComponent();
        _appViewModel = Locator.Current.GetService<IAppViewModel>();
        SystemBackdrop = new MicaBackdrop();
        _appViewModel.MainWindow = this;
        Locator.Current.RegisterVariable(typeof(Window), this);
        _appViewModel.PropertyChanged += OnAppViewModelPropertyChanged;
        _appViewModel.NavigateRequest += OnAppViewModelNavigateRequest;
        _appViewModel.RequestShowTip += OnAppViewModelRequestShowTip;
        _appViewModel.RequestShowMessage += OnAppViewModelRequestShowMessageAsync;
        Instance = this;
    }

    /// <summary>
    /// 当前窗口实例.
    /// </summary>
    public static MainWindow Instance { get; private set; }

    /// <summary>
    /// Display a tip message and close it after the specified delay.
    /// </summary>
    /// <param name="element">The element to insert.</param>
    /// <param name="delaySeconds">Delay time (seconds).</param>
    /// <returns><see cref="Task"/>.</returns>
    public async Task ShowTipAsync(UIElement element, double delaySeconds)
    {
        TipContainer.Visibility = Visibility.Visible;
        TipContainer.Children.Add(element);
        element.Visibility = Visibility.Visible;
        await Task.Delay(TimeSpan.FromSeconds(delaySeconds));
        element.Visibility = Visibility.Collapsed;
        TipContainer.Children.Remove(element);
        if (TipContainer.Children.Count == 0)
        {
            TipContainer.Visibility = Visibility.Collapsed;
        }
    }

    private void Initialize()
    {
        SetTitleBarDragRect();

        DispatcherQueue.TryEnqueue(async () =>
        {
            await _appViewModel.InitializeAsync();
        });
    }

    private void SetTitleBarDragRect()
    {
        if (AppWindow == null)
        {
            return;
        }

        var windowHandle = WindowNative.GetWindowHandle(this);
        var scaleFactor = Windows.Win32.PInvoke.GetDpiForWindow(new Windows.Win32.Foundation.HWND(windowHandle)) / 96d;
        var leftInset = AppWindow.TitleBar.LeftInset;
        var leftPadding = _appViewModel.IsBackButtonShown ? 48 * scaleFactor : 0;
        var dragRect = default(RectInt32);
        dragRect.X = Convert.ToInt32(leftInset + leftPadding);
        dragRect.Y = 0;
        dragRect.Height = Convert.ToInt32(AppTitleBar.ActualHeight * scaleFactor);
        dragRect.Width = Convert.ToInt32((AppTitleBar.ActualWidth * scaleFactor) - leftPadding);
        AppWindow.TitleBar.SetDragRectangles(new[] { dragRect });
    }

    private void OnAppTitleBarLoaded(object sender, RoutedEventArgs e)
        => Initialize();

    private void OnAppViewModelNavigateRequest(object sender, NavigateEventArgs e)
    {
        var pageType = e.Type switch
        {
            PageType.Welcome => typeof(WelcomePage),
            PageType.ChatSession => typeof(ChatSessionPage),
            PageType.PromptsAndSession => typeof(PromptsAndSessionsPage),
            PageType.Voice => typeof(VoicePage),
            PageType.Image => typeof(ImagePage),
            PageType.Translate => typeof(TranslatePage),
            PageType.StorageSearch => typeof(StoragePage),
            PageType.Settings => typeof(SettingsPage),
            PageType.Workspace => typeof(WorkspacePage),
            PageType.Knowledge => typeof(KnowledgePage),
            _ => throw new NotImplementedException(),
        };

        MainFrame.Navigate(pageType, e.Parameter);
    }

    private void OnAppViewModelPropertyChanged(object sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(IAppViewModel.IsBackButtonShown))
        {
            SetTitleBarDragRect();
        }
    }

    private void OnAppViewModelRequestShowTip(object sender, AppTipNotificationEventArgs e)
        => new TipPopup(e.Message).ShowAsync(e.Type);

    private async void OnAppViewModelRequestShowMessageAsync(object sender, string e)
    {
        var dialog = new TipDialog(e)
        {
            XamlRoot = Content.XamlRoot,
        };
        await dialog.ShowAsync();
    }

    private void OnSettingsButtonClick(object sender, RoutedEventArgs e)
        => _appViewModel.Navigate(PageType.Settings);

    private void OnConnectorViewerFlyoutOpened(object sender, object e)
        => ConnectorViewer.Initialize();
}
