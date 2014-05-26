// Type: FlickrNet.CollectionCollection
// Assembly: FlickrNet, Version=3.10.0.0, Culture=neutral, PublicKeyToken=2491df59efa5d132
// MVID: 8B8C756F-D5B2-4047-8A8B-B1C1E877A2EF
// Assembly location: D:\RMCK\vs\2014.05.24_FlickrAPI\FlickrNet.dll

using System.Collections.ObjectModel;
using System.Xml;

namespace FlickrNet
{
  public sealed class CollectionCollection : Collection<Collection>, IFlickrParsable
  {
    void IFlickrParsable.Load(XmlReader reader)
    {
      int num = reader.LocalName != "collections" ? 1 : 0;
      reader.Read();
      while (reader.LocalName == "collection")
      {
        Collection collection = new Collection();
        collection.Load(reader);
        this.Add(collection);
      }
      reader.Skip();
    }
  }
}
