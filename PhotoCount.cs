// Type: FlickrNet.PhotoCount
// Assembly: FlickrNet, Version=3.10.0.0, Culture=neutral, PublicKeyToken=2491df59efa5d132
// MVID: 8B8C756F-D5B2-4047-8A8B-B1C1E877A2EF
// Assembly location: D:\RMCK\vs\2014.05.24_FlickrAPI\FlickrNet.dll

using System;
using System.Globalization;
using System.Text.RegularExpressions;
using System.Xml;

namespace FlickrNet
{
  public sealed class PhotoCount : IFlickrParsable
  {
    public int Count { get; set; }

    public DateTime FromDate { get; set; }

    public DateTime ToDate { get; set; }

    void IFlickrParsable.Load(XmlReader reader)
    {
      if (!(reader.LocalName != "photocount"))
        ;
      while (reader.MoveToNextAttribute())
      {
        switch (reader.LocalName)
        {
          case "count":
            this.Count = int.Parse(reader.Value, (IFormatProvider) NumberFormatInfo.InvariantInfo);
            continue;
          case "fromdate":
            this.FromDate = !Regex.IsMatch(reader.Value, "^\\d+$") ? UtilityMethods.MySqlToDate(reader.Value) : UtilityMethods.UnixTimestampToDate(reader.Value);
            continue;
          case "todate":
            this.ToDate = !Regex.IsMatch(reader.Value, "^\\d+$") ? UtilityMethods.MySqlToDate(reader.Value) : UtilityMethods.UnixTimestampToDate(reader.Value);
            continue;
          default:
            continue;
        }
      }
      reader.Read();
    }
  }
}
