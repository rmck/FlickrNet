// Type: FlickrNet.GroupInfoRestrictions
// Assembly: FlickrNet, Version=3.10.0.0, Culture=neutral, PublicKeyToken=2491df59efa5d132
// MVID: 8B8C756F-D5B2-4047-8A8B-B1C1E877A2EF
// Assembly location: D:\RMCK\vs\2014.05.24_FlickrAPI\FlickrNet.dll

using System.Xml;

namespace FlickrNet
{
  public sealed class GroupInfoRestrictions : IFlickrParsable
  {
    public bool PhotosAccepted { get; set; }

    public bool VideosAccepted { get; set; }

    public bool ImagesAccepted { get; set; }

    public bool ScreenshotsAccepted { get; set; }

    public bool ArtIllustrationsAccepted { get; set; }

    public bool SafeItemsAccepted { get; set; }

    public bool ModeratedItemsAccepted { get; set; }

    public bool RestrictedItemsAccepted { get; set; }

    public bool GeoInfoRequired { get; set; }

    void IFlickrParsable.Load(XmlReader reader)
    {
      while (reader.MoveToNextAttribute())
      {
        switch (reader.LocalName)
        {
          case "photos_ok":
            this.PhotosAccepted = reader.Value == "1";
            continue;
          case "videos_ok":
            this.VideosAccepted = reader.Value == "1";
            continue;
          case "images_ok":
            this.ImagesAccepted = reader.Value == "1";
            continue;
          case "screens_ok":
            this.ScreenshotsAccepted = reader.Value == "1";
            continue;
          case "art_ok":
            this.ArtIllustrationsAccepted = reader.Value == "1";
            continue;
          case "safe_ok":
            this.SafeItemsAccepted = reader.Value == "1";
            continue;
          case "moderate_ok":
            this.ModeratedItemsAccepted = reader.Value == "1";
            continue;
          case "resitricted_ok":
            this.RestrictedItemsAccepted = reader.Value == "1";
            continue;
          case "has_geo":
            this.GeoInfoRequired = reader.Value == "1";
            continue;
          default:
            continue;
        }
      }
      reader.Read();
    }
  }
}
