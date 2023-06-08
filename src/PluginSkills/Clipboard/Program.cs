// Copyright (c) Fantasy Copilot. All rights reserved.

using System.CommandLine;
using TextCopy;

var inputOption = new Option<string?>("-Input");
var typeOption = new Option<string?>("-Type");

var rootCommand = new RootCommand("Clipboard operations")
{
    inputOption,
    typeOption,
};

rootCommand.SetHandler(
    (input, type) =>
    {
        type ??= "get";
        var result = string.Empty;
        if (type == "get")
        {
            result = ClipboardService.GetText();
        }
        else if (type == "set" && !string.IsNullOrEmpty(input))
        {
            ClipboardService.SetText(input);
        }

        Console.WriteLine(result);
    },
    inputOption,
    typeOption);

return await rootCommand.InvokeAsync(args);
