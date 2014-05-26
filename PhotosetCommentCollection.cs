// Type: FlickrNet.PhotosetCommentCollection
// Assembly: FlickrNet, Version=3.10.0.0, Culture=neutral, PublicKeyToken=2491df59efa5d132
// MVID: 8B8C756F-D5B2-4047-8A8B-B1C1E877A2EF
// Assembly location: D:\RMCK\vs\2014.05.24_FlickrAPI\FlickrNet.dll

using System.Collections.ObjectModel;
using System.Xml;

namespace FlickrNet
{
  public sealed class PhotosetCommentCollection : Collection<PhotoComment>, IFlickrParsable
  {
    public string PhotosetId { get; set; }

    void IFlickrParsable.Load(XmlReader reader)
    {
      if (!(reader.LocalName != "comments"))
        ;
      while (reader.MoveToNextAttribute())
      {
        switch (reader.LocalName)
        {
          case "photoset_id":
            this.PhotosetId = reader.Value;
            continue;
          default:
            continue;
        }
      }
      reader.Read();
      while (reader.LocalName == "comment")
      {
        PhotoComment photoComment = new PhotoComment();
        photoComment.Load(reader);
        this.Add(photoComment);
      }
      reader.Skip();
    }
  }
}
