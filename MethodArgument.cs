// Type: FlickrNet.MethodArgument
// Assembly: FlickrNet, Version=3.10.0.0, Culture=neutral, PublicKeyToken=2491df59efa5d132
// MVID: 8B8C756F-D5B2-4047-8A8B-B1C1E877A2EF
// Assembly location: D:\RMCK\vs\2014.05.24_FlickrAPI\FlickrNet.dll

using System.Xml;

namespace FlickrNet
{
  public sealed class MethodArgument : IFlickrParsable
  {
    public string Name { get; set; }

    public bool IsOptional { get; set; }

    public string Description { get; set; }

    void IFlickrParsable.Load(XmlReader reader)
    {
      if (reader.LocalName != "argument")
        return;
      while (reader.MoveToNextAttribute())
      {
        switch (reader.LocalName)
        {
          case "name":
            this.Name = reader.Value;
            continue;
          case "optional":
            this.IsOptional = reader.Value == "1";
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
