﻿// Copyright (c) Richasy Assistant. All rights reserved.

namespace RichasyAssistant.Models.App.Workspace;

/// <summary>
/// Work command group.
/// </summary>
public class WorkCommandGroup : WorkCommandBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="WorkCommandGroup"/> class.
    /// </summary>
    public WorkCommandGroup(string name, List<WorkCommandBase> commands)
    {
        Name = name;
        Commands = commands;
    }

    /// <summary>
    /// Commands.
    /// </summary>
    public List<WorkCommandBase> Commands { get; set; }
}
