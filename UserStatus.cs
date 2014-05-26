// Type: FlickrNet.UserStatus
// Assembly: FlickrNet, Version=3.10.0.0, Culture=neutral, PublicKeyToken=2491df59efa5d132
// MVID: 8B8C756F-D5B2-4047-8A8B-B1C1E877A2EF
// Assembly location: D:\RMCK\vs\2014.05.24_FlickrAPI\FlickrNet.dll

using System.Xml;

namespace FlickrNet
{
  public sealed class UserStatus : IFlickrParsable
  {
    public string UserId { get; set; }

    public string UserName { get; set; }

    public bool IsPro { get; set; }

    public long BandwidthMax { get; set; }

    public long BandwidthMaxKB { get; set; }

    public long BandwidthRemaining { get; set; }

    public long BandwidthRemainingKB { get; set; }

    public long BandwidthUsed { get; set; }

    public long BandwidthUsedKB { get; set; }

    public bool IsUnlimited { get; set; }

    public long FileSizeMax { get; set; }

    public long FileSizeMaxKB { get; set; }

    public long FileSizeMaxMB { get; set; }

    public long VideoSizeMax { get; set; }

    public long VideoSizeMaxKB { get; set; }

    public long VideoSizeMaxMB { get; set; }

    public int? SetsCreated { get; set; }

    public int? SetsRemaining { get; set; }

    public int? VideosUploaded { get; set; }

    public int? VideosRemaining { get; set; }

    public double PercentageUsed
    {
      get
      {
        return (double) this.BandwidthUsed * 1.0 / (double) this.BandwidthMax;
      }
    }

    void IFlickrParsable.Load(XmlReader reader)
    {
      this.LoadAttributes(reader);
      this.LoadElements(reader);
    }

    private void LoadElements(XmlReader reader)
    {
      while (reader.LocalName != "user")
      {
        switch (reader.LocalName)
        {
          case "username":
            this.UserName = reader.ReadElementContentAsString();
            continue;
          case "bandwidth":
            this.ParseBandwidth(reader);
            continue;
          case "filesize":
            this.ParseFileSize(reader);
            continue;
          case "sets":
            this.ParseSets(reader);
            continue;
          case "videosize":
            this.ParseVideoSize(reader);
            continue;
          case "videos":
            this.ParseVideos(reader);
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
          default:
            continue;
        }
      }
      reader.Read();
    }

    private void ParseVideos(XmlReader reader)
    {
      while (reader.MoveToNextAttribute())
      {
        switch (reader.LocalName)
        {
          case "uploaded":
            if (!string.IsNullOrEmpty(reader.Value))
            {
              this.VideosUploaded = new int?(reader.ReadContentAsInt());
              continue;
            }
            else
              continue;
          case "remaining":
            if (!string.IsNullOrEmpty(reader.Value) && reader.Value != "lots")
            {
              this.VideosRemaining = new int?(reader.ReadContentAsInt());
              continue;
            }
            else
              continue;
          default:
            continue;
        }
      }
      reader.Read();
    }

    private void ParseVideoSize(XmlReader reader)
    {
      while (reader.MoveToNextAttribute())
      {
        switch (reader.LocalName)
        {
          case "maxbytes":
            this.VideoSizeMax = reader.ReadContentAsLong();
            continue;
          case "maxkb":
            this.VideoSizeMaxKB = reader.ReadContentAsLong();
            continue;
          case "maxmb":
            this.VideoSizeMaxMB = reader.ReadContentAsLong();
            continue;
          default:
            continue;
        }
      }
      reader.Read();
    }

    private void ParseSets(XmlReader reader)
    {
      while (reader.MoveToNextAttribute())
      {
        switch (reader.LocalName)
        {
          case "created":
            if (!string.IsNullOrEmpty(reader.Value))
            {
              this.SetsCreated = new int?(reader.ReadContentAsInt());
              continue;
            }
            else
              continue;
          case "remaining":
            if (!string.IsNullOrEmpty(reader.Value) && reader.Value != "lots")
            {
              this.SetsRemaining = new int?(reader.ReadContentAsInt());
              continue;
            }
            else
              continue;
          default:
            continue;
        }
      }
      reader.Read();
    }

    private void ParseFileSize(XmlReader reader)
    {
      while (reader.MoveToNextAttribute())
      {
        switch (reader.LocalName)
        {
          case "maxbytes":
          case "max":
            this.FileSizeMax = reader.ReadContentAsLong();
            continue;
          case "maxkb":
            this.FileSizeMaxKB = reader.ReadContentAsLong();
            continue;
          case "maxmb":
            this.FileSizeMaxMB = reader.ReadContentAsLong();
            continue;
          default:
            continue;
        }
      }
      reader.Read();
    }

    private void ParseBandwidth(XmlReader reader)
    {
      while (reader.MoveToNextAttribute())
      {
        switch (reader.LocalName)
        {
          case "maxbytes":
          case "max":
            this.BandwidthMax = reader.ReadContentAsLong();
            continue;
          case "maxkb":
            this.BandwidthMaxKB = reader.ReadContentAsLong();
            continue;
          case "used":
          case "usedbytes":
            this.BandwidthUsed = reader.ReadContentAsLong();
            continue;
          case "usedkb":
            this.BandwidthUsedKB = reader.ReadContentAsLong();
            continue;
          case "remainingbytes":
            this.BandwidthRemaining = reader.ReadContentAsLong();
            continue;
          case "remainingkb":
            this.BandwidthRemainingKB = reader.ReadContentAsLong();
            continue;
          case "unlimited":
            this.IsUnlimited = reader.Value == "1";
            continue;
          default:
            continue;
        }
      }
      reader.Read();
    }
  }
}
