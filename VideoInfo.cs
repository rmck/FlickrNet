// Type: FlickrNet.VideoInfo
// Assembly: FlickrNet, Version=3.10.0.0, Culture=neutral, PublicKeyToken=2491df59efa5d132
// MVID: 8B8C756F-D5B2-4047-8A8B-B1C1E877A2EF
// Assembly location: D:\RMCK\vs\2014.05.24_FlickrAPI\FlickrNet.dll

using System.Xml;

namespace FlickrNet
{
  public sealed class VideoInfo : IFlickrParsable
  {
    public bool Ready { get; set; }

    public bool Failed { get; set; }

    public bool Pending { get; set; }

    public int Duration { get; set; }

    public int Width { get; set; }

    public int Height { get; set; }

    void IFlickrParsable.Load(XmlReader reader)
    {
      while (reader.MoveToNextAttribute())
      {
        switch (reader.LocalName)
        {
          case "ready":
            this.Ready = reader.Value == "1";
            continue;
          case "failed":
            this.Failed = reader.Value == "1";
            continue;
          case "pending":
            this.Pending = reader.Value == "1";
            continue;
          case "duration":
            this.Duration = !string.IsNullOrEmpty(reader.Value) ? reader.ReadContentAsInt() : -1;
            continue;
          case "width":
            this.Width = !string.IsNullOrEmpty(reader.Value) ? reader.ReadContentAsInt() : -1;
            continue;
          case "height":
            this.Height = !string.IsNullOrEmpty(reader.Value) ? reader.ReadContentAsInt() : -1;
            continue;
          default:
            continue;
        }
      }
      reader.Read();
    }
  }
}
