// Copyright (c) Richasy Assistant. All rights reserved.

using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Net.Http;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Threading.Tasks;
using CsvHelper;
using RichasyAssistant.Models.App;
using RichasyAssistant.Models.App.Web;
using RichasyAssistant.Models.Constants;

namespace RichasyAssistant.Services;

/// <summary>
/// Prompt explorer service.
/// </summary>
public sealed partial class PromptExplorerService
{
    private static async Task<OnlinePromptList> GetAwesomePromptsZhAsync()
    {
        using var client = new HttpClient();
        var uri = new Uri(AppConstants.AwesomePromptsZhSource);
        var response = await client.GetStringAsync(uri);
        var data = JsonSerializer.Deserialize<List<AwesomePromptItem>>(response);
        var result = new List<OnlinePrompt>();
        foreach (var item in data)
        {
            var prompt = new OnlinePrompt
            {
                Title = item.Act,
                Description = item.Act,
                Prompt = item.Prompt,
            };
            result.Add(prompt);
        }

        return new OnlinePromptList
        {
            CacheTime = DateTime.Now,
            List = result,
        };
    }

    private static async Task<OnlinePromptList> GetAwesomePromptsAsync()
    {
        using var client = new HttpClient();
        var uri = new Uri(AppConstants.AwesomePromptsSource);
        var response = await client.GetStreamAsync(uri);
        using var reader = new StreamReader(response);
        using var csv = new CsvReader(reader, CultureInfo.InvariantCulture);
        var records = csv.GetRecordsAsync<AwesomePromptItem>();
        var result = new List<OnlinePrompt>();
        await foreach (var item in records)
        {
            var prompt = new OnlinePrompt
            {
                Title = item.Act,
                Description = item.Act,
                Prompt = item.Prompt,
            };
            result.Add(prompt);
        }

        return new OnlinePromptList
        {
            CacheTime = DateTime.Now,
            List = result,
        };
    }

    private static async Task<OnlinePromptList> GetFlowGptTrendingAsync(OnlinePromptSource source)
    {
        var url = source == OnlinePromptSource.FlowGptEnTrending
                ? AppConstants.FlowGptEnTrendingSource
                : AppConstants.FlowGptZhTrendingSource;
        using var client = new HttpClient();
        var response = await client.GetStringAsync(url);
        var array = JsonArray.Parse(response);
        var data = JsonSerializer.Deserialize<List<FlowGptPromptItem>>(array[0]["result"]["data"]["json"].ToString());
        var result = new List<OnlinePrompt>();
        foreach (var item in data)
        {
            var prompt = new OnlinePrompt
            {
                Title = item.Title,
                Description = item.Description,
                Prompt = item.InitPrompt,
            };
            result.Add(prompt);
        }

        return new OnlinePromptList
        {
            CacheTime = DateTime.Now,
            List = result,
        };
    }

    private async Task<OnlinePromptList> GetLocalPromptListAsync(OnlinePromptSource source)
    {
        var path = Path.Combine(AppConstants.OnlinePromptFolderName, source.ToString() + ".json");
        var data = await _fileToolkit.GetDataFromFileAsync<OnlinePromptList>(path, default);
        return data;
    }

    private async Task SaveCacheAsync(OnlinePromptSource source, OnlinePromptList list)
    {
        var sourceLink = source switch
        {
            OnlinePromptSource.AwesomePrompt => "https://github.com/f/awesome-chatgpt-prompts",
            OnlinePromptSource.AwesomePromptZh => "https://github.com/PlexPt/awesome-chatgpt-prompts-zh",
            OnlinePromptSource.FlowGptZhTrending => "https://flowgpt.com/",
            OnlinePromptSource.FlowGptEnTrending => "https://flowgpt.com/",
            _ => throw new NotSupportedException(),
        };
        var path = Path.Combine(AppConstants.OnlinePromptFolderName, source.ToString() + ".json");
        await _fileToolkit.WriteContentAsync(JsonSerializer.Serialize(list), path);
    }

    private async Task<bool> TryLoadSourceAsync(OnlinePromptSource source)
    {
        if (_cacheList.TryGetValue(source, out var value) && value != null)
        {
            return true;
        }

        var localCache = await GetLocalPromptListAsync(source);
        if (localCache != null)
        {
            _cacheList[source] = localCache;
            return true;
        }

        return await ReloadSourceAsync(source);
    }
}
