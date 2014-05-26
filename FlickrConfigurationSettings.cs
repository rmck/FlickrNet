// Type: FlickrNet.FlickrConfigurationSettings
// Assembly: FlickrNet, Version=3.10.0.0, Culture=neutral, PublicKeyToken=2491df59efa5d132
// MVID: 8B8C756F-D5B2-4047-8A8B-B1C1E877A2EF
// Assembly location: D:\RMCK\vs\2014.05.24_FlickrAPI\FlickrNet.dll

using System;
using System.Configuration;
using System.Globalization;
using System.Xml;

namespace FlickrNet
{
  internal class FlickrConfigurationSettings
  {
    private TimeSpan cacheTimeout = TimeSpan.MinValue;
    private string apiKey;
    private string apiSecret;
    private string apiToken;
    private int cacheSize;
    private string proxyAddress;
    private int proxyPort;
    private bool proxyDefined;
    private string proxyUsername;
    private string proxyPassword;
    private string proxyDomain;
    private string cacheLocation;
    private bool cacheDisabled;
    private SupportedService service;

    public string ApiKey
    {
      get
      {
        return this.apiKey;
      }
    }

    public string SharedSecret
    {
      get
      {
        return this.apiSecret;
      }
    }

    public string ApiToken
    {
      get
      {
        return this.apiToken;
      }
    }

    public bool CacheDisabled
    {
      get
      {
        return this.cacheDisabled;
      }
    }

    public int CacheSize
    {
      get
      {
        return this.cacheSize;
      }
    }

    public TimeSpan CacheTimeout
    {
      get
      {
        return this.cacheTimeout;
      }
    }

    public string CacheLocation
    {
      get
      {
        return this.cacheLocation;
      }
    }

    public SupportedService Service
    {
      get
      {
        return this.service;
      }
    }

    public bool IsProxyDefined
    {
      get
      {
        return this.proxyDefined;
      }
    }

    public string ProxyIPAddress
    {
      get
      {
        return this.proxyAddress;
      }
    }

    public int ProxyPort
    {
      get
      {
        return this.proxyPort;
      }
    }

    public string ProxyUsername
    {
      get
      {
        return this.proxyUsername;
      }
    }

    public string ProxyPassword
    {
      get
      {
        return this.proxyPassword;
      }
    }

    public string ProxyDomain
    {
      get
      {
        return this.proxyDomain;
      }
    }

    public FlickrConfigurationSettings(XmlNode configNode)
    {
      if (configNode == null)
        throw new ArgumentNullException("configNode");
      foreach (XmlAttribute xmlAttribute in (XmlNamedNodeMap) configNode.Attributes)
      {
        switch (xmlAttribute.Name)
        {
          case "apiKey":
            this.apiKey = xmlAttribute.Value;
            continue;
          case "secret":
            this.apiSecret = xmlAttribute.Value;
            continue;
          case "token":
            this.apiToken = xmlAttribute.Value;
            continue;
          case "cacheDisabled":
            try
            {
              this.cacheDisabled = bool.Parse(xmlAttribute.Value);
              continue;
            }
            catch (FormatException ex)
            {
              throw new ConfigurationErrorsException("cacheDisbled should be \"true\" or \"false\"", (Exception) ex, configNode);
            }
          case "cacheSize":
            try
            {
              this.cacheSize = int.Parse(xmlAttribute.Value, (IFormatProvider) NumberFormatInfo.InvariantInfo);
              continue;
            }
            catch (FormatException ex)
            {
              throw new ConfigurationErrorsException("cacheSize should be integer value", (Exception) ex, configNode);
            }
          case "cacheTimeout":
            try
            {
              this.cacheTimeout = TimeSpan.Parse(xmlAttribute.Value);
              continue;
            }
            catch (FormatException ex)
            {
              throw new ConfigurationErrorsException("cacheTimeout should be TimeSpan value ([d:]HH:mm:ss)", (Exception) ex, configNode);
            }
          case "cacheLocation":
            this.cacheLocation = xmlAttribute.Value;
            continue;
          case "service":
            try
            {
              this.service = (SupportedService) Enum.Parse(typeof (SupportedService), xmlAttribute.Value, true);
              continue;
            }
            catch (ArgumentException ex)
            {
              throw new ConfigurationErrorsException("service must be one of the supported services (See SupportedServices enum)", (Exception) ex, configNode);
            }
          default:
            throw new ConfigurationErrorsException(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Unknown attribute '{0}' in flickrNet node", new object[1]
            {
              (object) xmlAttribute.Name
            }), configNode);
        }
      }
      foreach (XmlNode proxy in configNode.ChildNodes)
      {
        switch (proxy.Name)
        {
          case "proxy":
            this.ProcessProxyNode(proxy, configNode);
            continue;
          default:
            throw new ConfigurationErrorsException(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Unknown node '{0}' in flickrNet node", new object[1]
            {
              (object) proxy.Name
            }), configNode);
        }
      }
    }

    private void ProcessProxyNode(XmlNode proxy, XmlNode configNode)
    {
      if (proxy.ChildNodes.Count > 0)
        throw new ConfigurationErrorsException("proxy element does not support child elements");
      this.proxyDefined = true;
      foreach (XmlAttribute xmlAttribute in (XmlNamedNodeMap) proxy.Attributes)
      {
        switch (xmlAttribute.Name)
        {
          case "ipaddress":
            this.proxyAddress = xmlAttribute.Value;
            continue;
          case "port":
            try
            {
              this.proxyPort = int.Parse(xmlAttribute.Value, (IFormatProvider) CultureInfo.InvariantCulture);
              continue;
            }
            catch (FormatException ex)
            {
              throw new ConfigurationErrorsException("proxy port should be integer value", (Exception) ex, configNode);
            }
          case "username":
            this.proxyUsername = xmlAttribute.Value;
            continue;
          case "password":
            this.proxyPassword = xmlAttribute.Value;
            continue;
          case "domain":
            this.proxyDomain = xmlAttribute.Value;
            continue;
          default:
            throw new ConfigurationErrorsException(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Unknown attribute '{0}' in flickrNet/proxy node", new object[1]
            {
              (object) xmlAttribute.Name
            }), configNode);
        }
      }
      if (this.proxyAddress == null)
        throw new ConfigurationErrorsException("proxy ipaddress is mandatory if you specify the proxy element");
      if (this.proxyPort == 0)
        throw new ConfigurationErrorsException("proxy port is mandatory if you specify the proxy element");
      if (this.proxyUsername != null && this.proxyPassword == null)
        throw new ConfigurationErrorsException("proxy password must be specified if proxy username is specified");
      if (this.proxyUsername == null && this.proxyPassword != null)
        throw new ConfigurationErrorsException("proxy username must be specified if proxy password is specified");
      if (this.proxyDomain != null && this.proxyUsername == null)
        throw new ConfigurationErrorsException("proxy username/password must be specified if proxy domain is specified");
    }
  }
}
