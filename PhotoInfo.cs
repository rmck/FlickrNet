// Type: FlickrNet.PhotoInfo
// Assembly: FlickrNet, Version=3.10.0.0, Culture=neutral, PublicKeyToken=2491df59efa5d132
// MVID: 8B8C756F-D5B2-4047-8A8B-B1C1E877A2EF
// Assembly location: D:\RMCK\vs\2014.05.24_FlickrAPI\FlickrNet.dll

using System;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Xml;

namespace FlickrNet
{
  public sealed class PhotoInfo : IFlickrParsable
  {
    public string PhotoId { get; set; }

    public string Secret { get; set; }

    public string Server { get; set; }

    public string Farm { get; set; }

    public string PathAlias { get; set; }

    public string OriginalFormat { get; set; }

    public string OriginalSecret { get; set; }

    public DateTime DateUploaded { get; set; }

    public bool IsFavorite { get; set; }

    public LicenseType License { get; set; }

    public int ViewCount { get; set; }

    public int Rotation { get; set; }

    public MediaType Media { get; set; }

    public SafetyLevel SafetyLevel { get; set; }

    public string OwnerUserId { get; set; }

    public string OwnerUserName { get; set; }

    public string OwnerRealName { get; set; }

    public string OwnerLocation { get; set; }

    public string OwnerIconServer { get; set; }

    public string OwnerIconFarm { get; set; }

    public string OwnerBuddyIcon
    {
      get
      {
        return UtilityMethods.BuddyIcon(this.OwnerIconServer, this.OwnerIconFarm, this.OwnerUserId);
      }
    }

    public string Title { get; set; }

    public string Description { get; set; }

    public bool IsPublic { get; set; }

    public bool IsFriend { get; set; }

    public bool IsFamily { get; set; }

    public bool CanComment { get; set; }

    public bool CanPublicComment { get; set; }

    public bool CanAddMeta { get; set; }

    public bool CanPublicAddMeta { get; set; }

    public bool CanBlog { get; set; }

    public bool CanDownload { get; set; }

    public bool CanPrint { get; set; }

    public bool CanShare { get; set; }

    public int CommentsCount { get; set; }

    public Collection<PhotoInfoNote> Notes { get; set; }

    public Collection<PhotoInfoTag> Tags { get; set; }

    public Collection<PhotoInfoUrl> Urls { get; set; }

    public DateTime DatePosted { get; set; }

    public DateTime DateTaken { get; set; }

    public DateTime DateLastUpdated { get; set; }

    public DateGranularity DateTakenGranularity { get; set; }

    public PermissionComment? PermissionComment { get; set; }

    public PermissionAddMeta? PermissionAddMeta { get; set; }

    public PlaceInfo Location { get; set; }

    public GeoPermissions GeoPermissions { get; set; }

    public VideoInfo VideoInfo { get; set; }

    public bool HasPeople { get; set; }

    public string WebUrl
    {
      get
      {
        return string.Format((IFormatProvider) CultureInfo.InvariantCulture, "https://www.flickr.com/photos/{0}/{1}/", new object[2]
        {
          (object) (this.PathAlias ?? this.OwnerUserId),
          (object) this.PhotoId
        });
      }
    }

    public string SquareThumbnailUrl
    {
      get
      {
        return UtilityMethods.UrlFormat(this, "_s", "jpg");
      }
    }

    public string ThumbnailUrl
    {
      get
      {
        return UtilityMethods.UrlFormat(this, "_t", "jpg");
      }
    }

    public string SmallUrl
    {
      get
      {
        return UtilityMethods.UrlFormat(this, "_m", "jpg");
      }
    }

    public string Small320Url
    {
      get
      {
        return UtilityMethods.UrlFormat(this, "_n", "jpg");
      }
    }

    public string MediumUrl
    {
      get
      {
        return UtilityMethods.UrlFormat(this, string.Empty, "jpg");
      }
    }

    public string Medium640Url
    {
      get
      {
        return UtilityMethods.UrlFormat(this, "_z", "jpg");
      }
    }

    public string Medium800Url
    {
      get
      {
        return UtilityMethods.UrlFormat(this, "_c", "jpg");
      }
    }

    public string LargeUrl
    {
      get
      {
        return UtilityMethods.UrlFormat(this, "_b", "jpg");
      }
    }

    public string LargeSquareUrl
    {
      get
      {
        return UtilityMethods.UrlFormat(this, "_q", "jpg");
      }
    }

    public string OriginalUrl
    {
      get
      {
        if (string.IsNullOrEmpty(this.OriginalFormat))
          return (string) null;
        else
          return UtilityMethods.UrlFormat(this, "_o", this.OriginalFormat);
      }
    }

    public PhotoInfo()
    {
      this.Notes = new Collection<PhotoInfoNote>();
      this.Tags = new Collection<PhotoInfoTag>();
      this.Urls = new Collection<PhotoInfoUrl>();
    }

    void IFlickrParsable.Load(XmlReader reader)
    {
      int num = reader.LocalName != "photo" ? 1 : 0;
      this.LoadAttributes(reader);
      this.LoadElements(reader);
    }

    private void LoadElements(XmlReader reader)
    {
      while (reader.LocalName != "photo")
      {
        switch (reader.LocalName)
        {
          case "owner":
            this.ParseOwner(reader);
            continue;
          case "title":
            this.Title = reader.ReadElementContentAsString();
            continue;
          case "description":
            this.Description = reader.ReadElementContentAsString();
            continue;
          case "visibility":
            this.ParseVisibility(reader);
            continue;
          case "permissions":
            this.ParsePermissions(reader);
            continue;
          case "editability":
            this.ParseEditability(reader);
            continue;
          case "publiceditability":
            this.ParsePublicEditability(reader);
            continue;
          case "dates":
            this.ParseDates(reader);
            continue;
          case "usage":
            this.ParseUsage(reader);
            continue;
          case "comments":
            this.CommentsCount = reader.ReadElementContentAsInt();
            continue;
          case "notes":
            this.ParseNotes(reader);
            continue;
          case "tags":
            this.ParseTags(reader);
            continue;
          case "urls":
            this.ParseUrls(reader);
            continue;
          case "location":
            this.Location = new PlaceInfo();
            this.Location.Load(reader);
            continue;
          case "geoperms":
            this.GeoPermissions = new GeoPermissions();
            this.GeoPermissions.Load(reader);
            continue;
          case "video":
            this.VideoInfo = new VideoInfo();
            this.VideoInfo.Load(reader);
            continue;
          case "people":
            this.HasPeople = reader.GetAttribute("haspeople") == "1";
            reader.Skip();
            continue;
          case "path_alias":
            this.PathAlias = reader.Value;
            continue;
          default:
            reader.Skip();
            continue;
        }
      }
      reader.Skip();
    }

    private void LoadAttributes(XmlReader reader)
    {
      while (reader.MoveToNextAttribute())
      {
        switch (reader.LocalName)
        {
          case "id":
            this.PhotoId = reader.Value;
            continue;
          case "secret":
            this.Secret = reader.Value;
            continue;
          case "server":
            this.Server = reader.Value;
            continue;
          case "farm":
            this.Farm = reader.Value;
            continue;
          case "originalformat":
            this.OriginalFormat = reader.Value;
            continue;
          case "originalsecret":
            this.OriginalSecret = reader.Value;
            continue;
          case "dateuploaded":
            this.DateUploaded = UtilityMethods.UnixTimestampToDate(reader.Value);
            continue;
          case "isfavorite":
            this.IsFavorite = reader.Value == "1";
            continue;
          case "license":
            this.License = (LicenseType) int.Parse(reader.Value, (IFormatProvider) NumberFormatInfo.InvariantInfo);
            continue;
          case "views":
            this.ViewCount = int.Parse(reader.Value, (IFormatProvider) NumberFormatInfo.InvariantInfo);
            continue;
          case "rotation":
            this.Rotation = int.Parse(reader.Value, (IFormatProvider) NumberFormatInfo.InvariantInfo);
            continue;
          case "media":
            this.Media = reader.Value == "photo" ? MediaType.Photos : MediaType.Videos;
            continue;
          case "safety_level":
            this.SafetyLevel = (SafetyLevel) reader.ReadContentAsInt();
            continue;
          default:
            continue;
        }
      }
      reader.Read();
    }

    private void ParseUrls(XmlReader reader)
    {
      reader.Read();
      while (reader.LocalName == "url")
      {
        PhotoInfoUrl photoInfoUrl = new PhotoInfoUrl();
        photoInfoUrl.Load(reader);
        this.Urls.Add(photoInfoUrl);
      }
    }

    private void ParseTags(XmlReader reader)
    {
      reader.Read();
      while (reader.LocalName == "tag")
      {
        PhotoInfoTag photoInfoTag = new PhotoInfoTag();
        photoInfoTag.Load(reader);
        this.Tags.Add(photoInfoTag);
      }
    }

    private void ParseNotes(XmlReader reader)
    {
      reader.Read();
      while (reader.LocalName == "note")
      {
        PhotoInfoNote photoInfoNote = new PhotoInfoNote();
        photoInfoNote.Load(reader);
        this.Notes.Add(photoInfoNote);
      }
    }

    private void ParseUsage(XmlReader reader)
    {
      while (reader.MoveToNextAttribute())
      {
        switch (reader.LocalName)
        {
          case "canblog":
            this.CanBlog = reader.Value == "1";
            continue;
          case "candownload":
            this.CanDownload = reader.Value == "1";
            continue;
          case "canprint":
            this.CanPrint = reader.Value == "1";
            continue;
          case "canshare":
            this.CanShare = reader.Value == "1";
            continue;
          default:
            continue;
        }
      }
      reader.Read();
    }

    private void ParseVisibility(XmlReader reader)
    {
      while (reader.MoveToNextAttribute())
      {
        switch (reader.LocalName)
        {
          case "isfamily":
            this.IsFamily = reader.Value == "1";
            continue;
          case "ispublic":
            this.IsPublic = reader.Value == "1";
            continue;
          case "isfriend":
            this.IsFriend = reader.Value == "1";
            continue;
          default:
            continue;
        }
      }
      reader.Read();
    }

    private void ParseEditability(XmlReader reader)
    {
      while (reader.MoveToNextAttribute())
      {
        switch (reader.LocalName)
        {
          case "cancomment":
            this.CanComment = reader.Value == "1";
            continue;
          case "canaddmeta":
            this.CanAddMeta = reader.Value == "1";
            continue;
          default:
            continue;
        }
      }
      reader.Read();
    }

    private void ParsePublicEditability(XmlReader reader)
    {
      while (reader.MoveToNextAttribute())
      {
        switch (reader.LocalName)
        {
          case "cancomment":
            this.CanPublicComment = reader.Value == "1";
            continue;
          case "canaddmeta":
            this.CanPublicAddMeta = reader.Value == "1";
            continue;
          default:
            continue;
        }
      }
      reader.Read();
    }

    private void ParsePermissions(XmlReader reader)
    {
      while (reader.MoveToNextAttribute())
      {
        switch (reader.LocalName)
        {
          case "permcomment":
            this.PermissionComment = new PermissionComment?((PermissionComment) int.Parse(reader.Value, (IFormatProvider) NumberFormatInfo.InvariantInfo));
            continue;
          case "permaddmeta":
            this.PermissionAddMeta = new PermissionAddMeta?((PermissionAddMeta) int.Parse(reader.Value, (IFormatProvider) NumberFormatInfo.InvariantInfo));
            continue;
          default:
            continue;
        }
      }
      reader.Read();
    }

    private void ParseDates(XmlReader reader)
    {
      while (reader.MoveToNextAttribute())
      {
        switch (reader.LocalName)
        {
          case "posted":
            this.DatePosted = UtilityMethods.UnixTimestampToDate(reader.Value);
            continue;
          case "taken":
            this.DateTaken = UtilityMethods.ParseDateWithGranularity(reader.Value);
            continue;
          case "takengranularity":
            this.DateTakenGranularity = (DateGranularity) int.Parse(reader.Value, (IFormatProvider) NumberFormatInfo.InvariantInfo);
            continue;
          case "lastupdate":
            this.DateLastUpdated = UtilityMethods.UnixTimestampToDate(reader.Value);
            continue;
          default:
            continue;
        }
      }
      reader.Read();
    }

    private void ParseOwner(XmlReader reader)
    {
      while (reader.MoveToNextAttribute())
      {
        switch (reader.LocalName)
        {
          case "nsid":
            this.OwnerUserId = reader.Value;
            continue;
          case "username":
            this.OwnerUserName = reader.Value;
            continue;
          case "realname":
            this.OwnerRealName = reader.Value;
            continue;
          case "location":
            this.OwnerLocation = reader.Value;
            continue;
          case "iconserver":
            this.OwnerIconServer = reader.Value;
            continue;
          case "iconfarm":
            this.OwnerIconFarm = reader.Value;
            continue;
          case "path_alias":
            this.PathAlias = reader.Value;
            continue;
          default:
            continue;
        }
      }
      reader.Read();
    }
  }
}
