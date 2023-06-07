// Copyright (c) Fantasy Copilot. All rights reserved.

using System.CommandLine;

var fileOption = new Option<FileInfo?>("-Input");
var rootCommand = new RootCommand("Reads a file")
{
    fileOption,
};

rootCommand.SetHandler(
    file =>
    {
        if (file == null || !file.Exists)
        {
            throw new Exception("File does not exist");
        }

        File.ReadAllLines(file.FullName).ToList().ForEach(Console.WriteLine);
    },
    fileOption);

return await rootCommand.InvokeAsync(args);
