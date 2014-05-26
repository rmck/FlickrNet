// Type: FlickrNet.License
// Assembly: FlickrNet, Version=3.10.0.0, Culture=neutral, PublicKeyToken=2491df59efa5d132
// MVID: 8B8C756F-D5B2-4047-8A8B-B1C1E877A2EF
// Assembly location: D:\RMCK\vs\2014.05.24_FlickrAPI\FlickrNet.dll

using System.Xml;

namespace FlickrNet
{
  public sealed class License : IFlickrParsable
  {
    public LicenseType LicenseId { get; set; }

    public string LicenseName { get; set; }

    public string LicenseUrl { get; set; }

    void IFlickrParsable.Load(XmlReader reader)
    {
      while (reader.MoveToNextAttribute())
      {
        switch (reader.LocalName)
        {
          case "id":
            this.LicenseId = (LicenseType) reader.ReadContentAsInt();
            continue;
          case "name":
            this.LicenseName = reader.Value;
            continue;
          case "url":
            if (!string.IsNullOrEmpty(reader.Value))
            {
              this.LicenseUrl = reader.Value;
              continue;
            }
            else
              continue;
          default:
            continue;
        }
      }
      reader.Read();
    }
  }
}
