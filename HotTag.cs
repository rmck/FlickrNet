// Type: FlickrNet.HotTag
// Assembly: FlickrNet, Version=3.10.0.0, Culture=neutral, PublicKeyToken=2491df59efa5d132
// MVID: 8B8C756F-D5B2-4047-8A8B-B1C1E877A2EF
// Assembly location: D:\RMCK\vs\2014.05.24_FlickrAPI\FlickrNet.dll

using System.Xml;

namespace FlickrNet
{
  public sealed class HotTag : IFlickrParsable
  {
    public string Tag { get; set; }

    public int Score { get; set; }

    void IFlickrParsable.Load(XmlReader reader)
    {
      while (reader.MoveToNextAttribute())
      {
        switch (reader.LocalName)
        {
          case "score":
            this.Score = reader.ReadContentAsInt();
            continue;
          default:
            continue;
        }
      }
      reader.Read();
      if (reader.NodeType == XmlNodeType.Text)
        this.Tag = reader.ReadContentAsString();
      reader.Read();
    }
  }
}
