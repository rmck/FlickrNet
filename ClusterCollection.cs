// Type: FlickrNet.ClusterCollection
// Assembly: FlickrNet, Version=3.10.0.0, Culture=neutral, PublicKeyToken=2491df59efa5d132
// MVID: 8B8C756F-D5B2-4047-8A8B-B1C1E877A2EF
// Assembly location: D:\RMCK\vs\2014.05.24_FlickrAPI\FlickrNet.dll

using System.Collections.ObjectModel;
using System.Xml;

namespace FlickrNet
{
  public sealed class ClusterCollection : Collection<Cluster>, IFlickrParsable
  {
    public string SourceTag { get; set; }

    public int TotalClusters { get; set; }

    void IFlickrParsable.Load(XmlReader reader)
    {
      while (reader.MoveToNextAttribute())
      {
        switch (reader.LocalName)
        {
          case "source":
            this.SourceTag = reader.Value;
            continue;
          case "total":
            this.TotalClusters = reader.ReadContentAsInt();
            continue;
          default:
            continue;
        }
      }
      reader.Read();
      while (reader.LocalName == "cluster")
      {
        Cluster cluster = new Cluster();
        cluster.Load(reader);
        cluster.SourceTag = this.SourceTag;
        this.Add(cluster);
      }
      reader.Skip();
    }
  }
}
