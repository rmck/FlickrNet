// Type: FlickrNet.Camera
// Assembly: FlickrNet, Version=3.10.0.0, Culture=neutral, PublicKeyToken=2491df59efa5d132
// MVID: 8B8C756F-D5B2-4047-8A8B-B1C1E877A2EF
// Assembly location: D:\RMCK\vs\2014.05.24_FlickrAPI\FlickrNet.dll

using System.Xml;

namespace FlickrNet
{
  public class Camera : IFlickrParsable
  {
    public string LargeImage { get; set; }

    public string SmallImage { get; set; }

    public string MemoryType { get; set; }

    public string LcdScreenSize { get; set; }

    public string MegaPixels { get; set; }

    public string CameraId { get; set; }

    public string CameraName { get; set; }

    void IFlickrParsable.Load(XmlReader reader)
    {
      if (!(reader.LocalName != "camera"))
        ;
      while (reader.MoveToNextAttribute())
      {
        switch (reader.LocalName)
        {
          case "id":
            this.CameraId = reader.Value;
            continue;
          default:
            continue;
        }
      }
      reader.Read();
      while (reader.LocalName != "camera")
      {
        switch (reader.LocalName)
        {
          case "details":
          case "images":
            reader.Read();
            continue;
          case "name":
            this.CameraName = reader.ReadElementContentAsString();
            continue;
          case "megapixels":
            this.MegaPixels = reader.ReadElementContentAsString();
            continue;
          case "lcd_screen_size":
            this.LcdScreenSize = reader.ReadElementContentAsString();
            continue;
          case "memory_type":
            this.MemoryType = reader.ReadElementContentAsString();
            continue;
          case "small":
            this.SmallImage = reader.ReadElementContentAsString();
            continue;
          case "large":
            this.LargeImage = reader.ReadElementContentAsString();
            continue;
          default:
            continue;
        }
      }
      reader.Skip();
    }
  }
}
