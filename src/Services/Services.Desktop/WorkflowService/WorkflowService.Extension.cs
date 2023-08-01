// Copyright (c) Fantasy Copilot. All rights reserved.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using FantasyCopilot.DI.Container;
using FantasyCopilot.Models.App;
using FantasyCopilot.Models.App.Plugins;
using FantasyCopilot.Models.Constants;
using FantasyCopilot.Toolkits.Interfaces;
using Microsoft.Extensions.Logging;
using Microsoft.SemanticKernel.AI.TextCompletion;
using Microsoft.SemanticKernel.Orchestration;
using Microsoft.SemanticKernel.SkillDefinition;
using Windows.Storage;

namespace FantasyCopilot.Services;

/// <summary>
/// Workflow service.
/// </summary>
public sealed partial class WorkflowService
{
    internal class CommandFunction : ISKFunction
    {
        private readonly PluginCommand _source;
        private readonly string _packageId;

        public CommandFunction(PluginCommand command, string packageId)
        {
            _source = command;
            _packageId = packageId;
            Name = command.Identity;
            Description = command.Description;
        }

        public string Name { get; }

        public string SkillName { get; } = WorkflowConstants.GlobalFunctions;

        public string Description { get; }

        public bool IsSemantic { get; } = false;

        public CompleteRequestSettings RequestSettings => default;

        public FunctionView Describe()
        {
            var parameterList = new List<ParameterView>();
            if (_source.Parameters != null && _source.Parameters.Any())
            {
                foreach (var item in _source.Parameters)
                {
                    if (!string.IsNullOrEmpty(item.Id) || !string.IsNullOrEmpty(item.Name))
                    {
                        var pv = new ParameterView(item.Id ?? item.Name, item.Description ?? string.Empty, string.Empty);
                        parameterList.Add(pv);
                    }
                }
            }

            return new FunctionView(Name, SkillName, Description, parameterList, IsSemantic);
        }

        public async Task<SKContext> InvokeAsync(SKContext context, CompleteRequestSettings settings, CancellationToken cancellationToken)
        {
            var workflowContext = Locator.Current.GetVariable<WorkflowContext>();
            var settingsToolkit = Locator.Current.GetService<ISettingsToolkit>();
            var isShowWindow = settingsToolkit.ReadLocalSetting(SettingNames.ShowConsoleWhenPluginRunning, false);
            var pluginFolder = settingsToolkit.ReadLocalSetting(SettingNames.PluginFolderPath, string.Empty);
            if (string.IsNullOrEmpty(pluginFolder))
            {
                pluginFolder = Path.Combine(ApplicationData.Current.LocalFolder.Path, "Plugins");
            }

            pluginFolder = Path.Combine(pluginFolder, _packageId);
            var data = workflowContext.StepParameters[workflowContext.CurrentStepIndex];
            var parameters = new Dictionary<string, string>();

            if (_source.Parameters?.Any() ?? false)
            {
                foreach (var parameter in _source.Parameters)
                {
                    if (string.IsNullOrEmpty(parameter.Name))
                    {
                        if (!string.IsNullOrEmpty(parameter.Id))
                        {
                            parameter.Name = parameter.Id;
                        }
                        else
                        {
                            continue;
                        }
                    }

                    if (string.IsNullOrEmpty(parameter.Id) && !string.IsNullOrEmpty(parameter.Name) && !string.IsNullOrEmpty(parameter.DefaultValue))
                    {
                        parameters.TryAdd(parameter.Name, parameter.DefaultValue);
                    }
                    else if (context.Variables.TryGetValue(parameter.Id, out var v))
                    {
                        parameters.TryAdd(parameter.Name, v);
                    }
                    else if (!string.IsNullOrEmpty(parameter.DefaultValue))
                    {
                        parameters.TryAdd(parameter.Name, parameter.DefaultValue);
                    }
                    else if (parameter.Required)
                    {
                        context.Fail($"Missing variable: {parameter.Id}");
                    }
                }
            }

            var exeFile = Path.Combine(pluginFolder, _source.ExecuteName);
            var finalOutput = await RunProcessAsync(exeFile, isShowWindow, parameters, context, cancellationToken);
            finalOutput = finalOutput.Trim();
            if (!string.IsNullOrEmpty(finalOutput))
            {
                if (_source.Output.Type.Equals("plain", StringComparison.OrdinalIgnoreCase))
                {
                    context.Variables.Update(finalOutput);
                }
                else if (_source.Output.Type.Equals("json", StringComparison.OrdinalIgnoreCase))
                {
                    var jdoc = JsonSerializer.Deserialize<JsonElement>(finalOutput);

                    if (string.IsNullOrEmpty(_source.Output.OutputKey))
                    {
                        context.Variables.Update(finalOutput);
                    }
                    else if (jdoc.TryGetProperty(_source.Output.OutputKey, out var outputEle))
                    {
                        context.Variables.Update(outputEle.ToString());
                    }

                    if (_source.Output.ContextItems?.Any() ?? false)
                    {
                        if (_source.Output.ContextItems.Any(p => p.Key == "*"))
                        {
                            var theItem = _source.Output.ContextItems.First(p => p.Key == "*");

                            // Store all items.
                            var enumObj = jdoc.EnumerateObject();
                            while (enumObj.MoveNext())
                            {
                                var current = enumObj.Current;
                                if ((context.Variables.ContainsKey(current.Name) && !theItem.Override)
                                    || !jdoc.TryGetProperty(current.Name, out var v))
                                {
                                    continue;
                                }

                                context.Variables.Set(current.Name, v.ToString());
                            }
                        }
                        else
                        {
                            foreach (var item in _source.Output.ContextItems)
                            {
                                if ((context.Variables.ContainsKey(item.Key) && !item.Override)
                                    || !jdoc.TryGetProperty(item.Key, out var v))
                                {
                                    continue;
                                }

                                context.Variables.Set(item.VariableName, v.ToString());
                            }
                        }
                    }
                }
            }

            return context;
        }

        public Task<SKContext> InvokeAsync(string input = null, CompleteRequestSettings settings = null, ILogger logger = null, CancellationToken cancellationToken = default)
        {
            var context = new SKContext(
                new ContextVariables(input),
                logger: logger);

            return InvokeAsync(context, settings, cancellationToken);
        }

        public ISKFunction SetAIConfiguration(CompleteRequestSettings settings) => this;

        public ISKFunction SetAIService(Func<ITextCompletion> serviceFactory) => this;

        public ISKFunction SetDefaultSkillCollection(IReadOnlySkillCollection skills) => this;

        private async Task<string> RunProcessAsync(string fileName, bool isShowWindow, Dictionary<string, string> parameters, SKContext context, CancellationToken cancellationToken)
        {
            var finalOutput = string.Empty;
            var skipError = false;
            var newProcess = new Process();
            newProcess.StartInfo.FileName = fileName;
            newProcess.StartInfo.WorkingDirectory = Path.GetDirectoryName(fileName);
            newProcess.StartInfo.RedirectStandardOutput = true;
            newProcess.StartInfo.RedirectStandardError = true;
            newProcess.StartInfo.UseShellExecute = false;
            newProcess.StartInfo.CreateNoWindow = !isShowWindow;
            var args = string.Join(' ', parameters.Select(p => $"-{p.Key} \"{p.Value}\""));
            if (!parameters.Any(p => p.Key.Equals("input", StringComparison.OrdinalIgnoreCase)) && !string.IsNullOrEmpty(context.Result))
            {
                args += $" -Input \"{context.Result}\"";
            }

            newProcess.StartInfo.Arguments = args.Trim();
            newProcess.Start();
            newProcess.BeginOutputReadLine();
            newProcess.BeginErrorReadLine();
            newProcess.OutputDataReceived += (_, e) =>
            {
                if (!string.IsNullOrEmpty(e.Data))
                {
                    if (_source.OnlyFinalOutput)
                    {
                        finalOutput = e.Data;
                    }
                    else
                    {
                        finalOutput += $"\n{e.Data}";
                    }
                }
            };

            newProcess.ErrorDataReceived += (_, e) =>
            {
                if (string.IsNullOrEmpty(e.Data) || skipError)
                {
                    return;
                }

                context.Fail($"{_source.Name} failed: {e.Data}");
            };

            await newProcess.WaitForExitAsync(cancellationToken);
            return finalOutput;
        }
    }
}
