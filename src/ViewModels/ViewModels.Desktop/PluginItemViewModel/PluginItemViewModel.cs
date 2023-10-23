// Copyright (c) Richasy Assistant. All rights reserved.

using System;
using System.IO;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.Input;
using RichasyAssistant.DI.Container;
using RichasyAssistant.Models.App;
using RichasyAssistant.Models.App.Plugins;
using RichasyAssistant.Models.Constants;
using RichasyAssistant.Toolkits.Interfaces;
using RichasyAssistant.ViewModels.Interfaces;
using Windows.Storage;
using Windows.System;

namespace RichasyAssistant.ViewModels;

/// <summary>
/// Plugin item view model.
/// </summary>
public sealed partial class PluginItemViewModel : ViewModelBase, IPluginItemViewModel
{
    /// <summary>
    /// Initializes a new instance of the <see cref="PluginItemViewModel"/> class.
    /// </summary>
    public PluginItemViewModel()
        => Commands = new SynchronizedObservableCollection<IPluginCommandItemViewModel>();

    /// <inheritdoc/>
    public void InjectData(PluginConfig config)
    {
        Id = config.Id;
        Name = config.Name;
        Description = config.Description;
        Author = config.Author;
        AuthorSite = config.AuthorSite;
        Logo = config.LogoUrl;
        Repository = config.Repository;
        CommandCount = config.Commands?.Count ?? 0;
        Version = config.Version;
        if (CommandCount > 0)
        {
            foreach (var item in config.Commands)
            {
                var vm = Locator.Current.GetService<IPluginCommandItemViewModel>();
                vm.InjectData(item);
                Commands.Add(vm);
            }
        }
    }

    [RelayCommand]
    private async Task OpenRepositoryAsync()
    {
        if (!string.IsNullOrEmpty(Repository) && Uri.IsWellFormedUriString(Repository, UriKind.Absolute))
        {
            await Launcher.LaunchUriAsync(new Uri(Repository));
        }
    }

    [RelayCommand]
    private async Task OpenAuthorSiteAsync()
    {
        if (!string.IsNullOrEmpty(AuthorSite) && Uri.IsWellFormedUriString(AuthorSite, UriKind.Absolute))
        {
            await Launcher.LaunchUriAsync(new Uri(AuthorSite));
        }
    }

    [RelayCommand]
    private async Task OpenPluginFolderAsync()
    {
        var settingsToolkit = Locator.Current.GetService<ISettingsToolkit>();
        var pluginFolder = settingsToolkit.ReadLocalSetting(SettingNames.PluginFolderPath, string.Empty);
        if (string.IsNullOrEmpty(pluginFolder))
        {
            pluginFolder = Path.Combine(ApplicationData.Current.LocalFolder.Path, WorkflowConstants.DefaultPluginFolderName);
        }

        var folder = await StorageFolder.GetFolderFromPathAsync(Path.Combine(pluginFolder, Id));
        await Launcher.LaunchFolderAsync(folder);
    }
}
