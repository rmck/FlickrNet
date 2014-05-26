// Type: FlickrNet.PhotoFavorite
// Assembly: FlickrNet, Version=3.10.0.0, Culture=neutral, PublicKeyToken=2491df59efa5d132
// MVID: 8B8C756F-D5B2-4047-8A8B-B1C1E877A2EF
// Assembly location: D:\RMCK\vs\2014.05.24_FlickrAPI\FlickrNet.dll

using System;
using System.Xml;

namespace FlickrNet
{
  public sealed class PhotoFavorite : IFlickrParsable
  {
    public string UserId { get; set; }

    public string UserName { get; set; }

    public DateTime FavoriteDate { get; set; }

    void IFlickrParsable.Load(XmlReader reader)
    {
      while (reader.MoveToNextAttribute())
      {
        switch (reader.Name)
        {
          case "nsid":
            this.UserId = reader.Value;
            continue;
          case "username":
            this.UserName = reader.Value;
            continue;
          case "favedate":
            this.FavoriteDate = UtilityMethods.UnixTimestampToDate(reader.Value);
            continue;
          default:
            continue;
        }
      }
      reader.Skip();
    }
  }
}
