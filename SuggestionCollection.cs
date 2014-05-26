// Type: FlickrNet.SuggestionCollection
// Assembly: FlickrNet, Version=3.10.0.0, Culture=neutral, PublicKeyToken=2491df59efa5d132
// MVID: 8B8C756F-D5B2-4047-8A8B-B1C1E877A2EF
// Assembly location: D:\RMCK\vs\2014.05.24_FlickrAPI\FlickrNet.dll

using System;
using System.Collections.ObjectModel;
using System.Xml;

namespace FlickrNet
{
  public sealed class SuggestionCollection : Collection<Suggestion>, IFlickrParsable
  {
    public int Total { get; set; }

    public int PerPage { get; set; }

    public int Page { get; set; }

    public int Pages
    {
      get
      {
        return (int) Math.Ceiling(1.0 * (double) this.Total / (double) this.PerPage);
      }
      set
      {
      }
    }

    void IFlickrParsable.Load(XmlReader reader)
    {
      if (reader == null)
        throw new ArgumentNullException("reader");
      if (reader.LocalName != "suggestions")
        return;
      while (reader.MoveToNextAttribute())
      {
        switch (reader.LocalName)
        {
          case "total":
            this.Total = reader.ReadContentAsInt();
            continue;
          case "page":
            this.Page = reader.ReadContentAsInt();
            continue;
          case "per_page":
            this.PerPage = reader.ReadContentAsInt();
            continue;
          default:
            continue;
        }
      }
      reader.Read();
      while (reader.LocalName == "suggestion")
      {
        Suggestion suggestion = new Suggestion();
        suggestion.Load(reader);
        this.Add(suggestion);
      }
    }
  }
}
