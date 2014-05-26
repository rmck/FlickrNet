// Type: FlickrNet.Collection
// Assembly: FlickrNet, Version=3.10.0.0, Culture=neutral, PublicKeyToken=2491df59efa5d132
// MVID: 8B8C756F-D5B2-4047-8A8B-B1C1E877A2EF
// Assembly location: D:\RMCK\vs\2014.05.24_FlickrAPI\FlickrNet.dll

using System.Collections.ObjectModel;
using System.Xml;

namespace FlickrNet
{
  public sealed class Collection : IFlickrParsable
  {
    private Collection<CollectionSet> subsets = new Collection<CollectionSet>();
    private Collection<Collection> subcollections = new Collection<Collection>();

    public string CollectionId { get; set; }

    public string Title { get; set; }

    public string Description { get; set; }

    public string IconLarge { get; set; }

    public string IconSmall { get; set; }

    public string Url { get; set; }

    public Collection<CollectionSet> Sets
    {
      get
      {
        return this.subsets;
      }
    }

    public Collection<Collection> Collections
    {
      get
      {
        return this.subcollections;
      }
    }

    void IFlickrParsable.Load(XmlReader reader)
    {
      if (!(reader.LocalName != "collection"))
        ;
      while (reader.MoveToNextAttribute())
      {
        switch (reader.LocalName)
        {
          case "id":
            this.CollectionId = reader.Value;
            continue;
          case "title":
            this.Title = reader.Value;
            continue;
          case "description":
            this.Description = reader.Value;
            continue;
          case "iconlarge":
            this.IconLarge = reader.Value;
            continue;
          case "iconsmall":
            this.IconSmall = reader.Value;
            continue;
          case "url":
            this.Url = reader.Value;
            continue;
          default:
            continue;
        }
      }
      reader.MoveToElement();
      if (reader.IsEmptyElement)
      {
        reader.Skip();
      }
      else
      {
        reader.Read();
        while (reader.NodeType == XmlNodeType.Element && (reader.LocalName == "collection" || reader.LocalName == "set"))
        {
          if (reader.LocalName == "collection")
          {
            Collection collection = new Collection();
            collection.Load(reader);
            this.subcollections.Add(collection);
          }
          else
          {
            CollectionSet collectionSet = new CollectionSet();
            collectionSet.Load(reader);
            this.subsets.Add(collectionSet);
          }
        }
        reader.Skip();
      }
    }
  }
}
