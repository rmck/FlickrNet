// Type: FlickrNet.GroupSearchResult
// Assembly: FlickrNet, Version=3.10.0.0, Culture=neutral, PublicKeyToken=2491df59efa5d132
// MVID: 8B8C756F-D5B2-4047-8A8B-B1C1E877A2EF
// Assembly location: D:\RMCK\vs\2014.05.24_FlickrAPI\FlickrNet.dll

using System.Xml;

namespace FlickrNet
{
  public sealed class GroupSearchResult : IFlickrParsable
  {
    public string GroupId { get; set; }

    public string GroupName { get; set; }

    public bool EighteenPlus { get; set; }

    public string IconServer { get; set; }

    public string IconFarm { get; set; }

    public int Members { get; set; }

    public int PoolCount { get; set; }

    public int TopicCount { get; set; }

    public string GroupIconUrl
    {
      get
      {
        return UtilityMethods.BuddyIcon(this.IconServer, this.IconFarm, this.GroupId);
      }
    }

    void IFlickrParsable.Load(XmlReader reader)
    {
      if (!(reader.LocalName != "group"))
        ;
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
          case "eighteenplus":
            this.EighteenPlus = reader.Value == "1";
            continue;
          case "iconserver":
            this.IconServer = reader.Value;
            continue;
          case "iconfarm":
            this.IconFarm = reader.Value;
            continue;
          case "members":
            this.Members = reader.ReadContentAsInt();
            continue;
          case "pool_count":
            this.PoolCount = reader.ReadContentAsInt();
            continue;
          case "topic_count":
            this.TopicCount = reader.ReadContentAsInt();
            continue;
          default:
            continue;
        }
      }
      reader.Skip();
    }
  }
}
