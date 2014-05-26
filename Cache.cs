// Type: FlickrNet.Cache
// Assembly: FlickrNet, Version=3.10.0.0, Culture=neutral, PublicKeyToken=2491df59efa5d132
// MVID: 8B8C756F-D5B2-4047-8A8B-B1C1E877A2EF
// Assembly location: D:\RMCK\vs\2014.05.24_FlickrAPI\FlickrNet.dll

using System;
using System.IO;
using System.Security;

namespace FlickrNet
{
  public static class Cache
  {
    private static object lockObject = new object();
    private static long cacheSizeLimit = 52428800L;
    private static TimeSpan cachetimeout = new TimeSpan(0, 1, 0, 0, 0);
    private static PersistentCache responses;
    private static Cache.Tristate cacheDisabled;
    private static string cacheLocation;

    public static PersistentCache Responses
    {
      get
      {
        lock (Cache.lockObject)
        {
          if (Cache.responses == null)
            Cache.responses = new PersistentCache(Path.Combine(Cache.CacheLocation, "responseCache.dat"), (CacheItemPersister) new ResponseCacheItemPersister());
          return Cache.responses;
        }
      }
    }

    public static bool CacheDisabled
    {
      get
      {
        if (Cache.cacheDisabled == Cache.Tristate.Null && FlickrConfigurationManager.Settings != null)
          Cache.cacheDisabled = FlickrConfigurationManager.Settings.CacheDisabled ? Cache.Tristate.True : Cache.Tristate.False;
        if (Cache.cacheDisabled == Cache.Tristate.Null)
          Cache.cacheDisabled = Cache.Tristate.False;
        return Cache.cacheDisabled == Cache.Tristate.True;
      }
      set
      {
        Cache.cacheDisabled = value ? Cache.Tristate.True : Cache.Tristate.False;
      }
    }

    public static string CacheLocation
    {
      get
      {
        if (Cache.cacheLocation == null && FlickrConfigurationManager.Settings != null)
          Cache.cacheLocation = FlickrConfigurationManager.Settings.CacheLocation;
        if (Cache.cacheLocation == null)
        {
          try
          {
            Cache.cacheLocation = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "FlickrNet");
          }
          catch (SecurityException ex)
          {
            throw new CacheException("Unable to read default cache location. Please cacheLocation in configuration file or set manually in code");
          }
        }
        if (Cache.cacheLocation == null)
          throw new CacheException("Unable to determine cache location. Please set cacheLocation in configuration file or set manually in code");
        else
          return Cache.cacheLocation;
      }
      set
      {
        Cache.cacheLocation = value;
      }
    }

    internal static long CacheSizeLimit
    {
      get
      {
        return Cache.cacheSizeLimit;
      }
      set
      {
        Cache.cacheSizeLimit = value;
      }
    }

    public static TimeSpan CacheTimeout
    {
      get
      {
        return Cache.cachetimeout;
      }
      set
      {
        Cache.cachetimeout = value;
      }
    }

    static Cache()
    {
    }

    public static void FlushCache(Uri url)
    {
      Cache.Responses[url.AbsoluteUri] = (ICacheItem) null;
    }

    public static void FlushCache()
    {
      Cache.Responses.Flush();
    }

    private enum Tristate
    {
      Null,
      True,
      False,
    }
  }
}
