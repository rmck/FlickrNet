// Type: FlickrNet.FoundUser
// Assembly: FlickrNet, Version=3.10.0.0, Culture=neutral, PublicKeyToken=2491df59efa5d132
// MVID: 8B8C756F-D5B2-4047-8A8B-B1C1E877A2EF
// Assembly location: D:\RMCK\vs\2014.05.24_FlickrAPI\FlickrNet.dll

using System.Xml;

namespace FlickrNet
{
  public sealed class FoundUser : IFlickrParsable
  {
    public string UserId { get; set; }

    public string UserName { get; set; }

    public string FullName { get; set; }

    void IFlickrParsable.Load(XmlReader reader)
    {
      if (!(reader.LocalName != "user"))
        ;
      while (reader.MoveToNextAttribute())
      {
        switch (reader.LocalName)
        {
          case "nsid":
          case "id":
            this.UserId = reader.Value;
            continue;
          case "username":
            this.UserName = reader.Value;
            continue;
          case "fullname":
            this.FullName = reader.Value;
            continue;
          default:
            continue;
        }
      }
      reader.Read();
      if (reader.NodeType == XmlNodeType.EndElement)
        return;
      this.UserName = reader.ReadElementContentAsString();
      reader.Skip();
    }
  }
}
