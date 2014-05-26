// Type: FlickrNet.PlaceInfo
// Assembly: FlickrNet, Version=3.10.0.0, Culture=neutral, PublicKeyToken=2491df59efa5d132
// MVID: 8B8C756F-D5B2-4047-8A8B-B1C1E877A2EF
// Assembly location: D:\RMCK\vs\2014.05.24_FlickrAPI\FlickrNet.dll

using System;
using System.Xml;

namespace FlickrNet
{
  public sealed class PlaceInfo : IFlickrParsable
  {
    public string PlaceId { get; set; }

    public string PlaceUrl { get; set; }

    public string PlaceFlickrUrl
    {
      get
      {
        return "https://www.flickr.com/places" + this.PlaceUrl;
      }
    }

    public PlaceType PlaceType { get; set; }

    public string WoeId { get; set; }

    public string WoeName { get; set; }

    public string Description { get; set; }

    public double Latitude { get; set; }

    public double Longitude { get; set; }

    public GeoAccuracy? Accuracy { get; set; }

    public GeoContext? Context { get; set; }

    public string TimeZone { get; set; }

    public bool HasShapeData { get; set; }

    public Place Neighbourhood { get; set; }

    public Place Locality { get; set; }

    public Place County { get; set; }

    public Place Region { get; set; }

    public Place Country { get; set; }

    public ShapeData ShapeData { get; set; }

    void IFlickrParsable.Load(XmlReader reader)
    {
      this.LoadAttributes(reader);
      this.LoadElements(reader);
    }

    private void LoadElements(XmlReader reader)
    {
      if (reader.NodeType == XmlNodeType.EndElement && reader.Name == "photo" || reader.NodeType == XmlNodeType.Element && reader.Name == "geoperms")
        return;
      while (reader.NodeType != XmlNodeType.EndElement)
      {
        switch (reader.LocalName)
        {
          case "neighbourhood":
            this.Neighbourhood = new Place();
            this.Neighbourhood.Load(reader);
            continue;
          case "locality":
            this.Locality = new Place();
            this.Locality.Load(reader);
            continue;
          case "county":
            this.County = new Place();
            this.County.Load(reader);
            continue;
          case "region":
            this.Region = new Place();
            this.Region.Load(reader);
            continue;
          case "country":
            this.Country = new Place();
            this.Country.Load(reader);
            continue;
          case "shapedata":
            this.ShapeData = new ShapeData();
            this.ShapeData.Load(reader);
            continue;
          default:
            reader.Skip();
            continue;
        }
      }
      reader.Read();
    }

    private void LoadAttributes(XmlReader reader)
    {
      while (reader.MoveToNextAttribute())
      {
        switch (reader.LocalName)
        {
          case "name":
            this.Description = reader.Value;
            continue;
          case "place_id":
            this.PlaceId = reader.Value;
            continue;
          case "place_url":
            this.PlaceUrl = reader.Value;
            continue;
          case "place_type_id":
            this.PlaceType = (PlaceType) reader.ReadContentAsInt();
            continue;
          case "place_type":
            this.PlaceType = (PlaceType) Enum.Parse(typeof (PlaceType), reader.Value, true);
            continue;
          case "woeid":
            this.WoeId = reader.Value;
            continue;
          case "woe_name":
            this.WoeName = reader.Value;
            continue;
          case "latitude":
            this.Latitude = reader.ReadContentAsDouble();
            continue;
          case "longitude":
            this.Longitude = reader.ReadContentAsDouble();
            continue;
          case "accuracy":
            this.Accuracy = new GeoAccuracy?((GeoAccuracy) reader.ReadContentAsInt());
            continue;
          case "context":
            this.Context = new GeoContext?((GeoContext) reader.ReadContentAsInt());
            continue;
          case "timezone":
            this.TimeZone = reader.Value;
            continue;
          case "has_shapedata":
            this.HasShapeData = reader.Value == "1";
            continue;
          default:
            continue;
        }
      }
      reader.Read();
    }
  }
}
