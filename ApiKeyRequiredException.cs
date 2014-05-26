// Type: FlickrNet.ApiKeyRequiredException
// Assembly: FlickrNet, Version=3.10.0.0, Culture=neutral, PublicKeyToken=2491df59efa5d132
// MVID: 8B8C756F-D5B2-4047-8A8B-B1C1E877A2EF
// Assembly location: D:\RMCK\vs\2014.05.24_FlickrAPI\FlickrNet.dll

using System;

namespace FlickrNet
{
  [Serializable]
  public class ApiKeyRequiredException : FlickrException
  {
    public ApiKeyRequiredException()
      : base("API Key is required for all method calls")
    {
    }

    public ApiKeyRequiredException(string message)
      : base(message)
    {
    }

    public ApiKeyRequiredException(string message, Exception innerException)
      : base(message, innerException)
    {
    }
  }
}
