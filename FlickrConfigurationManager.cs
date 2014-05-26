// Type: FlickrNet.FlickrConfigurationManager
// Assembly: FlickrNet, Version=3.10.0.0, Culture=neutral, PublicKeyToken=2491df59efa5d132
// MVID: 8B8C756F-D5B2-4047-8A8B-B1C1E877A2EF
// Assembly location: D:\RMCK\vs\2014.05.24_FlickrAPI\FlickrNet.dll

using System.Configuration;
using System.Xml;

namespace FlickrNet
{
  internal class FlickrConfigurationManager : IConfigurationSectionHandler
  {
    private static string configSection = "flickrNet";
    private static FlickrConfigurationSettings settings;

    public static FlickrConfigurationSettings Settings
    {
      get
      {
        if (FlickrConfigurationManager.settings == null)
          FlickrConfigurationManager.settings = (FlickrConfigurationSettings) ConfigurationManager.GetSection(FlickrConfigurationManager.configSection);
        return FlickrConfigurationManager.settings;
      }
    }

    static FlickrConfigurationManager()
    {
    }

    public object Create(object parent, object configContext, XmlNode section)
    {
      FlickrConfigurationManager.configSection = section.Name;
      return (object) new FlickrConfigurationSettings(section);
    }
  }
}
