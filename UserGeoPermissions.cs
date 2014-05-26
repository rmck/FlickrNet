// Type: FlickrNet.UserGeoPermissions
// Assembly: FlickrNet, Version=3.10.0.0, Culture=neutral, PublicKeyToken=2491df59efa5d132
// MVID: 8B8C756F-D5B2-4047-8A8B-B1C1E877A2EF
// Assembly location: D:\RMCK\vs\2014.05.24_FlickrAPI\FlickrNet.dll

using System.Xml;

namespace FlickrNet
{
  public sealed class UserGeoPermissions : IFlickrParsable
  {
    public string UserId { get; set; }

    public GeoPermissionType GeoPermissions { get; set; }

    public bool ImportGeoExif { get; set; }

    void IFlickrParsable.Load(XmlReader reader)
    {
      while (reader.MoveToNextAttribute())
      {
        switch (reader.LocalName)
        {
          case "nsid":
            this.UserId = reader.Value;
            continue;
          case "geoperms":
            this.GeoPermissions = (GeoPermissionType) reader.ReadContentAsInt();
            continue;
          case "importgeoexif":
            this.ImportGeoExif = reader.Value == "1";
            continue;
          default:
            continue;
        }
      }
    }
  }
}
