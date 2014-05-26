// Type: FlickrNet.Place
// Assembly: FlickrNet, Version=3.10.0.0, Culture=neutral, PublicKeyToken=2491df59efa5d132
// MVID: 8B8C756F-D5B2-4047-8A8B-B1C1E877A2EF
// Assembly location: D:\RMCK\vs\2014.05.24_FlickrAPI\FlickrNet.dll

using System;
using System.Xml;

namespace FlickrNet
{
  public sealed class Place : IFlickrParsable
  {
    public string PlaceId { get; set; }

    public string PlaceUrl { get; set; }

    public PlaceType PlaceType { get; set; }

    public string WoeId { get; set; }

    public string WoeName { get; set; }

    public string Description { get; set; }

    public double Latitude { get; set; }

    public double Longitude { get; set; }

    public string TimeZone { get; set; }

    public int? PhotoCount { get; set; }

    void IFlickrParsable.Load(XmlReader reader)
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
          case "timezone":
            this.TimeZone = reader.Value;
            continue;
          case "photo_count":
            this.PhotoCount = new int?(reader.ReadContentAsInt());
            continue;
          default:
            continue;
        }
      }
      reader.Read();
      while (reader.NodeType != XmlNodeType.EndElement)
      {
        if (reader.NodeType == XmlNodeType.Text)
        {
          this.Description = reader.ReadContentAsString();
        }
        else
        {
          string localName = reader.LocalName;
        }
      }
      reader.Read();
    }
  }
}
