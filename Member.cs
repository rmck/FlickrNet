// Type: FlickrNet.Member
// Assembly: FlickrNet, Version=3.10.0.0, Culture=neutral, PublicKeyToken=2491df59efa5d132
// MVID: 8B8C756F-D5B2-4047-8A8B-B1C1E877A2EF
// Assembly location: D:\RMCK\vs\2014.05.24_FlickrAPI\FlickrNet.dll

using System.Xml;

namespace FlickrNet
{
  public sealed class Member : IFlickrParsable
  {
    public string MemberId { get; set; }

    public string UserName { get; set; }

    public string IconServer { get; set; }

    public string IconFarm { get; set; }

    public MemberTypes MemberType { get; set; }

    public string IconUrl
    {
      get
      {
        return UtilityMethods.BuddyIcon(this.IconServer, this.IconFarm, this.MemberId);
      }
    }

    void IFlickrParsable.Load(XmlReader reader)
    {
      this.MemberId = reader.GetAttribute("nsid");
      this.UserName = reader.GetAttribute("username");
      this.IconServer = reader.GetAttribute("iconserver");
      this.IconFarm = reader.GetAttribute("iconfarm");
      this.MemberType = UtilityMethods.ParseIdToMemberType(reader.GetAttribute("membertype"));
    }
  }
}
