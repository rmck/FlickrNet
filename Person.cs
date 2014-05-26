// Type: FlickrNet.Person
// Assembly: FlickrNet, Version=3.10.0.0, Culture=neutral, PublicKeyToken=2491df59efa5d132
// MVID: 8B8C756F-D5B2-4047-8A8B-B1C1E877A2EF
// Assembly location: D:\RMCK\vs\2014.05.24_FlickrAPI\FlickrNet.dll

using System.Xml;

namespace FlickrNet
{
  public sealed class Person : IFlickrParsable
  {
    public string UserId { get; set; }

    public bool IsPro { get; set; }

    public string IconServer { get; set; }

    public string IconFarm { get; set; }

    public string Gender { get; set; }

    public bool? IsIgnored { get; set; }

    public bool? IsContact { get; set; }

    public bool? IsFriend { get; set; }

    public bool? IsFamily { get; set; }

    public bool? IsReverseContact { get; set; }

    public bool? IsReverseFriend { get; set; }

    public bool? IsReverseFamily { get; set; }

    public string UserName { get; set; }

    public string RealName { get; set; }

    public string MailboxSha1Hash { get; set; }

    public string Location { get; set; }

    public PersonPhotosSummary PhotosSummary { get; set; }

    public string PathAlias { get; set; }

    public string PhotosUrl { get; set; }

    public string ProfileUrl { get; set; }

    public string MobileUrl { get; set; }

    public string BuddyIconUrl
    {
      get
      {
        return UtilityMethods.BuddyIcon(this.IconServer, this.IconFarm, this.UserId);
      }
    }

    public string TimeZoneLabel { get; set; }

    public string TimeZoneOffset { get; set; }

    public string Description { get; set; }

    void IFlickrParsable.Load(XmlReader reader)
    {
      this.LoadAttributes(reader);
      this.LoadElements(reader);
    }

    private void LoadElements(XmlReader reader)
    {
      while (reader.LocalName != "person")
      {
        switch (reader.LocalName)
        {
          case "username":
            this.UserName = reader.ReadElementContentAsString();
            continue;
          case "location":
            this.Location = reader.ReadElementContentAsString();
            continue;
          case "realname":
            this.RealName = reader.ReadElementContentAsString();
            continue;
          case "photosurl":
            this.PhotosUrl = reader.ReadElementContentAsString();
            continue;
          case "profileurl":
            this.ProfileUrl = reader.ReadElementContentAsString();
            continue;
          case "mobileurl":
            this.MobileUrl = reader.ReadElementContentAsString();
            continue;
          case "photos":
            this.PhotosSummary = new PersonPhotosSummary();
            this.PhotosSummary.Load(reader);
            continue;
          case "mbox_sha1sum":
            this.MailboxSha1Hash = reader.ReadElementContentAsString();
            continue;
          case "timezone":
            this.TimeZoneLabel = reader.GetAttribute("label");
            this.TimeZoneOffset = reader.GetAttribute("offset");
            reader.Read();
            continue;
          case "description":
            this.Description = reader.ReadElementContentAsString();
            continue;
          default:
            reader.Skip();
            continue;
        }
      }
    }

    private void LoadAttributes(XmlReader reader)
    {
      while (reader.MoveToNextAttribute())
      {
        switch (reader.LocalName)
        {
          case "id":
          case "nsid":
            this.UserId = reader.Value;
            continue;
          case "ispro":
            this.IsPro = reader.Value == "1";
            continue;
          case "iconserver":
            this.IconServer = reader.Value;
            continue;
          case "iconfarm":
            this.IconFarm = reader.Value;
            continue;
          case "path_alias":
            this.PathAlias = reader.Value;
            continue;
          case "gender":
            this.Gender = reader.Value;
            continue;
          case "ignored":
            this.IsIgnored = new bool?(reader.Value == "1");
            continue;
          case "contact":
            this.IsContact = new bool?(reader.Value == "1");
            continue;
          case "friend":
            this.IsFriend = new bool?(reader.Value == "1");
            continue;
          case "family":
            this.IsFamily = new bool?(reader.Value == "1");
            continue;
          case "revcontact":
            this.IsReverseContact = new bool?(reader.Value == "1");
            continue;
          case "revfriend":
            this.IsReverseFriend = new bool?(reader.Value == "1");
            continue;
          case "revfamily":
            this.IsReverseFamily = new bool?(reader.Value == "1");
            continue;
          default:
            continue;
        }
      }
      reader.Read();
    }
  }
}
