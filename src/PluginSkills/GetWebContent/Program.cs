// Copyright (c) Fantasy Copilot. All rights reserved.

using System.CommandLine;
using HtmlAgilityPack;
using SmartReader;

const string DefaultUserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/113.0.0.0 Safari/537.36 Edg/113.0.1774.50";
var linkOption = new Option<string?>("-Input");
var typeOption = new Option<string?>("-Type");
var onlyTextOption = new Option<bool?>("-OnlyText");
var rootCommand = new RootCommand("Get web content")
{
    linkOption,
    typeOption,
    onlyTextOption,
};

rootCommand.SetHandler(
    async (data, type, onlyText) =>
    {
        var link = FormatLink(data ?? string.Empty);
        var isLink = Uri.TryCreate(link, UriKind.Absolute, out var uriResult)
            && (uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps);
        if (!isLink)
        {
            Console.WriteLine(data);
            return;
        }

        var onlyTextValue = onlyText ?? false;
        var result = string.Empty;
        if (type == "readable")
        {
            var article = await Reader.ParseArticleAsync(link, DefaultUserAgent);
            result = onlyTextValue ? article.TextContent : article.Content;
        }
        else
        {
            using var httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Add("User-Agent", DefaultUserAgent);
            var content = await httpClient.GetStringAsync(link);
            var doc = new HtmlDocument();
            doc.LoadHtml(content);
            result = onlyTextValue ? doc.DocumentNode.InnerText : content;
        }

        Console.WriteLine(result);
    },
    linkOption,
    typeOption,
    onlyTextOption);

return await rootCommand.InvokeAsync(args);

string FormatLink(string link)
    => link.StartsWith("http://") || link.StartsWith("https://") ? link : $"http://{link}";
