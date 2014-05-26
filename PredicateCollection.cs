// Type: FlickrNet.PredicateCollection
// Assembly: FlickrNet, Version=3.10.0.0, Culture=neutral, PublicKeyToken=2491df59efa5d132
// MVID: 8B8C756F-D5B2-4047-8A8B-B1C1E877A2EF
// Assembly location: D:\RMCK\vs\2014.05.24_FlickrAPI\FlickrNet.dll

using System;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Xml;

namespace FlickrNet
{
  public sealed class PredicateCollection : Collection<Predicate>, IFlickrParsable
  {
    public int Total { get; set; }

    public int Page { get; set; }

    public int PerPage { get; set; }

    public int Pages { get; set; }

    void IFlickrParsable.Load(XmlReader reader)
    {
      if (!(reader.LocalName != "predicates"))
        ;
      while (reader.MoveToNextAttribute())
      {
        switch (reader.LocalName)
        {
          case "page":
            this.Page = int.Parse(reader.Value, (IFormatProvider) NumberFormatInfo.InvariantInfo);
            continue;
          case "total":
            this.Total = int.Parse(reader.Value, (IFormatProvider) NumberFormatInfo.InvariantInfo);
            continue;
          case "pages":
            this.Pages = int.Parse(reader.Value, (IFormatProvider) NumberFormatInfo.InvariantInfo);
            continue;
          case "per_page":
          case "perpage":
            this.PerPage = int.Parse(reader.Value, (IFormatProvider) NumberFormatInfo.InvariantInfo);
            continue;
          default:
            continue;
        }
      }
      reader.Read();
      while (reader.LocalName == "predicate")
      {
        Predicate predicate = new Predicate();
        predicate.Load(reader);
        this.Add(predicate);
      }
      reader.Skip();
    }
  }
}
