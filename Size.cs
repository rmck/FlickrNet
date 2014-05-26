// Type: FlickrNet.Size
// Assembly: FlickrNet, Version=3.10.0.0, Culture=neutral, PublicKeyToken=2491df59efa5d132
// MVID: 8B8C756F-D5B2-4047-8A8B-B1C1E877A2EF
// Assembly location: D:\RMCK\vs\2014.05.24_FlickrAPI\FlickrNet.dll

using System.Xml;

namespace FlickrNet
{
  public sealed class Size : IFlickrParsable
  {
    public string Label { get; set; }

    public int Width { get; set; }

    public int Height { get; set; }

    public string Source { get; set; }

    public string Url { get; set; }

    public MediaType MediaType { get; set; }

    void IFlickrParsable.Load(XmlReader reader)
    {
      while (reader.MoveToNextAttribute())
      {
        switch (reader.LocalName)
        {
          case "label":
            this.Label = reader.Value;
            continue;
          case "width":
            this.Width = reader.ReadContentAsInt();
            continue;
          case "height":
            this.Height = reader.ReadContentAsInt();
            continue;
          case "source":
            this.Source = reader.Value;
            continue;
          case "url":
            this.Url = reader.Value;
            continue;
          case "media":
            this.MediaType = reader.Value == "photo" ? MediaType.Photos : MediaType.Videos;
            continue;
          default:
            continue;
        }
      }
      reader.Read();
    }
  }
}
