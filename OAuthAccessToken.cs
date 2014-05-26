// Type: FlickrNet.OAuthAccessToken
// Assembly: FlickrNet, Version=3.10.0.0, Culture=neutral, PublicKeyToken=2491df59efa5d132
// MVID: 8B8C756F-D5B2-4047-8A8B-B1C1E877A2EF
// Assembly location: D:\RMCK\vs\2014.05.24_FlickrAPI\FlickrNet.dll

using System;
using System.Collections.Generic;
using System.Xml;

namespace FlickrNet
{
  [Serializable]
  public class OAuthAccessToken : IFlickrParsable
  {
    public string Token { get; set; }

    public string TokenSecret { get; set; }

    public string UserId { get; set; }

    public string Username { get; set; }

    public string FullName { get; set; }

    internal static OAuthAccessToken ParseResponse(string response)
    {
      Dictionary<string, string> dictionary = UtilityMethods.StringToDictionary(response);
      OAuthAccessToken oauthAccessToken = new OAuthAccessToken();
      if (dictionary.ContainsKey("oauth_token"))
        oauthAccessToken.Token = dictionary["oauth_token"];
      if (dictionary.ContainsKey("oauth_token_secret"))
        oauthAccessToken.TokenSecret = dictionary["oauth_token_secret"];
      if (dictionary.ContainsKey("user_nsid"))
        oauthAccessToken.UserId = dictionary["user_nsid"];
      if (dictionary.ContainsKey("fullname"))
        oauthAccessToken.FullName = dictionary["fullname"];
      if (dictionary.ContainsKey("username"))
        oauthAccessToken.Username = dictionary["username"];
      return oauthAccessToken;
    }

    void IFlickrParsable.Load(XmlReader reader)
    {
      if (reader.LocalName != "auth")
        return;
      reader.ReadToDescendant("access_token");
      this.Token = reader.GetAttribute("oauth_token");
      this.TokenSecret = reader.GetAttribute("oauth_token_secret");
    }
  }
}
