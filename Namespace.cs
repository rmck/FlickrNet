﻿// Type: FlickrNet.Namespace
// Assembly: FlickrNet, Version=3.10.0.0, Culture=neutral, PublicKeyToken=2491df59efa5d132
// MVID: 8B8C756F-D5B2-4047-8A8B-B1C1E877A2EF
// Assembly location: D:\RMCK\vs\2014.05.24_FlickrAPI\FlickrNet.dll

using System.Xml;

namespace FlickrNet
{
  public sealed class Namespace : IFlickrParsable
  {
    public string NamespaceName { get; set; }

    public int Usage { get; set; }

    public int Predicates { get; set; }

    void IFlickrParsable.Load(XmlReader reader)
    {
      while (reader.MoveToNextAttribute())
      {
        switch (reader.LocalName)
        {
          case "usage":
            this.Usage = reader.ReadContentAsInt();
            continue;
          case "predicates":
            this.Predicates = reader.ReadContentAsInt();
            continue;
          default:
            continue;
        }
      }
      reader.Read();
      if (reader.NodeType == XmlNodeType.Text)
        this.NamespaceName = reader.ReadContentAsString();
      reader.Read();
    }
  }
}
