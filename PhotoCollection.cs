// Type: FlickrNet.PhotoCollection
// Assembly: FlickrNet, Version=3.10.0.0, Culture=neutral, PublicKeyToken=2491df59efa5d132
// MVID: 8B8C756F-D5B2-4047-8A8B-B1C1E877A2EF
// Assembly location: D:\RMCK\vs\2014.05.24_FlickrAPI\FlickrNet.dll

using System;
using System.Globalization;
using System.Xml;

namespace FlickrNet
{
  public sealed class PhotoCollection : PagedPhotoCollection, IFlickrParsable
  {
    void IFlickrParsable.Load(XmlReader reader)
    {
      if (!(reader.LocalName != "photos"))
        ;
      while (reader.MoveToNextAttribute())
      {
        switch (reader.LocalName)
        {
          case "total":
            this.Total = string.IsNullOrEmpty(reader.Value) ? 0 : int.Parse(reader.Value, (IFormatProvider) CultureInfo.InvariantCulture);
            continue;
          case "perpage":
          case "per_page":
            this.PerPage = string.IsNullOrEmpty(reader.Value) ? 0 : int.Parse(reader.Value, (IFormatProvider) CultureInfo.InvariantCulture);
            continue;
          case "page":
            this.Page = string.IsNullOrEmpty(reader.Value) ? 0 : int.Parse(reader.Value, (IFormatProvider) CultureInfo.InvariantCulture);
            continue;
          case "pages":
            this.Pages = string.IsNullOrEmpty(reader.Value) ? 0 : int.Parse(reader.Value, (IFormatProvider) CultureInfo.InvariantCulture);
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
