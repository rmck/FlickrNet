// Type: FlickrNet.Auth
// Assembly: FlickrNet, Version=3.10.0.0, Culture=neutral, PublicKeyToken=2491df59efa5d132
// MVID: 8B8C756F-D5B2-4047-8A8B-B1C1E877A2EF
// Assembly location: D:\RMCK\vs\2014.05.24_FlickrAPI\FlickrNet.dll

using System;
using System.Xml;

namespace FlickrNet
{
  [Serializable]
  public sealed class Auth : IFlickrParsable
  {
    public string Token { get; set; }

    public AuthLevel Permissions { get; set; }

    public FoundUser User { get; set; }

    void IFlickrParsable.Load(XmlReader reader)
    {
      reader.Read();
      while (reader.LocalName != "auth" && reader.LocalName != "oauth")
      {
        switch (reader.LocalName)
        {
          case "token":
            this.Token = reader.ReadElementContentAsString();
            continue;
          case "perms":
            this.Permissions = (AuthLevel) Enum.Parse(typeof (AuthLevel), reader.ReadElementContentAsString(), true);
            continue;
          case "user":
            this.User = new FoundUser();
            this.User.Load(reader);
            continue;
          default:
            reader.Skip();
            continue;
        }
      }
    }
  }
}
