// Type: FlickrNet.FavoriteContext
// Assembly: FlickrNet, Version=3.10.0.0, Culture=neutral, PublicKeyToken=2491df59efa5d132
// MVID: 8B8C756F-D5B2-4047-8A8B-B1C1E877A2EF
// Assembly location: D:\RMCK\vs\2014.05.24_FlickrAPI\FlickrNet.dll

using System.Collections.ObjectModel;
using System.Xml;

namespace FlickrNet
{
  public sealed class FavoriteContext : IFlickrParsable
  {
    public int Count { get; set; }

    public Collection<FavoriteContextPhoto> PreviousPhotos { get; set; }

    public Collection<FavoriteContextPhoto> NextPhotos { get; set; }

    public FavoriteContext()
    {
      this.PreviousPhotos = new Collection<FavoriteContextPhoto>();
      this.NextPhotos = new Collection<FavoriteContextPhoto>();
    }

    void IFlickrParsable.Load(XmlReader reader)
    {
      if (reader.LocalName != "count" && !reader.ReadToFollowing("count"))
        return;
      this.Count = reader.ReadElementContentAsInt();
      if (reader.LocalName != "prevphotos")
        reader.ReadToFollowing("prevphotos");
      reader.ReadToDescendant("photo");
      while (reader.LocalName == "photo")
      {
        FavoriteContextPhoto favoriteContextPhoto = new FavoriteContextPhoto();
        favoriteContextPhoto.Load(reader);
        this.PreviousPhotos.Add(favoriteContextPhoto);
      }
      if (reader.LocalName != "nextphotos")
        reader.ReadToFollowing("nextphotos");
      reader.ReadToDescendant("photo");
      while (reader.LocalName == "photo")
      {
        FavoriteContextPhoto favoriteContextPhoto = new FavoriteContextPhoto();
        favoriteContextPhoto.Load(reader);
        this.NextPhotos.Add(favoriteContextPhoto);
      }
    }
  }
}
