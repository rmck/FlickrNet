// Type: FlickrNet.PopularPhoto
// Assembly: FlickrNet, Version=3.10.0.0, Culture=neutral, PublicKeyToken=2491df59efa5d132
// MVID: 8B8C756F-D5B2-4047-8A8B-B1C1E877A2EF
// Assembly location: D:\RMCK\vs\2014.05.24_FlickrAPI\FlickrNet.dll

using System;
using System.Globalization;
using System.Xml;

namespace FlickrNet
{
  public class PopularPhoto : Photo, IFlickrParsable
  {
    public int StatViews { get; set; }

    public int StatComments { get; set; }

    public int StatFavorites { get; set; }

    void IFlickrParsable.Load(XmlReader reader)
    {
      this.Load(reader, false);
      if (reader.LocalName == "photo")
        return;
      if (!(reader.LocalName != "stats"))
        ;
      while (reader.MoveToNextAttribute())
      {
        switch (reader.LocalName)
        {
          case "views":
            this.StatViews = int.Parse(reader.Value, (IFormatProvider) NumberFormatInfo.InvariantInfo);
            continue;
          case "comments":
            this.StatComments = int.Parse(reader.Value, (IFormatProvider) NumberFormatInfo.InvariantInfo);
            continue;
          case "favorites":
            this.StatFavorites = int.Parse(reader.Value, (IFormatProvider) NumberFormatInfo.InvariantInfo);
            continue;
          default:
            continue;
        }
      }
      reader.Read();
      if (reader.LocalName == "description")
        this.Description = reader.ReadElementContentAsString();
      reader.Skip();
    }
  }
}
