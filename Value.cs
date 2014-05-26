// Type: FlickrNet.Value
// Assembly: FlickrNet, Version=3.10.0.0, Culture=neutral, PublicKeyToken=2491df59efa5d132
// MVID: 8B8C756F-D5B2-4047-8A8B-B1C1E877A2EF
// Assembly location: D:\RMCK\vs\2014.05.24_FlickrAPI\FlickrNet.dll

using System;
using System.Xml;

namespace FlickrNet
{
  public sealed class Value : IFlickrParsable
  {
    public int Usage { get; set; }

    public string NamespaceName { get; set; }

    public string PredicateName { get; set; }

    public string ValueText { get; set; }

    public DateTime? DateFirstAdded { get; set; }

    public DateTime? DateLastUsed { get; set; }

    public string MachineTag
    {
      get
      {
        return this.NamespaceName + ":" + this.PredicateName + "=" + this.ValueText;
      }
    }

    void IFlickrParsable.Load(XmlReader reader)
    {
      while (reader.MoveToNextAttribute())
      {
        switch (reader.LocalName)
        {
          case "usage":
            this.Usage = reader.ReadContentAsInt();
            continue;
          case "predicate":
            this.PredicateName = reader.Value;
            continue;
          case "namespace":
            this.NamespaceName = reader.Value;
            continue;
          case "first_added":
            this.DateFirstAdded = new DateTime?(UtilityMethods.UnixTimestampToDate(reader.Value));
            continue;
          case "last_added":
            this.DateLastUsed = new DateTime?(UtilityMethods.UnixTimestampToDate(reader.Value));
            continue;
          default:
            continue;
        }
      }
      reader.Read();
      if (reader.NodeType == XmlNodeType.Text)
        this.ValueText = reader.ReadContentAsString();
      reader.Read();
    }
  }
}
