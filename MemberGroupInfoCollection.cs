// Type: FlickrNet.MemberGroupInfoCollection
// Assembly: FlickrNet, Version=3.10.0.0, Culture=neutral, PublicKeyToken=2491df59efa5d132
// MVID: 8B8C756F-D5B2-4047-8A8B-B1C1E877A2EF
// Assembly location: D:\RMCK\vs\2014.05.24_FlickrAPI\FlickrNet.dll

using System.Collections.ObjectModel;
using System.Xml;

namespace FlickrNet
{
  public sealed class MemberGroupInfoCollection : Collection<MemberGroupInfo>, IFlickrParsable
  {
    public int Pages { get; set; }

    public int Total { get; set; }

    public int Page { get; set; }

    public int PerPage { get; set; }

    void IFlickrParsable.Load(XmlReader reader)
    {
      if (!(reader.LocalName != "groups"))
        ;
      while (reader.MoveToNextAttribute())
      {
        switch (reader.LocalName)
        {
          case "per_page":
            this.PerPage = reader.ReadContentAsInt();
            continue;
          case "page":
            this.Page = reader.ReadContentAsInt();
            continue;
          case "pages":
            this.Pages = reader.ReadContentAsInt();
            continue;
          case "total":
            this.Total = reader.ReadContentAsInt();
            continue;
          default:
            continue;
        }
      }
      reader.Read();
      while (reader.LocalName == "group")
      {
        MemberGroupInfo memberGroupInfo = new MemberGroupInfo();
        memberGroupInfo.Load(reader);
        this.Add(memberGroupInfo);
      }
      reader.Skip();
    }
  }
}
