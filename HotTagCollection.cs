// Type: FlickrNet.HotTagCollection
// Assembly: FlickrNet, Version=3.10.0.0, Culture=neutral, PublicKeyToken=2491df59efa5d132
// MVID: 8B8C756F-D5B2-4047-8A8B-B1C1E877A2EF
// Assembly location: D:\RMCK\vs\2014.05.24_FlickrAPI\FlickrNet.dll

using System.Collections.ObjectModel;
using System.Xml;

namespace FlickrNet
{
  public sealed class HotTagCollection : Collection<HotTag>, IFlickrParsable
  {
    public string Period { get; set; }

    public int TagCount { get; set; }

    void IFlickrParsable.Load(XmlReader reader)
    {
      while (reader.MoveToNextAttribute())
      {
        switch (reader.LocalName)
        {
          case "period":
            this.Period = reader.Value;
            continue;
          case "count":
            this.TagCount = reader.ReadContentAsInt();
            continue;
          default:
            continue;
        }
      }
      reader.Read();
      while (reader.LocalName == "tag")
      {
        HotTag hotTag = new HotTag();
        hotTag.Load(reader);
        this.Add(hotTag);
      }
      reader.Skip();
    }
  }
}
