// Copyright (c) Fantasy Copilot. All rights reserved.

using System.CommandLine;
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
               serializer.Serialize(Console.Out, yamlObject);
           }
           else
           {
               var lines = Regex.Split(text, "\r\n|\r|\n");
               var dict = new Dictionary<string, string>();
               foreach (var line in lines)
               {
               }
           }

           Console.WriteLine(result);
       },
       textOption,
       typeOption);
