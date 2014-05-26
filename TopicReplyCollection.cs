// Type: FlickrNet.TopicReplyCollection
// Assembly: FlickrNet, Version=3.10.0.0, Culture=neutral, PublicKeyToken=2491df59efa5d132
// MVID: 8B8C756F-D5B2-4047-8A8B-B1C1E877A2EF
// Assembly location: D:\RMCK\vs\2014.05.24_FlickrAPI\FlickrNet.dll

using System;
using System.Collections.ObjectModel;
using System.Xml;

namespace FlickrNet
{
  public class TopicReplyCollection : Collection<TopicReply>, IFlickrParsable
  {
    public string TopicId { get; set; }

    public string Subject { get; set; }

    public string GroupId { get; set; }

    public string GroupName { get; set; }

    public string GroupIconServer { get; set; }

    public string GroupIconFarm { get; set; }

    public string AuthorUserId { get; set; }

    public string AuthorName { get; set; }

    public bool AuthorIsPro { get; set; }

    public MemberTypes AuthorRole { get; set; }

    public string AuthorIconServer { get; set; }

    public string AuthorIconFarm { get; set; }

    public bool CanEdit { get; set; }

    public bool CanDelete { get; set; }

    public bool CanReply { get; set; }

    public bool IsSticky { get; set; }

    public bool IsLocked { get; set; }

    public DateTime DateCreated { get; set; }

    public DateTime DateLastPost { get; set; }

    public string Message { get; set; }

    public int Total { get; set; }

    public int PerPage { get; set; }

    public int Page { get; set; }

    public int Pages { get; set; }

    public string AuthorBuddyIcon
    {
      get
      {
        return UtilityMethods.BuddyIcon(this.AuthorIconServer, this.AuthorIconFarm, this.AuthorUserId);
      }
    }

    public string GroupIconUrl
    {
      get
      {
        return UtilityMethods.BuddyIcon(this.GroupIconServer, this.GroupIconFarm, this.GroupId);
      }
    }

    void IFlickrParsable.Load(XmlReader reader)
    {
      if (reader == null)
        throw new ArgumentNullException("reader");
      if (reader.LocalName != "replies")
        return;
      reader.Read();
      if (reader.LocalName != "topic")
        return;
      while (reader.MoveToNextAttribute())
      {
        switch (reader.LocalName)
        {
          case "topic_id":
            this.TopicId = reader.Value;
            continue;
          case "subject":
            this.Subject = reader.Value;
            continue;
          case "group_id":
            this.GroupId = reader.Value;
            continue;
          case "name":
            this.GroupName = reader.Value;
            continue;
          case "author":
            this.AuthorUserId = reader.Value;
            continue;
          case "authorname":
            this.AuthorName = reader.Value;
            continue;
          case "is_pro":
            this.AuthorIsPro = reader.Value == "1";
            continue;
          case "role":
            this.AuthorRole = UtilityMethods.ParseRoleToMemberType(reader.Value);
            continue;
          case "iconserver":
            this.GroupIconServer = reader.Value;
            continue;
          case "iconfarm":
            this.GroupIconFarm = reader.Value;
            continue;
          case "author_iconfarm":
            this.AuthorIconFarm = reader.Value;
            continue;
          case "author_iconserver":
            this.AuthorIconServer = reader.Value;
            continue;
          case "can_edit":
            this.CanEdit = reader.Value == "1";
            continue;
          case "can_delete":
            this.CanDelete = reader.Value == "1";
            continue;
          case "can_reply":
            this.CanReply = reader.Value == "1";
            continue;
          case "is_sticky":
            this.IsSticky = reader.Value == "1";
            continue;
          case "is_locked":
            this.IsLocked = reader.Value == "1";
            continue;
          case "datecreate":
            this.DateCreated = UtilityMethods.UnixTimestampToDate(reader.Value);
            continue;
          case "datelastpost":
            this.DateLastPost = UtilityMethods.UnixTimestampToDate(reader.Value);
            continue;
          case "total":
            this.Total = reader.ReadContentAsInt();
            continue;
          case "pages":
            this.Pages = reader.ReadContentAsInt();
            continue;
          case "page":
            this.Page = reader.ReadContentAsInt();
            continue;
          case "per_page":
            this.PerPage = reader.ReadContentAsInt();
            continue;
          default:
            continue;
        }
      }
      reader.Read();
      if (reader.LocalName == "message")
      {
        this.Message = reader.ReadElementContentAsString();
        reader.Read();
      }
      while (reader.LocalName == "reply" && reader.NodeType == XmlNodeType.Element)
      {
        TopicReply topicReply = new TopicReply();
        topicReply.Load(reader);
        this.Add(topicReply);
      }
    }
  }
}
