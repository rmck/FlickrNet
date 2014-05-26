// Type: FlickrNet.FlickrException
// Assembly: FlickrNet, Version=3.10.0.0, Culture=neutral, PublicKeyToken=2491df59efa5d132
// MVID: 8B8C756F-D5B2-4047-8A8B-B1C1E877A2EF
// Assembly location: D:\RMCK\vs\2014.05.24_FlickrAPI\FlickrNet.dll

using System;

namespace FlickrNet
{
  public class FlickrException : Exception
  {
    public FlickrException()
    {
    }

    public FlickrException(string message)
      : base(message)
    {
    }

    public FlickrException(string message, Exception innerException)
      : base(message, innerException)
    {
    }
  }
}
