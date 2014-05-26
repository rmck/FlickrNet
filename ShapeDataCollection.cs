// Type: FlickrNet.ShapeDataCollection
// Assembly: FlickrNet, Version=3.10.0.0, Culture=neutral, PublicKeyToken=2491df59efa5d132
// MVID: 8B8C756F-D5B2-4047-8A8B-B1C1E877A2EF
// Assembly location: D:\RMCK\vs\2014.05.24_FlickrAPI\FlickrNet.dll

using System.Collections.ObjectModel;
using System.Xml;

namespace FlickrNet
{
  public sealed class ShapeDataCollection : Collection<ShapeData>, IFlickrParsable
  {
    public string WoeId { get; set; }

    public string PlaceId { get; set; }

    public PlaceType PlaceType { get; set; }

    public int Total { get; set; }

    public int Page { get; set; }

    public int PerPage { get; set; }

    public int Pages { get; set; }

    void IFlickrParsable.Load(XmlReader reader)
    {
      if (!(reader.LocalName != "shapes"))
        ;
      while (reader.MoveToNextAttribute())
      {
        switch (reader.LocalName)
        {
          case "page":
            this.Page = reader.ReadContentAsInt();
            continue;
          case "total":
            this.Total = reader.ReadContentAsInt();
            continue;
          case "pages":
            this.Pages = reader.ReadContentAsInt();
            continue;
          case "per_page":
          case "perpage":
            this.PerPage = reader.ReadContentAsInt();
            continue;
          case "woe_id":
            this.WoeId = reader.Value;
            continue;
          case "place_id":
            this.PlaceId = reader.Value;
            continue;
          case "place_type_id":
            this.PlaceType = (PlaceType) reader.ReadContentAsInt();
            continue;
          default:
            continue;
        }
      }
      reader.Read();
      while (reader.LocalName == "shape")
      {
        ShapeData shapeData = new ShapeData();
        shapeData.Load(reader);
        this.Add(shapeData);
      }
      reader.Skip();
    }
  }
}
