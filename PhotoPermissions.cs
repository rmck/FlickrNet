// Type: FlickrNet.PhotoPermissions
// Assembly: FlickrNet, Version=3.10.0.0, Culture=neutral, PublicKeyToken=2491df59efa5d132
// MVID: 8B8C756F-D5B2-4047-8A8B-B1C1E877A2EF
// Assembly location: D:\RMCK\vs\2014.05.24_FlickrAPI\FlickrNet.dll

using System;
using System.Xml;

namespace FlickrNet
{
  public sealed class PhotoPermissions : IFlickrParsable
  {
    public string PhotoId { get; set; }

    public bool IsPublic { get; set; }

    public bool IsFriend { get; set; }

    public bool IsFamily { get; set; }

    public PermissionComment PermissionComment { get; set; }

    public PermissionAddMeta PermissionAddMeta { get; set; }

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
          case "isfamily":
            this.IsFamily = reader.Value == "1";
            continue;
          case "isfriend":
            this.IsFriend = reader.Value == "1";
            continue;
          case "permcomment":
            this.PermissionComment = (PermissionComment) Enum.Parse(typeof (PermissionComment), reader.Value, true);
            continue;
          case "permaddmeta":
            this.PermissionAddMeta = (PermissionAddMeta) Enum.Parse(typeof (PermissionAddMeta), reader.Value, true);
            continue;
          default:
            continue;
        }
      }
      reader.Read();
    }
  }
}
