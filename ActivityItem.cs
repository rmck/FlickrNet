// Type: FlickrNet.ActivityItem
// Assembly: FlickrNet, Version=3.10.0.0, Culture=neutral, PublicKeyToken=2491df59efa5d132
// MVID: 8B8C756F-D5B2-4047-8A8B-B1C1E877A2EF
// Assembly location: D:\RMCK\vs\2014.05.24_FlickrAPI\FlickrNet.dll

using System;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Xml;

namespace FlickrNet
{
  public sealed class ActivityItem : IFlickrParsable
  {
    public ActivityItemType ItemType { get; set; }

    public string Id { get; set; }

    public string Secret { get; set; }

    public string Server { get; set; }

    public string Farm { get; set; }

    public string Title { get; set; }

    public int NewComments { get; set; }

    public int OldComments { get; set; }

    public int Comments { get; set; }

    public int Views { get; set; }

    public bool More { get; set; }

    public string OwnerId { get; set; }

    public string RealName { get; set; }

    public string OwnerName { get; set; }

    public string OwnerServer { get; set; }

    public string OwnerFarm { get; set; }

    public string OwnerBuddyIcon
    {
      get
      {
        return UtilityMethods.BuddyIcon(this.OwnerServer, this.OwnerFarm, this.OwnerId);
      }
    }

    public int? Photos { get; set; }

    public string PrimaryPhotoId { get; set; }

    public int? NewNotes { get; set; }

    public int? OldNotes { get; set; }

    public int? Notes { get; set; }

    public int? Favorites { get; set; }

    public MediaType Media { get; set; }

    public Collection<ActivityEvent> Events { get; set; }

    public string SquareThumbnailUrl
    {
      get
      {
        if (this.ItemType == ActivityItemType.Photo)
          return UtilityMethods.UrlFormat(this.Farm, this.Server, this.Id, this.Secret, "_s", "jpg");
        if (this.ItemType == ActivityItemType.Photoset || this.ItemType == ActivityItemType.Gallery)
          return UtilityMethods.UrlFormat(this.Farm, this.Server, this.PrimaryPhotoId, this.Secret, "_s", "jpg");
        else
          return (string) null;
      }
    }

    public string SmallUrl
    {
      get
      {
        if (this.ItemType == ActivityItemType.Photo)
          return UtilityMethods.UrlFormat(this.Farm, this.Server, this.Id, this.Secret, "_m", "jpg");
        if (this.ItemType == ActivityItemType.Photoset || this.ItemType == ActivityItemType.Gallery)
          return UtilityMethods.UrlFormat(this.Farm, this.Server, this.PrimaryPhotoId, this.Secret, "_m", "jpg");
        else
          return (string) null;
      }
    }

    public ActivityItem()
    {
      this.Events = new Collection<ActivityEvent>();
    }

    void IFlickrParsable.Load(XmlReader reader)
    {
      this.LoadAttributes(reader);
      this.LoadElements(reader);
    }

    private void LoadElements(XmlReader reader)
    {
      while (reader.LocalName != "item")
      {
        switch (reader.LocalName)
        {
          case "title":
            this.Title = reader.ReadElementContentAsString();
            continue;
          case "activity":
            reader.ReadToDescendant("event");
            while (reader.LocalName == "event")
            {
              ActivityEvent activityEvent = new ActivityEvent();
              activityEvent.Load(reader);
              this.Events.Add(activityEvent);
            }
            reader.Read();
            continue;
          default:
            reader.Skip();
            continue;
        }
      }
      reader.Read();
    }

    private void LoadAttributes(XmlReader reader)
    {
      while (reader.MoveToNextAttribute())
      {
        switch (reader.LocalName)
        {
          case "type":
            switch (reader.Value)
            {
              case "photoset":
                this.ItemType = ActivityItemType.Photoset;
                continue;
              case "photo":
                this.ItemType = ActivityItemType.Photo;
                continue;
              case "gallery":
                this.ItemType = ActivityItemType.Gallery;
                continue;
              default:
                continue;
            }
          case "media":
            switch (reader.Value)
            {
              case "photo":
                this.Media = MediaType.Photos;
                continue;
              case "video":
                this.Media = MediaType.Videos;
                continue;
              default:
                continue;
            }
          case "owner":
            this.OwnerId = reader.Value;
            continue;
          case "ownername":
            this.OwnerName = reader.Value;
            continue;
          case "id":
            this.Id = reader.Value;
            continue;
          case "secret":
            this.Secret = reader.Value;
            continue;
          case "server":
            this.Server = reader.Value;
            continue;
          case "farm":
            this.Farm = reader.Value;
            continue;
          case "iconserver":
            this.OwnerServer = reader.Value;
            continue;
          case "iconfarm":
            this.OwnerFarm = reader.Value;
            continue;
          case "commentsnew":
            this.NewComments = int.Parse(reader.Value, (IFormatProvider) NumberFormatInfo.InvariantInfo);
            continue;
          case "commentsold":
            this.OldComments = int.Parse(reader.Value, (IFormatProvider) NumberFormatInfo.InvariantInfo);
            continue;
          case "comments":
            this.Comments = int.Parse(reader.Value, (IFormatProvider) NumberFormatInfo.InvariantInfo);
            continue;
          case "views":
            this.Views = int.Parse(reader.Value, (IFormatProvider) NumberFormatInfo.InvariantInfo);
            continue;
          case "photos":
            this.Photos = new int?(int.Parse(reader.Value, (IFormatProvider) NumberFormatInfo.InvariantInfo));
            continue;
          case "more":
            this.More = reader.Value == "0";
            continue;
          case "primary":
            this.PrimaryPhotoId = reader.Value;
            continue;
          case "notesnew":
            this.NewNotes = new int?(int.Parse(reader.Value, (IFormatProvider) NumberFormatInfo.InvariantInfo));
            continue;
          case "notesold":
            this.OldNotes = new int?(int.Parse(reader.Value, (IFormatProvider) NumberFormatInfo.InvariantInfo));
            continue;
          case "notes":
            this.Notes = new int?(int.Parse(reader.Value, (IFormatProvider) NumberFormatInfo.InvariantInfo));
            continue;
          case "faves":
            this.Favorites = new int?(int.Parse(reader.Value, (IFormatProvider) NumberFormatInfo.InvariantInfo));
            continue;
          case "realname":
            this.RealName = reader.Value;
            continue;
          default:
            continue;
        }
      }
      reader.Read();
    }
  }
}
