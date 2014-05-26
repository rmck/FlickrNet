// Type: FlickrNet.FlickrApiException
// Assembly: FlickrNet, Version=3.10.0.0, Culture=neutral, PublicKeyToken=2491df59efa5d132
// MVID: 8B8C756F-D5B2-4047-8A8B-B1C1E877A2EF
// Assembly location: D:\RMCK\vs\2014.05.24_FlickrAPI\FlickrNet.dll

using System;

namespace FlickrNet
{
  [Serializable]
  public class FlickrApiException : FlickrException
  {
    public int Code { get; set; }

    public string OriginalMessage { get; set; }

    public override string Message
    {
      get
      {
        return string.Concat(new object[4]
        {
          (object) this.OriginalMessage,
          (object) " (",
          (object) this.Code,
          (object) ")"
        });
      }
    }

    public FlickrApiException(int code, string message)
    {
      this.Code = code;
      this.OriginalMessage = message;
    }

    public FlickrApiException()
    {
    }

    public FlickrApiException(string message)
      : base(message)
    {
    }

    public FlickrApiException(string message, Exception innerException)
      : base(message, innerException)
    {
    }
  }
}
