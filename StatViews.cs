// Type: FlickrNet.StatViews
// Assembly: FlickrNet, Version=3.10.0.0, Culture=neutral, PublicKeyToken=2491df59efa5d132
// MVID: 8B8C756F-D5B2-4047-8A8B-B1C1E877A2EF
// Assembly location: D:\RMCK\vs\2014.05.24_FlickrAPI\FlickrNet.dll

using System;
using System.Globalization;
using System.Xml;

namespace FlickrNet
{
  public sealed class StatViews : IFlickrParsable
  {
    public int TotalViews { get; set; }

    public int PhotostreamViews { get; set; }

    public int PhotoViews { get; set; }

    public int PhotosetViews { get; set; }

    public int CollectionViews { get; set; }

    public int GalleryViews { get; set; }

    void IFlickrParsable.Load(XmlReader reader)
    {
      if (!(reader.LocalName != "stats"))
        ;
      while (reader.Read() && reader.LocalName != "stats")
      {
        switch (reader.LocalName)
        {
          case "total":
            this.TotalViews = int.Parse(reader.GetAttribute("views"), (IFormatProvider) NumberFormatInfo.InvariantInfo);
            continue;
          case "photos":
            this.PhotoViews = int.Parse(reader.GetAttribute("views"), (IFormatProvider) NumberFormatInfo.InvariantInfo);
            continue;
          case "photostream":
            this.PhotostreamViews = int.Parse(reader.GetAttribute("views"), (IFormatProvider) NumberFormatInfo.InvariantInfo);
            continue;
          case "sets":
            this.PhotosetViews = int.Parse(reader.GetAttribute("views"), (IFormatProvider) NumberFormatInfo.InvariantInfo);
            continue;
          case "collections":
            this.CollectionViews = int.Parse(reader.GetAttribute("views"), (IFormatProvider) NumberFormatInfo.InvariantInfo);
            continue;
          case "galleries":
            this.GalleryViews = int.Parse(reader.GetAttribute("views"), (IFormatProvider) NumberFormatInfo.InvariantInfo);
            continue;
          default:
            continue;
        }
      }
      reader.Skip();
    }
  }
}
