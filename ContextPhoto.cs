// Type: FlickrNet.ContextPhoto
// Assembly: FlickrNet, Version=3.10.0.0, Culture=neutral, PublicKeyToken=2491df59efa5d132
// MVID: 8B8C756F-D5B2-4047-8A8B-B1C1E877A2EF
// Assembly location: D:\RMCK\vs\2014.05.24_FlickrAPI\FlickrNet.dll

using System.Xml;

namespace FlickrNet
{
  public sealed class ContextPhoto : IFlickrParsable
  {
    public string PhotoId { get; set; }

    public string Secret { get; set; }

    public string Server { get; set; }

    public string Farm { get; set; }

    public string Title { get; set; }

    public string Url { get; set; }

    public string ThumbnailUrl { get; set; }

    public MediaType MediaType { get; set; }

    void IFlickrParsable.Load(XmlReader reader)
    {
      while (reader.MoveToNextAttribute())
      {
        switch (reader.LocalName)
        {
          case "id":
            this.PhotoId = reader.Value;
            continue;
          case "secret":
            this.Secret = reader.Value;
            continue;
          case "server":
            this.Server = reader.Value;
            continue;
          case "farm":
            this.Farm = reader.Value;
            continue;
          case "title":
            this.Title = reader.Value;
            continue;
          case "url":
            this.Url = "https://www.flickr.com" + reader.Value;
            continue;
          case "thumb":
            this.ThumbnailUrl = reader.Value;
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
