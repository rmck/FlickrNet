// Type: FlickrNet.PhotoInfoTag
// Assembly: FlickrNet, Version=3.10.0.0, Culture=neutral, PublicKeyToken=2491df59efa5d132
// MVID: 8B8C756F-D5B2-4047-8A8B-B1C1E877A2EF
// Assembly location: D:\RMCK\vs\2014.05.24_FlickrAPI\FlickrNet.dll

using System.Xml;

namespace FlickrNet
{
  public sealed class PhotoInfoTag : IFlickrParsable
  {
    public string TagId { get; set; }

    public string AuthorId { get; set; }

    public string AuthorName { get; set; }

    public string Raw { get; set; }

    public bool IsMachineTag { get; set; }

    public string TagText { get; set; }

    void IFlickrParsable.Load(XmlReader reader)
    {
      if (!(reader.LocalName != "tag"))
        ;
      while (reader.MoveToNextAttribute())
      {
        switch (reader.LocalName)
        {
          case "id":
            this.TagId = reader.Value;
            continue;
          case "author":
            this.AuthorId = reader.Value;
            continue;
          case "authorname":
            this.AuthorName = reader.Value;
            continue;
          case "raw":
            this.Raw = reader.Value;
            continue;
          case "machine_tag":
            this.IsMachineTag = reader.Value == "1";
            continue;
          default:
            continue;
        }
      }
      reader.Read();
      this.TagText = reader.ReadContentAsString();
      reader.Skip();
    }
  }
}
