// Type: FlickrNet.ActivityEvent
// Assembly: FlickrNet, Version=3.10.0.0, Culture=neutral, PublicKeyToken=2491df59efa5d132
// MVID: 8B8C756F-D5B2-4047-8A8B-B1C1E877A2EF
// Assembly location: D:\RMCK\vs\2014.05.24_FlickrAPI\FlickrNet.dll

using System;
using System.Xml;

namespace FlickrNet
{
  public sealed class ActivityEvent : IFlickrParsable
  {
    public ActivityEventType EventType { get; set; }

    public string UserId { get; set; }

    public string UserName { get; set; }

    public string RealName { get; set; }

    public string IconServer { get; set; }

    public string IconFarm { get; set; }

    public DateTime DateAdded { get; set; }

    public string Value { get; set; }

    public string CommentId { get; set; }

    public string NoteId { get; set; }

    public string GroupId { get; set; }

    public string GroupName { get; set; }

    public string GalleryId { get; set; }

    void IFlickrParsable.Load(XmlReader reader)
    {
      while (reader.MoveToNextAttribute())
      {
        switch (reader.LocalName)
        {
          case "type":
            switch (reader.Value)
            {
              case "fave":
                this.EventType = ActivityEventType.Favorite;
                continue;
              case "note":
                this.EventType = ActivityEventType.Note;
                continue;
              case "comment":
                this.EventType = ActivityEventType.Comment;
                continue;
              case "added_to_gallery":
                this.EventType = ActivityEventType.Gallery;
                continue;
              case "tag":
                this.EventType = ActivityEventType.Tag;
                continue;
              case "group_invite":
                this.EventType = ActivityEventType.GroupInvite;
                continue;
              default:
                continue;
            }
          case "user":
            this.UserId = reader.Value;
            continue;
          case "username":
            this.UserName = reader.Value;
            continue;
          case "dateadded":
            this.DateAdded = UtilityMethods.UnixTimestampToDate(reader.Value);
            continue;
          case "commentid":
            this.CommentId = reader.Value;
            continue;
          case "noteid":
            this.NoteId = reader.Value;
            continue;
          case "galleryid":
            this.GalleryId = reader.Value;
            continue;
          case "iconserver":
            this.IconServer = reader.Value;
            continue;
          case "iconfarm":
            this.IconFarm = reader.Value;
            continue;
          case "realname":
            this.RealName = reader.Value;
            continue;
          case "group_id":
            this.GroupId = reader.Value;
            continue;
          case "group_name":
            this.GroupName = reader.Value;
            continue;
          default:
            continue;
        }
      }
      reader.Read();
      if (reader.NodeType != XmlNodeType.Text)
        return;
      this.Value = reader.ReadContentAsString();
      reader.Read();
    }
  }
}
