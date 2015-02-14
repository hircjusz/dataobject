using System;
using System.Web;
using System.Web.Caching;
using SoftwareMind.Logger;
using System.Configuration;
using log4net;

namespace SoftwareMind.Utils
{
    public static class CacheHelper
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(CacheHelper));

        /// <summary>
        /// Specifies the relative priority of items stored in the System.Web.Caching.Cache object
        /// </summary>
        private static string CachePriority = Convert.ToString(ConfigurationManager.AppSettings["CachePriority"]);

        /// <summary>
        /// Cached elements are likely to expire after this amount of time
        /// </summary>
        private static string CacheTimeout = Convert.ToString(ConfigurationManager.AppSettings["CacheTimeout"]);

        private static bool LogEvents = Convert.ToBoolean(ConfigurationManager.AppSettings["LogCacheEvents"]);

        /// <summary>
        /// Is cache object actually configured
        /// </summary>
        private static bool Configured;

        private static CacheItemPriority Priority;
        private static int Timeout;

        private static void Configure()
        {
            CacheItemPriority priority;
            if (!Enum.TryParse<CacheItemPriority>(CachePriority, out priority))
            {
                if (LogEvents)
                    log.InfoFormat("HttpRuntime.Cache: Invalid configuration of CachePriority ({0}) attribute", CachePriority);
                return;
            }

            Priority = priority;

            //1m = 1
            //1h = 60 * 1
            //1d = 24 * 60 * 1
            int minutes = 0;
            string timeout = CacheTimeout.Trim().ToLower();
            string volume = "";
            if (timeout.EndsWith("m"))
            {
                volume = "m";
                minutes = 1;
            }
            else if (timeout.EndsWith("h"))
            {
                volume = "h";
                minutes = 60;
            }
            else if (timeout.EndsWith("d"))
            {
                volume = "d";
                minutes = 24 * 60;
            }

            minutes *= int.Parse(timeout.Replace(volume, ""));
            Timeout = minutes;

            Configured = true;
        }

        /// <summary>
        /// Insert value into the cache using
        /// appropriate name/value pairs
        /// </summary>
        /// <typeparam name="T">Type of cached item</typeparam>
        /// <param name="sid">User provided session id (HTTP Session!)</param>
        /// <param name="o">Item to be cached</param>
        /// <param name="key">Name of item</param>
        public static void Set<T>(T o, string sid, string key) where T : class
        {
            if (!Configured)
            {
                Configure();
            }

            TimeSpan slidingExpiration = DateTime.UtcNow.AddMinutes(Timeout) - DateTime.UtcNow;
            HttpRuntime.Cache.Insert
            (
                MakeKey(sid, key),
                o,
                null,
                Cache.NoAbsoluteExpiration,
                slidingExpiration,
                Priority,
                delegate
                {
                    if (LogEvents)
                        log.DebugFormat("HttpRuntime.Cache: parameter {0} has expired and was removed from cache for session {1}", key, sid);
                }
            );
            if (LogEvents)
                log.InfoFormat("HttpRuntime.Cache: parameter {0} was added for session {1}", key, sid);
        }

        /// <summary>
        /// Remove item from cache
        /// </summary>
        /// <param name="sid">User provided session id (HTTP Session!)</param>
        /// <param name="key">Name of cached item</param>
        public static void Unset(string sid, string key)
        {
            if (LogEvents)
                log.InfoFormat("HttpRuntime.Cache: parameter {0} was removed for session {1}", key, sid);
            Clear(MakeKey(sid, key));
        }

        /// <summary>
        /// Remove item from cache
        /// </summary>
        /// <param name="key">Name of cached item</param>
        private static void Clear(string key)
        {
            HttpRuntime.Cache.Remove(key);
        }

        /// <summary>
        /// Check for item in cache
        /// </summary>
        /// <param name="sid">User provided session id (HTTP Session!)</param>
        /// <param name="key">Name of cached item</param>
        /// <returns></returns>
        public static bool Exists(string sid, string key)
        {
            return Exists(MakeKey(sid, key));
        }

        /// <summary>
        /// Check for item in cache
        /// </summary>
        /// <param name="key">Name of cached item</param>
        /// <returns></returns>
        private static bool Exists(string key)
        {
            return HttpRuntime.Cache[key] != null;
        }

        /// <summary>
        /// Retrieve cached item
        /// </summary>
        /// <typeparam name="T">Type of cached item</typeparam>
        /// <param name="sid">User provided session id (HTTP Session!)</param>
        /// <param name="key">Name of cached item</param>
        /// <returns>Cached item as type</returns>
        public static T Get<T>(string sid, string key) where T : class
        {
            var value = Get<T>(MakeKey(sid, key));
            if (LogEvents)
                log.InfoFormat("HttpRuntime.Cache: parameter {0} retrieved for session {1}: {2}", key, sid, value == null ? "null" : value.GetHashCode().ToString());

            return value;
        }

        /// <summary>
        /// Retrieve cached item
        /// </summary>
        /// <typeparam name="T">Type of cached item</typeparam>
        /// <param name="key">Name of cached item</param>
        /// <returns>Cached item as type</returns>
        private static T Get<T>(string key) where T : class
        {
            try
            {
                return (T)HttpRuntime.Cache[key];
            }
            catch
            {
                return null;
            }
        }

        private static string MakeKey(string sid, string key)
        {
            return sid + SEPARATOR + key;
        }

        /// <summary>
        /// Key is maked of session id and key provided and is separated by this value
        /// </summary>
        private const string SEPARATOR = "_";

        /// <summary>
        /// Means that this cache entry is for all users, not specific logged one
        /// </summary>
        public const string EVERYONE = "EVERYONE";
    }
}