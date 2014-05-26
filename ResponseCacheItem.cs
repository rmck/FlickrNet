// Type: FlickrNet.ResponseCacheItem
// Assembly: FlickrNet, Version=3.10.0.0, Culture=neutral, PublicKeyToken=2491df59efa5d132
// MVID: 8B8C756F-D5B2-4047-8A8B-B1C1E877A2EF
// Assembly location: D:\RMCK\vs\2014.05.24_FlickrAPI\FlickrNet.dll

using System;

namespace FlickrNet
{
  public sealed class ResponseCacheItem : ICacheItem
  {
    public Uri Url { get; set; }

    public string Response { get; set; }

    public DateTime CreationTime { get; set; }

    public long FileSize
    {
      get
      {
        return this.Response == null ? 0L : (long) this.Response.Length;
      }
    }

    public ResponseCacheItem(Uri url, string response, DateTime creationTime)
    {
      this.Url = url;
      this.Response = response;
      this.CreationTime = creationTime;
    }

    void ICacheItem.OnItemFlushed()
    {
    }
  }
}
