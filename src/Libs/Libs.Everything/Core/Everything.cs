﻿// <auto-generated />


namespace FantasyCopilot.Libs.Everything.Core
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using FantasyCopilot.Libs.Everything.Interfaces;
    using FantasyCopilot.Libs.Everything.Query;

    public class Everything : IEverythingInternal, IDisposable
    {
        private static int lastReplyId;

        private const uint DefaultSearchFlags = (uint)(
          RequestFlags.EVERYTHING_REQUEST_SIZE
        | RequestFlags.EVERYTHING_REQUEST_FILE_NAME
        | RequestFlags.EVERYTHING_REQUEST_EXTENSION
        | RequestFlags.EVERYTHING_REQUEST_PATH
        | RequestFlags.EVERYTHING_REQUEST_FULL_PATH_AND_FILE_NAME
        | RequestFlags.EVERYTHING_REQUEST_DATE_MODIFIED);

        private readonly uint replyId;

        public Everything(string appPath)
        {
            EverythingState.AppPath = appPath;
            this.ResulKind = ResultKind.Both;
            Interlocked.Increment(ref lastReplyId);
            this.replyId = Convert.ToUInt32(lastReplyId);
            EverythingState.StartService(false, EverythingState.StartMode.Service);
        }

        public ResultKind ResulKind { get; set; }

        public bool MatchCase { get; set; }

        public bool MatchPath { get; set; }

        public bool MatchWholeWord { get; set; }

        public SortingKey SortKey { get; set; }

        public ErrorCode LastErrorCode { get; set; }

        public string SearchText { get; set; }

        public long Count => EverythingWrapper.Everything_GetNumResults();

        public IQuery Search()
        {
            return new Query(this);
        }

        public void Reset()
        {
            EverythingWrapper.Everything_Reset();
        }

        public void Dispose()
        {
            this.Reset();
        }

        IEnumerable<ISearchResult> IEverythingInternal.SendSearch(string searchPattern, RequestFlags flags)
        {
            using (EverythingWrapper.Lock())
            {
                EverythingWrapper.Everything_SetReplyID(this.replyId);
                EverythingWrapper.Everything_SetMatchWholeWord(this.MatchWholeWord);
                EverythingWrapper.Everything_SetMatchPath(this.MatchPath);
                EverythingWrapper.Everything_SetMatchCase(this.MatchCase);
                EverythingWrapper.Everything_SetRequestFlags((uint)flags | DefaultSearchFlags);
                searchPattern = this.ApplySearchResultKind(searchPattern);
                EverythingWrapper.Everything_SetSearch(searchPattern);

                if (this.SortKey != SortingKey.None)
                {
                    EverythingWrapper.Everything_SetSort((uint)this.SortKey);
                }

                EverythingWrapper.Everything_Query(true);

                this.LastErrorCode = this.GetError();

                return this.GetResults();
            }
        }

        private string ApplySearchResultKind(string searchPatten)
        {
            switch (this.ResulKind)
            {
                case ResultKind.FilesOnly:
                    return $"files: {searchPatten}";
                case ResultKind.FoldersOnly:
                    return $"folders: {searchPatten}";
                default:
                    return searchPatten;
            }
        }

        private IEnumerable<ISearchResult> GetResults()
        {
            var numResults = EverythingWrapper.Everything_GetNumResults();

            return Enumerable.Range(0, (int)numResults).Select(x => new SearchResult(x, this.replyId));
        }

        private ErrorCode GetError()
        {
            var error = EverythingWrapper.Everything_GetLastError();

            return (ErrorCode)error;
        }
    }
}
