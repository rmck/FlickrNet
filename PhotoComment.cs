// Type: FlickrNet.PhotoComment
// Assembly: FlickrNet, Version=3.10.0.0, Culture=neutral, PublicKeyToken=2491df59efa5d132
// MVID: 8B8C756F-D5B2-4047-8A8B-B1C1E877A2EF
// Assembly location: D:\RMCK\vs\2014.05.24_FlickrAPI\FlickrNet.dll

using System;
using System.Xml;

namespace FlickrNet
{
  public sealed class PhotoComment : IFlickrParsable
  {
    public string CommentId { get; set; }

    public string AuthorUserId { get; set; }

    public string AuthorUserName { get; set; }

    public string AuthorRealName { get; set; }

    public string Permalink { get; set; }

    public DateTime DateCreated { get; set; }

    public string IconServer { get; set; }

    public string IconFarm { get; set; }

    public string AuthorBuddyIcon
    {
      get
      {
        return UtilityMethods.BuddyIcon(this.IconServer, this.IconFarm, this.AuthorUserId);
      }
    }

    public string AuthorPathAlias { get; set; }

    public string CommentHtml { get; set; }

    void IFlickrParsable.Load(XmlReader reader)
    {
      if (!(reader.LocalName != "comment"))
        ;
      while (reader.MoveToNextAttribute())
      {
        switch (reader.LocalName)
        {
          case "id":
            this.CommentId = reader.Value;
            continue;
          case "author":
            this.AuthorUserId = reader.Value;
            continue;
          case "authorname":
            this.AuthorUserName = reader.Value;
            continue;
          case "permalink":
            this.Permalink = reader.Value;
            continue;
          case "datecreate":
            this.DateCreated = UtilityMethods.UnixTimestampToDate(reader.Value);
            continue;
          case "iconserver":
            this.IconServer = reader.Value;
            continue;
          case "iconfarm":
            this.IconFarm = reader.Value;
            continue;
          case "path_alias":
            this.AuthorPathAlias = reader.Value;
            continue;
          case "realname":
            this.AuthorRealName = reader.Value;
            continue;
          default:
            continue;
        }
      }
      reader.Read();
      this.CommentHtml = reader.ReadContentAsString();
      reader.Skip();
    }
  }
}
