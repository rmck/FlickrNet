// Type: FlickrNet.Tag
// Assembly: FlickrNet, Version=3.10.0.0, Culture=neutral, PublicKeyToken=2491df59efa5d132
// MVID: 8B8C756F-D5B2-4047-8A8B-B1C1E877A2EF
// Assembly location: D:\RMCK\vs\2014.05.24_FlickrAPI\FlickrNet.dll

using System.Xml;

namespace FlickrNet
{
  public sealed class Tag : IFlickrParsable
  {
    public string TagName { get; set; }

    public int Count { get; set; }

    void IFlickrParsable.Load(XmlReader reader)
    {
      while (reader.MoveToNextAttribute())
      {
        switch (reader.LocalName)
        {
          case "count":
            this.Count = reader.ReadContentAsInt();
            continue;
          default:
            continue;
        }
      }
      reader.Read();
      this.TagName = reader.ReadContentAsString();
      reader.Read();
    }
  }
}
