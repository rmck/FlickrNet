// Type: FlickrNet.ExifTag
// Assembly: FlickrNet, Version=3.10.0.0, Culture=neutral, PublicKeyToken=2491df59efa5d132
// MVID: 8B8C756F-D5B2-4047-8A8B-B1C1E877A2EF
// Assembly location: D:\RMCK\vs\2014.05.24_FlickrAPI\FlickrNet.dll

using System;
using System.Globalization;
using System.Xml;

namespace FlickrNet
{
  public sealed class ExifTag : IFlickrParsable
  {
    public string TagSpace { get; set; }

    public int TagSpaceId { get; set; }

    public string Tag { get; set; }

    public string Label { get; set; }

    public string Raw { get; set; }

    public string Clean { get; set; }

    public string CleanOrRaw
    {
      get
      {
        if (string.IsNullOrEmpty(this.Clean))
          return this.Raw;
        else
          return this.Clean;
      }
    }

    void IFlickrParsable.Load(XmlReader reader)
    {
      if (!(reader.LocalName != "exif"))
        ;
      while (reader.MoveToNextAttribute())
      {
        switch (reader.LocalName)
        {
          case "tagspace":
            this.TagSpace = reader.Value;
            continue;
          case "tagspaceid":
            this.TagSpaceId = int.Parse(reader.Value, (IFormatProvider) NumberFormatInfo.InvariantInfo);
            continue;
          case "tag":
            this.Tag = reader.Value;
            continue;
          case "label":
            this.Label = reader.Value;
            continue;
          default:
            continue;
        }
      }
      reader.Read();
      while (reader.LocalName != "exif")
      {
        switch (reader.LocalName)
        {
          case "raw":
            this.Raw = reader.ReadElementContentAsString();
            continue;
          case "clean":
            this.Clean = reader.ReadElementContentAsString();
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
