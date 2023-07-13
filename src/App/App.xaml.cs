// Copyright (c) Fantasy Copilot. All rights reserved.

using System.Linq;
using FantasyCopilot.App.Controls;
using FantasyCopilot.DI.Container;
using FantasyCopilot.Toolkits.Interfaces;
using FantasyCopilot.ViewModels.Interfaces;
using H.NotifyIcon;
using Microsoft.Extensions.Logging;
using Microsoft.UI.Dispatching;
using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml.Input;
using Microsoft.Windows.AppLifecycle;
using Windows.Graphics;
using Windows.Win32;
using Windows.Win32.Foundation;
using WinRT.Interop;

namespace FantasyCopilot.App;

/// <summary>
/// Provides application-specific behavior to supplement the default Application class.
/// </summary>
public partial class App : Application
{
    /// <summary>
    /// App id.
    /// </summary>
    public const string Guid = "376AEAAB-331B-42AC-A069-146F7230765E";

    private ISettingsToolkit _settingsToolkit;
    private Window _window;
    private DispatcherQueue _dispatcherQueue;

    /// <summary>
    /// Initializes the singleton application object.  This is the first line of authored code
    /// executed, and as such is the logical equivalent of main() or WinMain().
    /// </summary>
    public App()
    {
        InitializeComponent();
        UnhandledException += OnUnhandledException;
    }

    /// <summary>
    /// DI factory.
    /// </summary>
    public static DI.App.Factory Factory { get; private set; }

    private TaskbarIcon TrayIcon { get; set; }

    private bool HandleCloseEvents { get; set; }

    /// <summary>
    /// Try activating the window and bringing it to the foreground.
    /// </summary>
    public void ActivateWindow()
    {
        _dispatcherQueue.TryEnqueue(() =>
        {
            if (_window == null)
            {
                LaunchWindow();
            }
            else if (_window.Visible)
            {
                _window.Hide();
            }
            else
            {
                _window.Activate();
                PInvoke.SetForegroundWindow(new HWND(WindowNative.GetWindowHandle(_window)));
            }
        });
    }

    /// <summary>
    /// Invoked when the application is launched.
    /// </summary>
    /// <param name="args">Details about the launch request and process.</param>
    protected override async void OnLaunched(LaunchActivatedEventArgs args)
    {
        // We expect our app is single instanced.
        var instance = AppInstance.FindOrRegisterForKey(Guid);
        var activatedArgs = AppInstance.GetCurrent().GetActivatedEventArgs();

        // If the current instance is not the previously registered instance
        if (!instance.IsCurrent)
        {
            // Redirect to the existing instance
            await instance.RedirectActivationToAsync(activatedArgs);

            // Kill the current instance
            Current.Exit();
            return;
        }

        instance.Activated += OnInstanceActivated;
        _dispatcherQueue = DispatcherQueue.GetForCurrentThread();
        DI.App.Factory.RegisterAppRequiredServices();
        LaunchWindow();
    }

    private static RectInt32 GetRenderRect(DisplayArea displayArea, IntPtr windowHandle)
    {
        var workArea = displayArea.WorkArea;
        var scaleFactor = PInvoke.GetDpiForWindow(new HWND(windowHandle)) / 96d;
        var width = Convert.ToInt32(500 * scaleFactor);
        var height = Convert.ToInt32(800 * scaleFactor);

        // Ensure the window is not larger than the work area.
        if (height > workArea.Height - 20)
        {
            height = workArea.Height - 20;
        }

        var lastPoint = GetSavedWindowPosition();
        var isZeroPoint = lastPoint.X == 0 && lastPoint.Y == 0;
        var isValidPosition = lastPoint.X >= workArea.X && lastPoint.Y >= workArea.Y;
        var left = isZeroPoint || !isValidPosition
            ? (workArea.Width - width) / 2d
            : lastPoint.X - workArea.X;
        var top = isZeroPoint || !isValidPosition
            ? (workArea.Height - height) / 2d
            : lastPoint.Y - workArea.Y;
        return new RectInt32(Convert.ToInt32(left), Convert.ToInt32(top), width, height);
    }

    private static PointInt32 GetSavedWindowPosition()
    {
        var settingToolkit = Locator.Current.GetService<ISettingsToolkit>();
        var left = settingToolkit.ReadLocalSetting(SettingNames.WindowPositionLeft, 0);
        var top = settingToolkit.ReadLocalSetting(SettingNames.WindowPositionTop, 0);
        return new PointInt32(left, top);
    }

    private static void CleanupConnectors()
    {
        var appVM = Locator.Current.GetService<IAppViewModel>();
        if (appVM.ConnectorGroup.Any())
        {
            foreach (var item in appVM.ConnectorGroup)
            {
                item.Value.ExitCommand.Execute(default);
            }
        }
    }

    private void InitializeTrayIcon()
    {
        if (TrayIcon != null)
        {
            return;
        }

        var showHideWindowCommand = (XamlUICommand)Resources["ShowHideWindowCommand"];
        showHideWindowCommand.ExecuteRequested += OnShowHideWindowCommandExecuteRequested;

        var exitApplicationCommand = (XamlUICommand)Resources["QuitCommand"];
        exitApplicationCommand.ExecuteRequested += OnQuitCommandExecuteRequested;

        TrayIcon = (TaskbarIcon)Resources["TrayIcon"];
        TrayIcon.ForceCreate();
    }

    private void LaunchWindow()
    {
        _window = new MainWindow();
        var appWindow = _window.AppWindow;
        appWindow.TitleBar.ExtendsContentIntoTitleBar = true;
        appWindow.TitleBar.ButtonBackgroundColor = Colors.Transparent;
        appWindow.TitleBar.ButtonInactiveBackgroundColor = Colors.Transparent;
        appWindow.Title = Locator.Current.GetService<IResourceToolkit>().GetLocalizedString(StringNames.AppName);
        appWindow.SetIcon("Assets/logo.ico");
        var presenter = appWindow.Presenter as OverlappedPresenter;
        presenter.IsResizable = false;
        presenter.IsMaximizable = false;
        MoveAndResize();
        _window.Closed += OnMainWindowClosedAsync;

        _settingsToolkit = Locator.Current.GetService<ISettingsToolkit>();
        HandleCloseEvents = _settingsToolkit.ReadLocalSetting(SettingNames.HideWhenCloseWindow, true);
        if (HandleCloseEvents)
        {
            InitializeTrayIcon();
        }

        _window.Activate();
    }

    private void OnInstanceActivated(object sender, AppActivationArguments e)
    {
        if (e.Kind == ExtendedActivationKind.Protocol)
        {
            return;
        }
    }

    private async void OnMainWindowClosedAsync(object sender, WindowEventArgs args)
    {
        SaveCurrentWindowPosition();
        HandleCloseEvents = _settingsToolkit.ReadLocalSetting(SettingNames.HideWhenCloseWindow, true);
        if (HandleCloseEvents)
        {
            args.Handled = true;

            var shouldAsk = _settingsToolkit.ReadLocalSetting(SettingNames.ShouldAskBeforeWindowClosed, true);
            if (shouldAsk)
            {
                _window.Activate();
                var dialog = new CloseWindowTipDialog
                {
                    XamlRoot = _window.Content.XamlRoot,
                };
                var result = await dialog.ShowAsync();
                if (result == ContentDialogResult.None)
                {
                    return;
                }

                var shouldHide = result == ContentDialogResult.Secondary;
                if (dialog.IsNeverAskChecked)
                {
                    _settingsToolkit.WriteLocalSetting(SettingNames.ShouldAskBeforeWindowClosed, false);
                    _settingsToolkit.WriteLocalSetting(SettingNames.HideWhenCloseWindow, shouldHide);
                }

                if (!shouldHide)
                {
                    ExitApp();
                    return;
                }
            }

            InitializeTrayIcon();
            _window.Hide();
        }
        else
        {
            CleanupConnectors();
        }
    }

    private void MoveAndResize()
    {
        var hwnd = WindowNative.GetWindowHandle(_window);
        var windowId = Win32Interop.GetWindowIdFromWindow(hwnd);
        var lastPoint = GetSavedWindowPosition();
        var displayArea = lastPoint.X == 0 && lastPoint.Y == 0
            ? DisplayArea.GetFromWindowId(windowId, DisplayAreaFallback.Nearest)
            : DisplayArea.GetFromPoint(lastPoint, DisplayAreaFallback.Nearest);
        if (displayArea != null)
        {
            var rect = GetRenderRect(displayArea, hwnd);
            _window.AppWindow.MoveAndResize(rect);
        }
    }

    private void SaveCurrentWindowPosition()
    {
        var left = _window.AppWindow.Position.X;
        var top = _window.AppWindow.Position.Y;
        var settingToolkit = Locator.Current.GetService<ISettingsToolkit>();
        settingToolkit.WriteLocalSetting(SettingNames.WindowPositionLeft, left);
        settingToolkit.WriteLocalSetting(SettingNames.WindowPositionTop, top);
    }

    private void ExitApp()
    {
        CleanupConnectors();
        HandleCloseEvents = false;
        TrayIcon?.Dispose();
        _window?.Close();
        Exit();
    }

    private void OnUnhandledException(object sender, Microsoft.UI.Xaml.UnhandledExceptionEventArgs e)
    {
        var logger = Locator.Current.GetLogger<App>();
        logger.LogError(e.Exception, "An exception occurred while the application was running");
        e.Handled = true;
    }

    private void OnQuitCommandExecuteRequested(XamlUICommand sender, ExecuteRequestedEventArgs args)
        => ExitApp();

    private void OnShowHideWindowCommandExecuteRequested(XamlUICommand sender, ExecuteRequestedEventArgs args)
        => ActivateWindow();
}
