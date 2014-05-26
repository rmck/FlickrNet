// Type: FlickrNet.Stats
// Assembly: FlickrNet, Version=3.10.0.0, Culture=neutral, PublicKeyToken=2491df59efa5d132
// MVID: 8B8C756F-D5B2-4047-8A8B-B1C1E877A2EF
// Assembly location: D:\RMCK\vs\2014.05.24_FlickrAPI\FlickrNet.dll

using System;
using System.Globalization;
using System.Xml;

namespace FlickrNet
{
  public sealed class Stats : IFlickrParsable
  {
    public int Views { get; set; }

    public int Comments { get; set; }

    public int Favorites { get; set; }

    void IFlickrParsable.Load(XmlReader reader)
    {
      if (!(reader.LocalName != "stats"))
        ;
      while (reader.MoveToNextAttribute())
      {
        switch (reader.LocalName)
        {
          case "views":
            this.Views = !string.IsNullOrEmpty(reader.Value) ? int.Parse(reader.Value, (IFormatProvider) NumberFormatInfo.InvariantInfo) : 0;
            continue;
          case "comments":
            this.Comments = !string.IsNullOrEmpty(reader.Value) ? int.Parse(reader.Value, (IFormatProvider) NumberFormatInfo.InvariantInfo) : 0;
            continue;
          case "favorites":
            this.Favorites = !string.IsNullOrEmpty(reader.Value) ? int.Parse(reader.Value, (IFormatProvider) NumberFormatInfo.InvariantInfo) : 0;
            continue;
          default:
            continue;
        }
      }
      reader.Skip();
    }
  }
}
