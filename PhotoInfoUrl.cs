// Type: FlickrNet.PhotoInfoUrl
// Assembly: FlickrNet, Version=3.10.0.0, Culture=neutral, PublicKeyToken=2491df59efa5d132
// MVID: 8B8C756F-D5B2-4047-8A8B-B1C1E877A2EF
// Assembly location: D:\RMCK\vs\2014.05.24_FlickrAPI\FlickrNet.dll

using System.Xml;

namespace FlickrNet
{
  public sealed class PhotoInfoUrl : IFlickrParsable
  {
    public string Url { get; set; }

    public string UrlType { get; set; }

    void IFlickrParsable.Load(XmlReader reader)
    {
      if (!(reader.LocalName != "url"))
        ;
      while (reader.MoveToNextAttribute())
      {
        switch (reader.LocalName)
        {
          case "type":
            this.UrlType = reader.Value;
            continue;
          default:
            continue;
        }
      }
      reader.Read();
      this.Url = reader.ReadContentAsString();
      if (this.Url.Contains("www.flickr.com"))
        this.Url = this.Url.Replace("http://", "https://");
      reader.Skip();
    }
  }
}
