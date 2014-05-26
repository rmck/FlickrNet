// Type: FlickrNet.PlaceTypeInfo
// Assembly: FlickrNet, Version=3.10.0.0, Culture=neutral, PublicKeyToken=2491df59efa5d132
// MVID: 8B8C756F-D5B2-4047-8A8B-B1C1E877A2EF
// Assembly location: D:\RMCK\vs\2014.05.24_FlickrAPI\FlickrNet.dll

using System.Xml;

namespace FlickrNet
{
  public sealed class PlaceTypeInfo : IFlickrParsable
  {
    public int Id { get; set; }

    public string Name { get; set; }

    void IFlickrParsable.Load(XmlReader reader)
    {
      if (!(reader.LocalName != "place_type"))
        ;
      while (reader.MoveToNextAttribute())
      {
        switch (reader.LocalName)
        {
          case "place_type_id":
          case "id":
            this.Id = reader.ReadContentAsInt();
            continue;
          default:
            continue;
        }
      }
      reader.Read();
      this.Name = reader.ReadContentAsString();
      reader.Skip();
    }
  }
}
