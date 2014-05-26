// Type: FlickrNet.AllContexts
// Assembly: FlickrNet, Version=3.10.0.0, Culture=neutral, PublicKeyToken=2491df59efa5d132
// MVID: 8B8C756F-D5B2-4047-8A8B-B1C1E877A2EF
// Assembly location: D:\RMCK\vs\2014.05.24_FlickrAPI\FlickrNet.dll

using System.Collections.ObjectModel;
using System.Xml;

namespace FlickrNet
{
  public sealed class AllContexts : IFlickrParsable
  {
    public Collection<ContextSet> Sets { get; set; }

    public Collection<ContextGroup> Groups { get; set; }

    public AllContexts()
    {
      this.Sets = new Collection<ContextSet>();
      this.Groups = new Collection<ContextGroup>();
    }

    void IFlickrParsable.Load(XmlReader reader)
    {
      while (reader.NodeType != XmlNodeType.EndElement)
      {
        switch (reader.LocalName)
        {
          case "set":
            this.Sets.Add(new ContextSet()
            {
              PhotosetId = reader.GetAttribute("id"),
              Title = reader.GetAttribute("title")
            });
            reader.Read();
            continue;
          case "pool":
            this.Groups.Add(new ContextGroup()
            {
              GroupId = reader.GetAttribute("id"),
              Title = reader.GetAttribute("title")
            });
            reader.Read();
            continue;
          default:
            continue;
        }
      }
    }
  }
}
