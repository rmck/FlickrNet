﻿// Type: FlickrNet.FlickrWebException
// Assembly: FlickrNet, Version=3.10.0.0, Culture=neutral, PublicKeyToken=2491df59efa5d132
// MVID: 8B8C756F-D5B2-4047-8A8B-B1C1E877A2EF
// Assembly location: D:\RMCK\vs\2014.05.24_FlickrAPI\FlickrNet.dll

using System;

namespace FlickrNet
{
  [Serializable]
  public class FlickrWebException : FlickrException
  {
    public FlickrWebException()
    {
    }

    public FlickrWebException(string message)
      : base(message)
    {
    }

    public FlickrWebException(string message, Exception innerException)
      : base(message, innerException)
    {
    }
  }
}
