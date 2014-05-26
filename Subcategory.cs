// Type: FlickrNet.Subcategory
// Assembly: FlickrNet, Version=3.10.0.0, Culture=neutral, PublicKeyToken=2491df59efa5d132
// MVID: 8B8C756F-D5B2-4047-8A8B-B1C1E877A2EF
// Assembly location: D:\RMCK\vs\2014.05.24_FlickrAPI\FlickrNet.dll

using System;
using System.Globalization;
using System.Xml;

namespace FlickrNet
{
  public sealed class Subcategory : IFlickrParsable
  {
    public string SubcategoryId { get; set; }

    public string SubcategoryName { get; set; }

    public int GroupCount { get; set; }

    void IFlickrParsable.Load(XmlReader reader)
    {
      if (!(reader.LocalName != "category"))
        ;
      while (reader.MoveToNextAttribute())
      {
        switch (reader.LocalName)
        {
          case "id":
            this.SubcategoryId = reader.Value;
            continue;
          case "name":
            this.SubcategoryName = reader.Value;
            continue;
          case "count":
            this.GroupCount = int.Parse(reader.Value, (IFormatProvider) NumberFormatInfo.InvariantInfo);
            continue;
          default:
            continue;
        }
      }
      reader.Read();
    }
  }
}
