// Type: FlickrNet.GalleryPhoto
// Assembly: FlickrNet, Version=3.10.0.0, Culture=neutral, PublicKeyToken=2491df59efa5d132
// MVID: 8B8C756F-D5B2-4047-8A8B-B1C1E877A2EF
// Assembly location: D:\RMCK\vs\2014.05.24_FlickrAPI\FlickrNet.dll

using System.Xml;

namespace FlickrNet
{
  public class GalleryPhoto : Photo, IFlickrParsable
  {
    public string Comment { get; set; }

    void IFlickrParsable.Load(XmlReader reader)
    {
      this.Load(reader, false);
      if (reader.LocalName == "comment")
        this.Comment = reader.ReadElementContentAsString();
      if (reader.LocalName == "description")
        this.Description = reader.ReadElementContentAsString();
      if (reader.NodeType != XmlNodeType.EndElement || !(reader.LocalName == "photo"))
        return;
      reader.Skip();
    }
  }
}
