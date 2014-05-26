// Type: FlickrNet.MethodError
// Assembly: FlickrNet, Version=3.10.0.0, Culture=neutral, PublicKeyToken=2491df59efa5d132
// MVID: 8B8C756F-D5B2-4047-8A8B-B1C1E877A2EF
// Assembly location: D:\RMCK\vs\2014.05.24_FlickrAPI\FlickrNet.dll

using System;
using System.Globalization;
using System.Xml;

namespace FlickrNet
{
  public sealed class MethodError : IFlickrParsable
  {
    public int Code { get; set; }

    public string Message { get; set; }

    public string Description { get; set; }

    void IFlickrParsable.Load(XmlReader reader)
    {
      if (reader.LocalName != "error")
        return;
      while (reader.MoveToNextAttribute())
      {
        switch (reader.LocalName)
        {
          case "code":
            this.Code = int.Parse(reader.Value, (IFormatProvider) NumberFormatInfo.InvariantInfo);
            continue;
          case "message":
            this.Message = reader.Value;
            continue;
          default:
            continue;
        }
      }
      reader.Read();
      if (reader.NodeType != XmlNodeType.Text)
        return;
      this.Description = reader.ReadContentAsString();
      reader.Read();
    }
  }
}
