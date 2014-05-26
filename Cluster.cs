// Type: FlickrNet.Cluster
// Assembly: FlickrNet, Version=3.10.0.0, Culture=neutral, PublicKeyToken=2491df59efa5d132
// MVID: 8B8C756F-D5B2-4047-8A8B-B1C1E877A2EF
// Assembly location: D:\RMCK\vs\2014.05.24_FlickrAPI\FlickrNet.dll

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Xml;

namespace FlickrNet
{
  public sealed class Cluster : IFlickrParsable
  {
    public string SourceTag { get; set; }

    public int TotalTags { get; set; }

    public Collection<string> Tags { get; set; }

    public string ClusterId
    {
      get
      {
        if (this.Tags.Count >= 3)
        {
          return this.Tags[0] + "-" + this.Tags[1] + "-" + this.Tags[2];
        }
        else
        {
          List<string> list = new List<string>();
          foreach (string str in this.Tags)
            list.Add(str);
          return string.Join("-", list.ToArray());
        }
      }
    }

    public string ClusterUrl
    {
      get
      {
        return string.Format((IFormatProvider) CultureInfo.InvariantCulture, "https://www.flickr.com/photos/tags/{0}/clusters/{1}/", new object[2]
        {
          (object) this.SourceTag,
          (object) this.ClusterId
        });
      }
    }

    void IFlickrParsable.Load(XmlReader reader)
    {
      this.Tags = new Collection<string>();
      while (reader.MoveToNextAttribute())
      {
        switch (reader.LocalName)
        {
          case "total":
            this.TotalTags = reader.ReadContentAsInt();
            continue;
          default:
            continue;
        }
      }
      reader.Read();
      while (reader.LocalName == "tag")
        this.Tags.Add(reader.ReadElementContentAsString());
      reader.Read();
    }
  }
}
