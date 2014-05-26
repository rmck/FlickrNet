// Type: FlickrNet.PermissionComment
// Assembly: FlickrNet, Version=3.10.0.0, Culture=neutral, PublicKeyToken=2491df59efa5d132
// MVID: 8B8C756F-D5B2-4047-8A8B-B1C1E877A2EF
// Assembly location: D:\RMCK\vs\2014.05.24_FlickrAPI\FlickrNet.dll

using System;
using System.Xml.Serialization;

namespace FlickrNet
{
  [Serializable]
  public enum PermissionComment
  {
    [XmlEnum("0")] Nobody,
    [XmlEnum("1")] FriendsAndFamily,
    [XmlEnum("2")] ContactsOnly,
    [XmlEnum("3")] Everybody,
  }
}
