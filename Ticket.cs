// Type: FlickrNet.Ticket
// Assembly: FlickrNet, Version=3.10.0.0, Culture=neutral, PublicKeyToken=2491df59efa5d132
// MVID: 8B8C756F-D5B2-4047-8A8B-B1C1E877A2EF
// Assembly location: D:\RMCK\vs\2014.05.24_FlickrAPI\FlickrNet.dll

using System.Xml;

namespace FlickrNet
{
  public sealed class Ticket : IFlickrParsable
  {
    public string TicketId { get; set; }

    public string PhotoId { get; set; }

    public bool InvalidTicketId { get; set; }

    public int CompleteStatus { get; set; }

    void IFlickrParsable.Load(XmlReader reader)
    {
      while (reader.MoveToNextAttribute())
      {
        switch (reader.LocalName)
        {
          case "id":
            this.TicketId = reader.Value;
            continue;
          case "invalid":
            this.InvalidTicketId = true;
            continue;
          case "photoid":
            this.PhotoId = reader.Value;
            continue;
          case "complete":
            this.CompleteStatus = reader.ReadContentAsInt();
            continue;
          default:
            continue;
        }
      }
      reader.Skip();
    }
  }
}
