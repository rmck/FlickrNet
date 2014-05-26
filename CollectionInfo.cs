// Type: FlickrNet.CollectionInfo
// Assembly: FlickrNet, Version=3.10.0.0, Culture=neutral, PublicKeyToken=2491df59efa5d132
// MVID: 8B8C756F-D5B2-4047-8A8B-B1C1E877A2EF
// Assembly location: D:\RMCK\vs\2014.05.24_FlickrAPI\FlickrNet.dll

using System;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Xml;

namespace FlickrNet
{
  public sealed class CollectionInfo : IFlickrParsable
  {
    private Collection<Photo> iconPhotos = new Collection<Photo>();

    public string CollectionId { get; set; }

    public int ChildCount { get; set; }

    public DateTime DateCreated { get; set; }

    public string IconLarge { get; set; }

    public string IconSmall { get; set; }

    public string Server { get; set; }

    public string Secret { get; set; }

    public string Description { get; set; }

    public string Title { get; set; }

    public Collection<Photo> IconPhotos
    {
      get
      {
        return this.iconPhotos;
      }
    }

    void IFlickrParsable.Load(XmlReader reader)
    {
      if (!(reader.LocalName != "collection"))
        ;
      while (reader.MoveToNextAttribute())
      {
        switch (reader.Name)
        {
          case "id":
            this.CollectionId = reader.Value;
            continue;
          case "child_count":
            this.ChildCount = int.Parse(reader.Value, (IFormatProvider) CultureInfo.InvariantCulture);
            continue;
          case "datecreate":
            this.DateCreated = UtilityMethods.UnixTimestampToDate(reader.Value);
            continue;
          case "iconlarge":
            this.IconLarge = reader.Value;
            continue;
          case "iconsmall":
            this.IconSmall = reader.Value;
            continue;
          case "server":
            this.Server = reader.Value;
            continue;
          case "secret":
            this.Secret = reader.Value;
            continue;
          default:
            continue;
        }
      }
      reader.Read();
      while (reader.LocalName != "collection")
      {
        switch (reader.Name)
        {
          case "title":
            this.Title = reader.ReadElementContentAsString();
            continue;
          case "description":
            this.Description = reader.ReadElementContentAsString();
            continue;
          case "iconphotos":
            reader.Read();
            while (reader.LocalName == "photo")
            {
              Photo photo = new Photo();
              photo.Load(reader);
              this.iconPhotos.Add(photo);
            }
            reader.Read();
            return;
          default:
            continue;
        }
      }
      reader.Skip();
    }
  }
}
