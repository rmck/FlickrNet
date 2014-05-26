// Type: FlickrNet.Institution
// Assembly: FlickrNet, Version=3.10.0.0, Culture=neutral, PublicKeyToken=2491df59efa5d132
// MVID: 8B8C756F-D5B2-4047-8A8B-B1C1E877A2EF
// Assembly location: D:\RMCK\vs\2014.05.24_FlickrAPI\FlickrNet.dll

using System;
using System.Xml;

namespace FlickrNet
{
  public sealed class Institution : IFlickrParsable
  {
    public string InstitutionId { get; set; }

    public DateTime DateLaunched { get; set; }

    public string InstitutionName { get; set; }

    public string SiteUrl { get; set; }

    public string FlickrUrl { get; set; }

    public string LicenseUrl { get; set; }

    void IFlickrParsable.Load(XmlReader reader)
    {
      while (reader.MoveToNextAttribute())
      {
        switch (reader.LocalName)
        {
          case "nsid":
            this.InstitutionId = reader.Value;
            continue;
          case "date_launch":
            this.DateLaunched = UtilityMethods.UnixTimestampToDate(reader.Value);
            continue;
          default:
            continue;
        }
      }
      reader.Read();
      while (reader.NodeType != XmlNodeType.EndElement)
      {
        switch (reader.LocalName)
        {
          case "name":
            this.InstitutionName = reader.ReadElementContentAsString();
            continue;
          case "urls":
            reader.Read();
            while (reader.LocalName == "url")
            {
              string attribute = reader.GetAttribute("type");
              string str = reader.ReadElementContentAsString();
              switch (attribute)
              {
                case "site":
                  this.SiteUrl = str;
                  continue;
                case "flickr":
                  this.FlickrUrl = str;
                  continue;
                case "license":
                  this.LicenseUrl = str;
                  continue;
                default:
                  continue;
              }
            }
            reader.Read();
            continue;
          default:
            continue;
        }
      }
      reader.Read();
    }
  }
}
