// Type: FlickrNet.PhotosetPhotoCollection
// Assembly: FlickrNet, Version=3.10.0.0, Culture=neutral, PublicKeyToken=2491df59efa5d132
// MVID: 8B8C756F-D5B2-4047-8A8B-B1C1E877A2EF
// Assembly location: D:\RMCK\vs\2014.05.24_FlickrAPI\FlickrNet.dll

using System;
using System.Globalization;
using System.Xml;

namespace FlickrNet
{
  public sealed class PhotosetPhotoCollection : PagedPhotoCollection, IFlickrParsable
  {
    public string PhotosetId { get; set; }

    public string PrimaryPhotoId { get; set; }

    public string OwnerId { get; set; }

    public string OwnerName { get; set; }

    public string Title { get; set; }

    void IFlickrParsable.Load(XmlReader reader)
    {
      if (!(reader.LocalName != "photoset"))
        ;
      while (reader.MoveToNextAttribute())
      {
        switch (reader.LocalName)
        {
          case "id":
            this.PhotosetId = reader.Value;
            continue;
          case "primary":
            this.PrimaryPhotoId = reader.Value;
            continue;
          case "owner":
            this.OwnerId = reader.Value;
            continue;
          case "ownername":
            this.OwnerName = reader.Value;
            continue;
          case "page":
            this.Page = int.Parse(reader.Value, (IFormatProvider) NumberFormatInfo.InvariantInfo);
            continue;
          case "total":
            this.Total = int.Parse(reader.Value, (IFormatProvider) NumberFormatInfo.InvariantInfo);
            continue;
          case "pages":
            this.Pages = int.Parse(reader.Value, (IFormatProvider) NumberFormatInfo.InvariantInfo);
            continue;
          case "perpage":
          case "per_page":
            this.PerPage = int.Parse(reader.Value, (IFormatProvider) NumberFormatInfo.InvariantInfo);
            continue;
          case "title":
            this.Title = reader.Value;
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
        if (string.IsNullOrEmpty(photo.UserId))
          photo.UserId = this.OwnerId;
        this.Add(photo);
      }
      reader.Skip();
    }
  }
}
