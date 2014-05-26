// Type: FlickrNet.PeoplePhotoCollection
// Assembly: FlickrNet, Version=3.10.0.0, Culture=neutral, PublicKeyToken=2491df59efa5d132
// MVID: 8B8C756F-D5B2-4047-8A8B-B1C1E877A2EF
// Assembly location: D:\RMCK\vs\2014.05.24_FlickrAPI\FlickrNet.dll

using System;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Xml;

namespace FlickrNet
{
  public sealed class PeoplePhotoCollection : Collection<Photo>, IFlickrParsable
  {
    public int Pages { get; set; }

    public int Total { get; set; }

    public int Page { get; set; }

    public bool HasNextPage { get; set; }

    public int PerPage { get; set; }

    void IFlickrParsable.Load(XmlReader reader)
    {
      if (!(reader.LocalName != "photos"))
        ;
      while (reader.MoveToNextAttribute())
      {
        switch (reader.LocalName)
        {
          case "perpage":
            this.PerPage = int.Parse(reader.Value, (IFormatProvider) CultureInfo.InvariantCulture);
            continue;
          case "page":
            this.Page = int.Parse(reader.Value, (IFormatProvider) CultureInfo.InvariantCulture);
            continue;
          case "pages":
            this.Pages = int.Parse(reader.Value, (IFormatProvider) CultureInfo.InvariantCulture);
            continue;
          case "has_next_page":
            this.HasNextPage = reader.Value == "1";
            continue;
          case "total":
            this.Total = int.Parse(reader.Value, (IFormatProvider) CultureInfo.InvariantCulture);
            continue;
          default:
            continue;
        }
      }
      reader.Read();
      while (reader.LocalName == "photo")
      {
        Photo photo = new Photo();
        photo.Load(reader);
        if (!string.IsNullOrEmpty(photo.PhotoId))
          this.Add(photo);
      }
      reader.Skip();
    }
  }
}
