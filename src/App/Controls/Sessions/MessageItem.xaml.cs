// Copyright (c) Fantasy Copilot. All rights reserved.

using FantasyCopilot.DI.Container;
using FantasyCopilot.Toolkits.Interfaces;
using FantasyCopilot.ViewModels.Interfaces;
using Windows.ApplicationModel.DataTransfer;

namespace FantasyCopilot.App.Controls.Sessions;

/// <summary>
/// Message item.
/// </summary>
public sealed partial class MessageItem : UserControl
{
    /// <summary>
    /// Dependency property of <see cref="Data"/>.
    /// </summary>
    public static readonly DependencyProperty DataProperty =
        DependencyProperty.Register(
            nameof(Data),
            typeof(Message),
            typeof(MessageItem),
            new PropertyMetadata(default, new PropertyChangedCallback(OnDataChanged)));

    /// <summary>
    /// Initializes a new instance of the <see cref="MessageItem"/> class.
    /// </summary>
    public MessageItem() => InitializeComponent();

    /// <summary>
    /// Message data.
    /// </summary>
    public Message Data
    {
        get => (Message)GetValue(DataProperty);
        set => SetValue(DataProperty, value);
    }

    private static void OnDataChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var instance = d as MessageItem;
        var newData = e.NewValue as Message;
        if (newData.IsUser)
        {
            VisualStateManager.GoToState(instance, nameof(instance.MyState), false);
        }
        else
        {
            VisualStateManager.GoToState(instance, nameof(instance.DefaultState), false);
        }

        instance.MessageBlock.Text = newData.Content;
        instance.DateBlock.Text = newData.Time.ToString("MM/dd HH:mm:ss");
        if (!string.IsNullOrEmpty(newData.AdditionalMessage))
        {
            instance.InfoIconContainer.Visibility = Visibility.Visible;
            ToolTipService.SetToolTip(instance.InfoIconContainer, newData.AdditionalMessage);
        }
        else
        {
            instance.InfoIconContainer.Visibility = Visibility.Collapsed;
            ToolTipService.SetToolTip(instance.InfoIconContainer, default);
        }
    }

    private void OnLoaded(object sender, RoutedEventArgs e)
    {
        if (Data != null)
        {
            if (Data.IsUser)
            {
                VisualStateManager.GoToState(this, nameof(MyState), false);
            }
            else
            {
                VisualStateManager.GoToState(this, nameof(DefaultState), false);
            }
        }
    }

    private void OnRemoveItemClick(object sender, RoutedEventArgs e)
        => Locator.Current.GetService<IChatSessionPageViewModel>().CurrentSession?.RemoveMessageCommand.Execute(Data);

    private void OnCopyButtonClick(object sender, RoutedEventArgs e)
    {
        var dp = new DataPackage();
        dp.SetText(Data.Content);
        Clipboard.SetContent(dp);
        Locator.Current.GetService<IAppViewModel>().ShowTip(
            Locator.Current.GetService<IResourceToolkit>().GetLocalizedString(StringNames.Copied),
            InfoType.Success);
    }
}
