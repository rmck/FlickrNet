// Type: FlickrNet.Blog
// Assembly: FlickrNet, Version=3.10.0.0, Culture=neutral, PublicKeyToken=2491df59efa5d132
// MVID: 8B8C756F-D5B2-4047-8A8B-B1C1E877A2EF
// Assembly location: D:\RMCK\vs\2014.05.24_FlickrAPI\FlickrNet.dll

using System.Xml;

namespace FlickrNet
{
  public sealed class Blog : IFlickrParsable
  {
    public string BlogId { get; set; }

    public string BlogName { get; set; }

    public string BlogUrl { get; set; }

    public bool NeedsPassword { get; set; }

    public string Service { get; set; }

    void IFlickrParsable.Load(XmlReader reader)
    {
      if (!(reader.LocalName != "blog"))
        ;
      while (reader.MoveToNextAttribute())
      {
        switch (reader.LocalName)
        {
          case "id":
            this.BlogId = reader.Value;
            continue;
          case "name":
            this.BlogName = reader.Value;
            continue;
          case "url":
            this.BlogUrl = reader.Value;
            continue;
          case "needspassword":
            this.NeedsPassword = reader.Value == "1";
            continue;
          case "service":
            this.Service = reader.Value;
            continue;
          default:
            continue;
        }
      }
    }
  }
}
