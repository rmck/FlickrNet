// Type: FlickrNet.ShapeData
// Assembly: FlickrNet, Version=3.10.0.0, Culture=neutral, PublicKeyToken=2491df59efa5d132
// MVID: 8B8C756F-D5B2-4047-8A8B-B1C1E877A2EF
// Assembly location: D:\RMCK\vs\2014.05.24_FlickrAPI\FlickrNet.dll

using System;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Xml;

namespace FlickrNet
{
  public sealed class ShapeData : IFlickrParsable
  {
    public DateTime DateCreated { get; set; }

    public double Alpha { get; set; }

    public int PointCount { get; set; }

    public int EdgeCount { get; set; }

    public bool HasDonutHole { get; set; }

    public bool IsDonutHole { get; set; }

    public Collection<Collection<PointD>> PolyLines { get; set; }

    public Collection<string> Urls { get; set; }

    public ShapeData()
    {
      this.PolyLines = new Collection<Collection<PointD>>();
      this.Urls = new Collection<string>();
    }

    void IFlickrParsable.Load(XmlReader reader)
    {
      while (reader.MoveToNextAttribute())
      {
        switch (reader.LocalName)
        {
          case "created":
            this.DateCreated = UtilityMethods.UnixTimestampToDate(reader.Value);
            continue;
          case "alpha":
            this.Alpha = reader.ReadContentAsDouble();
            continue;
          case "count_points":
            this.PointCount = reader.ReadContentAsInt();
            continue;
          case "count_edges":
            this.EdgeCount = reader.ReadContentAsInt();
            continue;
          case "has_donuthole":
            this.HasDonutHole = reader.Value == "1";
            continue;
          case "is_donuthole":
            this.IsDonutHole = reader.Value == "1";
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
          case "polylines":
            reader.Read();
            while (reader.LocalName == "polyline")
            {
              Collection<PointD> collection = new Collection<PointD>();
              string str1 = reader.ReadElementContentAsString();
              string str2 = str1;
              char[] chArray1 = new char[1]
              {
                ' '
              };
              foreach (string str3 in str2.Split(chArray1))
              {
                char[] chArray2 = new char[1]
                {
                  ','
                };
                string[] strArray = str3.Split(chArray2);
                if (strArray.Length != 2)
                  throw new ParsingException("Invalid polypoint found in polyline : '" + str1 + "'");
                collection.Add(new PointD(double.Parse(strArray[0], (IFormatProvider) NumberFormatInfo.InvariantInfo), double.Parse(strArray[1], (IFormatProvider) NumberFormatInfo.InvariantInfo)));
              }
              this.PolyLines.Add(collection);
            }
            reader.Read();
            continue;
          case "urls":
            reader.Skip();
            continue;
          default:
            continue;
        }
      }
      reader.Read();
    }
  }
}
