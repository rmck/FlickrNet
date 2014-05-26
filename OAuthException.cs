// Type: FlickrNet.OAuthException
// Assembly: FlickrNet, Version=3.10.0.0, Culture=neutral, PublicKeyToken=2491df59efa5d132
// MVID: 8B8C756F-D5B2-4047-8A8B-B1C1E877A2EF
// Assembly location: D:\RMCK\vs\2014.05.24_FlickrAPI\FlickrNet.dll

using System;
using System.Collections.Generic;
using System.IO;
using System.Net;

namespace FlickrNet
{
  public class OAuthException : Exception
  {
    private string mess;

    public string FullResponse { get; set; }

    public Dictionary<string, string> OAuthErrorPameters { get; set; }

    public override string Message
    {
      get
      {
        return this.mess;
      }
    }

    public OAuthException(string response, Exception innerException)
      : base("OAuth Exception", innerException)
    {
      this.FullResponse = response;
      try
      {
        this.OAuthErrorPameters = UtilityMethods.StringToDictionary(response);
      }
      catch (Exception ex)
      {
        throw new Exception("Failed to parse OAuth error message: " + this.FullResponse, innerException);
      }
      this.mess = "OAuth Exception occurred: " + this.OAuthErrorPameters["oauth_problem"];
    }

    public OAuthException(Exception innerException)
      : base("OAuth Exception", innerException)
    {
      WebException webException = innerException as WebException;
      if (webException == null)
        return;
      HttpWebResponse httpWebResponse = webException.Response as HttpWebResponse;
      if (httpWebResponse == null)
        return;
      using (StreamReader streamReader = new StreamReader(httpWebResponse.GetResponseStream()))
      {
        string response = streamReader.ReadToEnd();
        this.FullResponse = response;
        this.OAuthErrorPameters = UtilityMethods.StringToDictionary(response);
        this.mess = "OAuth Exception occurred: " + this.OAuthErrorPameters["oauth_problem"];
        streamReader.Close();
      }
    }
  }
}
