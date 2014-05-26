﻿// Type: FlickrNet.CsvFile
// Assembly: FlickrNet, Version=3.10.0.0, Culture=neutral, PublicKeyToken=2491df59efa5d132
// MVID: 8B8C756F-D5B2-4047-8A8B-B1C1E877A2EF
// Assembly location: D:\RMCK\vs\2014.05.24_FlickrAPI\FlickrNet.dll

using System;
using System.Globalization;
using System.Xml;

namespace FlickrNet
{
  public sealed class CsvFile : IFlickrParsable
  {
    public string Href { get; set; }

    public DateTime Date { get; set; }

    public string Type { get; set; }

    void IFlickrParsable.Load(XmlReader reader)
    {
      while (reader.MoveToNextAttribute())
      {
        switch (reader.LocalName)
        {
          case "href":
            this.Href = reader.Value;
            continue;
          case "type":
            this.Type = reader.Value;
            continue;
          case "date":
            this.Date = DateTime.Parse(reader.Value, (IFormatProvider) DateTimeFormatInfo.CurrentInfo, DateTimeStyles.None);
            continue;
          default:
            continue;
        }
      }
      reader.Read();
    }
  }
}
