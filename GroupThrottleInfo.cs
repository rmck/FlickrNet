// Type: FlickrNet.GroupThrottleInfo
// Assembly: FlickrNet, Version=3.10.0.0, Culture=neutral, PublicKeyToken=2491df59efa5d132
// MVID: 8B8C756F-D5B2-4047-8A8B-B1C1E877A2EF
// Assembly location: D:\RMCK\vs\2014.05.24_FlickrAPI\FlickrNet.dll

using System;
using System.Globalization;
using System.Xml;

namespace FlickrNet
{
  public sealed class GroupThrottleInfo : IFlickrParsable
  {
    public int Count { get; set; }

    public GroupThrottleMode Mode { get; set; }

    public int Remaining { get; set; }

    private static GroupThrottleMode ParseMode(string mode)
    {
      switch (mode)
      {
        case "day":
          return GroupThrottleMode.PerDay;
        case "week":
          return GroupThrottleMode.PerWeek;
        case "month":
          return GroupThrottleMode.PerMonth;
        case "ever":
          return GroupThrottleMode.Ever;
        case "none":
          return GroupThrottleMode.NoLimit;
        case "disabled":
          return GroupThrottleMode.Disabled;
        default:
          throw new ArgumentException(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Unknown mode found {0}", new object[1]
          {
            (object) mode
          }), "mode");
      }
    }

    void IFlickrParsable.Load(XmlReader reader)
    {
      while (reader.MoveToNextAttribute())
      {
        switch (reader.LocalName)
        {
          case "count":
            this.Count = reader.ReadContentAsInt();
            continue;
          case "mode":
            this.Mode = GroupThrottleInfo.ParseMode(reader.Value);
            continue;
          case "remaining":
            this.Remaining = reader.ReadContentAsInt();
            continue;
          default:
            continue;
        }
      }
      reader.Read();
    }
  }
}
