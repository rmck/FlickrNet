// Type: FlickrNet.Subscription
// Assembly: FlickrNet, Version=3.10.0.0, Culture=neutral, PublicKeyToken=2491df59efa5d132
// MVID: 8B8C756F-D5B2-4047-8A8B-B1C1E877A2EF
// Assembly location: D:\RMCK\vs\2014.05.24_FlickrAPI\FlickrNet.dll

using System;
using System.Xml;

namespace FlickrNet
{
  public sealed class Subscription : IFlickrParsable
  {
    public string Topic { get; set; }

    public string Callback { get; set; }

    public bool IsPending { get; set; }

    public DateTime DateCreated { get; set; }

    public int LeaseSeconds { get; set; }

    public DateTime Expiry { get; set; }

    public int VerifyAttempts { get; set; }

    void IFlickrParsable.Load(XmlReader reader)
    {
      if (!(reader.LocalName != "subscription"))
        ;
      while (reader.MoveToNextAttribute())
      {
        switch (reader.LocalName)
        {
          case "topic":
            this.Topic = reader.Value;
            continue;
          case "callback":
            this.Callback = reader.Value;
            continue;
          case "pending":
            this.IsPending = reader.Value == "1";
            continue;
          case "date_create":
            this.DateCreated = UtilityMethods.UnixTimestampToDate(reader.Value);
            continue;
          case "lease_seconds":
            this.LeaseSeconds = reader.ReadContentAsInt();
            continue;
          case "expiry":
            this.Expiry = UtilityMethods.UnixTimestampToDate(reader.Value);
            continue;
          case "verify_attempts":
            this.VerifyAttempts = reader.ReadContentAsInt();
            continue;
          default:
            continue;
        }
      }
      reader.Read();
    }
  }
}
