// Type: FlickrNet.SizeCollection
// Assembly: FlickrNet, Version=3.10.0.0, Culture=neutral, PublicKeyToken=2491df59efa5d132
// MVID: 8B8C756F-D5B2-4047-8A8B-B1C1E877A2EF
// Assembly location: D:\RMCK\vs\2014.05.24_FlickrAPI\FlickrNet.dll

using System.Collections.ObjectModel;
using System.Xml;

namespace FlickrNet
{
  public sealed class SizeCollection : Collection<Size>, IFlickrParsable
  {
    void IFlickrParsable.Load(XmlReader reader)
    {
      reader.Read();
      while (reader.LocalName == "size")
      {
        Size size = new Size();
        size.Load(reader);
        this.Add(size);
      }
      reader.Skip();
    }
  }
}
