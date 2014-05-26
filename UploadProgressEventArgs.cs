// Type: FlickrNet.UploadProgressEventArgs
// Assembly: FlickrNet, Version=3.10.0.0, Culture=neutral, PublicKeyToken=2491df59efa5d132
// MVID: 8B8C756F-D5B2-4047-8A8B-B1C1E877A2EF
// Assembly location: D:\RMCK\vs\2014.05.24_FlickrAPI\FlickrNet.dll

using System;

namespace FlickrNet
{
  public class UploadProgressEventArgs : EventArgs
  {
    public long BytesSent { get; internal set; }

    public long TotalBytesToSend { get; internal set; }

    public bool UploadComplete
    {
      get
      {
        return this.ProcessPercentage == 100;
      }
    }

    public int ProcessPercentage
    {
      get
      {
        return Convert.ToInt32(this.BytesSent * 100L / this.TotalBytesToSend);
      }
    }

    internal UploadProgressEventArgs()
    {
    }

    internal UploadProgressEventArgs(long bytes, long totalBytes)
    {
      this.BytesSent = bytes;
      this.TotalBytesToSend = totalBytes;
    }
  }
}
