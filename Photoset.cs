// Type: FlickrNet.Photoset
// Assembly: FlickrNet, Version=3.10.0.0, Culture=neutral, PublicKeyToken=2491df59efa5d132
// MVID: 8B8C756F-D5B2-4047-8A8B-B1C1E877A2EF
// Assembly location: D:\RMCK\vs\2014.05.24_FlickrAPI\FlickrNet.dll

using System;
using System.Globalization;
using System.Xml;

namespace FlickrNet
{
  public sealed class Photoset : IFlickrParsable
  {
    private string url;

    public string PhotosetId { get; set; }

    public string Url
    {
      get
      {
        if (this.url == null)
          this.url = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "https://www.flickr.com/photos/{0}/sets/{1}/", new object[2]
          {
            (object) this.OwnerId,
            (object) this.PhotosetId
          });
        return this.url;
      }
      set
      {
        this.url = value;
      }
    }

    public string OwnerId { get; set; }

    public string OwnerName { get; set; }

    public string PrimaryPhotoId { get; set; }

    public string Secret { get; set; }

    public string Server { get; set; }

    public string Farm { get; set; }

    public string CoverPhotoServer { get; set; }

    public string CoverPhotoFarm { get; set; }

    public int NumberOfPhotos { get; set; }

    public int NumberOfVideos { get; set; }

    public string Title { get; set; }

    public string Description { get; set; }

    public DateTime DateCreated { get; set; }

    public DateTime DateUpdated { get; set; }

    public int ViewCount { get; set; }

    public int CommentCount { get; set; }

    public bool? CanComment { get; set; }

    public string PhotosetThumbnailUrl
    {
      get
      {
        return UtilityMethods.UrlFormat(this, "_t", "jpg");
      }
    }

    public string PhotosetSquareThumbnailUrl
    {
      get
      {
        return UtilityMethods.UrlFormat(this, "_s", "jpg");
      }
    }

    public string PhotosetSmallUrl
    {
      get
      {
        return UtilityMethods.UrlFormat(this, "_m", "jpg");
      }
    }

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
          case "url":
            this.Url = reader.Value;
            continue;
          case "owner_id":
          case "owner":
            this.OwnerId = reader.Value;
            continue;
          case "username":
            this.OwnerName = reader.Value;
            continue;
          case "primary":
            this.PrimaryPhotoId = reader.Value;
            continue;
          case "secret":
            this.Secret = reader.Value;
            continue;
          case "farm":
            this.Farm = reader.Value;
            continue;
          case "server":
            this.Server = reader.Value;
            continue;
          case "photos":
          case "count_photos":
            this.NumberOfPhotos = reader.ReadContentAsInt();
            continue;
          case "videos":
          case "count_videos":
            this.NumberOfVideos = reader.ReadContentAsInt();
            continue;
          case "date_create":
            this.DateCreated = UtilityMethods.UnixTimestampToDate(reader.Value);
            continue;
          case "date_update":
            this.DateUpdated = UtilityMethods.UnixTimestampToDate(reader.Value);
            continue;
          case "view_count":
          case "count_views":
            this.ViewCount = reader.ReadContentAsInt();
            continue;
          case "comment_count":
          case "count_comments":
            this.CommentCount = reader.ReadContentAsInt();
            continue;
          case "can_comment":
            this.CanComment = new bool?(reader.Value == "1");
            continue;
          case "coverphoto_server":
            this.CoverPhotoServer = reader.Value;
            continue;
          case "coverphoto_farm":
            this.CoverPhotoFarm = reader.Value;
            continue;
          default:
            continue;
        }
      }
      reader.Read();
      while (reader.LocalName != "photoset" && reader.NodeType != XmlNodeType.EndElement)
      {
        switch (reader.LocalName)
        {
          case "title":
            this.Title = reader.ReadElementContentAsString();
            continue;
          case "description":
            this.Description = reader.ReadElementContentAsString();
            continue;
          default:
            reader.Skip();
            continue;
        }
      }
      reader.Read();
    }
  }
}
