// Type: FlickrNet.RawTag
// Assembly: FlickrNet, Version=3.10.0.0, Culture=neutral, PublicKeyToken=2491df59efa5d132
// MVID: 8B8C756F-D5B2-4047-8A8B-B1C1E877A2EF
// Assembly location: D:\RMCK\vs\2014.05.24_FlickrAPI\FlickrNet.dll

using System.Collections.ObjectModel;
using System.Xml;

namespace FlickrNet
{
  public sealed class RawTag : IFlickrParsable
  {
    public Collection<string> RawTags { get; set; }

    public string CleanTag { get; set; }

    public RawTag()
    {
      this.RawTags = new Collection<string>();
    }

    void IFlickrParsable.Load(XmlReader reader)
    {
      while (reader.MoveToNextAttribute())
      {
        switch (reader.LocalName)
        {
          case "clean":
            this.CleanTag = reader.ReadContentAsString();
            continue;
          default:
            continue;
        }
      }
      reader.Read();
      while (reader.LocalName == "raw")
        this.RawTags.Add(reader.ReadElementContentAsString());
      reader.Read();
    }
  }
}
