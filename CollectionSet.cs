// Type: FlickrNet.CollectionSet
// Assembly: FlickrNet, Version=3.10.0.0, Culture=neutral, PublicKeyToken=2491df59efa5d132
// MVID: 8B8C756F-D5B2-4047-8A8B-B1C1E877A2EF
// Assembly location: D:\RMCK\vs\2014.05.24_FlickrAPI\FlickrNet.dll

using System.Xml;

namespace FlickrNet
{
  public sealed class CollectionSet : IFlickrParsable
  {
    public string SetId { get; set; }

    public string Title { get; set; }

    public string Description { get; set; }

    void IFlickrParsable.Load(XmlReader reader)
    {
      if (!(reader.LocalName != "set"))
        ;
      while (reader.MoveToNextAttribute())
      {
        switch (reader.LocalName)
        {
          case "id":
            this.SetId = reader.Value;
            continue;
          case "title":
            this.Title = reader.Value;
            continue;
          case "description":
            this.Description = reader.Value;
            continue;
          default:
            continue;
        }
      }
      reader.Skip();
    }
  }
}
