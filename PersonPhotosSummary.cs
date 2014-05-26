// Type: FlickrNet.PersonPhotosSummary
// Assembly: FlickrNet, Version=3.10.0.0, Culture=neutral, PublicKeyToken=2491df59efa5d132
// MVID: 8B8C756F-D5B2-4047-8A8B-B1C1E877A2EF
// Assembly location: D:\RMCK\vs\2014.05.24_FlickrAPI\FlickrNet.dll

using System;
using System.Xml;

namespace FlickrNet
{
  public sealed class PersonPhotosSummary : IFlickrParsable
  {
    public DateTime FirstDate { get; set; }

    public DateTime FirstTakenDate { get; set; }

    public int PhotoCount { get; set; }

    public int Views { get; set; }

    void IFlickrParsable.Load(XmlReader reader)
    {
      reader.Read();
      while (reader.LocalName != "photos")
      {
        switch (reader.LocalName)
        {
          case "firstdatetaken":
            this.FirstTakenDate = UtilityMethods.ParseDateWithGranularity(reader.ReadElementContentAsString());
            continue;
          case "firstdate":
            this.FirstDate = UtilityMethods.UnixTimestampToDate(reader.ReadElementContentAsString());
            continue;
          case "count":
            this.PhotoCount = reader.ReadElementContentAsInt();
            continue;
          case "views":
            this.Views = reader.ReadElementContentAsInt();
            continue;
          default:
            reader.Skip();
            continue;
        }
      }
      reader.Read();
    }
  }
}
