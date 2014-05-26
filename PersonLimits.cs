// Type: FlickrNet.PersonLimits
// Assembly: FlickrNet, Version=3.10.0.0, Culture=neutral, PublicKeyToken=2491df59efa5d132
// MVID: 8B8C756F-D5B2-4047-8A8B-B1C1E877A2EF
// Assembly location: D:\RMCK\vs\2014.05.24_FlickrAPI\FlickrNet.dll

using System;
using System.Xml;

namespace FlickrNet
{
  public class PersonLimits : IFlickrParsable
  {
    public int MaximumDisplayPixels { get; set; }

    public long MaximumPhotoUpload { get; set; }

    public long MaximumVideoUpload { get; set; }

    public int MaximumVideoDuration { get; set; }

    void IFlickrParsable.Load(XmlReader reader)
    {
      if (reader == null)
        throw new ArgumentNullException("reader");
      if (!reader.ReadToFollowing("photos"))
        throw new ResponseXmlException("Unable to find \"photos\" element in response.");
      while (reader.MoveToNextAttribute())
      {
        switch (reader.LocalName)
        {
          case "maxdisplaypx":
            this.MaximumDisplayPixels = reader.ReadContentAsInt();
            continue;
          case "maxupload":
            this.MaximumPhotoUpload = reader.ReadContentAsLong();
            continue;
          default:
            continue;
        }
      }
      if (!reader.ReadToFollowing("videos"))
        throw new ResponseXmlException("Unable to find \"videos\" element in response.");
      while (reader.MoveToNextAttribute())
      {
        switch (reader.LocalName)
        {
          case "maxduration":
            this.MaximumVideoDuration = reader.ReadContentAsInt();
            continue;
          case "maxupload":
            this.MaximumVideoUpload = reader.ReadContentAsLong();
            continue;
          default:
            continue;
        }
      }
      reader.Skip();
    }
  }
}
