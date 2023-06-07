﻿// <auto-generated />
// Copyright (c) Fantasy Copilot. All rights reserved.

namespace FantasyCopilot.Models.Constants;

/// <summary>
/// Constants used in the application.
/// </summary>
public static class AppConstants
{
    public const string SavedSessionFileName = "SavedSessions.json";
    public const string FavoritePromptsFileName = "FavoritePrompts.json";
    public const string PromptLibraryFileName = "PromptLibrary.json";
    public const string AzureVoicesFileName = "AzureVoices.json";
    public const string AzureTranslateLanguagesFileName = "AzureTranslateLanguages.json";
    public const string SemanticSkillsFileName = "SemanticSkills.json";
    public const string ImageSkillsFileName = "ImageSkills.json";
    public const string WorkflowFileName = "Workflows.json";
    public const string KnowledgeBaseFileName = "KnowledgeBases.json";

    public const string LocalSessionFolderName = "LocalSessions";
    public const string OnlinePromptFolderName = "OnlinePrompts";
    public const string LocalWorkflowFolderName = "LocalWorkflows";
    public const string LogFolderName = "Logger";

    public const string ClearSessionId = "CLEAR_SESSION";

    public const string ExceptionTag = "<|exception|>";
    public const string SystemTag = "<|system|>";
    public const string UserTag = "<|user|>";
    public const string AssistantTag = "<|assistant|>";
    public const string SessionOptionsKey = "SessionOptions";
    public const string MessageCancelledContent = "The operation was canceled.";

    public const string AwesomePromptsZhSource = "https://raw.githubusercontent.com/PlexPt/awesome-chatgpt-prompts-zh/main/prompts-zh.json";
    public const string AwesomePromptsSource = "https://raw.githubusercontent.com/f/awesome-chatgpt-prompts/main/prompts.csv";
    public const string FlowGptZhTrendingSource = "https://flowgpt.com/api/trpc/prompt.getPrompts?batch=1&input={\"0\":{\"json\":{\"skip\":0,\"tag\":null,\"sort\":null,\"q\":null,\"language\":\"zh\"},\"meta\":{\"values\":{\"tag\":[\"undefined\"],\"sort\":[\"undefined\"],\"q\":[\"undefined\"]}}}}";
    public const string FlowGptEnTrendingSource = "https://flowgpt.com/api/trpc/prompt.getPrompts?batch=1&input={\"0\":{\"json\":{\"skip\":0,\"tag\":null,\"sort\":null,\"q\":null,\"language\":\"en\"},\"meta\":{\"values\":{\"tag\":[\"undefined\"],\"sort\":[\"undefined\"],\"q\":[\"undefined\"]}}}}";

    public const string DefaultUserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/113.0.0.0 Safari/537.36 Edg/113.0.1774.50";
    public const string KnowledgeBaseCollectionId = "KnowledgeBase";
}
