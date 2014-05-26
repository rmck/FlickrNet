// Type: FlickrNet.GroupFullInfo
// Assembly: FlickrNet, Version=3.10.0.0, Culture=neutral, PublicKeyToken=2491df59efa5d132
// MVID: 8B8C756F-D5B2-4047-8A8B-B1C1E877A2EF
// Assembly location: D:\RMCK\vs\2014.05.24_FlickrAPI\FlickrNet.dll

using System;
using System.Xml;

namespace FlickrNet
{
  public sealed class GroupFullInfo : IFlickrParsable
  {
    public string GroupId { get; set; }

    public string GroupName { get; set; }

    public string Description { get; set; }

    public int Members { get; set; }

    public int PoolCount { get; set; }

    public int TopicCount { get; set; }

    public string IconServer { get; set; }

    public string IconFarm { get; set; }

    public string Language { get; set; }

    public bool IsPoolModerated { get; set; }

    public string BlastHtml { get; set; }

    public string BlastUserId { get; set; }

    public DateTime? BlastDateAdded { get; set; }

    public string MemberRoleName { get; set; }

    public string ModeratorRoleName { get; set; }

    public string AdminRoleName { get; set; }

    public string GroupIconUrl
    {
      get
      {
        return UtilityMethods.BuddyIcon(this.IconServer, this.IconFarm, this.GroupId);
      }
    }

    public PoolPrivacy Privacy { get; set; }

    public GroupThrottleInfo ThrottleInfo { get; set; }

    public GroupInfoRestrictions Restrictions { get; set; }

    public string Rules { get; set; }

    public static implicit operator Group(GroupFullInfo groupInfo)
    {
      return new Group()
      {
        GroupId = groupInfo.GroupId,
        GroupName = groupInfo.GroupName,
        Members = groupInfo.Members
      };
    }

    public Group ToGroup()
    {
      return (Group) this;
    }

    void IFlickrParsable.Load(XmlReader reader)
    {
      while (reader.MoveToNextAttribute())
      {
        switch (reader.LocalName)
        {
          case "nsid":
          case "id":
            this.GroupId = reader.Value;
            continue;
          case "iconserver":
            this.IconServer = reader.Value;
            continue;
          case "iconfarm":
            this.IconFarm = reader.Value;
            continue;
          case "lang":
            this.Language = reader.Value;
            continue;
          case "ispoolmoderated":
            this.IsPoolModerated = reader.Value == "1";
            continue;
          default:
            continue;
        }
      }
      reader.Read();
      while (reader.LocalName != "group")
      {
        switch (reader.LocalName)
        {
          case "name":
            this.GroupName = reader.ReadElementContentAsString();
            continue;
          case "description":
            this.Description = reader.ReadElementContentAsString();
            continue;
          case "members":
            this.Members = reader.ReadElementContentAsInt();
            continue;
          case "privacy":
            this.Privacy = (PoolPrivacy) reader.ReadElementContentAsInt();
            continue;
          case "blast":
            this.BlastDateAdded = new DateTime?(UtilityMethods.UnixTimestampToDate(reader.GetAttribute("date_blast_added")));
            this.BlastUserId = reader.GetAttribute("user_id");
            this.BlastHtml = reader.ReadElementContentAsString();
            continue;
          case "throttle":
            this.ThrottleInfo = new GroupThrottleInfo();
            this.ThrottleInfo.Load(reader);
            continue;
          case "restrictions":
            this.Restrictions = new GroupInfoRestrictions();
            this.Restrictions.Load(reader);
            continue;
          case "roles":
            this.MemberRoleName = reader.GetAttribute("member");
            this.ModeratorRoleName = reader.GetAttribute("moderator");
            this.AdminRoleName = reader.GetAttribute("admin");
            reader.Read();
            continue;
          case "rules":
            this.Rules = reader.ReadElementContentAsString();
            continue;
          case "pool_count":
            this.PoolCount = reader.ReadElementContentAsInt();
            continue;
          case "topic_count":
            this.TopicCount = reader.ReadElementContentAsInt();
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
