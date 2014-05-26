// Type: FlickrNet.GroupInfo
// Assembly: FlickrNet, Version=3.10.0.0, Culture=neutral, PublicKeyToken=2491df59efa5d132
// MVID: 8B8C756F-D5B2-4047-8A8B-B1C1E877A2EF
// Assembly location: D:\RMCK\vs\2014.05.24_FlickrAPI\FlickrNet.dll

using System;
using System.Globalization;
using System.Xml;

namespace FlickrNet
{
  public sealed class GroupInfo : IFlickrParsable
  {
    public string GroupId { get; set; }

    public string GroupName { get; set; }

    public string IconFarm { get; set; }

    public string IconServer { get; set; }

    public string GroupIconUrl
    {
      get
      {
        return UtilityMethods.BuddyIcon(this.IconServer, this.IconFarm, this.GroupId);
      }
    }

    public bool IsAdmin { get; set; }

    public bool? IsModerator { get; set; }

    public bool? IsMember { get; set; }

    public bool EighteenPlus { get; set; }

    public bool InvitationOnly { get; set; }

    public int Members { get; set; }

    public long PoolCount { get; set; }

    public string GroupUrl
    {
      get
      {
        return string.Format((IFormatProvider) CultureInfo.InvariantCulture, "https://www.flickr.com/groups/{0}/", new object[1]
        {
          (object) this.GroupId
        });
      }
    }

    void IFlickrParsable.Load(XmlReader reader)
    {
      while (reader.MoveToNextAttribute())
      {
        switch (reader.LocalName)
        {
          case "nsid":
            this.GroupId = reader.Value;
            continue;
          case "name":
            this.GroupName = reader.Value;
            continue;
          case "admin":
          case "is_admin":
            this.IsAdmin = reader.Value == "1";
            continue;
          case "is_member":
            this.IsMember = new bool?(reader.Value == "1");
            continue;
          case "is_moderator":
            this.IsModerator = new bool?(reader.Value == "1");
            continue;
          case "eighteenplus":
            this.EighteenPlus = reader.Value == "1";
            continue;
          case "invitation_only":
            this.InvitationOnly = reader.Value == "1";
            continue;
          case "iconfarm":
            this.IconFarm = reader.Value;
            continue;
          case "iconserver":
            this.IconServer = reader.Value;
            continue;
          case "members":
            this.Members = reader.ReadContentAsInt();
            continue;
          case "pool_count":
            this.PoolCount = reader.ReadContentAsLong();
            continue;
          default:
            continue;
        }
      }
      reader.Read();
    }
  }
}
