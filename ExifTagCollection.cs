// Type: FlickrNet.ExifTagCollection
// Assembly: FlickrNet, Version=3.10.0.0, Culture=neutral, PublicKeyToken=2491df59efa5d132
// MVID: 8B8C756F-D5B2-4047-8A8B-B1C1E877A2EF
// Assembly location: D:\RMCK\vs\2014.05.24_FlickrAPI\FlickrNet.dll

using System.Collections.ObjectModel;
using System.Xml;

namespace FlickrNet
{
  public sealed class ExifTagCollection : Collection<ExifTag>, IFlickrParsable
  {
    public string PhotoId { get; set; }

    public string Secret { get; set; }

    public string Server { get; set; }

    public string Farm { get; set; }

    public string Camera { get; set; }

    void IFlickrParsable.Load(XmlReader reader)
    {
      if (!(reader.LocalName != "photo"))
        ;
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
          case "camera":
            this.Camera = reader.Value;
            continue;
          default:
            continue;
        }
      }
      reader.Read();
      while (reader.LocalName == "exif")
      {
        ExifTag exifTag = new ExifTag();
        exifTag.Load(reader);
        this.Add(exifTag);
      }
      reader.Skip();
    }
  }
}
