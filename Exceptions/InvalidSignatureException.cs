// Type: FlickrNet.Exceptions.InvalidSignatureException
// Assembly: FlickrNet, Version=3.10.0.0, Culture=neutral, PublicKeyToken=2491df59efa5d132
// MVID: 8B8C756F-D5B2-4047-8A8B-B1C1E877A2EF
// Assembly location: D:\RMCK\vs\2014.05.24_FlickrAPI\FlickrNet.dll

using FlickrNet;

namespace FlickrNet.Exceptions
{
  public class InvalidSignatureException : FlickrApiException
  {
    internal InvalidSignatureException(string message)
      : base(96, message)
    {
    }
  }
}
