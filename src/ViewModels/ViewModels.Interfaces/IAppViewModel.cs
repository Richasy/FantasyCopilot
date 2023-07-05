// Copyright (c) Fantasy Copilot. All rights reserved.

using System.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FantasyCopilot.Models.App;
using FantasyCopilot.Models.Constants;

namespace FantasyCopilot.ViewModels.Interfaces;

/// <summary>
/// Application view model interface.
/// </summary>
public interface IAppViewModel : INotifyPropertyChanged
{
    /// <summary>
    /// Fired when the request back.
    /// </summary>
    event EventHandler BackRequest;

    /// <summary>
    /// Fired when there is a new navigation request.
    /// </summary>
    event EventHandler<NavigateEventArgs> NavigateRequest;

    /// <summary>
    /// Fired when request show tip.
    /// </summary>
    event EventHandler<AppTipNotificationEventArgs> RequestShowTip;

    /// <summary>
    /// Fired when request show message.
    /// </summary>
    event EventHandler<string> RequestShowMessage;

    /// <summary>
    /// Main window object.
    /// </summary>
    object MainWindow { get; set; }

    /// <summary>
    /// Whether to display the back button in the title bar.
    /// </summary>
    bool IsBackButtonShown { get; set; }

    /// <summary>
    /// Whether to display the navigation panel at the bottom of the application.
    /// </summary>
    bool IsNavigationMenuShown { get; set; }

    /// <summary>
    /// Is the chat service available.
    /// </summary>
    bool IsChatAvailable { get; }

    /// <summary>
    /// Whether the voice service is available.
    /// </summary>
    bool IsVoiceAvailable { get; }

    /// <summary>
    /// Whether the image service is available.
    /// </summary>
    bool IsImageAvailable { get; }

    /// <summary>
    /// Whether the translate service is available.
    /// </summary>
    bool IsTranslateAvailable { get; }

    /// <summary>
    /// Whether the local file search service is available.
    /// </summary>
    bool IsStorageAvailable { get; }

    /// <summary>
    /// Whether the knowledge database is available.
    /// </summary>
    bool IsKnowledgeAvailable { get; }

    /// <summary>
    /// Currently selected navigation item.
    /// </summary>
    NavigateItem CurrentNavigateItem { get; set; }

    /// <summary>
    /// Current page ID.
    /// </summary>
    PageType CurrentPage { get; }

    /// <summary>
    /// List of navigation items.
    /// </summary>
    SynchronizedObservableCollection<NavigateItem> NavigateItems { get; }

    /// <summary>
    /// Collection of all connectors.
    /// </summary>
    SynchronizedObservableCollection<IConnectorConfigViewModel> Connectors { get; }

    /// <summary>
    /// Custom connector group.
    /// </summary>
    Dictionary<ConnectorType, IConnectorConfigViewModel> ConnectorGroup { get; }

    /// <summary>
    /// Whether to display the connector viewer button.
    /// </summary>
    bool IsConnectorViewerShown { get; }

    /// <summary>
    /// Navigate back command.
    /// </summary>
    IRelayCommand BackCommand { get; }

    /// <summary>
    /// Check the status of all services.
    /// </summary>
    IRelayCommand ReloadAllServicesCommand { get; }

    /// <summary>
    /// Verify the availability of the storage search service.
    /// </summary>
    IRelayCommand CheckStorageServiceCommand { get; }

    /// <summary>
    /// Verify the availability of the image service.
    /// </summary>
    IRelayCommand CheckImageServiceCommand { get; }

    /// <summary>
    /// Restart the application as admin.
    /// </summary>
    IRelayCommand RestartAsAdminCommand { get; }

    /// <summary>
    /// Refresh connectors.
    /// </summary>
    IAsyncRelayCommand<bool> RefreshConnectorsCommand { get; }

    /// <summary>
    /// Navigate to a page.
    /// </summary>
    /// <param name="page">Page id.</param>
    /// <param name="parameter">Navigate parameter.</param>
    void Navigate(PageType page, object parameter = null);

    /// <summary>
    /// Initialize the view model.
    /// </summary>
    /// <returns><see cref="Task"/>.</returns>
    Task InitializeAsync();

    /// <summary>
    /// Show tip.
    /// </summary>
    /// <param name="message">Message content.</param>
    /// <param name="type">Message type.</param>
    void ShowTip(string message, InfoType type = InfoType.Information);

    /// <summary>
    /// Display a dialog box to remind the user of something.
    /// </summary>
    /// <param name="message">Message.</param>
    void ShowMessage(string message);
}
