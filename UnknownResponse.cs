// Type: FlickrNet.UnknownResponse
// Assembly: FlickrNet, Version=3.10.0.0, Culture=neutral, PublicKeyToken=2491df59efa5d132
// MVID: 8B8C756F-D5B2-4047-8A8B-B1C1E877A2EF
// Assembly location: D:\RMCK\vs\2014.05.24_FlickrAPI\FlickrNet.dll

using System.Collections.Generic;
using System.Xml;

namespace FlickrNet
{
  public sealed class UnknownResponse : IFlickrParsable
  {
    public string ResponseXml { get; set; }

    public XmlDocument GetXmlDocument()
    {
      XmlDocument xmlDocument = new XmlDocument();
      xmlDocument.LoadXml(this.ResponseXml);
      return xmlDocument;
    }

    public string GetAttributeValue(string element, string attribute)
    {
      XmlNode xmlNode = this.GetXmlDocument().SelectSingleNode("//" + element + "/@" + attribute);
      if (xmlNode != null)
        return xmlNode.Value;
      else
        return (string) null;
    }

    public string GetElementValue(string element)
    {
      XmlNode xmlNode = this.GetXmlDocument().SelectSingleNode("//" + element + "[1]");
      if (xmlNode != null)
        return xmlNode.InnerText;
      else
        return (string) null;
    }

    void IFlickrParsable.Load(XmlReader reader)
    {
      this.ResponseXml = reader.ReadOuterXml();
    }

    public string[] GetElementArray(string elementName)
    {
      List<string> list = new List<string>();
      foreach (XmlNode xmlNode in this.GetXmlDocument().SelectNodes("//" + elementName))
        list.Add(xmlNode.InnerText);
      return list.ToArray();
    }

    public string[] GetElementArray(string elementName, string attributeName)
    {
      List<string> list = new List<string>();
      foreach (XmlNode xmlNode in this.GetXmlDocument().SelectNodes("//" + elementName + "/@" + attributeName))
        list.Add(xmlNode.InnerText);
      return list.ToArray();
    }
  }
}
