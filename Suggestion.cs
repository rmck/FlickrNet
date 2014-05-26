// Type: FlickrNet.Suggestion
// Assembly: FlickrNet, Version=3.10.0.0, Culture=neutral, PublicKeyToken=2491df59efa5d132
// MVID: 8B8C756F-D5B2-4047-8A8B-B1C1E877A2EF
// Assembly location: D:\RMCK\vs\2014.05.24_FlickrAPI\FlickrNet.dll

using System;
using System.Xml;

namespace FlickrNet
{
  public sealed class Suggestion : IFlickrParsable
  {
    public string SuggestionId { get; set; }

    public string PhotoId { get; set; }

    public DateTime DateSuggested { get; set; }

    public string SuggestedByUserId { get; set; }

    public string SuggestedByUserName { get; set; }

    public string Note { get; set; }

    public string WoeId { get; set; }

    public string PlaceId { get; set; }

    public double Latitude { get; set; }

    public double Longitude { get; set; }

    public GeoAccuracy Accuracy { get; set; }

    void IFlickrParsable.Load(XmlReader reader)
    {
      if (reader == null)
        throw new ArgumentNullException("reader");
      if (reader.LocalName != "suggestion")
        return;
      while (reader.MoveToNextAttribute())
      {
        switch (reader.LocalName)
        {
          case "id":
            this.SuggestionId = reader.Value;
            continue;
          case "photo_id":
            this.PhotoId = reader.Value;
            continue;
          case "date_suggested":
            this.DateSuggested = UtilityMethods.UnixTimestampToDate(reader.Value);
            continue;
          default:
            continue;
        }
      }
      reader.Read();
      while (reader.LocalName != "suggestion" && reader.NodeType != XmlNodeType.EndElement)
      {
        switch (reader.LocalName)
        {
          case "suggested_by":
            this.SuggestedByUserId = reader.GetAttribute("nsid");
            this.SuggestedByUserName = reader.GetAttribute("username");
            reader.Skip();
            continue;
          case "note":
            this.Note = reader.ReadElementContentAsString();
            continue;
          case "location":
            while (reader.MoveToNextAttribute())
            {
              switch (reader.LocalName)
              {
                case "woeid":
                  this.WoeId = reader.Value;
                  continue;
                case "latitude":
                  this.Latitude = reader.ReadContentAsDouble();
                  continue;
                case "longitude":
                  this.Longitude = reader.ReadContentAsDouble();
                  continue;
                case "accuracy":
                  this.Accuracy = (GeoAccuracy) reader.ReadContentAsInt();
                  continue;
                default:
                  continue;
              }
            }
            reader.Skip();
            continue;
          default:
            continue;
        }
      }
    }
  }
}
