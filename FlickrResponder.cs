// Type: FlickrNet.FlickrResponder
// Assembly: FlickrNet, Version=3.10.0.0, Culture=neutral, PublicKeyToken=2491df59efa5d132
// MVID: 8B8C756F-D5B2-4047-8A8B-B1C1E877A2EF
// Assembly location: D:\RMCK\vs\2014.05.24_FlickrAPI\FlickrNet.dll

using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Net;
using System.Text;

namespace FlickrNet
{
  public static class FlickrResponder
  {
    private const string PostContentType = "application/x-www-form-urlencoded";

    public static string OAuthCalculateAuthHeader(Dictionary<string, string> parameters)
    {
      StringBuilder stringBuilder = new StringBuilder("OAuth ");
      foreach (KeyValuePair<string, string> keyValuePair in parameters)
      {
        if (keyValuePair.Key.StartsWith("oauth"))
          stringBuilder.Append(keyValuePair.Key + "=\"" + Uri.EscapeDataString(keyValuePair.Value) + "\",");
      }
      return ((object) stringBuilder.Remove(stringBuilder.Length - 1, 1)).ToString();
    }

    public static string OAuthCalculatePostData(Dictionary<string, string> parameters)
    {
      string str = string.Empty;
      foreach (KeyValuePair<string, string> keyValuePair in parameters)
      {
        if (!keyValuePair.Key.StartsWith("oauth"))
          str = str + keyValuePair.Key + "=" + Uri.EscapeDataString(keyValuePair.Value) + "&";
      }
      return str;
    }

    public static void GetDataResponseAsync(Flickr flickr, string baseUrl, Dictionary<string, string> parameters, Action<FlickrResult<string>> callback)
    {
      if (parameters.ContainsKey("oauth_consumer_key"))
        FlickrResponder.GetDataResponseOAuthAsync(flickr, baseUrl, parameters, callback);
      else
        FlickrResponder.GetDataResponseNormalAsync(flickr, baseUrl, parameters, callback);
    }

    private static void GetDataResponseNormalAsync(Flickr flickr, string baseUrl, Dictionary<string, string> parameters, Action<FlickrResult<string>> callback)
    {
      string method = flickr.CurrentService == SupportedService.Zooomr ? "GET" : "POST";
      string data = string.Empty;
      foreach (KeyValuePair<string, string> keyValuePair in parameters)
        data = data + keyValuePair.Key + "=" + Uri.EscapeDataString(keyValuePair.Value) + "&";
      if (method == "GET" && data.Length > 2000)
        method = "POST";
      if (method == "GET")
        FlickrResponder.DownloadDataAsync(method, baseUrl + "?" + data, (string) null, (string) null, (string) null, callback);
      else
        FlickrResponder.DownloadDataAsync(method, baseUrl, data, "application/x-www-form-urlencoded", (string) null, callback);
    }

    private static void GetDataResponseOAuthAsync(Flickr flickr, string baseUrl, Dictionary<string, string> parameters, Action<FlickrResult<string>> callback)
    {
      if (parameters.ContainsKey("api_key"))
        parameters.Remove("api_key");
      if (parameters.ContainsKey("api_sig"))
        parameters.Remove("api_sig");
      if (!string.IsNullOrEmpty(flickr.OAuthAccessToken) && !parameters.ContainsKey("oauth_token"))
        parameters.Add("oauth_token", flickr.OAuthAccessToken);
      if (!string.IsNullOrEmpty(flickr.OAuthAccessTokenSecret) && !parameters.ContainsKey("oauth_signature"))
      {
        string str = flickr.OAuthCalculateSignature("POST", baseUrl, parameters, flickr.OAuthAccessTokenSecret);
        parameters.Add("oauth_signature", str);
      }
      string data = FlickrResponder.OAuthCalculatePostData(parameters);
      string authHeader = FlickrResponder.OAuthCalculateAuthHeader(parameters);
      try
      {
        FlickrResponder.DownloadDataAsync("POST", baseUrl, data, "application/x-www-form-urlencoded", authHeader, callback);
      }
      catch (WebException ex)
      {
        HttpWebResponse httpWebResponse = ex.Response as HttpWebResponse;
        if (httpWebResponse == null)
          throw;
        else if (httpWebResponse.StatusCode != HttpStatusCode.BadRequest && httpWebResponse.StatusCode != HttpStatusCode.Unauthorized)
        {
          throw;
        }
        else
        {
          using (StreamReader streamReader = new StreamReader(httpWebResponse.GetResponseStream()))
          {
            string response = streamReader.ReadToEnd();
            streamReader.Close();
            throw new OAuthException(response, (Exception) ex);
          }
        }
      }
    }

    private static void DownloadDataAsync(string method, string baseUrl, string data, string contentType, string authHeader, Action<FlickrResult<string>> callback)
    {
      WebClient webClient = new WebClient();
      webClient.Encoding = Encoding.UTF8;
      if (!string.IsNullOrEmpty(contentType))
        ((NameValueCollection) webClient.Headers)["Content-Type"] = contentType;
      if (!string.IsNullOrEmpty(authHeader))
        ((NameValueCollection) webClient.Headers)["Authorization"] = authHeader;
      if (method == "POST")
      {
        webClient.UploadStringCompleted += (UploadStringCompletedEventHandler) ((sender, e) =>
        {
          FlickrResult<string> local_0 = new FlickrResult<string>();
          if (e.Error != null)
          {
            local_0.Error = e.Error;
            callback(local_0);
          }
          else
          {
            local_0.Result = e.Result;
            callback(local_0);
          }
        });
        webClient.UploadStringAsync(new Uri(baseUrl), data);
      }
      else
      {
        webClient.DownloadStringCompleted += (DownloadStringCompletedEventHandler) ((sender, e) =>
        {
          FlickrResult<string> local_0 = new FlickrResult<string>();
          if (e.Error != null)
          {
            local_0.Error = e.Error;
            callback(local_0);
          }
          else
          {
            local_0.Result = e.Result;
            callback(local_0);
          }
        });
        webClient.DownloadStringAsync(new Uri(baseUrl));
      }
    }

    public static string GetDataResponse(Flickr flickr, string baseUrl, Dictionary<string, string> parameters)
    {
      if (parameters.ContainsKey("oauth_consumer_key"))
        return FlickrResponder.GetDataResponseOAuth(flickr, baseUrl, parameters);
      else
        return FlickrResponder.GetDataResponseNormal(flickr, baseUrl, parameters);
    }

    private static string GetDataResponseNormal(Flickr flickr, string baseUrl, Dictionary<string, string> parameters)
    {
      string method = flickr.CurrentService == SupportedService.Zooomr ? "GET" : "POST";
      string data = string.Empty;
      foreach (KeyValuePair<string, string> keyValuePair in parameters)
        data = data + keyValuePair.Key + "=" + Uri.EscapeDataString(keyValuePair.Value) + "&";
      if (method == "GET" && data.Length > 2000)
        method = "POST";
      if (method == "GET")
        return FlickrResponder.DownloadData(method, baseUrl + "?" + data, (string) null, (string) null, (string) null);
      else
        return FlickrResponder.DownloadData(method, baseUrl, data, "application/x-www-form-urlencoded", (string) null);
    }

    private static string GetDataResponseOAuth(Flickr flickr, string baseUrl, Dictionary<string, string> parameters)
    {
      string method = "POST";
      if (parameters.ContainsKey("api_key"))
        parameters.Remove("api_key");
      if (parameters.ContainsKey("api_sig"))
        parameters.Remove("api_sig");
      if (!string.IsNullOrEmpty(flickr.OAuthAccessToken) && !parameters.ContainsKey("oauth_token"))
        parameters.Add("oauth_token", flickr.OAuthAccessToken);
      if (!string.IsNullOrEmpty(flickr.OAuthAccessTokenSecret) && !parameters.ContainsKey("oauth_signature"))
      {
        string str = flickr.OAuthCalculateSignature(method, baseUrl, parameters, flickr.OAuthAccessTokenSecret);
        parameters.Add("oauth_signature", str);
      }
      string data = FlickrResponder.OAuthCalculatePostData(parameters);
      string authHeader = FlickrResponder.OAuthCalculateAuthHeader(parameters);
      try
      {
        return FlickrResponder.DownloadData(method, baseUrl, data, "application/x-www-form-urlencoded", authHeader);
      }
      catch (WebException ex)
      {
        if (ex.Status != WebExceptionStatus.ProtocolError)
        {
          throw;
        }
        else
        {
          HttpWebResponse httpWebResponse = ex.Response as HttpWebResponse;
          if (httpWebResponse == null)
          {
            throw;
          }
          else
          {
            string response = (string) null;
            using (Stream responseStream = httpWebResponse.GetResponseStream())
            {
              if (responseStream != null)
              {
                using (StreamReader streamReader = new StreamReader(responseStream))
                {
                  response = streamReader.ReadToEnd();
                  streamReader.Close();
                }
              }
            }
            if (httpWebResponse.StatusCode == HttpStatusCode.BadRequest || httpWebResponse.StatusCode == HttpStatusCode.Unauthorized)
              throw new OAuthException(response, (Exception) ex);
            if (!string.IsNullOrEmpty(response))
              throw new WebException("WebException occurred with the following body content: " + response, (Exception) ex, ex.Status, ex.Response);
            throw;
          }
        }
      }
    }

    private static string DownloadData(string method, string baseUrl, string data, string contentType, string authHeader)
    {
      using (WebClient webClient = new WebClient())
      {
        webClient.Encoding = Encoding.UTF8;
        if (!string.IsNullOrEmpty(contentType))
          ((NameValueCollection) webClient.Headers).Add("Content-Type", contentType);
        if (!string.IsNullOrEmpty(authHeader))
          ((NameValueCollection) webClient.Headers).Add("Authorization", authHeader);
        if (method == "POST")
          return webClient.UploadString(baseUrl, data);
        else
          return webClient.DownloadString(baseUrl);
      }
    }
  }
}
