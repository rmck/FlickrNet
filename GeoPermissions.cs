// Type: FlickrNet.GeoPermissions
// Assembly: FlickrNet, Version=3.10.0.0, Culture=neutral, PublicKeyToken=2491df59efa5d132
// MVID: 8B8C756F-D5B2-4047-8A8B-B1C1E877A2EF
// Assembly location: D:\RMCK\vs\2014.05.24_FlickrAPI\FlickrNet.dll

using System.Xml;

namespace FlickrNet
{
  public sealed class GeoPermissions : IFlickrParsable
  {
    public string PhotoId { get; set; }

    public bool IsPublic { get; set; }

    public bool IsContact { get; set; }

    public bool IsFriend { get; set; }

    public bool IsFamily { get; set; }

    void IFlickrParsable.Load(XmlReader reader)
    {
      while (reader.MoveToNextAttribute())
      {
        switch (reader.LocalName)
        {
          case "id":
            this.PhotoId = reader.Value;
            continue;
          case "ispublic":
            this.IsPublic = reader.Value == "1";
            continue;
          case "iscontact":
            this.IsContact = reader.Value == "1";
            continue;
          case "isfamily":
            this.IsFamily = reader.Value == "1";
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
  }
}
