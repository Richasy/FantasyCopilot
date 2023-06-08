// Copyright (c) Fantasy Copilot. All rights reserved.

using System.CommandLine;
using System.Text.Json;
using System.Text.RegularExpressions;
using YamlDotNet.Serialization;

var textOption = new Option<string?>("-Input");
var typeOption = new Option<string?>("-Type");
var rootCommand = new RootCommand("Get input")
{
    textOption,
    typeOption,
};

rootCommand.SetHandler(
       (text, type) =>
       {
           type ??= "kv";
           var result = string.Empty;
           if (type.ToLower() == "json")
           {
               result = text;
           }
           else if (type.ToLower() == "yaml")
           {
               using var reader = new StringReader(text);
               var deserializer = new Deserializer();
               var yamlObject = deserializer.Deserialize(reader);
               var serializer = new SerializerBuilder()
                                   .JsonCompatible()
                                   .Build();
               var writer = new StringWriter();
               serializer.Serialize(writer, yamlObject);
               result = writer.GetStringBuilder().ToString();
           }
           else
           {
               var lines = Regex.Split(text, "\r\n|\r|\n");
               var dict = new Dictionary<string, string>();
               foreach (var line in lines)
               {
                   var firstColon = line.IndexOf(':');
                   if (firstColon == -1 || firstColon == line.Length - 1 || firstColon == 0)
                   {
                       continue;
                   }

                   var key = line.Substring(0, firstColon).Trim();
                   var value = line.Substring(firstColon + 1).Trim();
                   dict.Add(key, value);
               }

               result = JsonSerializer.Serialize(dict);
           }

           Console.WriteLine(result);
       },
       textOption,
       typeOption);

return await rootCommand.InvokeAsync(args);
