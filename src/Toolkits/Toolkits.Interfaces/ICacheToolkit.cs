// Copyright (c) Fantasy Copilot. All rights reserved.

using FantasyCopilot.Models.App.Gpt;
using FantasyCopilot.Models.App.Knowledge;
using FantasyCopilot.Models.App.Plugins;
using FantasyCopilot.Models.App.Workspace;

namespace FantasyCopilot.Toolkits.Interfaces;

/// <summary>
/// Interface for cache management tools.
/// </summary>
public interface ICacheToolkit
{
    /// <summary>
    /// Occurs when the session list is updated.
    /// </summary>
    event EventHandler SessionListChanged;

    /// <summary>
    /// Occurs when the prompt list is updated.
    /// </summary>
    event EventHandler PromptListChanged;

    /// <summary>
    /// Occurs when the semantic skill list is updated.
    /// </summary>
    event EventHandler SemanticSkillListChanged;

    /// <summary>
    /// Occurs when the image skill list is updated.
    /// </summary>
    event EventHandler ImageSkillListChanged;

    /// <summary>
    /// Occurs when the workflow list is updated.
    /// </summary>
    event EventHandler WorkflowListChanged;

    /// <summary>
    /// Occurs when the knowledge base list is updated.
    /// </summary>
    event EventHandler KnowledgeBaseListChanged;

    /// <summary>
    /// Initialize the cached sessions, if it has been initialized, return directly.
    /// </summary>
    /// <returns><see cref="Task"/>.</returns>
    Task InitializeCacheSessionsIfNotReadyAsync();

    /// <summary>
    /// Initialize the cached prompts, if it has been initialized, return directly.
    /// </summary>
    /// <returns><see cref="Task"/>.</returns>
    Task InitializeCustomPromptsIfNotReadyAsync();

    /// <summary>
    /// Initialize the cached semantic skills, if it has been initialized, return directly.
    /// </summary>
    /// <returns><see cref="Task"/>.</returns>
    Task InitializeSemanticSkillsIfNotReadyAsync();

    /// <summary>
    /// Initialize the cached image skills, if it has been initialized, return directly.
    /// </summary>
    /// <returns><see cref="Task"/>.</returns>
    Task InitializeImageSkillsIfNotReadyAsync();

    /// <summary>
    /// Initialize the cached workflows, if it has been initialized, return directly.
    /// </summary>
    /// <returns><see cref="Task"/>.</returns>
    Task InitializeWorkflowsIfNotReadyAsync();

    /// <summary>
    /// Initialize the cached knowledge bases, if it has been initialized, return directly.
    /// </summary>
    /// <returns><see cref="Task"/>.</returns>
    Task InitializeKnowledgeBasesIfNotReadyAsync();

    /// <summary>
    /// Get the cached session by Id.
    /// </summary>
    /// <param name="sessionId">Session id.</param>
    /// <returns><see cref="Session"/>.</returns>
    Task<Session> GetSessionByIdAsync(string sessionId);

    /// <summary>
    /// Get a list of all sessions.
    /// </summary>
    /// <returns>Session list.</returns>
    Task<IEnumerable<SessionMetadata>> GetSessionListAsync();

    /// <summary>
    /// Add or update session metadata.
    /// </summary>
    /// <param name="metadata">Metadata.</param>
    /// <returns><see cref="Task"/>.</returns>
    Task AddOrUpdateSessionMetadataAsync(SessionMetadata metadata);

    /// <summary>
    /// Import session package from outside.
    /// </summary>
    /// <returns>Import result.</returns>
    Task<bool?> ImportSessionsAsync();

    /// <summary>
    /// Export existing sessions to a file.
    /// </summary>
    /// <returns>Export result.</returns>
    Task<bool?> ExportSessionsAsync();

    /// <summary>
    /// Save session data.
    /// </summary>
    /// <param name="session">Session data.</param>
    /// <returns><see cref="Task"/>.</returns>
    Task SaveSessionAsync(Session session);

    /// <summary>
    /// Remove session.
    /// </summary>
    /// <param name="id">Session id.</param>
    /// <returns><see cref="Task"/>.</returns>
    Task DeleteSessionAsync(string id);

    /// <summary>
    /// Get the list of custom prompt words saved locally.
    /// </summary>
    /// <returns>Prompt list.</returns>
    Task<IEnumerable<SessionMetadata>> GetCustomPromptsAsync();

    /// <summary>
    /// Save custom prompt.
    /// </summary>
    /// <param name="prompt">Custom prompt template.</param>
    /// <returns><see cref="Task"/>.</returns>
    Task AddOrUpdatePromptAsync(SessionMetadata prompt);

    /// <summary>
    /// Import prompt configuration from outside.
    /// </summary>
    /// <returns>Import result.</returns>
    Task<bool?> ImportPromptsAsync();

    /// <summary>
    /// Export existing prompts to a file.
    /// </summary>
    /// <returns>Export result.</returns>
    Task<bool?> ExportPromptsAsync();

    /// <summary>
    /// Remove prompt.
    /// </summary>
    /// <param name="id">Prompt id.</param>
    /// <returns><see cref="Task"/>.</returns>
    Task DeletePromptAsync(string id);

    /// <summary>
    /// Get the list of semantic skills saved locally.
    /// </summary>
    /// <returns>Skill list.</returns>
    Task<IEnumerable<SemanticSkillConfig>> GetSemanticSkillsAsync();

    /// <summary>
    /// Get the cached semantic skill by Id.
    /// </summary>
    /// <param name="skillId">Session id.</param>
    /// <returns><see cref="SemanticSkillConfig"/>.</returns>
    Task<SemanticSkillConfig> GetSemanticSkillByIdAsync(string skillId);

    /// <summary>
    /// Save semantic skill.
    /// </summary>
    /// <param name="config">Semantic skill config.</param>
    /// <returns><see cref="Task"/>.</returns>
    Task AddOrUpdateSemanticSkillAsync(SemanticSkillConfig config);

    /// <summary>
    /// Import semantic skills from outside.
    /// </summary>
    /// <returns>Import result.</returns>
    Task<bool?> ImportSemanticSkillsAsync();

    /// <summary>
    /// Export existing semantic skills to a file.
    /// </summary>
    /// <returns>Export result.</returns>
    Task<bool?> ExportSemanticSkillsAsync();

    /// <summary>
    /// Remove semantic skill.
    /// </summary>
    /// <param name="id">Skill id.</param>
    /// <returns><see cref="Task"/>.</returns>
    Task DeleteSemanticSkillAsync(string id);

    /// <summary>
    /// Get the list of image skills saved locally.
    /// </summary>
    /// <returns>Skill list.</returns>
    Task<IEnumerable<ImageSkillConfig>> GetImageSkillsAsync();

    /// <summary>
    /// Get the cached image skill by Id.
    /// </summary>
    /// <param name="skillId">Session id.</param>
    /// <returns><see cref="ImageSkillConfig"/>.</returns>
    Task<ImageSkillConfig> GetImageSkillByIdAsync(string skillId);

    /// <summary>
    /// Save image skill.
    /// </summary>
    /// <param name="config">Image skill config.</param>
    /// <returns><see cref="Task"/>.</returns>
    Task AddOrUpdateImageSkillAsync(ImageSkillConfig config);

    /// <summary>
    /// Import image skills from outside.
    /// </summary>
    /// <returns>Import result.</returns>
    Task<bool?> ImportImageSkillsAsync();

    /// <summary>
    /// Export existing image skills to a file.
    /// </summary>
    /// <returns>Export result.</returns>
    Task<bool?> ExportImageSkillsAsync();

    /// <summary>
    /// Remove image skill.
    /// </summary>
    /// <param name="id">Skill id.</param>
    /// <returns><see cref="Task"/>.</returns>
    Task DeleteImageSkillAsync(string id);

    /// <summary>
    /// Get the cached workflow by Id.
    /// </summary>
    /// <param name="workflowId">Workflow id.</param>
    /// <returns><see cref="Workflow"/>.</returns>
    Task<WorkflowConfig> GetWorkflowByIdAsync(string workflowId);

    /// <summary>
    /// Get a list of all workflows.
    /// </summary>
    /// <returns>Workflow list.</returns>
    Task<IEnumerable<WorkflowMetadata>> GetWorkflowListAsync();

    /// <summary>
    /// Add or update workflow metadata.
    /// </summary>
    /// <param name="metadata">Metadata.</param>
    /// <returns><see cref="Task"/>.</returns>
    Task AddOrUpdateWorkflowMetadataAsync(WorkflowMetadata metadata);

    /// <summary>
    /// Save workflow data.
    /// </summary>
    /// <param name="workflow">Workflow data.</param>
    /// <returns><see cref="Task"/>.</returns>
    Task SaveWorkflowAsync(WorkflowConfig workflow);

    /// <summary>
    /// Remove workflow.
    /// </summary>
    /// <param name="id">Workflow id.</param>
    /// <returns><see cref="Task"/>.</returns>
    Task DeleteWorkflowAsync(string id);

    /// <summary>
    /// Get a list of all knowledge base.
    /// </summary>
    /// <returns>Session list.</returns>
    Task<IEnumerable<KnowledgeBase>> GetKnowledgeBasesAsync();

    /// <summary>
    /// Add or update knowledge base.
    /// </summary>
    /// <param name="data">Metadata.</param>
    /// <returns><see cref="Task"/>.</returns>
    Task AddOrUpdateKnowledgeBaseAsync(KnowledgeBase data);

    /// <summary>
    /// Remove knowledge base.
    /// </summary>
    /// <param name="id">Base id.</param>
    /// <returns><see cref="Task"/>.</returns>
    Task DeleteKnowledgeBaseAsync(string id);

    /// <summary>
    /// Get a list of plugins.
    /// </summary>
    /// <returns>Plugin config list.</returns>
    Task<IEnumerable<PluginConfig>> GetPluginConfigsAsync(bool refresh = false);

    /// <summary>
    /// Read configuration information from the plug-in package.
    /// </summary>
    /// <param name="pluginZipPath">The plug-in package path.</param>
    /// <returns><see cref="PluginConfig"/>.</returns>
    Task<PluginConfig> GetPluginConfigFromZipAsync(string pluginZipPath);

    /// <summary>
    /// Import the plug-in.
    /// </summary>
    /// <param name="config">Plugin config.</param>
    /// <param name="pluginZipPath">The plug-in package path.</param>
    /// <returns>The resulting plug-in configuration.</returns>
    Task ImportPluginConfigAsync(PluginConfig config, string pluginZipPath);

    /// <summary>
    /// Removes the specified plug-in.
    /// </summary>
    /// <param name="pluginId">Plugin id.</param>
    /// <returns><see cref="Task"/>.</returns>
    Task RemovePluginAsync(string pluginId);
}
