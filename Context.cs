// Type: FlickrNet.Context
// Assembly: FlickrNet, Version=3.10.0.0, Culture=neutral, PublicKeyToken=2491df59efa5d132
// MVID: 8B8C756F-D5B2-4047-8A8B-B1C1E877A2EF
// Assembly location: D:\RMCK\vs\2014.05.24_FlickrAPI\FlickrNet.dll

using System.Xml;

namespace FlickrNet
{
  public sealed class Context : IFlickrParsable
  {
    public int Count { get; set; }

    public ContextPhoto NextPhoto { get; set; }

    public ContextPhoto PreviousPhoto { get; set; }

    void IFlickrParsable.Load(XmlReader reader)
    {
      while (reader.NodeType != XmlNodeType.EndElement)
      {
        switch (reader.LocalName)
        {
          case "count":
            this.Count = reader.ReadElementContentAsInt();
            continue;
          case "prevphoto":
            this.PreviousPhoto = new ContextPhoto();
            this.PreviousPhoto.Load(reader);
            if (this.PreviousPhoto.PhotoId == "0")
            {
              this.PreviousPhoto = (ContextPhoto) null;
              continue;
            }
            else
              continue;
          case "nextphoto":
            this.NextPhoto = new ContextPhoto();
            this.NextPhoto.Load(reader);
            if (this.NextPhoto.PhotoId == "0")
            {
              this.NextPhoto = (ContextPhoto) null;
              continue;
            }
            else
              continue;
          default:
            continue;
        }
      }
    }
  }
}
