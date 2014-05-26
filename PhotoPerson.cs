// Type: FlickrNet.PhotoPerson
// Assembly: FlickrNet, Version=3.10.0.0, Culture=neutral, PublicKeyToken=2491df59efa5d132
// MVID: 8B8C756F-D5B2-4047-8A8B-B1C1E877A2EF
// Assembly location: D:\RMCK\vs\2014.05.24_FlickrAPI\FlickrNet.dll

using System.Xml;

namespace FlickrNet
{
  public sealed class PhotoPerson : IFlickrParsable
  {
    public string UserId { get; set; }

    public string IconServer { get; set; }

    public string IconFarm { get; set; }

    public string UserName { get; set; }

    public string RealName { get; set; }

    public string AddedByUserId { get; set; }

    public int? PositionX { get; set; }

    public int? PositionY { get; set; }

    public int? PositionWidth { get; set; }

    public int? PositionHeight { get; set; }

    public string PathAlias { get; set; }

    public string PhotostreamUrl
    {
      get
      {
        return "https://www.flickr.com/photos/" + (string.IsNullOrEmpty(this.PathAlias) ? this.UserId : this.PathAlias);
      }
    }

    public string BuddyIconUrl
    {
      get
      {
        return UtilityMethods.BuddyIcon(this.IconServer, this.IconFarm, this.UserId);
      }
    }

    void IFlickrParsable.Load(XmlReader reader)
    {
      while (reader.MoveToNextAttribute())
      {
        switch (reader.LocalName)
        {
          case "id":
          case "nsid":
            this.UserId = reader.Value;
            continue;
          case "iconserver":
            this.IconServer = reader.Value;
            continue;
          case "iconfarm":
            this.IconFarm = reader.Value;
            continue;
          case "username":
            this.UserName = reader.Value;
            continue;
          case "realname":
            this.RealName = reader.Value;
            continue;
          case "added_by":
            this.AddedByUserId = reader.Value;
            continue;
          case "path_alias":
            this.PathAlias = reader.Value;
            continue;
          case "x":
            this.PositionX = new int?(reader.ReadContentAsInt());
            continue;
          case "y":
            this.PositionY = new int?(reader.ReadContentAsInt());
            continue;
          case "w":
            this.PositionWidth = new int?(reader.ReadContentAsInt());
            continue;
          case "h":
            this.PositionHeight = new int?(reader.ReadContentAsInt());
            continue;
          default:
            continue;
        }
      }
      reader.Read();
    }
  }
}
