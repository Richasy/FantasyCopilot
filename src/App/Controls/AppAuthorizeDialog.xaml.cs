// Copyright (c) Fantasy Copilot. All rights reserved.

using System.Collections.Generic;
using System.Text;
using System.Text.Json;
using FantasyCopilot.DI.Container;
using FantasyCopilot.Models.App.Authorize;
using FantasyCopilot.Toolkits.Interfaces;

namespace FantasyCopilot.App.Controls;

/// <summary>
/// App authorize dialog.
/// </summary>
public sealed partial class AppAuthorizeDialog : ContentDialog
{
    private readonly string _packageId;
    private readonly string _packageName;
    private readonly string[] _scopes;

    /// <summary>
    /// Initializes a new instance of the <see cref="AppAuthorizeDialog"/> class.
    /// </summary>
    public AppAuthorizeDialog(string packageId, string packageName, string[] scopes)
    {
        InitializeComponent();
        _packageId = packageId;
        _packageName = packageName;
        _scopes = scopes;
        Loaded += OnLoaded;
    }

    private void OnLoaded(object sender, RoutedEventArgs e)
    {
        LoadingRing.IsActive = false;

        DetailContainer.Visibility = Visibility.Visible;
        var displayName = _packageName;
        AppNameBlock.Text = displayName;
        PackageNameBlock.Text = _packageId;

        var resourceToolkit = Locator.Current.GetService<IResourceToolkit>();
        var permissionContent = resourceToolkit.GetLocalizedString(StringNames.AppRegistrationTip);
        var scopes = new StringBuilder();
        foreach (var item in _scopes)
        {
            if (item.Equals("oai", StringComparison.InvariantCultureIgnoreCase))
            {
                scopes.AppendLine($"- **{resourceToolkit.GetLocalizedString(StringNames.AISetting)}**");
            }
            else if (item.Equals("voice", StringComparison.InvariantCultureIgnoreCase))
            {
                scopes.AppendLine($"- **{resourceToolkit.GetLocalizedString(StringNames.VoiceService)}**");
            }
            else if (item.Equals("translate", StringComparison.InvariantCultureIgnoreCase))
            {
                scopes.AppendLine($"- **{resourceToolkit.GetLocalizedString(StringNames.TranslateOptions)}**");
            }
        }

        PermissionBlock.Text = string.Format(permissionContent, displayName, scopes.ToString());
        Focus(FocusState.Programmatic);
    }

    private async void OnPrimaryButtonClickAsync(ContentDialog sender, ContentDialogButtonClickEventArgs args)
    {
        args.Cancel = true;
        IsPrimaryButtonEnabled = false;
        var newApp = new AuthorizedApp
        {
            PackageId = _packageId,
            PackageName = _packageName,
            RequestTime = DateTimeOffset.Now,
            Scopes = _scopes,
        };

        var fileToolkit = Locator.Current.GetService<IFileToolkit>();
        var localList = await fileToolkit.GetDataFromFileAsync(AppConstants.AuthorizedAppsFileName, new List<AuthorizedApp>());
        localList.RemoveAll(p => p.PackageId == _packageId);
        localList.Add(newApp);

        await fileToolkit.WriteContentAsync(JsonSerializer.Serialize(localList), AppConstants.AuthorizedAppsFileName);
        Hide();
    }
}
