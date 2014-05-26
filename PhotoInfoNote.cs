// Type: FlickrNet.PhotoInfoNote
// Assembly: FlickrNet, Version=3.10.0.0, Culture=neutral, PublicKeyToken=2491df59efa5d132
// MVID: 8B8C756F-D5B2-4047-8A8B-B1C1E877A2EF
// Assembly location: D:\RMCK\vs\2014.05.24_FlickrAPI\FlickrNet.dll

using System;
using System.Drawing;
using System.Globalization;
using System.Xml;

namespace FlickrNet
{
  public sealed class PhotoInfoNote : IFlickrParsable
  {
    public string NoteId { get; set; }

    public string AuthorId { get; set; }

    public string AuthorName { get; set; }

    public int XPosition { get; set; }

    public int YPosition { get; set; }

    public int Width { get; set; }

    public int Height { get; set; }

    public string NoteText { get; set; }

    public System.Drawing.Size Size
    {
      get
      {
        return new System.Drawing.Size(this.Width, this.Height);
      }
    }

    public Point Location
    {
      get
      {
        return new Point(this.XPosition, this.YPosition);
      }
    }

    void IFlickrParsable.Load(XmlReader reader)
    {
      if (!(reader.LocalName != "note"))
        ;
      while (reader.MoveToNextAttribute())
      {
        switch (reader.LocalName)
        {
          case "id":
            this.NoteId = reader.Value;
            continue;
          case "author":
            this.AuthorId = reader.Value;
            continue;
          case "authorname":
            this.AuthorName = reader.Value;
            continue;
          case "x":
            this.XPosition = int.Parse(reader.Value, (IFormatProvider) NumberFormatInfo.InvariantInfo);
            continue;
          case "y":
            this.YPosition = int.Parse(reader.Value, (IFormatProvider) NumberFormatInfo.InvariantInfo);
            continue;
          case "w":
            this.Width = int.Parse(reader.Value, (IFormatProvider) NumberFormatInfo.InvariantInfo);
            continue;
          case "h":
            this.Height = int.Parse(reader.Value, (IFormatProvider) NumberFormatInfo.InvariantInfo);
            continue;
          default:
            continue;
        }
      }
      reader.Read();
      this.NoteText = reader.ReadContentAsString();
      reader.Skip();
    }
  }
}
