// Copyright (c) Richasy Assistant. All rights reserved.

using System.IO;
using Microsoft.SemanticKernel;
using RichasyAssistant.DI.Container;
using RichasyAssistant.Models.App;
using RichasyAssistant.Models.Constants;
using RichasyAssistant.Libs.Services;
using RichasyAssistant.Libs.Services.Interfaces;
using RichasyAssistant.Toolkits;
using RichasyAssistant.Toolkits.Interfaces;
using RichasyAssistant.ViewModels;
using RichasyAssistant.ViewModels.Interfaces;
using Windows.Storage;

namespace RichasyAssistant.DI.App;

/// <summary>
/// Dependency Injection Factory.
/// </summary>
public sealed class Factory
{
    /// <summary>
    /// Register dependencies required by the application.
    /// </summary>
    public static void RegisterAppRequiredServices()
    {
        var rootFolder = ApplicationData.Current.LocalFolder;
        var logFolderName = AppConstants.LogFolderName;
        var fullPath = Path.Combine(rootFolder.Path, logFolderName);
        Locator.Current
            .RegisterVariable(typeof(WorkflowContext), new WorkflowContext())
            .RegisterVariable(typeof(IKernel), new KernelBuilder().Build())
            .RegisterSingleton<IFileToolkit, FileToolkit>()
            .RegisterSingleton<IResourceToolkit, ResourceToolkit>()
            .RegisterSingleton<ISettingsToolkit, SettingsToolkit>()
            .RegisterSingleton<ICacheToolkit, CacheToolkit>()

            .RegisterSingleton<IKernelService, KernelService>()
            .RegisterSingleton<IChatService, ChatService>()
            .RegisterSingleton<IMemoryService, MemoryService>()
            .RegisterSingleton<IWorkflowService, WorkflowService>()
            .RegisterSingleton<IVoiceService, AzureVoiceService>()
            .RegisterSingleton<IImageService, StableDiffusionImageService>()
            .RegisterSingleton<ITranslateService, TranslateService>()
            .RegisterSingleton<IStorageService, EverythingStorageService>()
            .RegisterSingleton<ISessionService, SessionService>()
            .RegisterSingleton<IPromptExplorerService, PromptExplorerService>()
            .RegisterSingleton<ICivitaiService, CivitaiService>()

            .RegisterSingleton<IAppViewModel, AppViewModel>()
            .RegisterTransient<ISessionOptionsViewModel, SessionOptionsViewModel>()
            .RegisterTransient<ISessionViewModel, SessionViewModel>()
            .RegisterTransient<IWorkflowStepViewModel, WorkflowStepViewModel>()
            .RegisterTransient<ICivitaiImageViewModel, CivitaiImageViewModel>()
            .RegisterTransient<IImageGenerateOptionsViewModel, ImageGenerateOptionsViewModel>()
            .RegisterTransient<IPluginCommandItemViewModel, PluginCommandItemViewModel>()
            .RegisterTransient<IPluginItemViewModel, PluginItemViewModel>()
            .RegisterTransient<IKnowledgeBaseItemViewModel, KnowledgeBaseItemViewModel>()
            .RegisterTransient<IKnowledgeContextViewModel, KnowledgeContextViewModel>()
            .RegisterTransient<IConnectorConfigViewModel, ConnectorConfigViewModel>()

            .RegisterSingleton<IChatSessionPageViewModel, ChatSessionPageViewModel>()
            .RegisterSingleton<ISavedSessionsModuleViewModel, SavedSessionsModuleViewModel>()
            .RegisterSingleton<IFavoritePromptsModuleViewModel, FavoritePromptsModuleViewModel>()
            .RegisterSingleton<IOnlinePromptsModuleViewModel, OnlinePromptsModuleViewModel>()
            .RegisterSingleton<ITextToSpeechModuleViewModel, TextToSpeechModuleViewModel>()
            .RegisterSingleton<ISpeechRecognizeModuleViewModel, SpeechRecognizeModuleViewModel>()
            .RegisterSingleton<ISemanticSkillEditModuleViewModel, SemanticSkillEditModuleViewModel>()
            .RegisterSingleton<ISemanticSkillsModuleViewModel, SemanticSkillsModuleViewModel>()
            .RegisterSingleton<IImageSkillEditModuleViewModel, ImageSkillEditModuleViewModel>()
            .RegisterSingleton<IImageSkillsModuleViewModel, ImageSkillsModuleViewModel>()
            .RegisterSingleton<IWorkflowEditorViewModel, WorkflowEditorViewModel>()
            .RegisterSingleton<IWorkflowRunnerViewModel, WorkflowRunnerViewModel>()
            .RegisterSingleton<IWorkflowsModuleViewModel, WorkflowsModuleViewModel>()
            .RegisterSingleton<IPluginsModuleViewModel, PluginsModuleViewModel>()
            .RegisterSingleton<IImageGalleryModuleViewModel, ImageGalleryModuleViewModel>()
            .RegisterSingleton<ITextToImageModuleViewModel, TextToImageModuleViewModel>()
            .RegisterSingleton<IPromptsAndSessionsPageViewModel, PromptsAndSessionsPageViewModel>()
            .RegisterSingleton<IKnowledgeBaseSessionViewModel, KnowledgeBaseSessionViewModel>()
            .RegisterSingleton<IKnowledgePageViewModel, KnowledgePageViewModel>()
            .RegisterSingleton<IVoicePageViewModel, VoicePageViewModel>()
            .RegisterSingleton<IImagePageViewModel, ImagePageViewModel>()
            .RegisterSingleton<ITranslatePageViewModel, TranslatePageViewModel>()
            .RegisterSingleton<IStoragePageViewModel, StoragePageViewModel>()
            .RegisterSingleton<IWorkspacePageViewModel, WorkspacePageViewModel>()
            .RegisterSingleton<ISettingsPageViewModel, SettingsPageViewModel>()

            .RegisterLogger(fullPath);

        Locator.Current.Build();
    }
}
