// Type: FlickrNet.EchoResponseDictionary
// Assembly: FlickrNet, Version=3.10.0.0, Culture=neutral, PublicKeyToken=2491df59efa5d132
// MVID: 8B8C756F-D5B2-4047-8A8B-B1C1E877A2EF
// Assembly location: D:\RMCK\vs\2014.05.24_FlickrAPI\FlickrNet.dll

using System;
using System.Collections.Generic;
using System.Xml;

namespace FlickrNet
{
  [Serializable]
  public sealed class EchoResponseDictionary : Dictionary<string, string>, IFlickrParsable
  {
    void IFlickrParsable.Load(XmlReader reader)
    {
      while (reader.NodeType != XmlNodeType.None && reader.NodeType != XmlNodeType.EndElement)
        this.Add(reader.Name, reader.ReadElementContentAsString());
    }
  }
}
