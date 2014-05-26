// Type: FlickrNet.MemberGroupInfo
// Assembly: FlickrNet, Version=3.10.0.0, Culture=neutral, PublicKeyToken=2491df59efa5d132
// MVID: 8B8C756F-D5B2-4047-8A8B-B1C1E877A2EF
// Assembly location: D:\RMCK\vs\2014.05.24_FlickrAPI\FlickrNet.dll

using System;
using System.Globalization;
using System.Xml;

namespace FlickrNet
{
  public sealed class MemberGroupInfo : IFlickrParsable
  {
    public string GroupId { get; set; }

    public string GroupName { get; set; }

    public bool IsAdmin { get; set; }

    public PoolPrivacy Privacy { get; set; }

    public string IconServer { get; set; }

    public string IconFarm { get; set; }

    public long Photos { get; set; }

    public string GroupIconUrl
    {
      get
      {
        return UtilityMethods.BuddyIcon(this.IconServer, this.IconFarm, this.GroupId);
      }
    }

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
          case "id":
            this.GroupId = reader.Value;
            continue;
          case "name":
            this.GroupName = reader.Value;
            continue;
          case "admin":
            this.IsAdmin = reader.Value == "1";
            continue;
          case "privacy":
            this.Privacy = (PoolPrivacy) Enum.Parse(typeof (PoolPrivacy), reader.Value, true);
            continue;
          case "iconserver":
            this.IconServer = reader.Value;
            continue;
          case "iconfarm":
            this.IconFarm = reader.Value;
            continue;
          case "photos":
            this.Photos = long.Parse(reader.Value, NumberStyles.Any, (IFormatProvider) NumberFormatInfo.InvariantInfo);
            continue;
          default:
            continue;
        }
      }
      reader.Read();
    }
  }
}
