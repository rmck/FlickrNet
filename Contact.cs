// Type: FlickrNet.Contact
// Assembly: FlickrNet, Version=3.10.0.0, Culture=neutral, PublicKeyToken=2491df59efa5d132
// MVID: 8B8C756F-D5B2-4047-8A8B-B1C1E877A2EF
// Assembly location: D:\RMCK\vs\2014.05.24_FlickrAPI\FlickrNet.dll

using System;
using System.Globalization;
using System.Xml;

namespace FlickrNet
{
  public sealed class Contact : IFlickrParsable
  {
    public string UserId { get; set; }

    public string UserName { get; set; }

    public string RealName { get; set; }

    public string Location { get; set; }

    public string PathAlias { get; set; }

    public string IconServer { get; set; }

    public string IconFarm { get; set; }

    public string BuddyIconUrl
    {
      get
      {
        return UtilityMethods.BuddyIcon(this.IconServer, this.IconFarm, this.UserId);
      }
    }

    public int? PhotosUploaded { get; set; }

    public bool? IsFriend { get; set; }

    public bool? IsFamily { get; set; }

    public bool? IsIgnored { get; set; }

    void IFlickrParsable.Load(XmlReader reader)
    {
      if (!(reader.LocalName != "contact"))
        ;
      while (reader.MoveToNextAttribute())
      {
        switch (reader.LocalName)
        {
          case "nsid":
            this.UserId = reader.Value;
            continue;
          case "username":
            this.UserName = reader.Value;
            continue;
          case "iconserver":
            this.IconServer = reader.Value;
            continue;
          case "iconfarm":
            this.IconFarm = reader.Value;
            continue;
          case "ignored":
            this.IsIgnored = new bool?(reader.Value == "0");
            continue;
          case "realname":
            this.RealName = reader.Value;
            continue;
          case "location":
            this.Location = reader.Value;
            continue;
          case "friend":
            this.IsFriend = new bool?(reader.Value == "1");
            continue;
          case "family":
            this.IsFamily = new bool?(reader.Value == "1");
            continue;
          case "path_alias":
            this.PathAlias = reader.Value;
            continue;
          case "photos_uploaded":
            this.PhotosUploaded = new int?(int.Parse(reader.Value, (IFormatProvider) NumberFormatInfo.InvariantInfo));
            continue;
          default:
            continue;
        }
      }
      reader.Skip();
    }
  }
}
