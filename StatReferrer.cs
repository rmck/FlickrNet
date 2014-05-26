// Type: FlickrNet.StatReferrer
// Assembly: FlickrNet, Version=3.10.0.0, Culture=neutral, PublicKeyToken=2491df59efa5d132
// MVID: 8B8C756F-D5B2-4047-8A8B-B1C1E877A2EF
// Assembly location: D:\RMCK\vs\2014.05.24_FlickrAPI\FlickrNet.dll

using System;
using System.Globalization;
using System.Xml;

namespace FlickrNet
{
  public sealed class StatReferrer : IFlickrParsable
  {
    public string Url { get; set; }

    public int Views { get; set; }

    public string SearchTerm { get; set; }

    void IFlickrParsable.Load(XmlReader reader)
    {
      if (!(reader.LocalName != "referrer"))
        ;
      while (reader.MoveToNextAttribute())
      {
        switch (reader.LocalName)
        {
          case "url":
            this.Url = reader.Value;
            continue;
          case "searchterm":
            this.SearchTerm = reader.Value;
            continue;
          case "views":
            this.Views = int.Parse(reader.Value, (IFormatProvider) NumberFormatInfo.InvariantInfo);
            continue;
          default:
            continue;
        }
      }
      reader.Skip();
    }
  }
}
