// Type: FlickrNet.TopicCollection
// Assembly: FlickrNet, Version=3.10.0.0, Culture=neutral, PublicKeyToken=2491df59efa5d132
// MVID: 8B8C756F-D5B2-4047-8A8B-B1C1E877A2EF
// Assembly location: D:\RMCK\vs\2014.05.24_FlickrAPI\FlickrNet.dll

using System;
using System.Collections.ObjectModel;
using System.Xml;

namespace FlickrNet
{
  public class TopicCollection : Collection<Topic>, IFlickrParsable
  {
    public string GroupId { get; set; }

    public string GroupIconServer { get; set; }

    public string GroupIconFarm { get; set; }

    public string GroupName { get; set; }

    public int MemberCount { get; set; }

    public PoolPrivacy Privacy { get; set; }

    public string Language { get; set; }

    public bool IsPoolModerated { get; set; }

    public int Total { get; set; }

    public int PerPage { get; set; }

    public int Page { get; set; }

    public int Pages { get; set; }

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
      if (reader.LocalName != "topics")
        throw new ResponseXmlException("Unknown initial element \"" + reader.LocalName + "\". Expecting \"topics\".");
      while (reader.MoveToNextAttribute())
      {
        switch (reader.LocalName)
        {
          case "group_id":
            this.GroupId = reader.Value;
            continue;
          case "iconserver":
            this.GroupIconServer = reader.Value;
            continue;
          case "iconfarm":
            this.GroupIconFarm = reader.Value;
            continue;
          case "name":
            this.GroupName = reader.Value;
            continue;
          case "members":
            this.MemberCount = reader.ReadContentAsInt();
            continue;
          case "privacy":
            this.Privacy = (PoolPrivacy) reader.ReadContentAsInt();
            continue;
          case "lang":
            this.Language = reader.Value;
            continue;
          case "ispoolmoderated":
            this.IsPoolModerated = reader.Value == "1";
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
      while (reader.LocalName == "topic")
      {
        Topic topic = new Topic();
        topic.Load(reader);
        this.Add(topic);
      }
    }
  }
}
