// Type: FlickrNet.Gallery
// Assembly: FlickrNet, Version=3.10.0.0, Culture=neutral, PublicKeyToken=2491df59efa5d132
// MVID: 8B8C756F-D5B2-4047-8A8B-B1C1E877A2EF
// Assembly location: D:\RMCK\vs\2014.05.24_FlickrAPI\FlickrNet.dll

using System;
using System.Xml;

namespace FlickrNet
{
  public sealed class Gallery : IFlickrParsable
  {
    public string GalleryId { get; set; }

    public string GalleryUrl { get; set; }

    public string OwnerId { get; set; }

    public string OwnerServer { get; set; }

    public string OwnerFarm { get; set; }

    public string OwnerBuddyIcon
    {
      get
      {
        return UtilityMethods.BuddyIcon(this.OwnerServer, this.OwnerFarm, this.OwnerId);
      }
    }

    public string Username { get; set; }

    public DateTime DateCreated { get; set; }

    public DateTime DateLastUpdated { get; set; }

    public string PrimaryPhotoId { get; set; }

    public string PrimaryPhotoServer { get; set; }

    public string PrimaryPhotoFarm { get; set; }

    public string PrimaryPhotoSecret { get; set; }

    public int PhotosCount { get; set; }

    public int VideosCount { get; set; }

    public int ViewCount { get; set; }

    public int CommentCount { get; set; }

    public string Title { get; set; }

    public string Description { get; set; }

    public string PrimaryPhotoThumbnailUrl
    {
      get
      {
        return UtilityMethods.UrlFormat(this.PrimaryPhotoFarm, this.PrimaryPhotoServer, this.PrimaryPhotoId, this.PrimaryPhotoSecret, "thumbnail", "jpg");
      }
    }

    public string PrimaryPhotoSmallUrl
    {
      get
      {
        return UtilityMethods.UrlFormat(this.PrimaryPhotoFarm, this.PrimaryPhotoServer, this.PrimaryPhotoId, this.PrimaryPhotoSecret, "small", "jpg");
      }
    }

    public string PrimaryPhotoSquareThumbnailUrl
    {
      get
      {
        return UtilityMethods.UrlFormat(this.PrimaryPhotoFarm, this.PrimaryPhotoServer, this.PrimaryPhotoId, this.PrimaryPhotoSecret, "square", "jpg");
      }
    }

    public string PrimaryPhotoMediumUrl
    {
      get
      {
        return UtilityMethods.UrlFormat(this.PrimaryPhotoFarm, this.PrimaryPhotoServer, this.PrimaryPhotoId, this.PrimaryPhotoSecret, "medium", "jpg");
      }
    }

    void IFlickrParsable.Load(XmlReader reader)
    {
      while (reader.MoveToNextAttribute())
      {
        switch (reader.LocalName)
        {
          case "id":
            this.GalleryId = reader.Value;
            continue;
          case "url":
            this.GalleryUrl = reader.Value.IndexOf("www.flickr.com", StringComparison.Ordinal) <= 0 ? "https://www.flickr.com" + reader.Value : reader.Value.Replace("http://", "https://");
            continue;
          case "owner":
            this.OwnerId = reader.Value;
            continue;
          case "username":
            this.Username = reader.Value;
            continue;
          case "date_create":
            this.DateCreated = UtilityMethods.UnixTimestampToDate(reader.Value);
            continue;
          case "date_update":
            this.DateLastUpdated = UtilityMethods.UnixTimestampToDate(reader.Value);
            continue;
          case "primary_photo_id":
            this.PrimaryPhotoId = reader.Value;
            continue;
          case "iconserver":
            this.OwnerServer = reader.Value;
            continue;
          case "iconfarm":
            this.OwnerFarm = reader.Value;
            continue;
          case "primary_photo_server":
          case "server":
            this.PrimaryPhotoServer = reader.Value;
            continue;
          case "primary_photo_farm":
          case "farm":
            this.PrimaryPhotoFarm = reader.Value;
            continue;
          case "primary_photo_secret":
          case "secret":
            this.PrimaryPhotoSecret = reader.Value;
            continue;
          case "count_photos":
            this.PhotosCount = reader.ReadContentAsInt();
            continue;
          case "count_videos":
            this.VideosCount = reader.ReadContentAsInt();
            continue;
          case "count_views":
            this.ViewCount = reader.ReadContentAsInt();
            continue;
          case "count_comments":
            this.CommentCount = reader.ReadContentAsInt();
            continue;
          default:
            continue;
        }
      }
      reader.Read();
      while (reader.NodeType != XmlNodeType.EndElement)
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
