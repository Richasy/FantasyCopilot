// Copyright (c) Richasy Assistant. All rights reserved.

using System;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.Logging;
using Microsoft.UI.Dispatching;
using RichasyAssistant.DI.Container;
using RichasyAssistant.Models.App;
using RichasyAssistant.Models.App.Gpt;
using RichasyAssistant.Models.App.Knowledge;
using RichasyAssistant.Models.Constants;
using RichasyAssistant.Services.Interfaces;
using RichasyAssistant.Toolkits.Interfaces;
using RichasyAssistant.ViewModels.Interfaces;
using Windows.Storage;

namespace RichasyAssistant.ViewModels;

/// <summary>
/// Knowledge base session view model.
/// </summary>
public sealed partial class KnowledgeBaseSessionViewModel : ViewModelBase, IKnowledgeBaseSessionViewModel
{
    /// <summary>
    /// Initializes a new instance of the <see cref="KnowledgeBaseSessionViewModel"/> class.
    /// </summary>
    public KnowledgeBaseSessionViewModel(
        IMemoryService memoryService,
        IFileToolkit fileToolkit,
        IResourceToolkit resourceToolkit,
        ISettingsToolkit settingsToolkit,
        ILogger<KnowledgeBaseSessionViewModel> logger,
        IAppViewModel appViewModel)
    {
        _memoryService = memoryService;
        _fileToolkit = fileToolkit;
        _resourceToolkit = resourceToolkit;
        _appViewModel = appViewModel;
        _settingsToolkit = settingsToolkit;
        _logger = logger;
        _dispatcherQueue = DispatcherQueue.GetForCurrentThread();

        Messages = new SynchronizedObservableCollection<Message>();
        Contexts = new SynchronizedObservableCollection<IKnowledgeContextViewModel>();
        SearchType = _settingsToolkit.ReadLocalSetting(SettingNames.LastKnowledgeSearchType, KnowledgeSearchType.Quick);
        QuickUserInput = string.Empty;
        Messages.CollectionChanged += OnMessageCollectionChanged;
        Contexts.CollectionChanged += OnContextCollectionChanged;
    }

    /// <inheritdoc/>
    public void Initialize(KnowledgeBase @base)
    {
        if (_sourceBase == null || _sourceBase != @base)
        {
            TryClear(Messages);
            TryClear(Contexts);
            AdvancedAnswerResult = default;
            QuickUserInput = string.Empty;
            AdvancedUserInput = string.Empty;
            ErrorText = string.Empty;
            var options = Locator.Current.GetService<ISessionOptionsViewModel>();
            options.Initialize(default);
            Options = options;
        }

        _sourceBase = @base;
        CheckChatEmpty();
        CheckSearchType();
        CheckAdvancedSearchStatus();

        if (_sourceBase != null)
        {
            Name = _sourceBase.Name;
            Description = _sourceBase.Description;
        }
    }

    [RelayCommand]
    private async Task SaveAsync()
    {
        if (IsQuickQA || Contexts.Count == 0)
        {
            return;
        }

        var fileObj = await _fileToolkit.SaveFileAsync($"{_sourceBase.Name}.txt", _appViewModel.MainWindow);
        if (fileObj is not StorageFile file)
        {
            return;
        }

        var sb = new StringBuilder();
        sb.AppendLine($"Query: {_sourceAdvancedQuery}");
        sb.AppendLine();
        foreach (var item in Contexts)
        {
            sb.AppendLine($"====================== {item.Context.FileName} ======================");
            sb.AppendLine();
            sb.AppendLine(item.Context.Content);
            sb.AppendLine();
        }

        await FileIO.WriteTextAsync(file, sb.ToString());
    }

    [RelayCommand]
    private async Task SendQueryAsync()
    {
        if (IsQuickQA)
        {
            await QuickQAAsync();
        }
        else
        {
            await AdvancedSearchAsync();
        }
    }

    [RelayCommand]
    private async Task GenerateAnswerAsync()
    {
        if (IsQuickQA)
        {
            return;
        }

        IsAdvancedAnswerResponding = true;
        AdvancedAnswerResult = default;
        CancelMessage();

        try
        {
            var selectedContexts = Contexts.Where(p => p.IsSelected).Select(p => p.Context).ToList();
            if (selectedContexts?.Any() ?? false)
            {
                _cancellationTokenSource = new CancellationTokenSource();
                var response = await _memoryService.GetAnswerFromContextAsync(_sourceAdvancedQuery, selectedContexts, Options.GetOptions(), _cancellationTokenSource.Token);
                var message = new Message
                {
                    Content = response.Content,
                    IsUser = false,
                    AdditionalMessage = response.AdditionalContent,
                    Time = DateTimeOffset.Now,
                };

                AdvancedAnswerResult = message;
                RequestScrollToBottom?.Invoke(this, EventArgs.Empty);
            }
        }
        catch (Exception ex)
        {
            if (ex is TaskCanceledException || ex.Message.Contains(AppConstants.MessageCancelledContent))
            {
                return;
            }

            _appViewModel.ShowTip(ex.Message, InfoType.Error);
            _logger.LogError(ex, "Failed to generate answer");
        }

        _cancellationTokenSource = default;
        IsAdvancedAnswerResponding = false;
    }

    [RelayCommand]
    private void CancelMessage()
    {
        if (_cancellationTokenSource != null && _cancellationTokenSource.Token.CanBeCanceled)
        {
            try
            {
                _cancellationTokenSource?.Cancel();
            }
            catch (Exception)
            {
            }

            _cancellationTokenSource = default;
        }
    }

    private async Task QuickQAAsync()
    {
        if (string.IsNullOrEmpty(QuickUserInput))
        {
            return;
        }

        IsQuickResponding = true;
        CancelMessage();
        _cancellationTokenSource = new CancellationTokenSource();
        ErrorText = string.Empty;

        try
        {
            TryClear(Messages);
            var msg = QuickUserInput;
            var userMsg = new Message
            {
                Content = msg,
                IsUser = true,
                Time = DateTimeOffset.Now,
            };
            Messages.Add(userMsg);
            QuickUserInput = string.Empty;
            var response = await _memoryService.QuickSearchMemoryAsync(msg, Options.GetOptions(), _cancellationTokenSource.Token);
            var message = new Message
            {
                Content = response.Content,
                IsUser = false,
                AdditionalMessage = response.AdditionalContent,
                Time = DateTimeOffset.Now,
            };

            Messages.Add(message);
            RequestScrollToBottom?.Invoke(this, EventArgs.Empty);
        }
        catch (Exception ex)
        {
            var lastMessage = Messages.Last();
            if (lastMessage.IsUser)
            {
                Messages.RemoveAt(Messages.Count - 1);
                if (string.IsNullOrEmpty(QuickUserInput))
                {
                    QuickUserInput = lastMessage.Content;
                }
            }

            ErrorText = string.IsNullOrEmpty(ex.Message) ? "Unknown error" : ex.Message;
            _logger.LogError(ex, "Failed to quick qa.");
        }

        _cancellationTokenSource = default;
        IsQuickResponding = false;
    }

    private async Task AdvancedSearchAsync()
    {
        if (string.IsNullOrEmpty(AdvancedUserInput))
        {
            return;
        }

        IsAdvancedSearchResponding = true;
        CancelMessage();
        TryClear(Contexts);
        AdvancedAnswerResult = default;
        _cancellationTokenSource = new CancellationTokenSource();
        _sourceAdvancedQuery = AdvancedUserInput;

        try
        {
            var contexts = await _memoryService.AdvancedSearchMemoryAsync(_sourceAdvancedQuery, _cancellationTokenSource.Token);
            if (contexts?.Any() ?? false)
            {
                foreach (var c in contexts)
                {
                    var vm = Locator.Current.GetService<IKnowledgeContextViewModel>();
                    vm.InjectData(c);
                    Contexts.Add(vm);
                }

                RequestScrollToBottom?.Invoke(this, EventArgs.Empty);
            }
        }
        catch (Exception ex)
        {
            if (ex is TaskCanceledException || ex.Message.Contains(AppConstants.MessageCancelledContent))
            {
                return;
            }

            _appViewModel.ShowTip(ex.Message, InfoType.Error);
            _logger.LogError(ex, "Failed to advanced search.");
        }

        _cancellationTokenSource = default;
        IsAdvancedSearchResponding = false;
    }

    private void CheckChatEmpty()
        => IsChatEmpty = Messages.Count == 0;

    private void CheckAdvancedSearchStatus()
    {
        IsNoContext = Contexts.Count == 0 && !string.IsNullOrEmpty(QuickUserInput);
        IsNoUserInputWhenAdvancedSearch = Contexts.Count == 0 && string.IsNullOrEmpty(QuickUserInput);
    }

    private void CheckSearchType()
    {
        IsAdvancedSearch = SearchType == KnowledgeSearchType.Advanced;
        IsQuickQA = SearchType == KnowledgeSearchType.Quick;
    }

    private void OnMessageCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        => CheckChatEmpty();

    private void OnContextCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        => CheckAdvancedSearchStatus();

    partial void OnAdvancedUserInputChanged(string value)
        => CheckAdvancedSearchStatus();

    partial void OnSearchTypeChanged(KnowledgeSearchType value)
    {
        CheckSearchType();
        _settingsToolkit.WriteLocalSetting(SettingNames.LastKnowledgeSearchType, value);
    }
}
