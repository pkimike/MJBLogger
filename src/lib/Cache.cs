using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.Caching.Memory;

namespace MJBLogger
{
    public partial class MJBLog
    {
        private int cacheTimeToLiveInMinutes = Default.CacheTimeToLiveInMinutes;
        private MemoryCacheEntryOptions CacheTtl = new MemoryCacheEntryOptions().SetSlidingExpiration(TimeSpan.FromMinutes(Default.CacheTimeToLiveInMinutes));
        public int CacheTimeToLiveInMinutes
        {
            get
            {
                return cacheTimeToLiveInMinutes;
            }
            set
            {
                cacheTimeToLiveInMinutes = value;
                CacheTtl = new MemoryCacheEntryOptions().SetSlidingExpiration(TimeSpan.FromMinutes(cacheTimeToLiveInMinutes));
            }
        }

        private int cacheIndex = 0;
        private IMemoryCache cache;
        private bool cacheInitialized = false;
        private IMemoryCache Cache
        {
            get
            {
                if (!cacheInitialized)
                {
                    InitCache();
                }
                return cache;
            }
        }

        private bool cachedMode = false;

        /// <summary>
        /// When enabled, log messages will be stored in a StringBuilder object until <see cref="CachedMode"/> is disabled, at which time, all collected log messages will be written to the current log file. Enabling this property is useful if you would like to start logging but don't have the means to write to the file system
        /// </summary>
        public bool CachedMode
        {
            get
            {
                return cachedMode;
            }
            set
            {
                bool oldVal = cachedMode;
                cachedMode = value;

                switch (oldVal)
                {
                    case true:
                        if (!cachedMode)
                        {
                            string message = string.Empty;
                            for (int x=0; x<cacheIndex; x++)
                            {
                                if (cache.TryGetValue(x, out message))
                                {
                                    WriteMessage(message);
                                }
                            }
                            InitCache();
                        }
                        break;
                    case false:
                        if (cachedMode)
                        {
                            InitCache();
                        }
                        break;
                }
            }
        }

        private void CacheEntry(string message)
        {
            cache.Set(cacheIndex++, message, CacheTtl);
        }

        private void InitCache()
        {
            cache = new MemoryCache(new MemoryCacheOptions());
            cacheIndex = 0;
        }
    }
}
