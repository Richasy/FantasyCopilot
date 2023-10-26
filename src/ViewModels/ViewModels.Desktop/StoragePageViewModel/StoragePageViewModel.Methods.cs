// Copyright (c) Richasy Assistant. All rights reserved.

using System.Collections.Generic;
using System.Linq;
using RichasyAssistant.Models.App.Files;
using RichasyAssistant.Models.Constants;

namespace RichasyAssistant.ViewModels;

/// <summary>
/// Storage page view model.
/// </summary>
public sealed partial class StoragePageViewModel
{
    private void InitializeFileSearchTypes()
    {
        FileSearchEntries.Add(GetFileSearchEntry(FluentSymbol.Folder, StringNames.Directory, StringNames.DirectoryPlaceholderText, FileSearchType.Children));
        FileSearchEntries.Add(GetFileSearchEntry(FluentSymbol.MusicNote2, StringNames.Audio, StringNames.AudioPlaceholderText, FileSearchType.Audio));
        FileSearchEntries.Add(GetFileSearchEntry(FluentSymbol.Video, StringNames.Video, StringNames.VideoPlaceholderText, FileSearchType.Video));
        FileSearchEntries.Add(GetFileSearchEntry(FluentSymbol.Image, StringNames.Image, StringNames.ImagePlaceholderText, FileSearchType.Picture));
        FileSearchEntries.Add(GetFileSearchEntry(FluentSymbol.Document, StringNames.Document, StringNames.DocumentPlaceholderText, FileSearchType.Document));
        FileSearchEntries.Add(GetFileSearchEntry(FluentSymbol.FolderZip, StringNames.Zip, StringNames.ZipPlaceholderText, FileSearchType.Zip));
        FileSearchEntries.Add(GetFileSearchEntry(FluentSymbol.Apps, StringNames.ExecuteFile, StringNames.ExecuteFilePlaceholderText, FileSearchType.Exe));
        FileSearchEntries.Add(GetFileSearchEntry(FluentSymbol.CopySelect, StringNames.DuplicateFiles, StringNames.DuplicateFilesPlaceholderText, FileSearchType.Duplicates));
        var localFileSearchType = _settingsToolkit.ReadLocalSetting(SettingNames.LastFileSearchType, FileSearchType.Children);
        CurrentFileSearchType = FileSearchEntries.First(p => p.Type == localFileSearchType);
    }

    private void InitializeAudioSearchTypes()
    {
        AudioSearchEntries.Add(GetAudioSearchEntry(FluentSymbol.TextCaseTitle, StringNames.Title, StringNames.MusicTitlePlaceholderText, AudioSearchType.Title));
        AudioSearchEntries.Add(GetAudioSearchEntry(FluentSymbol.Album, StringNames.Album, StringNames.AlbumPlaceholderText, AudioSearchType.Album));
        AudioSearchEntries.Add(GetAudioSearchEntry(FluentSymbol.Person, StringNames.Artist, StringNames.ArtistPlaceholderText, AudioSearchType.Artist));
        AudioSearchEntries.Add(GetAudioSearchEntry(FluentSymbol.MusicNote2Play, StringNames.Genre, StringNames.GenrePlaceholderText, AudioSearchType.Genre));
        AudioSearchEntries.Add(GetAudioSearchEntry(FluentSymbol.NavigationUnread, StringNames.MusicTrack, StringNames.MusicTrackPlaceholderText, AudioSearchType.Track));
        AudioSearchEntries.Add(GetAudioSearchEntry(FluentSymbol.CommentNote, StringNames.Comment, StringNames.MusicCommentPlaceholderText, AudioSearchType.Comment));
        var localAudioSearchType = _settingsToolkit.ReadLocalSetting(SettingNames.LastAudioSearchType, AudioSearchType.Album);
        CurrentAudioSearchType = AudioSearchEntries.First(p => p.Type == localAudioSearchType);
    }

    private void ResetSearchState()
    {
        IsGenericSearch = SearchType == StorageSearchType.Generic;
        IsFileSearch = SearchType == StorageSearchType.File;
        IsAudioSearch = SearchType == StorageSearchType.Audio;
        IsImageSearch = SearchType == StorageSearchType.Image;
        IsInitialTipVisible = true;
    }

    private FileSearchEntry GetFileSearchEntry(FluentSymbol icon, StringNames name, StringNames placeholder, FileSearchType type)
    {
        var localizedName = _resourceToolkit.GetLocalizedString(name);
        var localizedPlaceholder = _resourceToolkit.GetLocalizedString(placeholder);
        return new FileSearchEntry { Icon = icon, Name = localizedName, PlaceholderText = localizedPlaceholder, Type = type };
    }

    private AudioSearchEntry GetAudioSearchEntry(FluentSymbol icon, StringNames name, StringNames placeholder, AudioSearchType type)
    {
        var localizedName = _resourceToolkit.GetLocalizedString(name);
        var localizedPlaceholder = _resourceToolkit.GetLocalizedString(placeholder);
        return new AudioSearchEntry { Icon = icon, Name = localizedName, PlaceholderText = localizedPlaceholder, Type = type };
    }

    private List<FileItem> GetSortedList(IEnumerable<FileItem> items)
    {
        var list = items.ToList();
        switch (SortType)
        {
            case FileSortType.NameAtoZ:
                list = list.OrderBy(x => x.Name).ToList();
                break;
            case FileSortType.NameZtoA:
                list = list.OrderByDescending(x => x.Name).ToList();
                break;
            case FileSortType.ModifiedTime:
                list = list.OrderByDescending(x => x.LastModifiedTime).ToList();
                break;
            case FileSortType.Type:
                list = list.OrderBy(x => x.Extension).ThenBy(x => x.Name).ToList();
                break;
            case FileSortType.SizeLargeToSmall:
                list = list.OrderByDescending(x => x.ByteLength).ToList();
                break;
            case FileSortType.SizeSmallToLarge:
                list = list.OrderBy(x => x.ByteLength).ToList();
                break;
            default:
                break;
        }

        return list;
    }

    partial void OnSearchTypeChanged(StorageSearchType value)
    {
        ResetSearchState();
        _settingsToolkit.WriteLocalSetting(SettingNames.LastStorageSearchType, value);
    }

    partial void OnCurrentFileSearchTypeChanged(FileSearchEntry oldValue, FileSearchEntry newValue)
    {
        if (oldValue?.Type == FileSearchType.Children || newValue.Type == FileSearchType.Children)
        {
            FileKeyword = string.Empty;
        }

        _settingsToolkit.WriteLocalSetting(SettingNames.LastFileSearchType, newValue.Type);
    }

    partial void OnCurrentAudioSearchTypeChanged(AudioSearchEntry oldValue, AudioSearchEntry newValue)
    {
        if (oldValue?.Type == AudioSearchType.Track || newValue.Type == AudioSearchType.Track)
        {
            AudioKeyword = string.Empty;
        }

        _settingsToolkit.WriteLocalSetting(SettingNames.LastAudioSearchType, newValue.Type);
    }

    partial void OnGenericKeywordChanged(string value)
    {
        if (string.IsNullOrEmpty(value) && IsGenericSearch && Items.Count > 0)
        {
            IsInitialTipVisible = true;
            TryClear(Items);
        }
    }

    partial void OnFileKeywordChanged(string value)
    {
        if (string.IsNullOrEmpty(value) && IsFileSearch && Items.Count > 0)
        {
            IsInitialTipVisible = true;
            TryClear(Items);
        }
    }

    partial void OnAudioKeywordChanged(string value)
    {
        if (string.IsNullOrEmpty(value) && IsAudioSearch && Items.Count > 0)
        {
            IsInitialTipVisible = true;
            TryClear(Items);
        }
    }

    partial void OnSortTypeChanged(FileSortType value)
    {
        if (Items.Count > 0)
        {
            var list = GetSortedList(Items);
            TryClear(Items);
            foreach (var item in list)
            {
                Items.Add(item);
            }
        }

        _settingsToolkit.WriteLocalSetting(SettingNames.LastStorageSortType, value);
    }
}
