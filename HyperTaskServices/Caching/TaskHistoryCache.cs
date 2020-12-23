using HyperTaskCore.Models;
using HyperTaskTools;
using System;
using System.Collections.Generic;
using System.Text;

namespace HyperTaskServices.Caching
{
    [Obsolete("not tested", true)]
    public class TaskHistoryCache
    {
        private CachingManager Caching { get; set; }

        public TaskHistoryCache(CachingManager cachingManager)
        {
            this.Caching = cachingManager;
        }

        public CachedTaskHistories GetCachedHistories(GetCalendarTaskRequest request)
        {
            string key = GetCachingKey(request);
            CachedTaskHistories cachedHistories = (CachedTaskHistories)Caching.TryGet(key);
            return cachedHistories;
        }

        public void AddToCache(CachedTaskHistories cachedHistories)
        {
            if (cachedHistories.Request == null || cachedHistories.Request.UserId == null || cachedHistories.Request.UserId.Length == 0)
                throw new ArgumentException("getCachingKey, request is invalid");

            string cachingKey = GetCachingKey(cachedHistories.Request);
            this.Caching.TrySet(cachingKey,
                                cachedHistories,
                                null,
                                cachedHistories.Histories.Count);
        }

        public string GetCachingKey(GetCalendarTaskRequest request)
        {
            if (request == null || request.UserId == null || request.UserId.Length == 0)
                throw new ArgumentException("getCachingKey, request is invalid");

            var key = $"taskHistory{request.UserId}{request.IncludeVoid}";

            if (request.DateStart != null)
                key += request.DateStart.Value.ToString();

            if (request.DateEnd != null)
                key += request.DateEnd.Value.ToString();

            return key;
        }

        /*public string GetCachingKey(string userId)
        {
            if (userId == null || userId.Length == 0)
                throw new ArgumentException("getCachingKey, userId is invalid");

            return $"taskHistory{userId}";
        }*/
    }

    public class CachedTaskHistories
    {
        public List<ITaskHistory> Histories { get; set; }
        public GetCalendarTaskRequest Request { get; set; }

        public CachedTaskHistories()
        {

        }

        public CachedTaskHistories(GetCalendarTaskRequest request, List<ITaskHistory> histories)
        {
            this.Histories = histories;
            this.Request = request;
        }
    }
}
