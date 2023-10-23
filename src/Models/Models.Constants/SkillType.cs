// Copyright (c) Richasy Assistant. All rights reserved.

namespace RichasyAssistant.Models.Constants;

/// <summary>
/// Skill type.
/// </summary>
public enum SkillType
{
    /// <summary>
    /// Invalid skill.
    /// </summary>
    None = -1,

    /// <summary>
    /// Text input skill.
    /// </summary>
    InputText = 1,

    /// <summary>
    /// Voice input skill.
    /// </summary>
    InputVoice = 2,

    /// <summary>
    /// File input skill.
    /// </summary>
    InputFile = 3,

    /// <summary>
    /// Click to start.
    /// </summary>
    InputClick = 4,

    /// <summary>
    /// Text output skill.
    /// </summary>
    OutputText = 100,

    /// <summary>
    /// Voice output skill.
    /// </summary>
    OutputVoice = 101,

    /// <summary>
    /// Image output skill.
    /// </summary>
    OutputImage = 102,

    /// <summary>
    /// Semantic skill.
    /// </summary>
    Semantic = 1000,

    /// <summary>
    /// Translate skill.
    /// </summary>
    Translate = 1001,

    /// <summary>
    /// Text to speech skill.
    /// </summary>
    TextToSpeech = 1002,

    /// <summary>
    /// Text overwrite skill.
    /// </summary>
    TextOverwrite = 1003,

    /// <summary>
    /// Text to image skill.
    /// </summary>
    TextToImage = 1004,

    /// <summary>
    /// Get content from the knowledge base.
    /// </summary>
    GetKnowledge = 1005,

    /// <summary>
    /// Import the file into the knowledge base.
    /// </summary>
    ImportFileToKnowledge = 1006,

    /// <summary>
    /// Import the folder into the knowledge base.
    /// </summary>
    ImportFolderToKnowledge = 1007,

    /// <summary>
    /// Copy the value of one variable in the context to another variable.
    /// </summary>
    VariableRedirect = 1008,

    /// <summary>
    /// Send text notifications.
    /// </summary>
    TextNotification = 1009,

    /// <summary>
    /// Create variable.
    /// </summary>
    VariableCreate = 1010,

    /// <summary>
    /// Plugin command.
    /// </summary>
    PluginCommand = 10000,
}
