// Copyright (c) Richasy Assistant. All rights reserved.

using RichasyAssistant.Models.Constants;

namespace RichasyAssistant.Models.App.Workspace;

/// <summary>
/// Work command item.
/// </summary>
public sealed class WorkCommandItem : WorkCommandBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="WorkCommandItem"/> class.
    /// </summary>
    public WorkCommandItem(string name, SkillType type, string pluginId = default, string commandId = default)
    {
        Name = name;
        Skill = type;
        PluginId = pluginId;
        CommandId = commandId;
    }

    /// <summary>
    /// Skill type.
    /// </summary>
    public SkillType Skill { get; set; }

    /// <summary>
    /// Plugin id.
    /// </summary>
    public string PluginId { get; set; }

    /// <summary>
    /// Command id.
    /// </summary>
    public string CommandId { get; set; }
}
