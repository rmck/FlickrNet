// Type: FlickrNet.ValueCollection
// Assembly: FlickrNet, Version=3.10.0.0, Culture=neutral, PublicKeyToken=2491df59efa5d132
// MVID: 8B8C756F-D5B2-4047-8A8B-B1C1E877A2EF
// Assembly location: D:\RMCK\vs\2014.05.24_FlickrAPI\FlickrNet.dll

using System;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Xml;

namespace FlickrNet
{
  public sealed class ValueCollection : Collection<Value>, IFlickrParsable
  {
    public string PredicateName { get; set; }

    public string NamespaceName { get; set; }

    public DateTime? DateAddedSince { get; set; }

    public int Total { get; set; }

    public int Page { get; set; }

    public int PerPage { get; set; }

    public int Pages { get; set; }

    void IFlickrParsable.Load(XmlReader reader)
    {
      if (!(reader.LocalName != "values"))
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
          case "namespace":
            this.NamespaceName = reader.Value;
            continue;
          case "predicate":
            this.PredicateName = reader.Value;
            continue;
          case "added_since":
            this.DateAddedSince = new DateTime?(UtilityMethods.UnixTimestampToDate(reader.Value));
            continue;
          default:
            continue;
        }
      }
      reader.Read();
      while (reader.LocalName == "value")
      {
        Value obj = new Value();
        obj.Load(reader);
        if (!string.IsNullOrEmpty(this.NamespaceName) && string.IsNullOrEmpty(obj.NamespaceName))
          obj.NamespaceName = this.NamespaceName;
        if (!string.IsNullOrEmpty(this.PredicateName) && string.IsNullOrEmpty(obj.PredicateName))
          obj.PredicateName = this.PredicateName;
        this.Add(obj);
      }
      reader.Skip();
    }
  }
}
