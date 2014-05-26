// Type: FlickrNet.FlickrResult`1
// Assembly: FlickrNet, Version=3.10.0.0, Culture=neutral, PublicKeyToken=2491df59efa5d132
// MVID: 8B8C756F-D5B2-4047-8A8B-B1C1E877A2EF
// Assembly location: D:\RMCK\vs\2014.05.24_FlickrAPI\FlickrNet.dll

using System;

namespace FlickrNet
{
  public class FlickrResult<T>
  {
    private Exception error;

    public bool HasError { get; set; }

    public T Result { get; set; }

    public Exception Error
    {
      get
      {
        return this.error;
      }
      set
      {
        this.error = value;
        if (value == null)
        {
          this.HasError = false;
        }
        else
        {
          this.HasError = true;
          if (!(value is FlickrApiException))
            return;
          FlickrApiException flickrApiException = value as FlickrApiException;
          this.ErrorCode = flickrApiException.Code;
          this.ErrorMessage = flickrApiException.OriginalMessage;
        }
      }
    }

    public int ErrorCode { get; set; }

    public string ErrorMessage { get; set; }
  }
}
