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
using Windows.ApplicationModel.Activation;
using Windows.Graphics;
using Windows.Win32;
using Windows.Win32.Foundation;
using WinRT.Interop;
using WinUIEx;

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
    private WindowEx _window;
    private DispatcherQueue _dispatcherQueue;

    /// <summary>
    /// Initializes the singleton application object.  This is the first line of authored code
    /// executed, and as such is the logical equivalent of main() or WinMain().
    /// </summary>
    public App()
    {
        InitializeComponent();
        var mainAppInstance = AppInstance.FindOrRegisterForKey(Guid);
        mainAppInstance.Activated += OnAppInstanceActivated;
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
    public void ActivateWindow(AppActivationArguments arguments = default)
    {
        _dispatcherQueue.TryEnqueue(() =>
        {
            if (_window == null)
            {
                LaunchWindow();
            }
            else if (_window.Visible && HandleCloseEvents && arguments?.Data == null)
            {
                _window.Hide();
            }
            else
            {
                _window.Activate();
                PInvoke.SetForegroundWindow(new HWND(WindowNative.GetWindowHandle(_window)));
            }

            try
            {
                if (arguments?.Data is IActivatedEventArgs args)
                {
                    MainWindow.Instance.ActivateArgumentsAsync(args);
                }
            }
            catch (Exception)
            {
            }
        });
    }

    /// <summary>
    /// Invoked when the application is launched.
    /// </summary>
    /// <param name="args">Details about the launch request and process.</param>
    protected override void OnLaunched(Microsoft.UI.Xaml.LaunchActivatedEventArgs args)
    {
        // We expect our app is single instanced.
        var instance = AppInstance.FindOrRegisterForKey(Guid);
        var eventArgs = instance.GetActivatedEventArgs();
        _dispatcherQueue = DispatcherQueue.GetForCurrentThread();
        DI.App.Factory.RegisterAppRequiredServices();

        var data = eventArgs.Data is IActivatedEventArgs
            ? eventArgs.Data as IActivatedEventArgs
            : args.UWPLaunchActivatedEventArgs;

        LaunchWindow(data);
    }

    private static RectInt32 GetRenderRect(DisplayArea displayArea, IntPtr windowHandle)
    {
        var workArea = displayArea.WorkArea;
        var scaleFactor = PInvoke.GetDpiForWindow(new HWND(windowHandle)) / 96d;
        var settingToolkit = Locator.Current.GetService<ISettingsToolkit>();
        var previousHeight = settingToolkit.ReadLocalSetting(SettingNames.WindowHeight, 800d);
        var width = Convert.ToInt32(500 * scaleFactor);
        var height = Convert.ToInt32(previousHeight * scaleFactor);

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

    private void LaunchWindow(IActivatedEventArgs args = default)
    {
        _window = new MainWindow(args);
        var appWindow = _window.AppWindow;
        appWindow.TitleBar.ExtendsContentIntoTitleBar = true;
        appWindow.TitleBar.ButtonBackgroundColor = Colors.Transparent;
        appWindow.TitleBar.ButtonInactiveBackgroundColor = Colors.Transparent;
        appWindow.TitleBar.IconShowOptions = IconShowOptions.HideIconAndSystemMenu;
        appWindow.Title = Locator.Current.GetService<IResourceToolkit>().GetLocalizedString(StringNames.AppName);
        appWindow.SetIcon("Assets/logo.ico");
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

    private async void OnMainWindowClosedAsync(object sender, WindowEventArgs args)
    {
        SaveCurrentWindowStats();
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
            var scaleFactor = PInvoke.GetDpiForWindow(new HWND(hwnd)) / 96d;
            _window.MinWidth = 500;
            _window.MaxWidth = 500;
            _window.MinHeight = 500;

            var maxHeight = (displayArea.WorkArea.Height / scaleFactor) + 16;
            _window.MaxHeight = maxHeight < 400 ? 400 : maxHeight;
            _window.AppWindow.MoveAndResize(rect);
        }
    }

    private void SaveCurrentWindowStats()
    {
        if (_window.Height == _window.MaxHeight)
        {
            return;
        }

        var left = _window.AppWindow.Position.X;
        var top = _window.AppWindow.Position.Y;
        var settingToolkit = Locator.Current.GetService<ISettingsToolkit>();
        settingToolkit.WriteLocalSetting(SettingNames.WindowPositionLeft, left);
        settingToolkit.WriteLocalSetting(SettingNames.WindowPositionTop, top);
        settingToolkit.WriteLocalSetting(SettingNames.WindowHeight, _window.Height > 400 ? _window.Height : 800);
    }

    private void ExitApp()
    {
        CleanupConnectors();
        HandleCloseEvents = false;
        TrayIcon?.Dispose();
        _window?.Close();
        Environment.Exit(0);
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

    private void OnAppInstanceActivated(object sender, AppActivationArguments e)
        => ActivateWindow(e);
}
