// Type: FlickrNet.CsvFileCollection
// Assembly: FlickrNet, Version=3.10.0.0, Culture=neutral, PublicKeyToken=2491df59efa5d132
// MVID: 8B8C756F-D5B2-4047-8A8B-B1C1E877A2EF
// Assembly location: D:\RMCK\vs\2014.05.24_FlickrAPI\FlickrNet.dll

using System.Collections.ObjectModel;
using System.Xml;

namespace FlickrNet
{
  public sealed class CsvFileCollection : Collection<CsvFile>, IFlickrParsable
  {
    void IFlickrParsable.Load(XmlReader reader)
    {
      reader.ReadToDescendant("csv");
      while (reader.LocalName == "csv" && reader.NodeType != XmlNodeType.EndElement)
      {
        CsvFile csvFile = new CsvFile();
        csvFile.Load(reader);
        this.Add(csvFile);
      }
    }
  }
}
