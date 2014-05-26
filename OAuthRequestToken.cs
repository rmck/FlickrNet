// Type: FlickrNet.OAuthRequestToken
// Assembly: FlickrNet, Version=3.10.0.0, Culture=neutral, PublicKeyToken=2491df59efa5d132
// MVID: 8B8C756F-D5B2-4047-8A8B-B1C1E877A2EF
// Assembly location: D:\RMCK\vs\2014.05.24_FlickrAPI\FlickrNet.dll

using System.Collections.Generic;

namespace FlickrNet
{
  public class OAuthRequestToken
  {
    public string Token { get; set; }

    public string TokenSecret { get; set; }

    public static OAuthRequestToken ParseResponse(string response)
    {
      Dictionary<string, string> dictionary = UtilityMethods.StringToDictionary(response);
      return new OAuthRequestToken()
      {
        Token = dictionary["oauth_token"],
        TokenSecret = dictionary["oauth_token_secret"]
      };
    }
  }
}
