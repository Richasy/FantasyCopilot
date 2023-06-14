// Copyright (c) Fantasy Copilot. All rights reserved.

using System.Text.Json.Serialization;

namespace FantasyCopilot.Models.App.Plugins;

/// <summary>
/// Plugin command.
/// </summary>
public class PluginCommand
{
    /// <summary>
    /// The name of the command as it appears on the UI.
    /// </summary>
    [JsonPropertyName("command_name")]
    public string Name { get; set; }

    /// <summary>
    /// Command description, clear description helps to automatically generate the plan.
    /// </summary>
    [JsonPropertyName("command_desc")]
    public string Description { get; set; }

    /// <summary>
    /// The command identifier, which should be a GUID.
    /// </summary>
    [JsonPropertyName("command_id")]
    public string Identity { get; set; }

    /// <summary>
    /// The classification of the command.
    /// </summary>
    [JsonPropertyName("category")]
    public string Category { get; set; }

    /// <summary>
    /// Configuration of the command.
    /// </summary>
    [JsonPropertyName("config_set")]
    public List<InputConfig> ConfigSet { get; set; }

    /// <summary>
    /// The variables required by the command.
    /// </summary>
    [JsonPropertyName("parameters")]
    public List<InputParameter> Parameters { get; set; }

    /// <summary>
    /// Execution file name.
    /// </summary>
    /// <remarks>
    /// It's a relative path to the package directory.
    /// </remarks>
    [JsonPropertyName("execute_name")]
    public string ExecuteName { get; set; }

    /// <summary>
    /// The output of the command.
    /// </summary>
    [JsonPropertyName("output")]
    public Output Output { get; set; }

    /// <summary>
    /// Whether to keep only the last output text.
    /// </summary>
    [JsonPropertyName("only_final_output")]
    public bool OnlyFinalOutput { get; set; }
}

/// <summary>
/// Plugin command output.
/// </summary>
public class Output
{
    /// <summary>
    /// Output type, could be json or plain text.
    /// </summary>
    [JsonPropertyName("type")]
    public string Type { get; set; }

    /// <summary>
    /// If the output type is Json, this setting sets which key in the JSON
    /// corresponds to the value as the output text of the command.
    /// </summary>
    [JsonPropertyName("output_key")]
    public string OutputKey { get; set; }

    /// <summary>
    /// If the output item is JSON, the settings here save the value in JSON to the context.
    /// </summary>
    [JsonPropertyName("context_items")]
    public List<OutputContextParameter> ContextItems { get; set; }
}

/// <summary>
/// Parameters on output.
/// </summary>
public class OutputContextParameter
{
    /// <summary>
    /// Corresponding JSON key.
    /// </summary>
    [JsonPropertyName("key")]
    public string Key { get; set; }

    /// <summary>
    /// The name of the variable to save to.
    /// </summary>
    [JsonPropertyName("variable_name")]
    public string VariableName { get; set; }

    /// <summary>
    /// If a variable with the same name exists, whether to override it.
    /// </summary>
    [JsonPropertyName("override")]
    public bool Override { get; set; }
}

/// <summary>
/// Configuration at the creating time.
/// </summary>
public class InputConfig
{
    /// <summary>
    /// Input type, <c>input</c> or <c>option</c>.
    /// </summary>
    [JsonPropertyName("type")]
    public string Type { get; set; }

    /// <summary>
    /// The corresponding variable name.
    /// </summary>
    [JsonPropertyName("variable_name")]
    public string VariableName { get; set; }

    /// <summary>
    /// Config title.
    /// </summary>
    [JsonPropertyName("title")]
    public string Title { get; set; }

    /// <summary>
    /// Default value.
    /// </summary>
    [JsonPropertyName("default_value")]
    public string DefaultValue { get; set; }

    /// <summary>
    /// If type is option, here is a list of optional values.
    /// </summary>
    [JsonPropertyName("options")]
    public List<InputOptionItem> Options { get; set; }
}

/// <summary>
/// The option item of the input.
/// </summary>
public class InputOptionItem
{
    /// <summary>
    /// The option id, which is also the value that is finally written into the variable.
    /// </summary>
    [JsonPropertyName("id")]
    public string Id { get; set; }

    /// <summary>
    /// Options display name.
    /// </summary>
    [JsonPropertyName("display_name")]
    public string DisplayName { get; set; }

    /// <inheritdoc/>
    public override bool Equals(object? obj) => obj is InputOptionItem item && Id == item.Id;

    /// <inheritdoc/>
    public override int GetHashCode() => HashCode.Combine(Id);
}

/// <summary>
/// Parameter definitions required to execute the command.
/// </summary>
public class InputParameter
{
    /// <summary>
    /// Parameter id.
    /// </summary>
    [JsonPropertyName("id")]
    public string Id { get; set; }

    /// <summary>
    /// The final output is the name in the program.
    /// </summary>
    [JsonPropertyName("name")]
    public string Name { get; set; }

    /// <summary>
    /// The parameter description.
    /// </summary>
    [JsonPropertyName("description")]
    public string Description { get; set; }

    /// <summary>
    /// Whether it is a required parameter.
    /// </summary>
    /// <remarks>
    /// If so, then when there is no corresponding variable in the context,
    /// the workflow is interrupted and an exception is thrown.
    /// </remarks>
    [JsonPropertyName("required")]
    public bool Required { get; set; }

    /// <summary>
    /// Default value of the parameter.
    /// </summary>
    [JsonPropertyName("default_value")]
    public string DefaultValue { get; set; }
}
