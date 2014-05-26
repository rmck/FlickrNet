// Type: FlickrNet.PhotoPersonCollection
// Assembly: FlickrNet, Version=3.10.0.0, Culture=neutral, PublicKeyToken=2491df59efa5d132
// MVID: 8B8C756F-D5B2-4047-8A8B-B1C1E877A2EF
// Assembly location: D:\RMCK\vs\2014.05.24_FlickrAPI\FlickrNet.dll

using System.Collections.ObjectModel;
using System.Xml;

namespace FlickrNet
{
  public sealed class PhotoPersonCollection : Collection<PhotoPerson>, IFlickrParsable
  {
    public int Total { get; set; }

    public int PhotoWidth { get; set; }

    public int PhotoHeight { get; set; }

    void IFlickrParsable.Load(XmlReader reader)
    {
      if (!(reader.LocalName != "people"))
        ;
      while (reader.MoveToNextAttribute())
      {
        switch (reader.LocalName)
        {
          case "total":
            this.Total = reader.ReadContentAsInt();
            continue;
          case "photo_width":
            this.PhotoWidth = reader.ReadContentAsInt();
            continue;
          case "photo_height":
            this.PhotoHeight = reader.ReadContentAsInt();
            continue;
          default:
            continue;
        }
      }
      reader.Read();
      while (reader.LocalName == "person")
      {
        PhotoPerson photoPerson = new PhotoPerson();
        photoPerson.Load(reader);
        this.Add(photoPerson);
      }
      reader.Skip();
    }
  }
}
