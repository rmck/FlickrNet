// Type: FlickrNet.Flickr
// Assembly: FlickrNet, Version=3.10.0.0, Culture=neutral, PublicKeyToken=2491df59efa5d132
// MVID: 8B8C756F-D5B2-4047-8A8B-B1C1E877A2EF
// Assembly location: D:\RMCK\vs\2014.05.24_FlickrAPI\FlickrNet.dll

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Globalization;
using System.IO;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Xml;

namespace FlickrNet
{
  public class Flickr
  {
    private static SupportedService defaultService = SupportedService.Flickr;
    private static string[] uploadUrl = new string[3]
    {
      "https://up.flickr.com/services/upload/",
      "http://beta.zooomr.com/bluenote/api/upload",
      "http://www.23hq.com/services/upload/"
    };
    private static string[] replaceUrl = new string[3]
    {
      "https://up.flickr.com/services/replace/",
      "http://beta.zooomr.com/bluenote/api/replace",
      "http://www.23hq.com/services/replace/"
    };
    private static string[] authUrl = new string[3]
    {
      "https://www.flickr.com/services/auth/",
      "http://beta.zooomr.com/auth/",
      "http://www.23hq.com/services/auth/"
    };
    private readonly Uri[] baseUri = new Uri[3]
    {
      new Uri("https://api.flickr.com/services/rest/"),
      new Uri("http://beta.zooomr.com/bluenote/api/rest"),
      new Uri("http://www.23hq.com/services/rest/")
    };
    private int timeout = 100000;
    private static bool isServiceSet;
    private SupportedService service;
    private string apiKey;
    private string apiToken;
    private string sharedSecret;
    private string lastRequest;
    private string lastResponse;
    private WebProxy proxy;

    public Uri BaseUri
    {
      get
      {
        return this.baseUri[(int) this.service];
      }
    }

    private string UploadUrl
    {
      get
      {
        return Flickr.uploadUrl[(int) this.service];
      }
    }

    private string ReplaceUrl
    {
      get
      {
        return Flickr.replaceUrl[(int) this.service];
      }
    }

    private string AuthUrl
    {
      get
      {
        return Flickr.authUrl[(int) this.service];
      }
    }

    public string ApiKey
    {
      get
      {
        return this.apiKey;
      }
      set
      {
        this.apiKey = value == null || value.Length == 0 ? (string) null : value;
      }
    }

    public string ApiSecret
    {
      get
      {
        return this.sharedSecret;
      }
      set
      {
        this.sharedSecret = value == null || value.Length == 0 ? (string) null : value;
      }
    }

    [Obsolete("Use OAuthToken and OAuthTokenSecret now.")]
    public string AuthToken
    {
      get
      {
        return this.apiToken;
      }
      set
      {
        this.apiToken = value == null || value.Length == 0 ? (string) null : value;
      }
    }

    public string OAuthAccessToken { get; set; }

    public string OAuthAccessTokenSecret { get; set; }

    public static bool CacheDisabled
    {
      get
      {
        return Cache.CacheDisabled;
      }
      set
      {
        Cache.CacheDisabled = value;
      }
    }

    public bool InstanceCacheDisabled { get; set; }

    public static TimeSpan CacheTimeout
    {
      get
      {
        return Cache.CacheTimeout;
      }
      set
      {
        Cache.CacheTimeout = value;
      }
    }

    public static string CacheLocation
    {
      get
      {
        return Cache.CacheLocation;
      }
      set
      {
        Cache.CacheLocation = value;
      }
    }

    public static long CacheSizeLimit
    {
      get
      {
        return Cache.CacheSizeLimit;
      }
      set
      {
        Cache.CacheSizeLimit = value;
      }
    }

    public static SupportedService DefaultService
    {
      get
      {
        if (!Flickr.isServiceSet && FlickrConfigurationManager.Settings != null)
        {
          Flickr.defaultService = FlickrConfigurationManager.Settings.Service;
          Flickr.isServiceSet = true;
        }
        return Flickr.defaultService;
      }
      set
      {
        Flickr.defaultService = value;
        Flickr.isServiceSet = true;
      }
    }

    public SupportedService CurrentService
    {
      get
      {
        return this.service;
      }
      set
      {
        this.service = value;
        if (this.service != SupportedService.Zooomr)
          return;
        ServicePointManager.Expect100Continue = false;
      }
    }

    public int HttpTimeout
    {
      get
      {
        return this.timeout;
      }
      set
      {
        this.timeout = value;
      }
    }

    public bool IsAuthenticated
    {
      get
      {
        if (this.sharedSecret != null)
          return this.apiToken != null;
        else
          return false;
      }
    }

    public string LastResponse
    {
      get
      {
        return this.lastResponse;
      }
    }

    public string LastRequest
    {
      get
      {
        return this.lastRequest;
      }
    }

    public WebProxy Proxy
    {
      get
      {
        return this.proxy;
      }
      set
      {
        this.proxy = value;
      }
    }

    public event EventHandler<UploadProgressEventArgs> OnUploadProgress;

    static Flickr()
    {
    }

    public Flickr()
    {
      FlickrConfigurationSettings settings = FlickrConfigurationManager.Settings;
      if (settings == null)
        return;
      if (settings.CacheSize != 0)
        Flickr.CacheSizeLimit = (long) settings.CacheSize;
      if (settings.CacheTimeout != TimeSpan.MinValue)
        Flickr.CacheTimeout = settings.CacheTimeout;
      this.ApiKey = settings.ApiKey;
      this.AuthToken = settings.ApiToken;
      this.ApiSecret = settings.SharedSecret;
      if (settings.IsProxyDefined)
      {
        this.Proxy = new WebProxy();
        this.Proxy.Address = new Uri(string.Concat(new object[4]
        {
          (object) "http://",
          (object) settings.ProxyIPAddress,
          (object) ":",
          (object) settings.ProxyPort
        }));
        if (settings.ProxyUsername != null && settings.ProxyUsername.Length > 0)
          this.Proxy.Credentials = (ICredentials) new NetworkCredential()
          {
            UserName = settings.ProxyUsername,
            Password = settings.ProxyPassword,
            Domain = settings.ProxyDomain
          };
      }
      this.InstanceCacheDisabled = Flickr.CacheDisabled;
      this.CurrentService = Flickr.DefaultService;
    }

    public Flickr(string apiKey)
      : this(apiKey, (string) null, (string) null)
    {
    }

    public Flickr(string apiKey, string sharedSecret)
      : this(apiKey, sharedSecret, (string) null)
    {
    }

    public Flickr(string apiKey, string sharedSecret, string token)
      : this()
    {
      this.ApiKey = apiKey;
      this.ApiSecret = sharedSecret;
      this.AuthToken = token;
    }

    public void ActivityUserPhotosAsync(Action<FlickrResult<ActivityItemCollection>> callback)
    {
      this.ActivityUserPhotosAsync((string) null, 0, 0, callback);
    }

    public void ActivityUserPhotosAsync(int page, int perPage, Action<FlickrResult<ActivityItemCollection>> callback)
    {
      this.ActivityUserPhotosAsync((string) null, page, perPage, callback);
    }

    public void ActivityUserPhotosAsync(int timePeriod, string timeType, Action<FlickrResult<ActivityItemCollection>> callback)
    {
      this.ActivityUserPhotosAsync(timePeriod, timeType, 0, 0, callback);
    }

    public void ActivityUserPhotosAsync(int timePeriod, string timeType, int page, int perPage, Action<FlickrResult<ActivityItemCollection>> callback)
    {
      if (timePeriod == 0)
        throw new ArgumentOutOfRangeException("timePeriod", "Time Period should be greater than 0");
      if (timeType == null)
        throw new ArgumentNullException("timeType");
      if (timeType != "d" && timeType != "h")
        throw new ArgumentOutOfRangeException("timeType", "Time type must be 'd' or 'h'");
      this.ActivityUserPhotosAsync((string) (object) timePeriod + (object) timeType, page, perPage, callback);
    }

    private void ActivityUserPhotosAsync(string timeframe, int page, int perPage, Action<FlickrResult<ActivityItemCollection>> callback)
    {
      this.CheckRequiresAuthentication();
      Dictionary<string, string> parameters = new Dictionary<string, string>();
      parameters.Add("method", "flickr.activity.userPhotos");
      if (timeframe != null && timeframe.Length > 0)
        parameters.Add("timeframe", timeframe);
      if (page > 0)
        parameters.Add("page", page.ToString((IFormatProvider) NumberFormatInfo.InvariantInfo));
      if (perPage > 0)
        parameters.Add("per_page", perPage.ToString((IFormatProvider) NumberFormatInfo.InvariantInfo));
      this.GetResponseAsync<ActivityItemCollection>(parameters, callback);
    }

    public void ActivityUserCommentsAsync(int page, int perPage, Action<FlickrResult<ActivityItemCollection>> callback)
    {
      this.CheckRequiresAuthentication();
      Dictionary<string, string> parameters = new Dictionary<string, string>();
      parameters.Add("method", "flickr.activity.userComments");
      if (page > 0)
        parameters.Add("page", page.ToString((IFormatProvider) NumberFormatInfo.InvariantInfo));
      if (perPage > 0)
        parameters.Add("per_page", perPage.ToString((IFormatProvider) NumberFormatInfo.InvariantInfo));
      this.GetResponseAsync<ActivityItemCollection>(parameters, callback);
    }

    [Obsolete("Use OAuth now.")]
    public void AuthGetFrobAsync(Action<FlickrResult<string>> callback)
    {
      this.CheckSigned();
      this.GetResponseAsync<UnknownResponse>(new Dictionary<string, string>()
      {
        {
          "method",
          "flickr.auth.getFrob"
        }
      }, (Action<FlickrResult<UnknownResponse>>) (r =>
      {
        FlickrResult<string> local_0 = new FlickrResult<string>();
        local_0.HasError = r.HasError;
        if (r.HasError)
          local_0.Error = r.Error;
        else
          local_0.Result = r.Result.GetElementValue("frob");
        callback(local_0);
      }));
    }

    [Obsolete("Use OAuth now.")]
    public void AuthGetTokenAsync(string frob, Action<FlickrResult<Auth>> callback)
    {
      this.CheckSigned();
      this.GetResponseAsync<Auth>(new Dictionary<string, string>()
      {
        {
          "method",
          "flickr.auth.getToken"
        },
        {
          "frob",
          frob
        }
      }, (Action<FlickrResult<Auth>>) (r =>
      {
        if (!r.HasError)
          this.AuthToken = r.Result.Token;
        callback(r);
      }));
    }

    [Obsolete("Use OAuth now.")]
    public void AuthGetFullTokenAsync(string miniToken, Action<FlickrResult<Auth>> callback)
    {
      this.CheckSigned();
      this.GetResponseAsync<Auth>(new Dictionary<string, string>()
      {
        {
          "method",
          "flickr.auth.getFullToken"
        },
        {
          "mini_token",
          miniToken.Replace("-", string.Empty)
        }
      }, (Action<FlickrResult<Auth>>) (r =>
      {
        if (!r.HasError)
          this.AuthToken = r.Result.Token;
        callback(r);
      }));
    }

    public void AuthCheckTokenAsync(Action<FlickrResult<Auth>> callback)
    {
      this.AuthCheckTokenAsync(this.AuthToken, callback);
    }

    public void AuthCheckTokenAsync(string token, Action<FlickrResult<Auth>> callback)
    {
      this.CheckSigned();
      this.GetResponseAsync<Auth>(new Dictionary<string, string>()
      {
        {
          "method",
          "flickr.auth.checkToken"
        },
        {
          "auth_token",
          token
        }
      }, (Action<FlickrResult<Auth>>) (r =>
      {
        if (!r.HasError)
          this.AuthToken = r.Result.Token;
        callback(r);
      }));
    }

    public void AuthOAuthGetAccessTokenAsync(Action<FlickrResult<OAuthAccessToken>> callback)
    {
      this.CheckRequiresAuthentication();
      this.GetResponseAsync<OAuthAccessToken>(new Dictionary<string, string>()
      {
        {
          "method",
          "flickr.auth.oauth.getAccessToken"
        }
      }, (Action<FlickrResult<OAuthAccessToken>>) (r =>
      {
        if (!r.HasError)
        {
          this.OAuthAccessToken = r.Result.Token;
          this.OAuthAccessTokenSecret = r.Result.TokenSecret;
          this.AuthToken = (string) null;
        }
        callback(r);
      }));
    }

    public void AuthOAuthCheckTokenAsync(Action<FlickrResult<Auth>> callback)
    {
      this.CheckRequiresAuthentication();
      this.GetResponseAsync<Auth>(new Dictionary<string, string>()
      {
        {
          "method",
          "flickr.auth.oauth.checkToken"
        }
      }, callback);
    }

    [Obsolete("Use OAuth now.")]
    public string AuthCalcRevokeUrl(string appToken)
    {
      return "https://www.flickr.com/services/auth/revoke.gne?token=" + appToken;
    }

    [Obsolete("Use OAuth now.")]
    public string AuthCalcUrl(string frob, AuthLevel authLevel)
    {
      if (this.sharedSecret == null)
        throw new SignatureRequiredException();
      string str = UtilityMethods.MD5Hash(this.sharedSecret + "api_key" + this.apiKey + "frob" + frob + "perms" + UtilityMethods.AuthLevelToString(authLevel));
      return this.AuthUrl + "?api_key=" + this.apiKey + "&perms=" + UtilityMethods.AuthLevelToString(authLevel) + "&frob=" + frob + "&api_sig=" + str;
    }

    [Obsolete("Use OAuth now.")]
    public string AuthCalcUrlMobile(string frob, AuthLevel authLevel)
    {
      return this.AuthCalcUrl(frob, authLevel).Replace("www.flickr.com", "m.flickr.com");
    }

    [Obsolete("Use OAuth now.")]
    public string AuthCalcWebUrl(AuthLevel authLevel)
    {
      return this.AuthCalcWebUrl(authLevel, (string) null);
    }

    [Obsolete("Use OAuth now.")]
    public string AuthCalcWebUrl(AuthLevel authLevel, string extra)
    {
      this.CheckApiKey();
      this.CheckSigned();
      string str1 = this.sharedSecret + "api_key" + this.apiKey;
      string str2 = this.AuthUrl + "?api_key=" + this.apiKey + "&perms=" + UtilityMethods.AuthLevelToString(authLevel);
      if (!string.IsNullOrEmpty(extra))
      {
        str1 = str1 + "extra" + extra;
        str2 = str2 + "&extra=" + Uri.EscapeDataString(extra);
      }
      string str3 = UtilityMethods.MD5Hash(str1 + "perms" + UtilityMethods.AuthLevelToString(authLevel));
      return str2 + "&api_sig=" + str3;
    }

    [Obsolete("Use OAuth now.")]
    public string AuthCalcWebUrlMobile(AuthLevel authLevel)
    {
      return this.AuthCalcWebUrl(authLevel).Replace("www.flickr.com", "m.flickr.com");
    }

    public void BlogsGetListAsync(Action<FlickrResult<BlogCollection>> callback)
    {
      this.CheckRequiresAuthentication();
      this.GetResponseAsync<BlogCollection>(new Dictionary<string, string>()
      {
        {
          "method",
          "flickr.blogs.getList"
        }
      }, callback);
    }

    public void BlogsGetServicesAsync(Action<FlickrResult<BlogServiceCollection>> callback)
    {
      this.GetResponseAsync<BlogServiceCollection>(new Dictionary<string, string>()
      {
        {
          "method",
          "flickr.blogs.getServices"
        }
      }, callback);
    }

    public void BlogsPostPhotoAsync(string blogId, string photoId, string title, string description, Action<FlickrResult<NoResponse>> callback)
    {
      this.BlogsPostPhotoAsync(blogId, photoId, title, description, (string) null, callback);
    }

    public void BlogsPostPhotoAsync(string blogId, string photoId, string title, string description, string blogPassword, Action<FlickrResult<NoResponse>> callback)
    {
      Dictionary<string, string> parameters = new Dictionary<string, string>();
      parameters.Add("method", "flickr.blogs.postPhoto");
      parameters.Add("blog_id", blogId);
      parameters.Add("photo_id", photoId);
      parameters.Add("title", title);
      parameters.Add("description", description);
      if (blogPassword != null)
        parameters.Add("blog_password", blogPassword);
      this.GetResponseAsync<NoResponse>(parameters, callback);
    }

    public void CamerasGetBrandsAsync(Action<FlickrResult<BrandCollection>> callback)
    {
      this.GetResponseAsync<BrandCollection>(new Dictionary<string, string>()
      {
        {
          "method",
          "flickr.cameras.getBrands"
        }
      }, callback);
    }

    public void CamerasGetBrandModelsAsync(string brandId, Action<FlickrResult<CameraCollection>> callback)
    {
      this.GetResponseAsync<CameraCollection>(new Dictionary<string, string>()
      {
        {
          "method",
          "flickr.cameras.getBrandModels"
        },
        {
          "brand",
          brandId
        }
      }, callback);
    }

    public BrandCollection CamerasGetBrands()
    {
      return this.GetResponseCache<BrandCollection>(new Dictionary<string, string>()
      {
        {
          "method",
          "flickr.cameras.getBrands"
        }
      });
    }

    public CameraCollection CamerasGetBrandModels(string brandId)
    {
      return this.GetResponseCache<CameraCollection>(new Dictionary<string, string>()
      {
        {
          "method",
          "flickr.cameras.getBrandModels"
        },
        {
          "brand",
          brandId
        }
      });
    }

    public void CollectionsGetInfoAsync(string collectionId, Action<FlickrResult<CollectionInfo>> callback)
    {
      this.CheckRequiresAuthentication();
      this.GetResponseAsync<CollectionInfo>(new Dictionary<string, string>()
      {
        {
          "method",
          "flickr.collections.getInfo"
        },
        {
          "collection_id",
          collectionId
        }
      }, callback);
    }

    public void CollectionsGetTreeAsync(Action<FlickrResult<CollectionCollection>> callback)
    {
      this.CollectionsGetTreeAsync((string) null, (string) null, callback);
    }

    public void CollectionsGetTreeAsync(string collectionId, string userId, Action<FlickrResult<CollectionCollection>> callback)
    {
      if (string.IsNullOrEmpty(userId))
        this.CheckRequiresAuthentication();
      Dictionary<string, string> parameters = new Dictionary<string, string>();
      parameters.Add("method", "flickr.collections.getTree");
      if (collectionId != null)
        parameters.Add("collection_id", collectionId);
      if (userId != null)
        parameters.Add("user_id", userId);
      this.GetResponseAsync<CollectionCollection>(parameters, callback);
    }

    public InstitutionCollection CommonsGetInstitutions()
    {
      return this.GetResponseCache<InstitutionCollection>(new Dictionary<string, string>()
      {
        {
          "method",
          "flickr.commons.getInstitutions"
        }
      });
    }

    public void CommonsGetInstitutionsAsync(Action<FlickrResult<InstitutionCollection>> callback)
    {
      this.GetResponseAsync<InstitutionCollection>(new Dictionary<string, string>()
      {
        {
          "method",
          "flickr.commons.getInstitutions"
        }
      }, callback);
    }

    public void ContactsGetListAsync(Action<FlickrResult<ContactCollection>> callback)
    {
      this.ContactsGetListAsync((string) null, 0, 0, callback);
    }

    public void ContactsGetListAsync(int page, int perPage, Action<FlickrResult<ContactCollection>> callback)
    {
      this.ContactsGetListAsync((string) null, page, perPage, callback);
    }

    public void ContactsGetListAsync(string filter, Action<FlickrResult<ContactCollection>> callback)
    {
      this.ContactsGetListAsync(filter, 0, 0, callback);
    }

    public void ContactsGetListAsync(string filter, int page, int perPage, Action<FlickrResult<ContactCollection>> callback)
    {
      this.CheckRequiresAuthentication();
      Dictionary<string, string> parameters = new Dictionary<string, string>();
      parameters.Add("method", "flickr.contacts.getList");
      if (!string.IsNullOrEmpty(filter))
        parameters.Add("filter", filter);
      if (page > 0)
        parameters.Add("page", page.ToString((IFormatProvider) NumberFormatInfo.InvariantInfo));
      if (perPage > 0)
        parameters.Add("per_page", perPage.ToString((IFormatProvider) NumberFormatInfo.InvariantInfo));
      this.GetResponseAsync<ContactCollection>(parameters, callback);
    }

    public void ContactsGetListRecentlyUploadedAsync(Action<FlickrResult<ContactCollection>> callback)
    {
      this.ContactsGetListRecentlyUploadedAsync(DateTime.MinValue, (string) null, callback);
    }

    public void ContactsGetListRecentlyUploadedAsync(string filter, Action<FlickrResult<ContactCollection>> callback)
    {
      this.ContactsGetListRecentlyUploadedAsync(DateTime.MinValue, filter, callback);
    }

    public void ContactsGetListRecentlyUploadedAsync(DateTime dateLastUpdated, Action<FlickrResult<ContactCollection>> callback)
    {
      this.ContactsGetListRecentlyUploadedAsync(dateLastUpdated, (string) null, callback);
    }

    public void ContactsGetListRecentlyUploadedAsync(DateTime dateLastUpdated, string filter, Action<FlickrResult<ContactCollection>> callback)
    {
      this.CheckRequiresAuthentication();
      Dictionary<string, string> parameters = new Dictionary<string, string>();
      parameters.Add("method", "flickr.contacts.getListRecentlyUploaded");
      if (dateLastUpdated != DateTime.MinValue)
        parameters.Add("date_lastupload", UtilityMethods.DateToUnixTimestamp(dateLastUpdated));
      if (!string.IsNullOrEmpty(filter))
        parameters.Add("filter", filter);
      this.GetResponseAsync<ContactCollection>(parameters, callback);
    }

    public void ContactsGetPublicListAsync(string userId, Action<FlickrResult<ContactCollection>> callback)
    {
      this.ContactsGetPublicListAsync(userId, 0, 0, callback);
    }

    public void ContactsGetPublicListAsync(string userId, int page, int perPage, Action<FlickrResult<ContactCollection>> callback)
    {
      Dictionary<string, string> parameters = new Dictionary<string, string>();
      parameters.Add("method", "flickr.contacts.getPublicList");
      parameters.Add("user_id", userId);
      if (page > 0)
        parameters.Add("page", page.ToString((IFormatProvider) NumberFormatInfo.InvariantInfo));
      if (perPage > 0)
        parameters.Add("per_page", perPage.ToString((IFormatProvider) NumberFormatInfo.InvariantInfo));
      this.GetResponseAsync<ContactCollection>(parameters, callback);
    }

    public void ContactsGetTaggingSuggestionsAsync(Action<FlickrResult<ContactCollection>> callback)
    {
      this.ContactsGetTaggingSuggestionsAsync(0, 0, callback);
    }

    public void ContactsGetTaggingSuggestionsAsync(int page, int perPage, Action<FlickrResult<ContactCollection>> callback)
    {
      this.CheckRequiresAuthentication();
      Dictionary<string, string> parameters = new Dictionary<string, string>();
      parameters.Add("method", "flickr.contacts.getTaggingSuggestions");
      if (page > 0)
        parameters.Add("page", page.ToString((IFormatProvider) NumberFormatInfo.InvariantInfo));
      if (perPage > 0)
        parameters.Add("per_page", perPage.ToString((IFormatProvider) NumberFormatInfo.InvariantInfo));
      this.GetResponseAsync<ContactCollection>(parameters, callback);
    }

    public void FavoritesAddAsync(string photoId, Action<FlickrResult<NoResponse>> callback)
    {
      this.GetResponseAsync<NoResponse>(new Dictionary<string, string>()
      {
        {
          "method",
          "flickr.favorites.add"
        },
        {
          "photo_id",
          photoId
        }
      }, callback);
    }

    public void FavoritesRemoveAsync(string photoId, Action<FlickrResult<NoResponse>> callback)
    {
      this.GetResponseAsync<NoResponse>(new Dictionary<string, string>()
      {
        {
          "method",
          "flickr.favorites.remove"
        },
        {
          "photo_id",
          photoId
        }
      }, callback);
    }

    public void FavoritesGetListAsync(Action<FlickrResult<PhotoCollection>> callback)
    {
      this.FavoritesGetListAsync((string) null, DateTime.MinValue, DateTime.MinValue, PhotoSearchExtras.None, 0, 0, callback);
    }

    public void FavoritesGetListAsync(PhotoSearchExtras extras, Action<FlickrResult<PhotoCollection>> callback)
    {
      this.FavoritesGetListAsync((string) null, DateTime.MinValue, DateTime.MinValue, extras, 0, 0, callback);
    }

    public void FavoritesGetListAsync(int page, int perPage, Action<FlickrResult<PhotoCollection>> callback)
    {
      this.FavoritesGetListAsync((string) null, DateTime.MinValue, DateTime.MinValue, PhotoSearchExtras.None, page, perPage, callback);
    }

    public void FavoritesGetListAsync(PhotoSearchExtras extras, int page, int perPage, Action<FlickrResult<PhotoCollection>> callback)
    {
      this.FavoritesGetListAsync((string) null, DateTime.MinValue, DateTime.MinValue, extras, page, perPage, callback);
    }

    public void FavoritesGetListAsync(string userId, Action<FlickrResult<PhotoCollection>> callback)
    {
      this.FavoritesGetListAsync(userId, DateTime.MinValue, DateTime.MinValue, PhotoSearchExtras.None, 0, 0, callback);
    }

    public void FavoritesGetListAsync(string userId, PhotoSearchExtras extras, Action<FlickrResult<PhotoCollection>> callback)
    {
      this.FavoritesGetListAsync(userId, DateTime.MinValue, DateTime.MinValue, extras, 0, 0, callback);
    }

    public void FavoritesGetListAsync(string userId, DateTime minFavoriteDate, DateTime maxFavoriteDate, PhotoSearchExtras extras, int page, int perPage, Action<FlickrResult<PhotoCollection>> callback)
    {
      this.CheckRequiresAuthentication();
      Dictionary<string, string> parameters = new Dictionary<string, string>();
      parameters.Add("method", "flickr.favorites.getList");
      if (userId != null)
        parameters.Add("user_id", userId);
      if (minFavoriteDate != DateTime.MinValue)
        parameters.Add("min_fav_date", UtilityMethods.DateToUnixTimestamp(minFavoriteDate));
      if (maxFavoriteDate != DateTime.MinValue)
        parameters.Add("max_fav_date", UtilityMethods.DateToUnixTimestamp(maxFavoriteDate));
      if (extras != PhotoSearchExtras.None)
        parameters.Add("extras", UtilityMethods.ExtrasToString(extras));
      if (perPage > 0)
        parameters.Add("per_page", perPage.ToString((IFormatProvider) NumberFormatInfo.InvariantInfo));
      if (page > 0)
        parameters.Add("page", page.ToString((IFormatProvider) NumberFormatInfo.InvariantInfo));
      this.GetResponseAsync<PhotoCollection>(parameters, callback);
    }

    public void FavoritesGetPublicListAsync(string userId, Action<FlickrResult<PhotoCollection>> callback)
    {
      this.FavoritesGetPublicListAsync(userId, DateTime.MinValue, DateTime.MinValue, PhotoSearchExtras.None, 0, 0, callback);
    }

    public void FavoritesGetPublicListAsync(string userId, DateTime minFavoriteDate, DateTime maxFavoriteDate, PhotoSearchExtras extras, int page, int perPage, Action<FlickrResult<PhotoCollection>> callback)
    {
      Dictionary<string, string> parameters = new Dictionary<string, string>();
      parameters.Add("method", "flickr.favorites.getPublicList");
      parameters.Add("user_id", userId);
      if (minFavoriteDate != DateTime.MinValue)
        parameters.Add("min_fav_date", UtilityMethods.DateToUnixTimestamp(minFavoriteDate));
      if (maxFavoriteDate != DateTime.MinValue)
        parameters.Add("max_fav_date", UtilityMethods.DateToUnixTimestamp(maxFavoriteDate));
      if (extras != PhotoSearchExtras.None)
        parameters.Add("extras", UtilityMethods.ExtrasToString(extras));
      if (perPage > 0)
        parameters.Add("per_page", perPage.ToString((IFormatProvider) NumberFormatInfo.InvariantInfo));
      if (page > 0)
        parameters.Add("page", page.ToString((IFormatProvider) NumberFormatInfo.InvariantInfo));
      this.GetResponseAsync<PhotoCollection>(parameters, callback);
    }

    public void FavoritesGetContextAsync(string photoId, string userId, Action<FlickrResult<FavoriteContext>> callback)
    {
      this.FavoritesGetContextAsync(photoId, userId, 1, 1, PhotoSearchExtras.None, callback);
    }

    public void FavoritesGetContextAsync(string photoId, string userId, PhotoSearchExtras extras, Action<FlickrResult<FavoriteContext>> callback)
    {
      this.FavoritesGetContextAsync(photoId, userId, 1, 1, extras, callback);
    }

    public void FavoritesGetContextAsync(string photoId, string userId, int numPrevious, int numNext, Action<FlickrResult<FavoriteContext>> callback)
    {
      this.FavoritesGetContextAsync(photoId, userId, numPrevious, numNext, PhotoSearchExtras.None, callback);
    }

    public void FavoritesGetContextAsync(string photoId, string userId, int numPrevious, int numNext, PhotoSearchExtras extras, Action<FlickrResult<FavoriteContext>> callback)
    {
      Dictionary<string, string> parameters = new Dictionary<string, string>();
      parameters.Add("method", "flickr.favorites.getContext");
      parameters.Add("user_id", userId);
      parameters.Add("photo_id", photoId);
      parameters.Add("num_prev", Math.Max(1, numPrevious).ToString((IFormatProvider) NumberFormatInfo.InvariantInfo));
      parameters.Add("num_next", Math.Max(1, numNext).ToString((IFormatProvider) NumberFormatInfo.InvariantInfo));
      if (extras != PhotoSearchExtras.None)
        parameters.Add("extras", UtilityMethods.ExtrasToString(extras));
      this.GetResponseAsync<FavoriteContext>(parameters, callback);
    }

    public void GalleriesAddPhoto(string galleryId, string photoId)
    {
      this.GalleriesAddPhoto(galleryId, photoId, (string) null);
    }

    public void GalleriesAddPhoto(string galleryId, string photoId, string comment)
    {
      Dictionary<string, string> parameters = new Dictionary<string, string>();
      parameters.Add("method", "flickr.galleries.addPhoto");
      parameters.Add("gallery_id", galleryId);
      parameters.Add("photo_id", photoId);
      if (!string.IsNullOrEmpty(comment))
        parameters.Add("comment", comment);
      this.GetResponseNoCache<NoResponse>(parameters);
    }

    public void GalleriesCreate(string title, string description)
    {
      this.GalleriesCreate(title, description, (string) null);
    }

    public void GalleriesCreate(string title, string description, string primaryPhotoId)
    {
      this.CheckRequiresAuthentication();
      Dictionary<string, string> parameters = new Dictionary<string, string>();
      parameters.Add("method", "flickr.galleries.create");
      parameters.Add("title", title);
      parameters.Add("description", description);
      if (!string.IsNullOrEmpty(primaryPhotoId))
        parameters.Add("primary_photo_id", primaryPhotoId);
      this.GetResponseNoCache<NoResponse>(parameters);
    }

    public void GalleriesEditMeta(string galleryId, string title)
    {
      this.GalleriesEditMeta(galleryId, title, (string) null);
    }

    public void GalleriesEditMeta(string galleryId, string title, string description)
    {
      this.CheckRequiresAuthentication();
      Dictionary<string, string> parameters = new Dictionary<string, string>();
      parameters.Add("method", "flickr.galleries.editMeta");
      parameters.Add("gallery_id", galleryId);
      parameters.Add("title", title);
      if (!string.IsNullOrEmpty(description))
        parameters.Add("description", description);
      this.GetResponseNoCache<NoResponse>(parameters);
    }

    public void GalleriesEditPhoto(string galleryId, string photoId, string comment)
    {
      this.CheckRequiresAuthentication();
      this.GetResponseNoCache<NoResponse>(new Dictionary<string, string>()
      {
        {
          "method",
          "flickr.galleries.editPhoto"
        },
        {
          "gallery_id",
          galleryId
        },
        {
          "photo_id",
          photoId
        },
        {
          "comment",
          comment
        }
      });
    }

    public void GalleriesEditPhotos(string galleryId, string primaryPhotoId, IEnumerable<string> photoIds)
    {
      this.CheckRequiresAuthentication();
      Dictionary<string, string> parameters = new Dictionary<string, string>();
      parameters.Add("method", "flickr.galleries.editPhotos");
      parameters.Add("gallery_id", galleryId);
      parameters.Add("primary_photo_id", primaryPhotoId);
      List<string> list = new List<string>(photoIds);
      parameters.Add("photo_ids", string.Join(",", list.ToArray()));
      this.GetResponseNoCache<NoResponse>(parameters);
    }

    public Gallery GalleriesGetInfo(string galleryId)
    {
      return this.GetResponseCache<Gallery>(new Dictionary<string, string>()
      {
        {
          "method",
          "flickr.galleries.getInfo"
        },
        {
          "gallery_id",
          galleryId
        }
      });
    }

    public GalleryCollection GalleriesGetList()
    {
      this.CheckRequiresAuthentication();
      return this.GalleriesGetList((string) null, 0, 0);
    }

    public GalleryCollection GalleriesGetList(int page, int perPage)
    {
      this.CheckRequiresAuthentication();
      return this.GalleriesGetList((string) null, page, perPage);
    }

    public GalleryCollection GalleriesGetList(string userId)
    {
      return this.GalleriesGetList(userId, 0, 0);
    }

    public GalleryCollection GalleriesGetList(string userId, int page, int perPage)
    {
      Dictionary<string, string> parameters = new Dictionary<string, string>();
      parameters.Add("method", "flickr.galleries.getList");
      if (!string.IsNullOrEmpty(userId))
        parameters.Add("user_id", userId);
      if (page > 0)
        parameters.Add("page", page.ToString((IFormatProvider) NumberFormatInfo.InvariantInfo));
      if (perPage > 0)
        parameters.Add("per_page", perPage.ToString((IFormatProvider) NumberFormatInfo.InvariantInfo));
      return this.GetResponseCache<GalleryCollection>(parameters);
    }

    public GalleryCollection GalleriesGetListForPhoto(string photoId)
    {
      return this.GalleriesGetListForPhoto(photoId, 0, 0);
    }

    public GalleryCollection GalleriesGetListForPhoto(string photoId, int page, int perPage)
    {
      Dictionary<string, string> parameters = new Dictionary<string, string>();
      parameters.Add("method", "flickr.galleries.getListForPhoto");
      parameters.Add("photo_id", photoId);
      if (page > 0)
        parameters.Add("page", page.ToString((IFormatProvider) NumberFormatInfo.InvariantInfo));
      if (perPage > 0)
        parameters.Add("per_page", perPage.ToString((IFormatProvider) NumberFormatInfo.InvariantInfo));
      return this.GetResponseCache<GalleryCollection>(parameters);
    }

    public GalleryPhotoCollection GalleriesGetPhotos(string galleryId)
    {
      return this.GalleriesGetPhotos(galleryId, PhotoSearchExtras.None);
    }

    public GalleryPhotoCollection GalleriesGetPhotos(string galleryId, PhotoSearchExtras extras)
    {
      Dictionary<string, string> parameters = new Dictionary<string, string>();
      parameters.Add("method", "flickr.galleries.getPhotos");
      parameters.Add("gallery_id", galleryId);
      if (extras != PhotoSearchExtras.None)
        parameters.Add("extras", UtilityMethods.ExtrasToString(extras));
      return this.GetResponseCache<GalleryPhotoCollection>(parameters);
    }

    public void GalleriesAddPhotoAsync(string galleryId, string photoId, Action<FlickrResult<NoResponse>> callback)
    {
      this.GalleriesAddPhotoAsync(galleryId, photoId, (string) null, callback);
    }

    public void GalleriesAddPhotoAsync(string galleryId, string photoId, string comment, Action<FlickrResult<NoResponse>> callback)
    {
      Dictionary<string, string> parameters = new Dictionary<string, string>();
      parameters.Add("method", "flickr.galleries.addPhoto");
      parameters.Add("gallery_id", galleryId);
      parameters.Add("photo_id", photoId);
      if (!string.IsNullOrEmpty(comment))
        parameters.Add("comment", comment);
      this.GetResponseAsync<NoResponse>(parameters, callback);
    }

    public void GalleriesCreateAsync(string title, string description, Action<FlickrResult<NoResponse>> callback)
    {
      this.GalleriesCreateAsync(title, description, (string) null, callback);
    }

    public void GalleriesCreateAsync(string title, string description, string primaryPhotoId, Action<FlickrResult<NoResponse>> callback)
    {
      this.CheckRequiresAuthentication();
      Dictionary<string, string> parameters = new Dictionary<string, string>();
      parameters.Add("method", "flickr.galleries.create");
      parameters.Add("title", title);
      parameters.Add("description", description);
      if (!string.IsNullOrEmpty(primaryPhotoId))
        parameters.Add("primary_photo_id", primaryPhotoId);
      this.GetResponseAsync<NoResponse>(parameters, callback);
    }

    public void GalleriesEditMetaAsync(string galleryId, string title, Action<FlickrResult<NoResponse>> callback)
    {
      this.GalleriesEditMetaAsync(galleryId, title, (string) null, callback);
    }

    public void GalleriesEditMetaAsync(string galleryId, string title, string description, Action<FlickrResult<NoResponse>> callback)
    {
      this.CheckRequiresAuthentication();
      Dictionary<string, string> parameters = new Dictionary<string, string>();
      parameters.Add("method", "flickr.galleries.editMeta");
      parameters.Add("gallery_id", galleryId);
      parameters.Add("title", title);
      if (!string.IsNullOrEmpty(description))
        parameters.Add("description", description);
      this.GetResponseAsync<NoResponse>(parameters, callback);
    }

    public void GalleriesEditPhotoAsync(string galleryId, string photoId, string comment, Action<FlickrResult<NoResponse>> callback)
    {
      this.CheckRequiresAuthentication();
      this.GetResponseAsync<NoResponse>(new Dictionary<string, string>()
      {
        {
          "method",
          "flickr.galleries.editPhoto"
        },
        {
          "gallery_id",
          galleryId
        },
        {
          "photo_id",
          photoId
        },
        {
          "comment",
          comment
        }
      }, callback);
    }

    public void GalleriesEditPhotosAsync(string galleryId, string primaryPhotoId, IEnumerable<string> photoIds, Action<FlickrResult<NoResponse>> callback)
    {
      this.CheckRequiresAuthentication();
      Dictionary<string, string> parameters = new Dictionary<string, string>();
      parameters.Add("method", "flickr.galleries.editPhotos");
      parameters.Add("gallery_id", galleryId);
      parameters.Add("primary_photo_id", primaryPhotoId);
      List<string> list = new List<string>(photoIds);
      parameters.Add("photo_ids", string.Join(",", list.ToArray()));
      this.GetResponseAsync<NoResponse>(parameters, callback);
    }

    public void GalleriesGetInfoAsync(string galleryId, Action<FlickrResult<Gallery>> callback)
    {
      this.GetResponseAsync<Gallery>(new Dictionary<string, string>()
      {
        {
          "method",
          "flickr.galleries.getInfo"
        },
        {
          "gallery_id",
          galleryId
        }
      }, callback);
    }

    public void GalleriesGetListAsync(Action<FlickrResult<GalleryCollection>> callback)
    {
      this.CheckRequiresAuthentication();
      this.GalleriesGetListAsync((string) null, 0, 0, callback);
    }

    public void GalleriesGetListAsync(int page, int perPage, Action<FlickrResult<GalleryCollection>> callback)
    {
      this.CheckRequiresAuthentication();
      this.GalleriesGetListAsync((string) null, page, perPage, callback);
    }

    public void GalleriesGetListAsync(string userId, Action<FlickrResult<GalleryCollection>> callback)
    {
      this.GalleriesGetListAsync(userId, 0, 0, callback);
    }

    public void GalleriesGetListAsync(string userId, int page, int perPage, Action<FlickrResult<GalleryCollection>> callback)
    {
      Dictionary<string, string> parameters = new Dictionary<string, string>();
      parameters.Add("method", "flickr.galleries.getList");
      if (!string.IsNullOrEmpty(userId))
        parameters.Add("user_id", userId);
      if (page > 0)
        parameters.Add("page", page.ToString((IFormatProvider) NumberFormatInfo.InvariantInfo));
      if (perPage > 0)
        parameters.Add("per_page", perPage.ToString((IFormatProvider) NumberFormatInfo.InvariantInfo));
      this.GetResponseAsync<GalleryCollection>(parameters, callback);
    }

    public void GalleriesGetListForPhotoAsync(string photoId, Action<FlickrResult<GalleryCollection>> callback)
    {
      this.GalleriesGetListForPhotoAsync(photoId, 0, 0, callback);
    }

    public void GalleriesGetListForPhotoAsync(string photoId, int page, int perPage, Action<FlickrResult<GalleryCollection>> callback)
    {
      Dictionary<string, string> parameters = new Dictionary<string, string>();
      parameters.Add("method", "flickr.galleries.getListForPhoto");
      parameters.Add("photo_id", photoId);
      if (page > 0)
        parameters.Add("page", page.ToString((IFormatProvider) NumberFormatInfo.InvariantInfo));
      if (perPage > 0)
        parameters.Add("per_page", perPage.ToString((IFormatProvider) NumberFormatInfo.InvariantInfo));
      this.GetResponseAsync<GalleryCollection>(parameters, callback);
    }

    public void GalleriesGetPhotosAsync(string galleryId, Action<FlickrResult<GalleryPhotoCollection>> callback)
    {
      this.GalleriesGetPhotosAsync(galleryId, PhotoSearchExtras.None, callback);
    }

    public void GalleriesGetPhotosAsync(string galleryId, PhotoSearchExtras extras, Action<FlickrResult<GalleryPhotoCollection>> callback)
    {
      Dictionary<string, string> parameters = new Dictionary<string, string>();
      parameters.Add("method", "flickr.galleries.getPhotos");
      parameters.Add("gallery_id", galleryId);
      if (extras != PhotoSearchExtras.None)
        parameters.Add("extras", UtilityMethods.ExtrasToString(extras));
      this.GetResponseAsync<GalleryPhotoCollection>(parameters, callback);
    }

    private void GetResponseEvent<T>(Dictionary<string, string> parameters, EventHandler<FlickrResultArgs<T>> handler) where T : IFlickrParsable, new()
    {
      this.GetResponseAsync<T>(parameters, (Action<FlickrResult<T>>) (r => handler((object) this, new FlickrResultArgs<T>(r))));
    }

    private void GetResponseAsync<T>(Dictionary<string, string> parameters, Action<FlickrResult<T>> callback) where T : IFlickrParsable, new()
    {
      this.CheckApiKey();
      parameters["api_key"] = this.ApiKey;
      string str = parameters["method"];
      if (str.StartsWith("flickr.auth") && !str.EndsWith("oauth.checkToken"))
      {
        if (!string.IsNullOrEmpty(this.AuthToken))
          parameters["auth_token"] = this.AuthToken;
      }
      else if (!string.IsNullOrEmpty(this.OAuthAccessToken) || string.IsNullOrEmpty(this.AuthToken))
      {
        this.OAuthGetBasicParameters(parameters);
        if (!string.IsNullOrEmpty(this.OAuthAccessToken))
          parameters["oauth_token"] = this.OAuthAccessToken;
      }
      else
        parameters["auth_token"] = this.AuthToken;
      this.lastRequest = (string.IsNullOrEmpty(this.sharedSecret) ? this.CalculateUri(parameters, false) : this.CalculateUri(parameters, true)).AbsoluteUri;
      try
      {
        FlickrResponder.GetDataResponseAsync(this, this.BaseUri.AbsoluteUri, parameters, (Action<FlickrResult<string>>) (r =>
        {
          FlickrResult<T> local_0 = new FlickrResult<T>();
          if (r.HasError)
          {
            local_0.Error = r.Error;
          }
          else
          {
            try
            {
              this.lastResponse = r.Result;
              XmlReader local_2 = XmlReader.Create((TextReader) new StringReader(r.Result), new XmlReaderSettings()
              {
                IgnoreWhitespace = true
              });
              if (!local_2.ReadToDescendant("rsp"))
                throw new XmlException("Unable to find response element 'rsp' in Flickr response");
              while (local_2.MoveToNextAttribute())
              {
                if (local_2.LocalName == "stat" && local_2.Value == "fail")
                  throw ExceptionHandler.CreateResponseException(local_2);
              }
              local_2.MoveToElement();
              local_2.Read();
              T local_3 = new T();
              local_3.Load(local_2);
              local_0.Result = local_3;
              local_0.HasError = false;
            }
            catch (Exception exception_1)
            {
              local_0.Error = exception_1;
            }
          }
          if (callback == null)
            return;
          callback(local_0);
        }));
      }
      catch (Exception ex)
      {
        FlickrResult<T> flickrResult = new FlickrResult<T>();
        flickrResult.Error = ex;
        if (callback == null)
          return;
        callback(flickrResult);
      }
    }

    private void DoGetResponseAsync<T>(Uri url, Action<FlickrResult<T>> callback) where T : IFlickrParsable, new()
    {
      string postContents = string.Empty;
      if (url.AbsoluteUri.Length > 2000)
      {
        postContents = url.Query.Substring(1);
        url = new Uri(url, string.Empty);
      }
      FlickrResult<T> result = new FlickrResult<T>();
      HttpWebRequest request = (HttpWebRequest) WebRequest.Create(url);
      request.ContentType = "application/x-www-form-urlencoded";
      request.Method = "POST";
      request.BeginGetRequestStream((AsyncCallback) (requestAsyncResult =>
      {
        using (Stream resource_1 = ((WebRequest) request).EndGetRequestStream(requestAsyncResult))
        {
          using (StreamWriter resource_0 = new StreamWriter(resource_1))
          {
            resource_0.Write(postContents);
            resource_0.Close();
          }
          resource_1.Close();
        }
        request.BeginGetResponse((AsyncCallback) (responseAsyncResult =>
        {
          try
          {
            using (StreamReader resource_2 = new StreamReader(request.EndGetResponse(responseAsyncResult).GetResponseStream()))
            {
              string local_2 = resource_2.ReadToEnd();
              this.lastResponse = local_2;
              XmlReader local_4 = XmlReader.Create((TextReader) new StringReader(local_2), new XmlReaderSettings()
              {
                IgnoreWhitespace = true
              });
              if (!local_4.ReadToDescendant("rsp"))
                throw new XmlException("Unable to find response element 'rsp' in Flickr response");
              while (local_4.MoveToNextAttribute())
              {
                if (local_4.LocalName == "stat" && local_4.Value == "fail")
                  throw ExceptionHandler.CreateResponseException(local_4);
              }
              local_4.MoveToElement();
              local_4.Read();
              T local_5 = new T();
              local_5.Load(local_4);
              result.Result = local_5;
              result.HasError = false;
              resource_2.Close();
            }
            if (callback == null)
              return;
            callback(result);
          }
          catch (Exception exception_0)
          {
            result.Error = exception_0;
            if (callback == null)
              return;
            callback(result);
          }
        }), (object) null);
      }), (object) null);
    }

    public void GroupsDiscussRepliesAdd(string topicId, string message)
    {
      this.CheckRequiresAuthentication();
      if (string.IsNullOrEmpty(topicId))
        throw new ArgumentNullException("topicId");
      if (string.IsNullOrEmpty(message))
        throw new ArgumentNullException("message");
      this.GetResponseNoCache<NoResponse>(new Dictionary<string, string>()
      {
        {
          "method",
          "flickr.groups.discuss.replies.add"
        },
        {
          "topic_id",
          topicId
        },
        {
          "message",
          message
        }
      });
    }

    public void GroupsDiscussRepliesDelete(string topicId, string replyId)
    {
      this.CheckRequiresAuthentication();
      if (string.IsNullOrEmpty(topicId))
        throw new ArgumentNullException("topicId");
      if (string.IsNullOrEmpty(replyId))
        throw new ArgumentNullException("replyId");
      this.GetResponseNoCache<NoResponse>(new Dictionary<string, string>()
      {
        {
          "method",
          "flickr.groups.discuss.replies.delete"
        },
        {
          "topic_id",
          topicId
        },
        {
          "reply_id",
          replyId
        }
      });
    }

    public void GroupsDiscussRepliesEdit(string topicId, string replyId, string message)
    {
      this.CheckRequiresAuthentication();
      if (string.IsNullOrEmpty(topicId))
        throw new ArgumentNullException("topicId");
      if (string.IsNullOrEmpty(replyId))
        throw new ArgumentNullException("replyId");
      if (string.IsNullOrEmpty(message))
        throw new ArgumentNullException("message");
      this.GetResponseNoCache<NoResponse>(new Dictionary<string, string>()
      {
        {
          "method",
          "flickr.groups.discuss.replies.edit"
        },
        {
          "topic_id",
          topicId
        },
        {
          "reply_id",
          replyId
        },
        {
          "message",
          message
        }
      });
    }

    public TopicReply GroupsDiscussRepliesGetInfo(string topicId, string replyId)
    {
      if (string.IsNullOrEmpty(topicId))
        throw new ArgumentNullException("topicId");
      if (string.IsNullOrEmpty(replyId))
        throw new ArgumentNullException("replyId");
      return this.GetResponseCache<TopicReply>(new Dictionary<string, string>()
      {
        {
          "method",
          "flickr.groups.discuss.replies.getInfo"
        },
        {
          "topic_id",
          topicId
        },
        {
          "reply_id",
          replyId
        }
      });
    }

    public TopicReplyCollection GroupsDiscussRepliesGetList(string topicId, int page, int perPage)
    {
      if (string.IsNullOrEmpty(topicId))
        throw new ArgumentNullException("topicId");
      Dictionary<string, string> parameters = new Dictionary<string, string>();
      parameters.Add("method", "flickr.groups.discuss.replies.getList");
      parameters.Add("topic_id", topicId);
      if (page > 0)
        parameters.Add("page", page.ToString((IFormatProvider) NumberFormatInfo.InvariantInfo));
      if (perPage > 0)
        parameters.Add("per_page", perPage.ToString((IFormatProvider) NumberFormatInfo.InvariantInfo));
      return this.GetResponseCache<TopicReplyCollection>(parameters);
    }

    public void GroupsDiscussTopicsAdd(string groupId, string subject, string message)
    {
      this.CheckRequiresAuthentication();
      if (string.IsNullOrEmpty(groupId))
        throw new ArgumentNullException("groupId");
      if (string.IsNullOrEmpty(subject))
        throw new ArgumentNullException("subject");
      if (string.IsNullOrEmpty(message))
        throw new ArgumentNullException("message");
      this.GetResponseNoCache<NoResponse>(new Dictionary<string, string>()
      {
        {
          "method",
          "flickr.groups.discuss.topics.add"
        },
        {
          "group_id",
          groupId
        },
        {
          "subject",
          subject
        },
        {
          "message",
          message
        }
      });
    }

    public TopicCollection GroupsDiscussTopicsGetList(string groupId, int page, int perPage)
    {
      if (string.IsNullOrEmpty(groupId))
        throw new ArgumentNullException("groupId");
      Dictionary<string, string> parameters = new Dictionary<string, string>();
      parameters.Add("method", "flickr.groups.discuss.topics.getList");
      parameters.Add("group_id", groupId);
      if (page > 0)
        parameters.Add("page", page.ToString((IFormatProvider) NumberFormatInfo.InvariantInfo));
      if (perPage > 0)
        parameters.Add("per_page", perPage.ToString((IFormatProvider) NumberFormatInfo.InvariantInfo));
      return this.GetResponseCache<TopicCollection>(parameters);
    }

    public Topic GroupsDiscussTopicsGetInfo(string topicId)
    {
      if (string.IsNullOrEmpty(topicId))
        throw new ArgumentNullException("topicId");
      return this.GetResponseCache<Topic>(new Dictionary<string, string>()
      {
        {
          "method",
          "flickr.groups.discuss.topics.getInfo"
        },
        {
          "topic_id",
          topicId
        }
      });
    }

    public void GroupsBrowseAsync(Action<FlickrResult<GroupCategory>> callback)
    {
      this.GroupsBrowseAsync((string) null, callback);
    }

    public void GroupsBrowseAsync(string catId, Action<FlickrResult<GroupCategory>> callback)
    {
      Dictionary<string, string> parameters = new Dictionary<string, string>();
      parameters.Add("method", "flickr.groups.browse");
      if (!string.IsNullOrEmpty(catId))
        parameters.Add("cat_id", catId);
      this.GetResponseAsync<GroupCategory>(parameters, callback);
    }

    public void GroupsSearchAsync(string text, Action<FlickrResult<GroupSearchResultCollection>> callback)
    {
      this.GroupsSearchAsync(text, 0, 0, callback);
    }

    public void GroupsSearchAsync(string text, int page, Action<FlickrResult<GroupSearchResultCollection>> callback)
    {
      this.GroupsSearchAsync(text, page, 0, callback);
    }

    public void GroupsSearchAsync(string text, int page, int perPage, Action<FlickrResult<GroupSearchResultCollection>> callback)
    {
      Dictionary<string, string> parameters = new Dictionary<string, string>();
      parameters.Add("method", "flickr.groups.search");
      parameters.Add("api_key", this.apiKey);
      parameters.Add("text", text);
      if (page > 0)
        parameters.Add("page", page.ToString((IFormatProvider) NumberFormatInfo.InvariantInfo));
      if (perPage > 0)
        parameters.Add("per_page", perPage.ToString((IFormatProvider) NumberFormatInfo.InvariantInfo));
      this.GetResponseAsync<GroupSearchResultCollection>(parameters, callback);
    }

    public void GroupsGetInfoAsync(string groupId, Action<FlickrResult<GroupFullInfo>> callback)
    {
      this.GetResponseAsync<GroupFullInfo>(new Dictionary<string, string>()
      {
        {
          "method",
          "flickr.groups.getInfo"
        },
        {
          "api_key",
          this.apiKey
        },
        {
          "group_id",
          groupId
        }
      }, callback);
    }

    public void GroupsMembersGetListAsync(string groupId, Action<FlickrResult<MemberCollection>> callback)
    {
      this.GroupsMembersGetListAsync(groupId, 0, 0, MemberTypes.None, callback);
    }

    public void GroupsMembersGetListAsync(string groupId, int page, int perPage, MemberTypes memberTypes, Action<FlickrResult<MemberCollection>> callback)
    {
      this.CheckRequiresAuthentication();
      Dictionary<string, string> parameters = new Dictionary<string, string>();
      parameters.Add("method", "flickr.groups.members.getList");
      parameters.Add("api_key", this.apiKey);
      if (page > 0)
        parameters.Add("page", page.ToString((IFormatProvider) NumberFormatInfo.InvariantInfo));
      if (perPage > 0)
        parameters.Add("per_page", perPage.ToString((IFormatProvider) NumberFormatInfo.InvariantInfo));
      if (memberTypes != MemberTypes.None)
        parameters.Add("membertypes", UtilityMethods.MemberTypeToString(memberTypes));
      parameters.Add("group_id", groupId);
      this.GetResponseAsync<MemberCollection>(parameters, callback);
    }

    public void GroupsPoolsAddAsync(string photoId, string groupId, Action<FlickrResult<NoResponse>> callback)
    {
      this.CheckRequiresAuthentication();
      this.GetResponseAsync<NoResponse>(new Dictionary<string, string>()
      {
        {
          "method",
          "flickr.groups.pools.add"
        },
        {
          "photo_id",
          photoId
        },
        {
          "group_id",
          groupId
        }
      }, callback);
    }

    public void GroupsPoolsGetContextAsync(string photoId, string groupId, Action<FlickrResult<Context>> callback)
    {
      this.GetResponseAsync<Context>(new Dictionary<string, string>()
      {
        {
          "method",
          "flickr.groups.pools.getContext"
        },
        {
          "photo_id",
          photoId
        },
        {
          "group_id",
          groupId
        }
      }, callback);
    }

    public void GroupsPoolsRemoveAsync(string photoId, string groupId, Action<FlickrResult<NoResponse>> callback)
    {
      this.GetResponseAsync<NoResponse>(new Dictionary<string, string>()
      {
        {
          "method",
          "flickr.groups.pools.remove"
        },
        {
          "photo_id",
          photoId
        },
        {
          "group_id",
          groupId
        }
      }, callback);
    }

    public void GroupsPoolsGetGroupsAsync(Action<FlickrResult<MemberGroupInfoCollection>> callback)
    {
      this.GetResponseAsync<MemberGroupInfoCollection>(new Dictionary<string, string>()
      {
        {
          "method",
          "flickr.groups.pools.getGroups"
        }
      }, callback);
    }

    public void GroupsPoolsGetPhotosAsync(string groupId, Action<FlickrResult<PhotoCollection>> callback)
    {
      this.GroupsPoolsGetPhotosAsync(groupId, (string) null, (string) null, PhotoSearchExtras.None, 0, 0, callback);
    }

    public void GroupsPoolsGetPhotosAsync(string groupId, string tags, Action<FlickrResult<PhotoCollection>> callback)
    {
      this.GroupsPoolsGetPhotosAsync(groupId, tags, (string) null, PhotoSearchExtras.None, 0, 0, callback);
    }

    public void GroupsPoolsGetPhotosAsync(string groupId, int page, int perPage, Action<FlickrResult<PhotoCollection>> callback)
    {
      this.GroupsPoolsGetPhotosAsync(groupId, (string) null, (string) null, PhotoSearchExtras.None, page, perPage, callback);
    }

    public void GroupsPoolsGetPhotosAsync(string groupId, string tags, int page, int perPage, Action<FlickrResult<PhotoCollection>> callback)
    {
      this.GroupsPoolsGetPhotosAsync(groupId, tags, (string) null, PhotoSearchExtras.None, page, perPage, callback);
    }

    public void GroupsPoolsGetPhotosAsync(string groupId, string tags, string userId, PhotoSearchExtras extras, int page, int perPage, Action<FlickrResult<PhotoCollection>> callback)
    {
      Dictionary<string, string> parameters = new Dictionary<string, string>();
      parameters.Add("method", "flickr.groups.pools.getPhotos");
      parameters.Add("group_id", groupId);
      if (tags != null && tags.Length > 0)
        parameters.Add("tags", tags);
      if (page > 0)
        parameters.Add("page", page.ToString((IFormatProvider) NumberFormatInfo.InvariantInfo));
      if (perPage > 0)
        parameters.Add("per_page", perPage.ToString((IFormatProvider) NumberFormatInfo.InvariantInfo));
      if (userId != null && userId.Length > 0)
        parameters.Add("user_id", userId);
      if (extras != PhotoSearchExtras.None)
        parameters.Add("extras", UtilityMethods.ExtrasToString(extras));
      this.GetResponseAsync<PhotoCollection>(parameters, callback);
    }

    public void GroupsJoinAsync(string groupId, Action<FlickrResult<NoResponse>> callback)
    {
      this.GroupsJoinAsync(groupId, false, callback);
    }

    public void GroupsJoinAsync(string groupId, bool acceptsRules, Action<FlickrResult<NoResponse>> callback)
    {
      this.CheckRequiresAuthentication();
      Dictionary<string, string> parameters = new Dictionary<string, string>();
      parameters.Add("method", "flickr.groups.join");
      parameters.Add("group_id", groupId);
      if (acceptsRules)
        parameters.Add("accepts_rules", "1");
      this.GetResponseAsync<NoResponse>(parameters, callback);
    }

    public void GroupsJoinRequestAsync(string groupId, string message, bool acceptRules, Action<FlickrResult<NoResponse>> callback)
    {
      this.CheckRequiresAuthentication();
      Dictionary<string, string> parameters = new Dictionary<string, string>();
      parameters.Add("method", "flickr.groups.joinRequest");
      parameters.Add("group_id", groupId);
      parameters.Add("message", message);
      if (acceptRules)
        parameters.Add("accept_rules", "1");
      this.GetResponseAsync<NoResponse>(parameters, callback);
    }

    public void GroupsLeaveAsync(string groupId, Action<FlickrResult<NoResponse>> callback)
    {
      this.GroupsLeaveAsync(groupId, false, callback);
    }

    public void GroupsLeaveAsync(string groupId, bool deletePhotos, Action<FlickrResult<NoResponse>> callback)
    {
      this.CheckRequiresAuthentication();
      Dictionary<string, string> parameters = new Dictionary<string, string>();
      parameters.Add("method", "flickr.groups.leave");
      parameters.Add("group_id", groupId);
      if (deletePhotos)
        parameters.Add("delete_photos", "1");
      this.GetResponseAsync<NoResponse>(parameters, callback);
    }

    public void GroupsDiscussRepliesAddAsync(string topicId, string message, Action<FlickrResult<NoResponse>> callback)
    {
      this.CheckRequiresAuthentication();
      if (string.IsNullOrEmpty(topicId))
        throw new ArgumentNullException("topicId");
      if (string.IsNullOrEmpty(message))
        throw new ArgumentNullException("message");
      this.GetResponseAsync<NoResponse>(new Dictionary<string, string>()
      {
        {
          "method",
          "flickr.groups.discuss.replies.add"
        },
        {
          "topic_id",
          topicId
        },
        {
          "message",
          message
        }
      }, callback);
    }

    public void GroupsDiscussRepliesDeleteAsync(string topicId, string replyId, Action<FlickrResult<NoResponse>> callback)
    {
      this.CheckRequiresAuthentication();
      if (string.IsNullOrEmpty(topicId))
        throw new ArgumentNullException("topicId");
      if (string.IsNullOrEmpty(replyId))
        throw new ArgumentNullException("replyId");
      this.GetResponseAsync<NoResponse>(new Dictionary<string, string>()
      {
        {
          "method",
          "flickr.groups.discuss.replies.delete"
        },
        {
          "topic_id",
          topicId
        },
        {
          "reply_id",
          replyId
        }
      }, callback);
    }

    public void GroupsDiscussRepliesEditAsync(string topicId, string replyId, string message, Action<FlickrResult<NoResponse>> callback)
    {
      this.CheckRequiresAuthentication();
      if (string.IsNullOrEmpty(topicId))
        throw new ArgumentNullException("topicId");
      if (string.IsNullOrEmpty(replyId))
        throw new ArgumentNullException("replyId");
      if (string.IsNullOrEmpty(message))
        throw new ArgumentNullException("message");
      this.GetResponseAsync<NoResponse>(new Dictionary<string, string>()
      {
        {
          "method",
          "flickr.groups.discuss.replies.edit"
        },
        {
          "topic_id",
          topicId
        },
        {
          "reply_id",
          replyId
        },
        {
          "message",
          message
        }
      }, callback);
    }

    public void GroupsDiscussRepliesGetInfoAsync(string topicId, string replyId, Action<FlickrResult<TopicReply>> callback)
    {
      if (string.IsNullOrEmpty(topicId))
        throw new ArgumentNullException("topicId");
      if (string.IsNullOrEmpty(replyId))
        throw new ArgumentNullException("replyId");
      this.GetResponseAsync<TopicReply>(new Dictionary<string, string>()
      {
        {
          "method",
          "flickr.groups.discuss.replies.getInfo"
        },
        {
          "topic_id",
          topicId
        },
        {
          "reply_id",
          replyId
        }
      }, callback);
    }

    public void GroupsDiscussRepliesGetListAsync(string topicId, int page, int perPage, Action<FlickrResult<TopicReplyCollection>> callback)
    {
      if (string.IsNullOrEmpty(topicId))
        throw new ArgumentNullException("topicId");
      Dictionary<string, string> parameters = new Dictionary<string, string>();
      parameters.Add("method", "flickr.groups.discuss.replies.getList");
      parameters.Add("topic_id", topicId);
      if (page > 0)
        parameters.Add("page", page.ToString((IFormatProvider) NumberFormatInfo.InvariantInfo));
      if (perPage > 0)
        parameters.Add("per_page", perPage.ToString((IFormatProvider) NumberFormatInfo.InvariantInfo));
      this.GetResponseAsync<TopicReplyCollection>(parameters, callback);
    }

    public void GroupsDiscussTopicsAddAsync(string groupId, string subject, string message, Action<FlickrResult<NoResponse>> callback)
    {
      this.CheckRequiresAuthentication();
      if (string.IsNullOrEmpty(groupId))
        throw new ArgumentNullException("groupId");
      if (string.IsNullOrEmpty(subject))
        throw new ArgumentNullException("subject");
      if (string.IsNullOrEmpty(message))
        throw new ArgumentNullException("message");
      this.GetResponseAsync<NoResponse>(new Dictionary<string, string>()
      {
        {
          "method",
          "flickr.groups.discuss.topics.add"
        },
        {
          "group_id",
          groupId
        },
        {
          "subject",
          subject
        },
        {
          "message",
          message
        }
      }, callback);
    }

    public void GroupsDiscussTopicsGetListAsync(string groupId, int page, int perPage, Action<FlickrResult<TopicCollection>> callback)
    {
      if (string.IsNullOrEmpty(groupId))
        throw new ArgumentNullException("groupId");
      Dictionary<string, string> parameters = new Dictionary<string, string>();
      parameters.Add("method", "flickr.groups.discuss.topics.getList");
      parameters.Add("group_id", groupId);
      if (page > 0)
        parameters.Add("page", page.ToString((IFormatProvider) NumberFormatInfo.InvariantInfo));
      if (perPage > 0)
        parameters.Add("per_page", perPage.ToString((IFormatProvider) NumberFormatInfo.InvariantInfo));
      this.GetResponseAsync<TopicCollection>(parameters, callback);
    }

    public void GroupsDiscussTopicsGetInfoAsync(string topicId, Action<FlickrResult<Topic>> callback)
    {
      if (string.IsNullOrEmpty(topicId))
        throw new ArgumentNullException("topicId");
      this.GetResponseAsync<Topic>(new Dictionary<string, string>()
      {
        {
          "method",
          "flickr.groups.discuss.topics.getInfo"
        },
        {
          "topic_id",
          topicId
        }
      }, callback);
    }

    public void InterestingnessGetListAsync(PhotoSearchExtras extras, int page, int perPage, Action<FlickrResult<PhotoCollection>> callback)
    {
      this.InterestingnessGetListAsync(DateTime.MinValue, extras, page, perPage, callback);
    }

    public void InterestingnessGetListAsync(DateTime date, Action<FlickrResult<PhotoCollection>> callback)
    {
      this.InterestingnessGetListAsync(date, PhotoSearchExtras.None, 0, 0, callback);
    }

    public void InterestingnessGetListAsync(Action<FlickrResult<PhotoCollection>> callback)
    {
      this.InterestingnessGetListAsync(DateTime.MinValue, PhotoSearchExtras.None, 0, 0, callback);
    }

    public void InterestingnessGetListAsync(DateTime date, PhotoSearchExtras extras, int page, int perPage, Action<FlickrResult<PhotoCollection>> callback)
    {
      Dictionary<string, string> parameters = new Dictionary<string, string>();
      parameters.Add("method", "flickr.interestingness.getList");
      if (date > DateTime.MinValue)
        parameters.Add("date", date.ToString("yyyy-MM-dd", (IFormatProvider) DateTimeFormatInfo.InvariantInfo));
      if (perPage > 0)
        parameters.Add("per_page", perPage.ToString((IFormatProvider) NumberFormatInfo.InvariantInfo));
      if (page > 0)
        parameters.Add("page", page.ToString((IFormatProvider) NumberFormatInfo.InvariantInfo));
      if (extras != PhotoSearchExtras.None)
        parameters.Add("extras", UtilityMethods.ExtrasToString(extras));
      this.GetResponseAsync<PhotoCollection>(parameters, callback);
    }

    public NamespaceCollection MachineTagsGetNamespaces()
    {
      return this.MachineTagsGetNamespaces((string) null, 0, 0);
    }

    public NamespaceCollection MachineTagsGetNamespaces(int page, int perPage)
    {
      return this.MachineTagsGetNamespaces((string) null, page, perPage);
    }

    public NamespaceCollection MachineTagsGetNamespaces(string predicate)
    {
      return this.MachineTagsGetNamespaces(predicate, 0, 0);
    }

    public NamespaceCollection MachineTagsGetNamespaces(string predicate, int page, int perPage)
    {
      Dictionary<string, string> parameters = new Dictionary<string, string>();
      parameters.Add("method", "flickr.machinetags.getNamespaces");
      if (!string.IsNullOrEmpty(predicate))
        parameters.Add("predicate", predicate);
      if (page > 0)
        parameters.Add("page", page.ToString((IFormatProvider) NumberFormatInfo.InvariantInfo));
      if (perPage > 0)
        parameters.Add("per_page", perPage.ToString((IFormatProvider) NumberFormatInfo.InvariantInfo));
      return this.GetResponseCache<NamespaceCollection>(parameters);
    }

    public PredicateCollection MachineTagsGetPredicates()
    {
      return this.MachineTagsGetPredicates((string) null, 0, 0);
    }

    public PredicateCollection MachineTagsGetPredicates(int page, int perPage)
    {
      return this.MachineTagsGetPredicates((string) null, page, perPage);
    }

    public PredicateCollection MachineTagsGetPredicates(string namespaceName)
    {
      return this.MachineTagsGetPredicates(namespaceName, 0, 0);
    }

    public PredicateCollection MachineTagsGetPredicates(string namespaceName, int page, int perPage)
    {
      Dictionary<string, string> parameters = new Dictionary<string, string>();
      parameters.Add("method", "flickr.machinetags.getPredicates");
      if (!string.IsNullOrEmpty(namespaceName))
        parameters.Add("namespace", namespaceName);
      if (page > 0)
        parameters.Add("page", page.ToString((IFormatProvider) NumberFormatInfo.InvariantInfo));
      if (perPage > 0)
        parameters.Add("per_page", perPage.ToString((IFormatProvider) NumberFormatInfo.InvariantInfo));
      return this.GetResponseCache<PredicateCollection>(parameters);
    }

    public PairCollection MachineTagsGetPairs()
    {
      return this.MachineTagsGetPairs((string) null, (string) null, 0, 0);
    }

    public PairCollection MachineTagsGetPairs(int page, int perPage)
    {
      return this.MachineTagsGetPairs((string) null, (string) null, page, perPage);
    }

    public PairCollection MachineTagsGetPairs(string namespaceName, string predicate)
    {
      return this.MachineTagsGetPairs(namespaceName, predicate, 0, 0);
    }

    public PairCollection MachineTagsGetPairs(string namespaceName, string predicate, int page, int perPage)
    {
      Dictionary<string, string> parameters = new Dictionary<string, string>();
      parameters.Add("method", "flickr.machinetags.getPairs");
      if (!string.IsNullOrEmpty(namespaceName))
        parameters.Add("namespace", namespaceName);
      if (!string.IsNullOrEmpty(predicate))
        parameters.Add("predicate", predicate);
      if (page > 0)
        parameters.Add("page", page.ToString((IFormatProvider) NumberFormatInfo.InvariantInfo));
      if (perPage > 0)
        parameters.Add("per_page", perPage.ToString((IFormatProvider) NumberFormatInfo.InvariantInfo));
      return this.GetResponseCache<PairCollection>(parameters);
    }

    public ValueCollection MachineTagsGetValues(string namespaceName, string predicate)
    {
      return this.MachineTagsGetValues(namespaceName, predicate, 0, 0);
    }

    public ValueCollection MachineTagsGetValues(string namespaceName, string predicate, int page, int perPage)
    {
      Dictionary<string, string> parameters = new Dictionary<string, string>();
      parameters.Add("method", "flickr.machinetags.getValues");
      parameters.Add("namespace", namespaceName);
      parameters.Add("predicate", predicate);
      if (page > 0)
        parameters.Add("page", page.ToString((IFormatProvider) NumberFormatInfo.InvariantInfo));
      if (perPage > 0)
        parameters.Add("per_page", perPage.ToString((IFormatProvider) NumberFormatInfo.InvariantInfo));
      return this.GetResponseCache<ValueCollection>(parameters);
    }

    public ValueCollection MachineTagsGetRecentValues(DateTime addedSince)
    {
      return this.MachineTagsGetRecentValues((string) null, (string) null, addedSince, 0, 0);
    }

    public ValueCollection MachineTagsGetRecentValues(DateTime addedSince, int page, int perPage)
    {
      return this.MachineTagsGetRecentValues((string) null, (string) null, addedSince, page, perPage);
    }

    public ValueCollection MachineTagsGetRecentValues(string namespaceName, string predicate)
    {
      return this.MachineTagsGetRecentValues(namespaceName, predicate, DateTime.MinValue, 0, 0);
    }

    public ValueCollection MachineTagsGetRecentValues(string namespaceName, string predicate, int page, int perPage)
    {
      return this.MachineTagsGetRecentValues(namespaceName, predicate, DateTime.MinValue, page, perPage);
    }

    public ValueCollection MachineTagsGetRecentValues(string namespaceName, string predicate, DateTime addedSince, int page, int perPage)
    {
      if (string.IsNullOrEmpty(namespaceName) && string.IsNullOrEmpty(predicate) && addedSince == DateTime.MinValue)
        throw new ArgumentException("Must supply one of namespaceName, predicate or addedSince");
      Dictionary<string, string> parameters = new Dictionary<string, string>();
      parameters.Add("method", "flickr.machinetags.getRecentValues");
      if (!string.IsNullOrEmpty(namespaceName))
        parameters.Add("namespace", namespaceName);
      if (!string.IsNullOrEmpty(predicate))
        parameters.Add("predicate", predicate);
      if (addedSince != DateTime.MinValue)
        parameters.Add("added_since", UtilityMethods.DateToUnixTimestamp(addedSince));
      if (page > 0)
        parameters.Add("page", page.ToString((IFormatProvider) NumberFormatInfo.InvariantInfo));
      if (perPage > 0)
        parameters.Add("per_page", perPage.ToString((IFormatProvider) NumberFormatInfo.InvariantInfo));
      return this.GetResponseCache<ValueCollection>(parameters);
    }

    public void MachineTagsGetNamespacesAsync(Action<FlickrResult<NamespaceCollection>> callback)
    {
      this.MachineTagsGetNamespacesAsync((string) null, 0, 0, callback);
    }

    public void MachineTagsGetNamespacesAsync(int page, int perPage, Action<FlickrResult<NamespaceCollection>> callback)
    {
      this.MachineTagsGetNamespacesAsync((string) null, page, perPage, callback);
    }

    public void MachineTagsGetNamespacesAsync(string predicate, Action<FlickrResult<NamespaceCollection>> callback)
    {
      this.MachineTagsGetNamespacesAsync(predicate, 0, 0, callback);
    }

    public void MachineTagsGetNamespacesAsync(string predicate, int page, int perPage, Action<FlickrResult<NamespaceCollection>> callback)
    {
      Dictionary<string, string> parameters = new Dictionary<string, string>();
      parameters.Add("method", "flickr.machinetags.getNamespaces");
      if (!string.IsNullOrEmpty(predicate))
        parameters.Add("predicate", predicate);
      if (page > 0)
        parameters.Add("page", page.ToString((IFormatProvider) NumberFormatInfo.InvariantInfo));
      if (perPage > 0)
        parameters.Add("per_page", perPage.ToString((IFormatProvider) NumberFormatInfo.InvariantInfo));
      this.GetResponseAsync<NamespaceCollection>(parameters, callback);
    }

    public void MachineTagsGetPredicatesAsync(Action<FlickrResult<PredicateCollection>> callback)
    {
      this.MachineTagsGetPredicatesAsync((string) null, 0, 0, callback);
    }

    public void MachineTagsGetPredicatesAsync(int page, int perPage, Action<FlickrResult<PredicateCollection>> callback)
    {
      this.MachineTagsGetPredicatesAsync((string) null, page, perPage, callback);
    }

    public void MachineTagsGetPredicatesAsync(string namespaceName, Action<FlickrResult<PredicateCollection>> callback)
    {
      this.MachineTagsGetPredicatesAsync(namespaceName, 0, 0, callback);
    }

    public void MachineTagsGetPredicatesAsync(string namespaceName, int page, int perPage, Action<FlickrResult<PredicateCollection>> callback)
    {
      Dictionary<string, string> parameters = new Dictionary<string, string>();
      parameters.Add("method", "flickr.machinetags.getPredicates");
      if (!string.IsNullOrEmpty(namespaceName))
        parameters.Add("namespace", namespaceName);
      if (page > 0)
        parameters.Add("page", page.ToString((IFormatProvider) NumberFormatInfo.InvariantInfo));
      if (perPage > 0)
        parameters.Add("per_page", perPage.ToString((IFormatProvider) NumberFormatInfo.InvariantInfo));
      this.GetResponseAsync<PredicateCollection>(parameters, callback);
    }

    public void MachineTagsGetPairsAsync(Action<FlickrResult<PairCollection>> callback)
    {
      this.MachineTagsGetPairsAsync((string) null, (string) null, 0, 0, callback);
    }

    public void MachineTagsGetPairsAsync(int page, int perPage, Action<FlickrResult<PairCollection>> callback)
    {
      this.MachineTagsGetPairsAsync((string) null, (string) null, page, perPage, callback);
    }

    public void MachineTagsGetPairsAsync(string namespaceName, string predicate, Action<FlickrResult<PairCollection>> callback)
    {
      this.MachineTagsGetPairsAsync(namespaceName, predicate, 0, 0, callback);
    }

    public void MachineTagsGetPairsAsync(string namespaceName, string predicate, int page, int perPage, Action<FlickrResult<PairCollection>> callback)
    {
      Dictionary<string, string> parameters = new Dictionary<string, string>();
      parameters.Add("method", "flickr.machinetags.getPairs");
      if (!string.IsNullOrEmpty(namespaceName))
        parameters.Add("namespace", namespaceName);
      if (!string.IsNullOrEmpty(predicate))
        parameters.Add("predicate", predicate);
      if (page > 0)
        parameters.Add("page", page.ToString((IFormatProvider) NumberFormatInfo.InvariantInfo));
      if (perPage > 0)
        parameters.Add("per_page", perPage.ToString((IFormatProvider) NumberFormatInfo.InvariantInfo));
      this.GetResponseAsync<PairCollection>(parameters, callback);
    }

    public void MachineTagsGetValuesAsync(string namespaceName, string predicate, Action<FlickrResult<ValueCollection>> callback)
    {
      this.MachineTagsGetValuesAsync(namespaceName, predicate, 0, 0, callback);
    }

    public void MachineTagsGetValuesAsync(string namespaceName, string predicate, int page, int perPage, Action<FlickrResult<ValueCollection>> callback)
    {
      Dictionary<string, string> parameters = new Dictionary<string, string>();
      parameters.Add("method", "flickr.machinetags.getValues");
      parameters.Add("namespace", namespaceName);
      parameters.Add("predicate", predicate);
      if (page > 0)
        parameters.Add("page", page.ToString((IFormatProvider) NumberFormatInfo.InvariantInfo));
      if (perPage > 0)
        parameters.Add("per_page", perPage.ToString((IFormatProvider) NumberFormatInfo.InvariantInfo));
      this.GetResponseAsync<ValueCollection>(parameters, callback);
    }

    public void MachineTagsGetRecentValuesAsync(DateTime addedSince, Action<FlickrResult<ValueCollection>> callback)
    {
      this.MachineTagsGetRecentValuesAsync((string) null, (string) null, addedSince, 0, 0, callback);
    }

    public void MachineTagsGetRecentValuesAsync(DateTime addedSince, int page, int perPage, Action<FlickrResult<ValueCollection>> callback)
    {
      this.MachineTagsGetRecentValuesAsync((string) null, (string) null, addedSince, page, perPage, callback);
    }

    public void MachineTagsGetRecentValuesAsync(string namespaceName, string predicate, Action<FlickrResult<ValueCollection>> callback)
    {
      this.MachineTagsGetRecentValuesAsync(namespaceName, predicate, DateTime.MinValue, 0, 0, callback);
    }

    public void MachineTagsGetRecentValuesAsync(string namespaceName, string predicate, int page, int perPage, Action<FlickrResult<ValueCollection>> callback)
    {
      this.MachineTagsGetRecentValuesAsync(namespaceName, predicate, DateTime.MinValue, page, perPage, callback);
    }

    public void MachineTagsGetRecentValuesAsync(string namespaceName, string predicate, DateTime addedSince, int page, int perPage, Action<FlickrResult<ValueCollection>> callback)
    {
      if (string.IsNullOrEmpty(namespaceName) && string.IsNullOrEmpty(predicate) && addedSince == DateTime.MinValue)
        throw new ArgumentException("Must supply one of namespaceName, predicate or addedSince");
      Dictionary<string, string> parameters = new Dictionary<string, string>();
      parameters.Add("method", "flickr.machinetags.getRecentValues");
      if (!string.IsNullOrEmpty(namespaceName))
        parameters.Add("namespace", namespaceName);
      if (!string.IsNullOrEmpty(predicate))
        parameters.Add("predicate", predicate);
      if (addedSince != DateTime.MinValue)
        parameters.Add("added_since", UtilityMethods.DateToUnixTimestamp(addedSince));
      if (page > 0)
        parameters.Add("page", page.ToString((IFormatProvider) NumberFormatInfo.InvariantInfo));
      if (perPage > 0)
        parameters.Add("per_page", perPage.ToString((IFormatProvider) NumberFormatInfo.InvariantInfo));
      this.GetResponseAsync<ValueCollection>(parameters, callback);
    }

    public void PhotosNotesAddAsync(string photoId, int noteX, int noteY, int noteWidth, int noteHeight, string noteText, Action<FlickrResult<string>> callback)
    {
      this.GetResponseAsync<UnknownResponse>(new Dictionary<string, string>()
      {
        {
          "method",
          "flickr.photos.notes.add"
        },
        {
          "photo_id",
          photoId
        },
        {
          "note_x",
          noteX.ToString((IFormatProvider) NumberFormatInfo.InvariantInfo)
        },
        {
          "note_y",
          noteY.ToString((IFormatProvider) NumberFormatInfo.InvariantInfo)
        },
        {
          "note_w",
          noteWidth.ToString((IFormatProvider) NumberFormatInfo.InvariantInfo)
        },
        {
          "note_h",
          noteHeight.ToString((IFormatProvider) NumberFormatInfo.InvariantInfo)
        },
        {
          "note_text",
          noteText
        }
      }, (Action<FlickrResult<UnknownResponse>>) (r =>
      {
        FlickrResult<string> local_0 = new FlickrResult<string>();
        local_0.HasError = r.HasError;
        if (r.HasError)
          local_0.Error = r.Error;
        else
          local_0.Result = r.Result.GetAttributeValue("*", "id");
        callback(local_0);
      }));
    }

    public void PhotosNotesEditAsync(string noteId, int noteX, int noteY, int noteWidth, int noteHeight, string noteText, Action<FlickrResult<NoResponse>> callback)
    {
      this.GetResponseAsync<NoResponse>(new Dictionary<string, string>()
      {
        {
          "method",
          "flickr.photos.notes.edit"
        },
        {
          "note_id",
          noteId
        },
        {
          "note_x",
          noteX.ToString((IFormatProvider) NumberFormatInfo.InvariantInfo)
        },
        {
          "note_y",
          noteY.ToString((IFormatProvider) NumberFormatInfo.InvariantInfo)
        },
        {
          "note_w",
          noteWidth.ToString((IFormatProvider) NumberFormatInfo.InvariantInfo)
        },
        {
          "note_h",
          noteHeight.ToString((IFormatProvider) NumberFormatInfo.InvariantInfo)
        },
        {
          "note_text",
          noteText
        }
      }, callback);
    }

    public void PhotosNotesDeleteAsync(string noteId, Action<FlickrResult<NoResponse>> callback)
    {
      this.GetResponseAsync<NoResponse>(new Dictionary<string, string>()
      {
        {
          "method",
          "flickr.photos.notes.delete"
        },
        {
          "note_id",
          noteId
        }
      }, callback);
    }

    public string OAuthCalculateSignature(string method, string url, Dictionary<string, string> parameters, string tokenSecret)
    {
      byte[] bytes = Encoding.UTF8.GetBytes(this.ApiSecret + "&" + tokenSecret);
      SortedList<string, string> sortedList = new SortedList<string, string>();
      foreach (KeyValuePair<string, string> keyValuePair in parameters)
        sortedList.Add(keyValuePair.Key, keyValuePair.Value);
      StringBuilder stringBuilder = new StringBuilder();
      foreach (KeyValuePair<string, string> keyValuePair in sortedList)
      {
        stringBuilder.Append(keyValuePair.Key);
        stringBuilder.Append("=");
        stringBuilder.Append(UtilityMethods.EscapeOAuthString(keyValuePair.Value));
        stringBuilder.Append("&");
      }
      stringBuilder.Remove(stringBuilder.Length - 1, 1);
      string s = method + "&" + UtilityMethods.EscapeOAuthString(url) + "&" + UtilityMethods.EscapeOAuthString(((object) stringBuilder).ToString());
      return Convert.ToBase64String(new HMACSHA1(bytes).ComputeHash(Encoding.UTF8.GetBytes(s)));
    }

    public string OAuthCalculateAuthorizationUrl(string requestToken, AuthLevel perms)
    {
      return this.OAuthCalculateAuthorizationUrl(requestToken, perms, false);
    }

    public string OAuthCalculateAuthorizationUrl(string requestToken, AuthLevel perms, bool mobile)
    {
      string str = perms == AuthLevel.None ? "" : "&perms=" + UtilityMethods.AuthLevelToString(perms);
      return "https://" + (mobile ? "m" : "www") + ".flickr.com/services/oauth/authorize?oauth_token=" + requestToken + str;
    }

    private void OAuthGetBasicParameters(Dictionary<string, string> parameters)
    {
      foreach (KeyValuePair<string, string> keyValuePair in this.OAuthGetBasicParameters())
        parameters.Add(keyValuePair.Key, keyValuePair.Value);
    }

    private Dictionary<string, string> OAuthGetBasicParameters()
    {
      return new Dictionary<string, string>()
      {
        {
          "oauth_nonce",
          Guid.NewGuid().ToString("N")
        },
        {
          "oauth_timestamp",
          UtilityMethods.DateToUnixTimestamp(DateTime.UtcNow)
        },
        {
          "oauth_version",
          "1.0"
        },
        {
          "oauth_signature_method",
          "HMAC-SHA1"
        },
        {
          "oauth_consumer_key",
          this.ApiKey
        }
      };
    }

    public void OAuthGetRequestTokenAsync(string callbackUrl, Action<FlickrResult<OAuthRequestToken>> callback)
    {
      this.CheckApiKey();
      string str1 = "https://www.flickr.com/services/oauth/request_token";
      Dictionary<string, string> basicParameters = this.OAuthGetBasicParameters();
      basicParameters.Add("oauth_callback", callbackUrl);
      string str2 = this.OAuthCalculateSignature("POST", str1, basicParameters, (string) null);
      basicParameters.Add("oauth_signature", str2);
      FlickrResponder.GetDataResponseAsync(this, str1, basicParameters, (Action<FlickrResult<string>>) (r =>
      {
        FlickrResult<OAuthRequestToken> local_0 = new FlickrResult<OAuthRequestToken>();
        if (r.Error != null)
        {
          if (r.Error is WebException)
          {
            OAuthException local_1 = new OAuthException(r.Error);
            local_0.Error = (Exception) local_1;
          }
          else
            local_0.Error = r.Error;
          callback(local_0);
        }
        else
        {
          local_0.Result = OAuthRequestToken.ParseResponse(r.Result);
          callback(local_0);
        }
      }));
    }

    public void OAuthGetAccessTokenAsync(OAuthRequestToken requestToken, string verifier, Action<FlickrResult<OAuthAccessToken>> callback)
    {
      this.OAuthGetAccessTokenAsync(requestToken.Token, requestToken.TokenSecret, verifier, callback);
    }

    public void OAuthGetAccessTokenAsync(string requestToken, string requestTokenSecret, string verifier, Action<FlickrResult<OAuthAccessToken>> callback)
    {
      this.CheckApiKey();
      string str1 = "https://www.flickr.com/services/oauth/access_token";
      Dictionary<string, string> basicParameters = this.OAuthGetBasicParameters();
      basicParameters.Add("oauth_verifier", verifier);
      basicParameters.Add("oauth_token", requestToken);
      string str2 = this.OAuthCalculateSignature("POST", str1, basicParameters, requestTokenSecret);
      basicParameters.Add("oauth_signature", str2);
      FlickrResponder.GetDataResponseAsync(this, str1, basicParameters, (Action<FlickrResult<string>>) (r =>
      {
        FlickrResult<OAuthAccessToken> local_0 = new FlickrResult<OAuthAccessToken>();
        if (r.Error != null)
        {
          if (r.Error is WebException)
          {
            OAuthException local_1 = new OAuthException(r.Error);
            local_0.Error = (Exception) local_1;
          }
          else
            local_0.Error = r.Error;
          callback(local_0);
        }
        else
        {
          local_0.Result = OAuthAccessToken.ParseResponse(r.Result);
          callback(local_0);
        }
      }));
    }

    public OAuthRequestToken OAuthGetRequestToken(string callback)
    {
      this.CheckApiKey();
      string str1 = "http://www.flickr.com/services/oauth/request_token";
      Dictionary<string, string> basicParameters = this.OAuthGetBasicParameters();
      basicParameters.Add("oauth_callback", callback);
      string str2 = this.OAuthCalculateSignature("POST", str1, basicParameters, (string) null);
      basicParameters.Add("oauth_signature", str2);
      return OAuthRequestToken.ParseResponse(FlickrResponder.GetDataResponse(this, str1, basicParameters));
    }

    public OAuthAccessToken OAuthGetAccessToken(OAuthRequestToken requestToken, string verifier)
    {
      return this.OAuthGetAccessToken(requestToken.Token, requestToken.TokenSecret, verifier);
    }

    public OAuthAccessToken OAuthGetAccessToken(string requestToken, string requestTokenSecret, string verifier)
    {
      this.CheckApiKey();
      string str1 = "http://www.flickr.com/services/oauth/access_token";
      Dictionary<string, string> basicParameters = this.OAuthGetBasicParameters();
      basicParameters.Add("oauth_verifier", verifier);
      basicParameters.Add("oauth_token", requestToken);
      string str2 = this.OAuthCalculateSignature("POST", str1, basicParameters, requestTokenSecret);
      basicParameters.Add("oauth_signature", str2);
      OAuthAccessToken oauthAccessToken = OAuthAccessToken.ParseResponse(FlickrResponder.GetDataResponse(this, str1, basicParameters));
      this.OAuthAccessToken = oauthAccessToken.Token;
      this.OAuthAccessTokenSecret = oauthAccessToken.TokenSecret;
      return oauthAccessToken;
    }

    public void PandaGetListAsync(Action<FlickrResult<string[]>> callback)
    {
      this.GetResponseAsync<UnknownResponse>(new Dictionary<string, string>()
      {
        {
          "method",
          "flickr.panda.getList"
        }
      }, (Action<FlickrResult<UnknownResponse>>) (r =>
      {
        FlickrResult<string[]> local_0 = new FlickrResult<string[]>();
        local_0.HasError = r.HasError;
        if (r.HasError)
          local_0.Error = r.Error;
        else
          local_0.Result = r.Result.GetElementArray("panda");
        callback(local_0);
      }));
    }

    public void PandaGetPhotosAsync(string pandaName, Action<FlickrResult<PandaPhotoCollection>> callback)
    {
      this.PandaGetPhotosAsync(pandaName, PhotoSearchExtras.None, 0, 0, callback);
    }

    public void PandaGetPhotosAsync(string pandaName, PhotoSearchExtras extras, Action<FlickrResult<PandaPhotoCollection>> callback)
    {
      this.PandaGetPhotosAsync(pandaName, extras, 0, 0, callback);
    }

    public void PandaGetPhotosAsync(string pandaName, int page, int perPage, Action<FlickrResult<PandaPhotoCollection>> callback)
    {
      this.PandaGetPhotosAsync(pandaName, PhotoSearchExtras.None, page, perPage, callback);
    }

    public void PandaGetPhotosAsync(string pandaName, PhotoSearchExtras extras, int page, int perPage, Action<FlickrResult<PandaPhotoCollection>> callback)
    {
      Dictionary<string, string> parameters = new Dictionary<string, string>();
      parameters.Add("method", "flickr.panda.getPhotos");
      parameters.Add("panda_name", pandaName);
      if (extras != PhotoSearchExtras.None)
        parameters.Add("extras", UtilityMethods.ExtrasToString(extras));
      if (perPage > 0)
        parameters.Add("per_page", perPage.ToString((IFormatProvider) NumberFormatInfo.InvariantInfo));
      if (page > 0)
        parameters.Add("page", page.ToString((IFormatProvider) NumberFormatInfo.InvariantInfo));
      this.GetResponseAsync<PandaPhotoCollection>(parameters, callback);
    }

    public void PeopleFindByEmailAsync(string emailAddress, Action<FlickrResult<FoundUser>> callback)
    {
      this.GetResponseAsync<FoundUser>(new Dictionary<string, string>()
      {
        {
          "method",
          "flickr.people.findByEmail"
        },
        {
          "api_key",
          this.apiKey
        },
        {
          "find_email",
          emailAddress
        }
      }, callback);
    }

    public void PeopleFindByUserNameAsync(string userName, Action<FlickrResult<FoundUser>> callback)
    {
      this.GetResponseAsync<FoundUser>(new Dictionary<string, string>()
      {
        {
          "method",
          "flickr.people.findByUsername"
        },
        {
          "api_key",
          this.apiKey
        },
        {
          "username",
          userName
        }
      }, callback);
    }

    public void PeopleGetGroupsAsync(string userId, Action<FlickrResult<GroupInfoCollection>> callback)
    {
      this.CheckRequiresAuthentication();
      this.GetResponseAsync<GroupInfoCollection>(new Dictionary<string, string>()
      {
        {
          "method",
          "flickr.people.getGroups"
        },
        {
          "user_id",
          userId
        }
      }, callback);
    }

    public void PeopleGetInfoAsync(string userId, Action<FlickrResult<Person>> callback)
    {
      this.GetResponseAsync<Person>(new Dictionary<string, string>()
      {
        {
          "method",
          "flickr.people.getInfo"
        },
        {
          "api_key",
          this.apiKey
        },
        {
          "user_id",
          userId
        }
      }, callback);
    }

    public void PeopleGetLimitsAsync(Action<FlickrResult<PersonLimits>> callback)
    {
      this.CheckRequiresAuthentication();
      this.GetResponseAsync<PersonLimits>(new Dictionary<string, string>()
      {
        {
          "method",
          "flickr.people.getLimits"
        }
      }, callback);
    }

    public void PeopleGetUploadStatusAsync(Action<FlickrResult<UserStatus>> callback)
    {
      this.GetResponseAsync<UserStatus>(new Dictionary<string, string>()
      {
        {
          "method",
          "flickr.people.getUploadStatus"
        }
      }, callback);
    }

    public void PeopleGetPublicGroupsAsync(string userId, Action<FlickrResult<GroupInfoCollection>> callback)
    {
      this.GetResponseAsync<GroupInfoCollection>(new Dictionary<string, string>()
      {
        {
          "method",
          "flickr.people.getPublicGroups"
        },
        {
          "api_key",
          this.apiKey
        },
        {
          "user_id",
          userId
        }
      }, callback);
    }

    public void PeopleGetPublicPhotosAsync(string userId, Action<FlickrResult<PhotoCollection>> callback)
    {
      this.PeopleGetPublicPhotosAsync(userId, 0, 0, SafetyLevel.None, PhotoSearchExtras.None, callback);
    }

    public void PeopleGetPublicPhotosAsync(string userId, int page, int perPage, Action<FlickrResult<PhotoCollection>> callback)
    {
      this.PeopleGetPublicPhotosAsync(userId, page, perPage, SafetyLevel.None, PhotoSearchExtras.None, callback);
    }

    public void PeopleGetPublicPhotosAsync(string userId, int page, int perPage, SafetyLevel safetyLevel, PhotoSearchExtras extras, Action<FlickrResult<PhotoCollection>> callback)
    {
      if (!this.IsAuthenticated && safetyLevel > SafetyLevel.Safe)
        throw new ArgumentException("Safety level may only be 'Safe' for unauthenticated calls", "safetyLevel");
      Dictionary<string, string> parameters = new Dictionary<string, string>();
      parameters.Add("method", "flickr.people.getPublicPhotos");
      parameters.Add("api_key", this.apiKey);
      parameters.Add("user_id", userId);
      if (perPage > 0)
        parameters.Add("per_page", perPage.ToString((IFormatProvider) NumberFormatInfo.InvariantInfo));
      if (page > 0)
        parameters.Add("page", page.ToString((IFormatProvider) NumberFormatInfo.InvariantInfo));
      if (safetyLevel != SafetyLevel.None)
        parameters.Add("safety_level", safetyLevel.ToString("D"));
      if (extras != PhotoSearchExtras.None)
        parameters.Add("extras", UtilityMethods.ExtrasToString(extras));
      this.GetResponseAsync<PhotoCollection>(parameters, callback);
    }

    public void PeopleGetPhotosAsync(Action<FlickrResult<PhotoCollection>> callback)
    {
      this.PeopleGetPhotosAsync((string) null, SafetyLevel.None, DateTime.MinValue, DateTime.MinValue, DateTime.MinValue, DateTime.MinValue, ContentTypeSearch.None, PrivacyFilter.None, PhotoSearchExtras.None, 0, 0, callback);
    }

    public void PeopleGetPhotosAsync(int page, int perPage, Action<FlickrResult<PhotoCollection>> callback)
    {
      this.PeopleGetPhotosAsync((string) null, SafetyLevel.None, DateTime.MinValue, DateTime.MinValue, DateTime.MinValue, DateTime.MinValue, ContentTypeSearch.None, PrivacyFilter.None, PhotoSearchExtras.None, page, perPage, callback);
    }

    public void PeopleGetPhotosAsync(PhotoSearchExtras extras, Action<FlickrResult<PhotoCollection>> callback)
    {
      this.PeopleGetPhotosAsync((string) null, SafetyLevel.None, DateTime.MinValue, DateTime.MinValue, DateTime.MinValue, DateTime.MinValue, ContentTypeSearch.None, PrivacyFilter.None, extras, 0, 0, callback);
    }

    public void PeopleGetPhotosAsync(PhotoSearchExtras extras, int page, int perPage, Action<FlickrResult<PhotoCollection>> callback)
    {
      this.PeopleGetPhotosAsync((string) null, SafetyLevel.None, DateTime.MinValue, DateTime.MinValue, DateTime.MinValue, DateTime.MinValue, ContentTypeSearch.None, PrivacyFilter.None, extras, page, perPage, callback);
    }

    public void PeopleGetPhotosAsync(string userId, Action<FlickrResult<PhotoCollection>> callback)
    {
      this.PeopleGetPhotosAsync(userId, SafetyLevel.None, DateTime.MinValue, DateTime.MinValue, DateTime.MinValue, DateTime.MinValue, ContentTypeSearch.None, PrivacyFilter.None, PhotoSearchExtras.None, 0, 0, callback);
    }

    public void PeopleGetPhotosAsync(string userId, int page, int perPage, Action<FlickrResult<PhotoCollection>> callback)
    {
      this.PeopleGetPhotosAsync(userId, SafetyLevel.None, DateTime.MinValue, DateTime.MinValue, DateTime.MinValue, DateTime.MinValue, ContentTypeSearch.None, PrivacyFilter.None, PhotoSearchExtras.None, page, perPage, callback);
    }

    public void PeopleGetPhotosAsync(string userId, PhotoSearchExtras extras, Action<FlickrResult<PhotoCollection>> callback)
    {
      this.PeopleGetPhotosAsync(userId, SafetyLevel.None, DateTime.MinValue, DateTime.MinValue, DateTime.MinValue, DateTime.MinValue, ContentTypeSearch.None, PrivacyFilter.None, extras, 0, 0, callback);
    }

    public void PeopleGetPhotosAsync(string userId, PhotoSearchExtras extras, int page, int perPage, Action<FlickrResult<PhotoCollection>> callback)
    {
      this.PeopleGetPhotosAsync(userId, SafetyLevel.None, DateTime.MinValue, DateTime.MinValue, DateTime.MinValue, DateTime.MinValue, ContentTypeSearch.None, PrivacyFilter.None, extras, page, perPage, callback);
    }

    public void PeopleGetPhotosAsync(string userId, SafetyLevel safeSearch, DateTime minUploadDate, DateTime maxUploadDate, DateTime minTakenDate, DateTime maxTakenDate, ContentTypeSearch contentType, PrivacyFilter privacyFilter, PhotoSearchExtras extras, int page, int perPage, Action<FlickrResult<PhotoCollection>> callback)
    {
      this.CheckRequiresAuthentication();
      Dictionary<string, string> parameters = new Dictionary<string, string>();
      parameters.Add("method", "flickr.people.getPhotos");
      parameters.Add("user_id", userId ?? "me");
      if (safeSearch != SafetyLevel.None)
        parameters.Add("safe_search", safeSearch.ToString("d"));
      if (minUploadDate != DateTime.MinValue)
        parameters.Add("min_upload_date", UtilityMethods.DateToUnixTimestamp(minUploadDate));
      if (maxUploadDate != DateTime.MinValue)
        parameters.Add("max_upload_date", UtilityMethods.DateToUnixTimestamp(maxUploadDate));
      if (minTakenDate != DateTime.MinValue)
        parameters.Add("min_taken_date", UtilityMethods.DateToMySql(minTakenDate));
      if (maxTakenDate != DateTime.MinValue)
        parameters.Add("max_taken_date", UtilityMethods.DateToMySql(maxTakenDate));
      if (contentType != ContentTypeSearch.None)
        parameters.Add("content_type", contentType.ToString("d"));
      if (privacyFilter != PrivacyFilter.None)
        parameters.Add("privacy_filter", privacyFilter.ToString("d"));
      if (extras != PhotoSearchExtras.None)
        parameters.Add("extras", UtilityMethods.ExtrasToString(extras));
      if (page > 0)
        parameters.Add("page", page.ToString((IFormatProvider) NumberFormatInfo.InvariantInfo));
      if (perPage > 0)
        parameters.Add("per_page", perPage.ToString((IFormatProvider) NumberFormatInfo.InvariantInfo));
      this.GetResponseAsync<PhotoCollection>(parameters, callback);
    }

    public void PeopleGetPhotosOfAsync(Action<FlickrResult<PeoplePhotoCollection>> callback)
    {
      this.CheckRequiresAuthentication();
      this.PeopleGetPhotosOfAsync("me", PhotoSearchExtras.None, 0, 0, callback);
    }

    public void PeopleGetPhotosOfAsync(string userId, Action<FlickrResult<PeoplePhotoCollection>> callback)
    {
      this.PeopleGetPhotosOfAsync(userId, PhotoSearchExtras.None, 0, 0, callback);
    }

    public void PeopleGetPhotosOfAsync(string userId, PhotoSearchExtras extras, Action<FlickrResult<PeoplePhotoCollection>> callback)
    {
      this.PeopleGetPhotosOfAsync(userId, extras, 0, 0, callback);
    }

    public void PeopleGetPhotosOfAsync(string userId, int page, int perPage, Action<FlickrResult<PeoplePhotoCollection>> callback)
    {
      this.PeopleGetPhotosOfAsync(userId, PhotoSearchExtras.None, page, perPage, callback);
    }

    public void PeopleGetPhotosOfAsync(string userId, PhotoSearchExtras extras, int page, int perPage, Action<FlickrResult<PeoplePhotoCollection>> callback)
    {
      Dictionary<string, string> parameters = new Dictionary<string, string>();
      parameters.Add("method", "flickr.people.getPhotosOf");
      parameters.Add("user_id", userId);
      if (extras != PhotoSearchExtras.None)
        parameters.Add("extras", UtilityMethods.ExtrasToString(extras));
      if (perPage > 0)
        parameters.Add("per_page", perPage.ToString((IFormatProvider) NumberFormatInfo.InvariantInfo));
      if (page > 0)
        parameters.Add("page", page.ToString((IFormatProvider) NumberFormatInfo.InvariantInfo));
      this.GetResponseAsync<PeoplePhotoCollection>(parameters, callback);
    }

    public void PhotosAddTagsAsync(string photoId, string[] tags, Action<FlickrResult<NoResponse>> callback)
    {
      string tags1 = string.Join(",", tags);
      this.PhotosAddTagsAsync(photoId, tags1, callback);
    }

    public void PhotosAddTagsAsync(string photoId, string tags, Action<FlickrResult<NoResponse>> callback)
    {
      this.GetResponseAsync<NoResponse>(new Dictionary<string, string>()
      {
        {
          "method",
          "flickr.photos.addTags"
        },
        {
          "photo_id",
          photoId
        },
        {
          "tags",
          tags
        }
      }, callback);
    }

    public void PhotosDeleteAsync(string photoId, Action<FlickrResult<NoResponse>> callback)
    {
      this.GetResponseAsync<NoResponse>(new Dictionary<string, string>()
      {
        {
          "method",
          "flickr.photos.delete"
        },
        {
          "photo_id",
          photoId
        }
      }, callback);
    }

    public void PhotosGetAllContextsAsync(string photoId, Action<FlickrResult<AllContexts>> callback)
    {
      this.GetResponseAsync<AllContexts>(new Dictionary<string, string>()
      {
        {
          "method",
          "flickr.photos.getAllContexts"
        },
        {
          "photo_id",
          photoId
        }
      }, callback);
    }

    public void PhotosGetContactsPhotosAsync(Action<FlickrResult<PhotoCollection>> callback)
    {
      this.PhotosGetContactsPhotosAsync(0, false, false, false, PhotoSearchExtras.None, callback);
    }

    public void PhotosGetContactsPhotosAsync(int count, Action<FlickrResult<PhotoCollection>> callback)
    {
      this.PhotosGetContactsPhotosAsync(count, false, false, false, PhotoSearchExtras.None, callback);
    }

    public void PhotosGetContactsPhotosAsync(int count, bool justFriends, bool singlePhoto, bool includeSelf, PhotoSearchExtras extras, Action<FlickrResult<PhotoCollection>> callback)
    {
      this.CheckRequiresAuthentication();
      if (count != 0 && (count < 10 || count > 50) && !singlePhoto)
      {
        throw new ArgumentOutOfRangeException("count", string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Count must be between 10 and 50. ({0})", new object[1]
        {
          (object) count
        }));
      }
      else
      {
        Dictionary<string, string> parameters = new Dictionary<string, string>();
        parameters.Add("method", "flickr.photos.getContactsPhotos");
        if (count > 0 && !singlePhoto)
          parameters.Add("count", count.ToString((IFormatProvider) NumberFormatInfo.InvariantInfo));
        if (justFriends)
          parameters.Add("just_friends", "1");
        if (singlePhoto)
          parameters.Add("single_photo", "1");
        if (includeSelf)
          parameters.Add("include_self", "1");
        if (extras != PhotoSearchExtras.None)
          parameters.Add("extras", UtilityMethods.ExtrasToString(extras));
        this.GetResponseAsync<PhotoCollection>(parameters, callback);
      }
    }

    public void PhotosGetContactsPublicPhotosAsync(string userId, Action<FlickrResult<PhotoCollection>> callback)
    {
      this.PhotosGetContactsPublicPhotosAsync(userId, 0, false, false, false, PhotoSearchExtras.None, callback);
    }

    public void PhotosGetContactsPublicPhotosAsync(string userId, PhotoSearchExtras extras, Action<FlickrResult<PhotoCollection>> callback)
    {
      this.PhotosGetContactsPublicPhotosAsync(userId, 0, false, false, false, extras, callback);
    }

    public void PhotosGetContactsPublicPhotosAsync(string userId, int count, Action<FlickrResult<PhotoCollection>> callback)
    {
      this.PhotosGetContactsPublicPhotosAsync(userId, count, false, false, false, PhotoSearchExtras.None, callback);
    }

    public void PhotosGetContactsPublicPhotosAsync(string userId, int count, PhotoSearchExtras extras, Action<FlickrResult<PhotoCollection>> callback)
    {
      this.PhotosGetContactsPublicPhotosAsync(userId, count, false, false, false, extras, callback);
    }

    public void PhotosGetContactsPublicPhotosAsync(string userId, int count, bool justFriends, bool singlePhoto, bool includeSelf, Action<FlickrResult<PhotoCollection>> callback)
    {
      this.PhotosGetContactsPublicPhotosAsync(userId, count, justFriends, singlePhoto, includeSelf, PhotoSearchExtras.None, callback);
    }

    public void PhotosGetContactsPublicPhotosAsync(string userId, int count, bool justFriends, bool singlePhoto, bool includeSelf, PhotoSearchExtras extras, Action<FlickrResult<PhotoCollection>> callback)
    {
      Dictionary<string, string> parameters = new Dictionary<string, string>();
      parameters.Add("method", "flickr.photos.getContactsPublicPhotos");
      parameters.Add("api_key", this.apiKey);
      parameters.Add("user_id", userId);
      if (count > 0)
        parameters.Add("count", count.ToString((IFormatProvider) NumberFormatInfo.InvariantInfo));
      if (justFriends)
        parameters.Add("just_friends", "1");
      if (singlePhoto)
        parameters.Add("single_photo", "1");
      if (includeSelf)
        parameters.Add("include_self", "1");
      if (extras != PhotoSearchExtras.None)
        parameters.Add("extras", UtilityMethods.ExtrasToString(extras));
      this.GetResponseAsync<PhotoCollection>(parameters, callback);
    }

    public void PhotosGetContextAsync(string photoId, Action<FlickrResult<Context>> callback)
    {
      this.GetResponseAsync<Context>(new Dictionary<string, string>()
      {
        {
          "method",
          "flickr.photos.getContext"
        },
        {
          "photo_id",
          photoId
        }
      }, callback);
    }

    public void PhotosGetCountsAsync(DateTime[] dates, Action<FlickrResult<PhotoCountCollection>> callback)
    {
      this.PhotosGetCountsAsync(dates, false, callback);
    }

    public void PhotosGetCountsAsync(DateTime[] dates, bool taken, Action<FlickrResult<PhotoCountCollection>> callback)
    {
      if (taken)
        this.PhotosGetCountsAsync((DateTime[]) null, dates, callback);
      else
        this.PhotosGetCountsAsync(dates, (DateTime[]) null, callback);
    }

    public void PhotosGetCountsAsync(DateTime[] dates, DateTime[] takenDates, Action<FlickrResult<PhotoCountCollection>> callback)
    {
      this.CheckRequiresAuthentication();
      string str1 = (string) null;
      string str2 = (string) null;
      if (dates != null && dates.Length > 0)
      {
        Array.Sort<DateTime>(dates);
        str1 = string.Join(",", new List<DateTime>((IEnumerable<DateTime>) dates).ConvertAll<string>((Converter<DateTime, string>) (d => ((object) UtilityMethods.DateToUnixTimestamp(d)).ToString())).ToArray());
      }
      if (takenDates != null && takenDates.Length > 0)
      {
        Array.Sort<DateTime>(takenDates);
        str2 = string.Join(",", new List<DateTime>((IEnumerable<DateTime>) takenDates).ConvertAll<string>((Converter<DateTime, string>) (d => ((object) UtilityMethods.DateToUnixTimestamp(d)).ToString())).ToArray());
      }
      Dictionary<string, string> parameters = new Dictionary<string, string>();
      parameters.Add("method", "flickr.photos.getCounts");
      if (str1 != null && str1.Length > 0)
        parameters.Add("dates", str1);
      if (str2 != null && str2.Length > 0)
        parameters.Add("taken_dates", str2);
      this.GetResponseAsync<PhotoCountCollection>(parameters, callback);
    }

    public void PhotosGetExifAsync(string photoId, Action<FlickrResult<ExifTagCollection>> callback)
    {
      this.PhotosGetExifAsync(photoId, (string) null, callback);
    }

    public void PhotosGetExifAsync(string photoId, string secret, Action<FlickrResult<ExifTagCollection>> callback)
    {
      Dictionary<string, string> parameters = new Dictionary<string, string>();
      parameters.Add("method", "flickr.photos.getExif");
      parameters.Add("photo_id", photoId);
      if (secret != null)
        parameters.Add("secret", secret);
      this.GetResponseAsync<ExifTagCollection>(parameters, callback);
    }

    public void PhotosGetInfoAsync(string photoId, Action<FlickrResult<PhotoInfo>> callback)
    {
      this.PhotosGetInfoAsync(photoId, (string) null, callback);
    }

    public void PhotosGetInfoAsync(string photoId, string secret, Action<FlickrResult<PhotoInfo>> callback)
    {
      Dictionary<string, string> parameters = new Dictionary<string, string>();
      parameters.Add("method", "flickr.photos.getInfo");
      parameters.Add("photo_id", photoId);
      if (secret != null)
        parameters.Add("secret", secret);
      this.GetResponseAsync<PhotoInfo>(parameters, callback);
    }

    public void PhotosGetPermsAsync(string photoId, Action<FlickrResult<PhotoPermissions>> callback)
    {
      this.CheckRequiresAuthentication();
      this.GetResponseAsync<PhotoPermissions>(new Dictionary<string, string>()
      {
        {
          "method",
          "flickr.photos.getPerms"
        },
        {
          "photo_id",
          photoId
        }
      }, callback);
    }

    public void PhotosGetRecentAsync(Action<FlickrResult<PhotoCollection>> callback)
    {
      this.PhotosGetRecentAsync(0, 0, PhotoSearchExtras.None, callback);
    }

    public void PhotosGetRecentAsync(PhotoSearchExtras extras, Action<FlickrResult<PhotoCollection>> callback)
    {
      this.PhotosGetRecentAsync(0, 0, extras, callback);
    }

    public void PhotosGetRecentAsync(int page, int perPage, Action<FlickrResult<PhotoCollection>> callback)
    {
      this.PhotosGetRecentAsync(page, perPage, PhotoSearchExtras.None, callback);
    }

    public void PhotosGetRecentAsync(int page, int perPage, PhotoSearchExtras extras, Action<FlickrResult<PhotoCollection>> callback)
    {
      Dictionary<string, string> parameters = new Dictionary<string, string>();
      parameters.Add("method", "flickr.photos.getRecent");
      parameters.Add("api_key", this.apiKey);
      if (perPage > 0)
        parameters.Add("per_page", perPage.ToString((IFormatProvider) NumberFormatInfo.InvariantInfo));
      if (page > 0)
        parameters.Add("page", page.ToString((IFormatProvider) NumberFormatInfo.InvariantInfo));
      if (extras != PhotoSearchExtras.None)
        parameters.Add("extras", UtilityMethods.ExtrasToString(extras));
      this.GetResponseAsync<PhotoCollection>(parameters, callback);
    }

    public void PhotosGetSizesAsync(string photoId, Action<FlickrResult<SizeCollection>> callback)
    {
      this.GetResponseAsync<SizeCollection>(new Dictionary<string, string>()
      {
        {
          "method",
          "flickr.photos.getSizes"
        },
        {
          "photo_id",
          photoId
        }
      }, callback);
    }

    public void PhotosGetUntaggedAsync(Action<FlickrResult<PhotoCollection>> callback)
    {
      this.PhotosGetUntaggedAsync(0, 0, PhotoSearchExtras.None, callback);
    }

    public void PhotosGetUntaggedAsync(PhotoSearchExtras extras, Action<FlickrResult<PhotoCollection>> callback)
    {
      this.PhotosGetUntaggedAsync(0, 0, extras, callback);
    }

    public void PhotosGetUntaggedAsync(int page, int perPage, Action<FlickrResult<PhotoCollection>> callback)
    {
      this.PhotosGetUntaggedAsync(page, perPage, PhotoSearchExtras.None, callback);
    }

    public void PhotosGetUntaggedAsync(int page, int perPage, PhotoSearchExtras extras, Action<FlickrResult<PhotoCollection>> callback)
    {
      this.PhotosGetUntaggedAsync(new PartialSearchOptions()
      {
        Page = page,
        PerPage = perPage,
        Extras = extras
      }, callback);
    }

    public void PhotosGetUntaggedAsync(PartialSearchOptions options, Action<FlickrResult<PhotoCollection>> callback)
    {
      Dictionary<string, string> parameters = new Dictionary<string, string>();
      parameters.Add("method", "flickr.photos.getUntagged");
      UtilityMethods.PartialOptionsIntoArray(options, parameters);
      this.GetResponseAsync<PhotoCollection>(parameters, callback);
    }

    public void PhotosGetNotInSetAsync(Action<FlickrResult<PhotoCollection>> callback)
    {
      this.PhotosGetNotInSetAsync(new PartialSearchOptions(), callback);
    }

    public void PhotosGetNotInSetAsync(int page, Action<FlickrResult<PhotoCollection>> callback)
    {
      this.PhotosGetNotInSetAsync(page, 0, PhotoSearchExtras.None, callback);
    }

    public void PhotosGetNotInSetAsync(int page, int perPage, Action<FlickrResult<PhotoCollection>> callback)
    {
      this.PhotosGetNotInSetAsync(page, perPage, PhotoSearchExtras.None, callback);
    }

    public void PhotosGetNotInSetAsync(int page, int perPage, PhotoSearchExtras extras, Action<FlickrResult<PhotoCollection>> callback)
    {
      this.PhotosGetNotInSetAsync(new PartialSearchOptions()
      {
        PerPage = perPage,
        Page = page,
        Extras = extras
      }, callback);
    }

    public void PhotosGetNotInSetAsync(PartialSearchOptions options, Action<FlickrResult<PhotoCollection>> callback)
    {
      Dictionary<string, string> parameters = new Dictionary<string, string>();
      parameters.Add("method", "flickr.photos.getNotInSet");
      UtilityMethods.PartialOptionsIntoArray(options, parameters);
      this.GetResponseAsync<PhotoCollection>(parameters, callback);
    }

    public void PhotosLicensesGetInfoAsync(Action<FlickrResult<LicenseCollection>> callback)
    {
      this.GetResponseAsync<LicenseCollection>(new Dictionary<string, string>()
      {
        {
          "method",
          "flickr.photos.licenses.getInfo"
        },
        {
          "api_key",
          this.apiKey
        }
      }, callback);
    }

    public void PhotosLicensesSetLicenseAsync(string photoId, LicenseType license, Action<FlickrResult<NoResponse>> callback)
    {
      this.CheckRequiresAuthentication();
      this.GetResponseAsync<NoResponse>(new Dictionary<string, string>()
      {
        {
          "method",
          "flickr.photos.licenses.setLicense"
        },
        {
          "photo_id",
          photoId
        },
        {
          "license_id",
          license.ToString("d")
        }
      }, callback);
    }

    public void PhotosRemoveTagAsync(string tagId, Action<FlickrResult<NoResponse>> callback)
    {
      this.GetResponseAsync<NoResponse>(new Dictionary<string, string>()
      {
        {
          "method",
          "flickr.photos.removeTag"
        },
        {
          "tag_id",
          tagId
        }
      }, callback);
    }

    public void PhotosRecentlyUpdatedAsync(DateTime minDate, PhotoSearchExtras extras, Action<FlickrResult<PhotoCollection>> callback)
    {
      this.PhotosRecentlyUpdatedAsync(minDate, extras, 0, 0, callback);
    }

    public void PhotosRecentlyUpdatedAsync(DateTime minDate, int page, int perPage, Action<FlickrResult<PhotoCollection>> callback)
    {
      this.PhotosRecentlyUpdatedAsync(minDate, PhotoSearchExtras.None, page, perPage, callback);
    }

    public void PhotosRecentlyUpdatedAsync(DateTime minDate, Action<FlickrResult<PhotoCollection>> callback)
    {
      this.PhotosRecentlyUpdatedAsync(minDate, PhotoSearchExtras.None, 0, 0, callback);
    }

    public void PhotosRecentlyUpdatedAsync(DateTime minDate, PhotoSearchExtras extras, int page, int perPage, Action<FlickrResult<PhotoCollection>> callback)
    {
      this.CheckRequiresAuthentication();
      Dictionary<string, string> parameters = new Dictionary<string, string>();
      parameters.Add("method", "flickr.photos.recentlyUpdated");
      parameters.Add("min_date", UtilityMethods.DateToUnixTimestamp(minDate));
      if (extras != PhotoSearchExtras.None)
        parameters.Add("extras", UtilityMethods.ExtrasToString(extras));
      if (perPage > 0)
        parameters.Add("per_page", perPage.ToString((IFormatProvider) NumberFormatInfo.InvariantInfo));
      if (page > 0)
        parameters.Add("page", page.ToString((IFormatProvider) NumberFormatInfo.InvariantInfo));
      this.GetResponseAsync<PhotoCollection>(parameters, callback);
    }

    public void PhotosSearchAsync(PhotoSearchOptions options, Action<FlickrResult<PhotoCollection>> callback)
    {
      Dictionary<string, string> parameters = new Dictionary<string, string>();
      parameters.Add("method", "flickr.photos.search");
      options.AddToDictionary(parameters);
      this.GetResponseAsync<PhotoCollection>(parameters, callback);
    }

    public void PhotosSetDatesAsync(string photoId, DateTime dateTaken, DateGranularity granularity, Action<FlickrResult<NoResponse>> callback)
    {
      this.PhotosSetDatesAsync(photoId, DateTime.MinValue, dateTaken, granularity, callback);
    }

    public void PhotosSetDatesAsync(string photoId, DateTime datePosted, Action<FlickrResult<NoResponse>> callback)
    {
      this.PhotosSetDatesAsync(photoId, datePosted, DateTime.MinValue, DateGranularity.FullDate, callback);
    }

    public void PhotosSetDatesAsync(string photoId, DateTime datePosted, DateTime dateTaken, DateGranularity granularity, Action<FlickrResult<NoResponse>> callback)
    {
      Dictionary<string, string> parameters = new Dictionary<string, string>();
      parameters.Add("method", "flickr.photos.setDates");
      parameters.Add("photo_id", photoId);
      if (datePosted != DateTime.MinValue)
        parameters.Add("date_posted", ((object) UtilityMethods.DateToUnixTimestamp(datePosted)).ToString());
      if (dateTaken != DateTime.MinValue)
      {
        parameters.Add("date_taken", dateTaken.ToString("yyyy-MM-dd HH:mm:ss", (IFormatProvider) DateTimeFormatInfo.InvariantInfo));
        parameters.Add("date_taken_granularity", granularity.ToString("d"));
      }
      this.GetResponseAsync<NoResponse>(parameters, callback);
    }

    public void PhotosSetMetaAsync(string photoId, string title, string description, Action<FlickrResult<NoResponse>> callback)
    {
      this.GetResponseAsync<NoResponse>(new Dictionary<string, string>()
      {
        {
          "method",
          "flickr.photos.setMeta"
        },
        {
          "photo_id",
          photoId
        },
        {
          "title",
          title
        },
        {
          "description",
          description
        }
      }, callback);
    }

    public void PhotosSetPermsAsync(string photoId, int isPublic, int isFriend, int isFamily, PermissionComment permComment, PermissionAddMeta permAddMeta, Action<FlickrResult<NoResponse>> callback)
    {
      this.PhotosSetPermsAsync(photoId, isPublic == 1, isFriend == 1, isFamily == 1, permComment, permAddMeta, callback);
    }

    public void PhotosSetPermsAsync(string photoId, bool isPublic, bool isFriend, bool isFamily, PermissionComment permComment, PermissionAddMeta permAddMeta, Action<FlickrResult<NoResponse>> callback)
    {
      this.GetResponseAsync<NoResponse>(new Dictionary<string, string>()
      {
        {
          "method",
          "flickr.photos.setPerms"
        },
        {
          "photo_id",
          photoId
        },
        {
          "is_public",
          isPublic ? "1" : "0"
        },
        {
          "is_friend",
          isFriend ? "1" : "0"
        },
        {
          "is_family",
          isFamily ? "1" : "0"
        },
        {
          "perm_comment",
          permComment.ToString("d")
        },
        {
          "perm_addmeta",
          permAddMeta.ToString("d")
        }
      }, callback);
    }

    public void PhotosSetTagsAsync(string photoId, string[] tags, Action<FlickrResult<NoResponse>> callback)
    {
      string tags1 = string.Join(",", tags);
      this.PhotosSetTagsAsync(photoId, tags1, callback);
    }

    public void PhotosSetTagsAsync(string photoId, string tags, Action<FlickrResult<NoResponse>> callback)
    {
      this.GetResponseAsync<NoResponse>(new Dictionary<string, string>()
      {
        {
          "method",
          "flickr.photos.setTags"
        },
        {
          "photo_id",
          photoId
        },
        {
          "tags",
          tags
        }
      }, callback);
    }

    public void PhotosSetContentTypeAsync(string photoId, ContentType contentType, Action<FlickrResult<NoResponse>> callback)
    {
      this.CheckRequiresAuthentication();
      this.GetResponseAsync<NoResponse>(new Dictionary<string, string>()
      {
        {
          "method",
          "flickr.photos.setContentType"
        },
        {
          "photo_id",
          photoId
        },
        {
          "content_type",
          contentType.ToString("D")
        }
      }, callback);
    }

    public void PhotosSetSafetyLevelAsync(string photoId, HiddenFromSearch hidden, Action<FlickrResult<NoResponse>> callback)
    {
      this.PhotosSetSafetyLevelAsync(photoId, SafetyLevel.None, hidden, callback);
    }

    public void PhotosSetSafetyLevelAsync(string photoId, SafetyLevel safetyLevel, Action<FlickrResult<NoResponse>> callback)
    {
      this.PhotosSetSafetyLevelAsync(photoId, safetyLevel, HiddenFromSearch.None, callback);
    }

    public void PhotosSetSafetyLevelAsync(string photoId, SafetyLevel safetyLevel, HiddenFromSearch hidden, Action<FlickrResult<NoResponse>> callback)
    {
      this.CheckRequiresAuthentication();
      Dictionary<string, string> parameters = new Dictionary<string, string>();
      parameters.Add("method", "flickr.photos.setSafetyLevel");
      parameters.Add("photo_id", photoId);
      if (safetyLevel != SafetyLevel.None)
        parameters.Add("safety_level", safetyLevel.ToString("D"));
      switch (hidden)
      {
        case HiddenFromSearch.Visible:
          parameters.Add("hidden", "0");
          break;
        case HiddenFromSearch.Hidden:
          parameters.Add("hidden", "1");
          break;
      }
      this.GetResponseAsync<NoResponse>(parameters, callback);
    }

    public void PhotosGetFavoritesAsync(string photoId, Action<FlickrResult<PhotoFavoriteCollection>> callback)
    {
      this.PhotosGetFavoritesAsync(photoId, 0, 0, callback);
    }

    public void PhotosGetFavoritesAsync(string photoId, int perPage, int page, Action<FlickrResult<PhotoFavoriteCollection>> callback)
    {
      Dictionary<string, string> parameters = new Dictionary<string, string>();
      parameters.Add("method", "flickr.photos.getFavorites");
      parameters.Add("photo_id", photoId);
      if (page > 0)
        parameters.Add("page", page.ToString((IFormatProvider) NumberFormatInfo.InvariantInfo));
      if (perPage > 0)
        parameters.Add("per_page", perPage.ToString((IFormatProvider) NumberFormatInfo.InvariantInfo));
      this.GetResponseAsync<PhotoFavoriteCollection>(parameters, callback);
    }

    public void PhotosCommentsGetListAsync(string photoId, Action<FlickrResult<PhotoCommentCollection>> callback)
    {
      this.GetResponseAsync<PhotoCommentCollection>(new Dictionary<string, string>()
      {
        {
          "method",
          "flickr.photos.comments.getList"
        },
        {
          "photo_id",
          photoId
        }
      }, callback);
    }

    public void PhotosCommentsAddCommentAsync(string photoId, string commentText, Action<FlickrResult<string>> callback)
    {
      this.GetResponseAsync<UnknownResponse>(new Dictionary<string, string>()
      {
        {
          "method",
          "flickr.photos.comments.addComment"
        },
        {
          "photo_id",
          photoId
        },
        {
          "comment_text",
          commentText
        }
      }, (Action<FlickrResult<UnknownResponse>>) (r =>
      {
        FlickrResult<string> local_0 = new FlickrResult<string>();
        local_0.HasError = r.HasError;
        if (r.HasError)
          local_0.Error = r.Error;
        else
          local_0.Result = r.Result.GetAttributeValue("*", "id");
        callback(local_0);
      }));
    }

    public void PhotosCommentsDeleteCommentAsync(string commentId, Action<FlickrResult<NoResponse>> callback)
    {
      this.GetResponseAsync<NoResponse>(new Dictionary<string, string>()
      {
        {
          "method",
          "flickr.photos.comments.deleteComment"
        },
        {
          "comment_id",
          commentId
        }
      }, callback);
    }

    public void PhotosCommentsEditCommentAsync(string commentId, string commentText, Action<FlickrResult<NoResponse>> callback)
    {
      this.GetResponseAsync<NoResponse>(new Dictionary<string, string>()
      {
        {
          "method",
          "flickr.photos.comments.editComment"
        },
        {
          "comment_id",
          commentId
        },
        {
          "comment_text",
          commentText
        }
      }, callback);
    }

    public void PhotosCommentsGetRecentForContactsAsync(Action<FlickrResult<PhotoCollection>> callback)
    {
      this.PhotosCommentsGetRecentForContactsAsync(DateTime.MinValue, (string[]) null, PhotoSearchExtras.None, 0, 0, callback);
    }

    public void PhotosCommentsGetRecentForContactsAsync(int page, int perPage, Action<FlickrResult<PhotoCollection>> callback)
    {
      this.PhotosCommentsGetRecentForContactsAsync(DateTime.MinValue, (string[]) null, PhotoSearchExtras.None, page, perPage, callback);
    }

    public void PhotosCommentsGetRecentForContactsAsync(DateTime dateLastComment, PhotoSearchExtras extras, int page, int perPage, Action<FlickrResult<PhotoCollection>> callback)
    {
      this.PhotosCommentsGetRecentForContactsAsync(dateLastComment, (string[]) null, extras, page, perPage, callback);
    }

    public void PhotosCommentsGetRecentForContactsAsync(DateTime dateLastComment, string[] contactsFilter, PhotoSearchExtras extras, int page, int perPage, Action<FlickrResult<PhotoCollection>> callback)
    {
      this.CheckRequiresAuthentication();
      Dictionary<string, string> parameters = new Dictionary<string, string>();
      parameters.Add("method", "flickr.photos.comments.getRecentForContacts");
      if (dateLastComment != DateTime.MinValue)
        parameters.Add("date_lastcomment", UtilityMethods.DateToUnixTimestamp(dateLastComment));
      if (contactsFilter != null && contactsFilter.Length > 0)
        parameters.Add("contacts_filter", string.Join(",", contactsFilter));
      if (extras != PhotoSearchExtras.None)
        parameters.Add("extras", UtilityMethods.ExtrasToString(extras));
      if (page > 0)
        parameters.Add("page", page.ToString((IFormatProvider) NumberFormatInfo.InvariantInfo));
      if (perPage > 0)
        parameters.Add("per_page", perPage.ToString((IFormatProvider) NumberFormatInfo.InvariantInfo));
      this.GetResponseAsync<PhotoCollection>(parameters, callback);
    }

    public void PhotosetsAddPhotoAsync(string photosetId, string photoId, Action<FlickrResult<NoResponse>> callback)
    {
      this.CheckRequiresAuthentication();
      this.GetResponseAsync<NoResponse>(new Dictionary<string, string>()
      {
        {
          "method",
          "flickr.photosets.addPhoto"
        },
        {
          "photoset_id",
          photosetId
        },
        {
          "photo_id",
          photoId
        }
      }, callback);
    }

    public void PhotosetsCreateAsync(string title, string primaryPhotoId, Action<FlickrResult<Photoset>> callback)
    {
      this.PhotosetsCreateAsync(title, (string) null, primaryPhotoId, callback);
    }

    public void PhotosetsCreateAsync(string title, string description, string primaryPhotoId, Action<FlickrResult<Photoset>> callback)
    {
      this.CheckRequiresAuthentication();
      Dictionary<string, string> parameters = new Dictionary<string, string>();
      parameters.Add("method", "flickr.photosets.create");
      parameters.Add("primary_photo_id", primaryPhotoId);
      if (!string.IsNullOrEmpty(title))
        parameters.Add("title", title);
      if (!string.IsNullOrEmpty(description))
        parameters.Add("description", description);
      this.GetResponseAsync<Photoset>(parameters, callback);
    }

    public void PhotosetsDeleteAsync(string photosetId, Action<FlickrResult<NoResponse>> callback)
    {
      this.CheckRequiresAuthentication();
      this.GetResponseAsync<NoResponse>(new Dictionary<string, string>()
      {
        {
          "method",
          "flickr.photosets.delete"
        },
        {
          "photoset_id",
          photosetId
        }
      }, callback);
    }

    public void PhotosetsEditMetaAsync(string photosetId, string title, string description, Action<FlickrResult<NoResponse>> callback)
    {
      this.CheckRequiresAuthentication();
      this.GetResponseAsync<NoResponse>(new Dictionary<string, string>()
      {
        {
          "method",
          "flickr.photosets.editMeta"
        },
        {
          "photoset_id",
          photosetId
        },
        {
          "title",
          title
        },
        {
          "description",
          description
        }
      }, callback);
    }

    public void PhotosetsEditPhotosAsync(string photosetId, string primaryPhotoId, string[] photoIds, Action<FlickrResult<NoResponse>> callback)
    {
      this.PhotosetsEditPhotosAsync(photosetId, primaryPhotoId, string.Join(",", photoIds), callback);
    }

    public void PhotosetsEditPhotosAsync(string photosetId, string primaryPhotoId, string photoIds, Action<FlickrResult<NoResponse>> callback)
    {
      this.CheckRequiresAuthentication();
      this.GetResponseAsync<NoResponse>(new Dictionary<string, string>()
      {
        {
          "method",
          "flickr.photosets.editPhotos"
        },
        {
          "photoset_id",
          photosetId
        },
        {
          "primary_photo_id",
          primaryPhotoId
        },
        {
          "photo_ids",
          photoIds
        }
      }, callback);
    }

    public void PhotosetsGetContextAsync(string photoId, string photosetId, Action<FlickrResult<Context>> callback)
    {
      this.GetResponseAsync<Context>(new Dictionary<string, string>()
      {
        {
          "method",
          "flickr.photosets.getContext"
        },
        {
          "photo_id",
          photoId
        },
        {
          "photoset_id",
          photosetId
        }
      }, callback);
    }

    public void PhotosetsGetInfoAsync(string photosetId, Action<FlickrResult<Photoset>> callback)
    {
      this.GetResponseAsync<Photoset>(new Dictionary<string, string>()
      {
        {
          "method",
          "flickr.photosets.getInfo"
        },
        {
          "photoset_id",
          photosetId
        }
      }, callback);
    }

    public void PhotosetsGetListAsync(Action<FlickrResult<PhotosetCollection>> callback)
    {
      this.CheckRequiresAuthentication();
      this.PhotosetsGetListAsync((string) null, 0, 0, callback);
    }

    public void PhotosetsGetListAsync(int page, int perPage, Action<FlickrResult<PhotosetCollection>> callback)
    {
      this.CheckRequiresAuthentication();
      this.PhotosetsGetListAsync((string) null, page, perPage, callback);
    }

    public void PhotosetsGetListAsync(string userId, Action<FlickrResult<PhotosetCollection>> callback)
    {
      this.PhotosetsGetListAsync(userId, 0, 0, callback);
    }

    public void PhotosetsGetListAsync(string userId, int page, int perPage, Action<FlickrResult<PhotosetCollection>> callback)
    {
      Dictionary<string, string> parameters = new Dictionary<string, string>();
      parameters.Add("method", "flickr.photosets.getList");
      if (userId != null)
        parameters.Add("user_id", userId);
      if (page > 0)
        parameters.Add("page", page.ToString((IFormatProvider) NumberFormatInfo.InvariantInfo));
      if (perPage > 0)
        parameters.Add("per_page", perPage.ToString((IFormatProvider) NumberFormatInfo.InvariantInfo));
      this.GetResponseAsync<PhotosetCollection>(parameters, (Action<FlickrResult<PhotosetCollection>>) (r =>
      {
        if (!r.HasError)
        {
          foreach (Photoset item_0 in (Collection<Photoset>) r.Result)
            item_0.OwnerId = userId;
        }
        callback(r);
      }));
    }

    public void PhotosetsGetPhotosAsync(string photosetId, Action<FlickrResult<PhotosetPhotoCollection>> callback)
    {
      this.PhotosetsGetPhotosAsync(photosetId, PhotoSearchExtras.None, PrivacyFilter.None, 0, 0, callback);
    }

    public void PhotosetsGetPhotosAsync(string photosetId, int page, int perPage, Action<FlickrResult<PhotosetPhotoCollection>> callback)
    {
      this.PhotosetsGetPhotosAsync(photosetId, PhotoSearchExtras.None, PrivacyFilter.None, page, perPage, callback);
    }

    public void PhotosetsGetPhotosAsync(string photosetId, PrivacyFilter privacyFilter, Action<FlickrResult<PhotosetPhotoCollection>> callback)
    {
      this.PhotosetsGetPhotosAsync(photosetId, PhotoSearchExtras.None, privacyFilter, 0, 0, callback);
    }

    public void PhotosetsGetPhotosAsync(string photosetId, PrivacyFilter privacyFilter, int page, int perPage, Action<FlickrResult<PhotosetPhotoCollection>> callback)
    {
      this.PhotosetsGetPhotosAsync(photosetId, PhotoSearchExtras.None, privacyFilter, page, perPage, callback);
    }

    public void PhotosetsGetPhotosAsync(string photosetId, PhotoSearchExtras extras, Action<FlickrResult<PhotosetPhotoCollection>> callback)
    {
      this.PhotosetsGetPhotosAsync(photosetId, extras, PrivacyFilter.None, 0, 0, callback);
    }

    public void PhotosetsGetPhotosAsync(string photosetId, PhotoSearchExtras extras, int page, int perPage, Action<FlickrResult<PhotosetPhotoCollection>> callback)
    {
      this.PhotosetsGetPhotosAsync(photosetId, extras, PrivacyFilter.None, page, perPage, callback);
    }

    public void PhotosetsGetPhotosAsync(string photosetId, PhotoSearchExtras extras, PrivacyFilter privacyFilter, Action<FlickrResult<PhotosetPhotoCollection>> callback)
    {
      this.PhotosetsGetPhotosAsync(photosetId, extras, privacyFilter, 0, 0, callback);
    }

    public void PhotosetsGetPhotosAsync(string photosetId, PhotoSearchExtras extras, PrivacyFilter privacyFilter, int page, int perPage, Action<FlickrResult<PhotosetPhotoCollection>> callback)
    {
      this.PhotosetsGetPhotosAsync(photosetId, extras, privacyFilter, page, perPage, MediaType.None, callback);
    }

    public void PhotosetsGetPhotosAsync(string photosetId, PhotoSearchExtras extras, PrivacyFilter privacyFilter, int page, int perPage, MediaType media, Action<FlickrResult<PhotosetPhotoCollection>> callback)
    {
      Dictionary<string, string> parameters = new Dictionary<string, string>();
      parameters.Add("method", "flickr.photosets.getPhotos");
      parameters.Add("photoset_id", photosetId);
      if (extras != PhotoSearchExtras.None)
        parameters.Add("extras", UtilityMethods.ExtrasToString(extras));
      if (privacyFilter != PrivacyFilter.None)
        parameters.Add("privacy_filter", privacyFilter.ToString("d"));
      if (page > 0)
        parameters.Add("page", page.ToString((IFormatProvider) NumberFormatInfo.InvariantInfo));
      if (perPage > 0)
        parameters.Add("per_page", perPage.ToString((IFormatProvider) NumberFormatInfo.InvariantInfo));
      if (media != MediaType.None)
        parameters.Add("media", media == MediaType.All ? "all" : (media == MediaType.Photos ? "photos" : (media == MediaType.Videos ? "videos" : string.Empty)));
      this.GetResponseAsync<PhotosetPhotoCollection>(parameters, callback);
    }

    public void PhotosetsOrderSetsAsync(IEnumerable<string> photosetIds, Action<FlickrResult<NoResponse>> callback)
    {
      this.PhotosetsOrderSetsAsync(string.Join(",", new List<string>(photosetIds).ToArray()), callback);
    }

    public void PhotosetsOrderSetsAsync(string[] photosetIds, Action<FlickrResult<NoResponse>> callback)
    {
      this.PhotosetsOrderSetsAsync(string.Join(",", photosetIds), callback);
    }

    public void PhotosetsOrderSetsAsync(string photosetIds, Action<FlickrResult<NoResponse>> callback)
    {
      this.GetResponseAsync<NoResponse>(new Dictionary<string, string>()
      {
        {
          "method",
          "flickr.photosets.orderSets"
        },
        {
          "photoset_ids",
          photosetIds
        }
      }, callback);
    }

    public void PhotosetsRemovePhotoAsync(string photosetId, string photoId, Action<FlickrResult<NoResponse>> callback)
    {
      this.GetResponseAsync<NoResponse>(new Dictionary<string, string>()
      {
        {
          "method",
          "flickr.photosets.removePhoto"
        },
        {
          "photoset_id",
          photosetId
        },
        {
          "photo_id",
          photoId
        }
      }, callback);
    }

    public void PhotosetsRemovePhotosAsync(string photosetId, string[] photoIds, Action<FlickrResult<NoResponse>> callback)
    {
      this.GetResponseAsync<NoResponse>(new Dictionary<string, string>()
      {
        {
          "method",
          "flickr.photosets.removePhotos"
        },
        {
          "photoset_id",
          photosetId
        },
        {
          "photo_ids",
          string.Join(",", photoIds)
        }
      }, callback);
    }

    public void PhotosetsReorderPhotosAsync(string photosetId, string[] photoIds, Action<FlickrResult<NoResponse>> callback)
    {
      this.GetResponseAsync<NoResponse>(new Dictionary<string, string>()
      {
        {
          "method",
          "flickr.photosets.reorderPhotos"
        },
        {
          "photoset_id",
          photosetId
        },
        {
          "photo_ids",
          string.Join(",", photoIds)
        }
      }, callback);
    }

    public void PhotosetsSetPrimaryPhotoAsync(string photosetId, string photoId, Action<FlickrResult<NoResponse>> callback)
    {
      this.GetResponseAsync<NoResponse>(new Dictionary<string, string>()
      {
        {
          "method",
          "flickr.photosets.setPrimaryPhoto"
        },
        {
          "photoset_id",
          photosetId
        },
        {
          "photo_id",
          photoId
        }
      }, callback);
    }

    public void PhotosetsCommentsGetListAsync(string photosetId, Action<FlickrResult<PhotosetCommentCollection>> callback)
    {
      this.GetResponseAsync<PhotosetCommentCollection>(new Dictionary<string, string>()
      {
        {
          "method",
          "flickr.photosets.comments.getList"
        },
        {
          "photoset_id",
          photosetId
        }
      }, callback);
    }

    public void PhotosetsCommentsAddCommentAsync(string photosetId, string commentText, Action<FlickrResult<string>> callback)
    {
      this.GetResponseAsync<UnknownResponse>(new Dictionary<string, string>()
      {
        {
          "method",
          "flickr.photosets.comments.addComment"
        },
        {
          "photoset_id",
          photosetId
        },
        {
          "comment_text",
          commentText
        }
      }, (Action<FlickrResult<UnknownResponse>>) (r =>
      {
        FlickrResult<string> local_0 = new FlickrResult<string>();
        local_0.HasError = r.HasError;
        if (r.HasError)
          local_0.Error = r.Error;
        else
          local_0.Result = r.Result.GetAttributeValue("*", "id");
        callback(local_0);
      }));
    }

    public void PhotosetsCommentsDeleteCommentAsync(string commentId, Action<FlickrResult<NoResponse>> callback)
    {
      this.GetResponseAsync<NoResponse>(new Dictionary<string, string>()
      {
        {
          "method",
          "flickr.photosets.comments.deleteComment"
        },
        {
          "comment_id",
          commentId
        }
      }, callback);
    }

    public void PhotosetsCommentsEditCommentAsync(string commentId, string commentText, Action<FlickrResult<NoResponse>> callback)
    {
      this.GetResponseAsync<NoResponse>(new Dictionary<string, string>()
      {
        {
          "method",
          "flickr.photosets.comments.editComment"
        },
        {
          "comment_id",
          commentId
        },
        {
          "comment_text",
          commentText
        }
      }, callback);
    }

    public void PhotosGeoBatchCorrectLocationAsync(double latitude, double longitude, GeoAccuracy accuracy, string placeId, string woeId, Action<FlickrResult<NoResponse>> callback)
    {
      this.CheckRequiresAuthentication();
      this.GetResponseAsync<NoResponse>(new Dictionary<string, string>()
      {
        {
          "method",
          "flickr.photos.geo.batchCorrectLocation"
        },
        {
          "lat",
          latitude.ToString((IFormatProvider) NumberFormatInfo.InvariantInfo)
        },
        {
          "lon",
          longitude.ToString((IFormatProvider) NumberFormatInfo.InvariantInfo)
        },
        {
          "accuracy",
          accuracy.ToString("D")
        },
        {
          "place_id",
          placeId
        },
        {
          "woe_id",
          woeId
        }
      }, callback);
    }

    public void PhotosGeoCorrectLocationAsync(string photoId, string placeId, string woeId, Action<FlickrResult<NoResponse>> callback)
    {
      this.CheckRequiresAuthentication();
      this.GetResponseAsync<NoResponse>(new Dictionary<string, string>()
      {
        {
          "method",
          "flickr.photos.geo.correctLocation"
        },
        {
          "photo_id",
          photoId
        },
        {
          "place_id",
          placeId
        },
        {
          "woe_id",
          woeId
        }
      }, callback);
    }

    public void PhotosGeoGetLocationAsync(string photoId, Action<FlickrResult<PlaceInfo>> callback)
    {
      this.GetResponseAsync<PhotoInfo>(new Dictionary<string, string>()
      {
        {
          "method",
          "flickr.photos.geo.getLocation"
        },
        {
          "photo_id",
          photoId
        }
      }, (Action<FlickrResult<PhotoInfo>>) (r =>
      {
        FlickrResult<PlaceInfo> local_0 = new FlickrResult<PlaceInfo>();
        local_0.HasError = r.HasError;
        if (local_0.HasError)
        {
          if (local_0.ErrorCode == 2)
          {
            local_0.HasError = false;
            local_0.Result = (PlaceInfo) null;
            local_0.Error = (Exception) null;
          }
          else
            local_0.Error = r.Error;
        }
        else
          local_0.Result = r.Result.Location;
        callback(local_0);
      }));
    }

    public void PhotosGeoSetContextAsync(string photoId, GeoContext context, Action<FlickrResult<NoResponse>> callback)
    {
      this.GetResponseAsync<NoResponse>(new Dictionary<string, string>()
      {
        {
          "method",
          "flickr.photos.geo.setContext"
        },
        {
          "photo_id",
          photoId
        },
        {
          "context",
          context.ToString("D")
        }
      }, callback);
    }

    public void PhotosGeoSetLocationAsync(string photoId, double latitude, double longitude, Action<FlickrResult<NoResponse>> callback)
    {
      this.PhotosGeoSetLocationAsync(photoId, latitude, longitude, GeoAccuracy.None, callback);
    }

    public void PhotosGeoSetLocationAsync(string photoId, double latitude, double longitude, GeoAccuracy accuracy, Action<FlickrResult<NoResponse>> callback)
    {
      Dictionary<string, string> parameters = new Dictionary<string, string>();
      parameters.Add("method", "flickr.photos.geo.setLocation");
      parameters.Add("photo_id", photoId);
      parameters.Add("lat", latitude.ToString((IFormatProvider) NumberFormatInfo.InvariantInfo));
      parameters.Add("lon", longitude.ToString((IFormatProvider) NumberFormatInfo.InvariantInfo));
      if (accuracy != GeoAccuracy.None)
        parameters.Add("accuracy", accuracy.ToString("D"));
      this.GetResponseAsync<NoResponse>(parameters, callback);
    }

    public void PhotosGeoPhotosForLocationAsync(double latitude, double longitude, GeoAccuracy accuracy, PhotoSearchExtras extras, int perPage, int page, Action<FlickrResult<PhotoCollection>> callback)
    {
      this.CheckRequiresAuthentication();
      Dictionary<string, string> parameters = new Dictionary<string, string>();
      parameters.Add("method", "flickr.photos.geo.photosForLocation");
      parameters.Add("lat", latitude.ToString((IFormatProvider) NumberFormatInfo.InvariantInfo));
      parameters.Add("lon", longitude.ToString((IFormatProvider) NumberFormatInfo.InvariantInfo));
      parameters.Add("accuracy", accuracy.ToString("D"));
      if (extras != PhotoSearchExtras.None)
        parameters.Add("extras", UtilityMethods.ExtrasToString(extras));
      if (perPage > 0)
        parameters.Add("per_page", perPage.ToString((IFormatProvider) NumberFormatInfo.InvariantInfo));
      if (page > 0)
        parameters.Add("page", page.ToString((IFormatProvider) NumberFormatInfo.InvariantInfo));
      this.GetResponseAsync<PhotoCollection>(parameters, callback);
    }

    public void PhotosGeoRemoveLocationAsync(string photoId, Action<FlickrResult<NoResponse>> callback)
    {
      this.GetResponseAsync<NoResponse>(new Dictionary<string, string>()
      {
        {
          "method",
          "flickr.photos.geo.removeLocation"
        },
        {
          "photo_id",
          photoId
        }
      }, callback);
    }

    public void PhotosGetWithoutGeoDataAsync(Action<FlickrResult<PhotoCollection>> callback)
    {
      this.PhotosGetWithoutGeoDataAsync(new PartialSearchOptions(), callback);
    }

    public void PhotosGetWithoutGeoDataAsync(PartialSearchOptions options, Action<FlickrResult<PhotoCollection>> callback)
    {
      Dictionary<string, string> parameters = new Dictionary<string, string>();
      parameters.Add("method", "flickr.photos.getWithoutGeoData");
      UtilityMethods.PartialOptionsIntoArray(options, parameters);
      this.GetResponseAsync<PhotoCollection>(parameters, callback);
    }

    public void PhotosGetWithGeoDataAsync(Action<FlickrResult<PhotoCollection>> callback)
    {
      this.PhotosGetWithGeoDataAsync(new PartialSearchOptions(), callback);
    }

    public void PhotosGetWithGeoDataAsync(PartialSearchOptions options, Action<FlickrResult<PhotoCollection>> callback)
    {
      Dictionary<string, string> parameters = new Dictionary<string, string>();
      parameters.Add("method", "flickr.photos.getWithGeoData");
      UtilityMethods.PartialOptionsIntoArray(options, parameters);
      this.GetResponseAsync<PhotoCollection>(parameters, callback);
    }

    public void PhotosGeoGetPermsAsync(string photoId, Action<FlickrResult<GeoPermissions>> callback)
    {
      this.GetResponseAsync<GeoPermissions>(new Dictionary<string, string>()
      {
        {
          "method",
          "flickr.photos.geo.getPerms"
        },
        {
          "photo_id",
          photoId
        }
      }, callback);
    }

    public void PhotosGeoSetPermsAsync(string photoId, bool isPublic, bool isContact, bool isFamily, bool isFriend, Action<FlickrResult<NoResponse>> callback)
    {
      this.GetResponseAsync<NoResponse>(new Dictionary<string, string>()
      {
        {
          "method",
          "flickr.photos.geo.setPerms"
        },
        {
          "photo_id",
          photoId
        },
        {
          "is_public",
          isPublic ? "1" : "0"
        },
        {
          "is_contact",
          isContact ? "1" : "0"
        },
        {
          "is_friend",
          isFriend ? "1" : "0"
        },
        {
          "is_family",
          isFamily ? "1" : "0"
        }
      }, callback);
    }

    public void PhotosTransformRotateAsync(string photoId, int degrees, Action<FlickrResult<NoResponse>> callback)
    {
      if (photoId == null)
        throw new ArgumentNullException("photoId");
      if (degrees != 90 && degrees != 180 && degrees != 270)
        throw new ArgumentException("Must be 90, 180 or 270", "degrees");
      this.GetResponseAsync<NoResponse>(new Dictionary<string, string>()
      {
        {
          "method",
          "flickr.photos.transform.rotate"
        },
        {
          "photo_id",
          photoId
        },
        {
          "degrees",
          degrees.ToString((IFormatProvider) NumberFormatInfo.InvariantInfo)
        }
      }, callback);
    }

    public void PhotosUploadCheckTicketsAsync(string[] tickets, Action<FlickrResult<TicketCollection>> callback)
    {
      this.GetResponseAsync<TicketCollection>(new Dictionary<string, string>()
      {
        {
          "method",
          "flickr.photos.upload.checkTickets"
        },
        {
          "tickets",
          string.Join(",", tickets)
        }
      }, callback);
    }

    public void PhotosPeopleAdd(string photoId, string userId)
    {
      this.PhotosPeopleAdd(photoId, userId, new int?(), new int?(), new int?(), new int?());
    }

    public void PhotosPeopleAdd(string photoId, string userId, int? personX, int? personY, int? personWidth, int? personHeight)
    {
      this.CheckRequiresAuthentication();
      Dictionary<string, string> parameters = new Dictionary<string, string>();
      parameters.Add("method", "flickr.photos.people.add");
      parameters.Add("photo_id", photoId);
      parameters.Add("user_id", userId);
      if (personX.HasValue)
        parameters.Add("person_x", personX.Value.ToString((IFormatProvider) NumberFormatInfo.InvariantInfo));
      if (personY.HasValue)
        parameters.Add("person_y", personY.Value.ToString((IFormatProvider) NumberFormatInfo.InvariantInfo));
      if (personWidth.HasValue)
        parameters.Add("person_w", personWidth.Value.ToString((IFormatProvider) NumberFormatInfo.InvariantInfo));
      if (personHeight.HasValue)
        parameters.Add("person_h", personHeight.Value.ToString((IFormatProvider) NumberFormatInfo.InvariantInfo));
      this.GetResponseNoCache<NoResponse>(parameters);
    }

    public void PhotosPeopleDelete(string photoId, string userId)
    {
      this.CheckRequiresAuthentication();
      this.GetResponseNoCache<NoResponse>(new Dictionary<string, string>()
      {
        {
          "method",
          "flickr.photos.people.delete"
        },
        {
          "photo_id",
          photoId
        },
        {
          "user_id",
          userId
        }
      });
    }

    public void PhotosPeopleDeleteCoords(string photoId, string userId)
    {
      this.CheckRequiresAuthentication();
      this.GetResponseNoCache<NoResponse>(new Dictionary<string, string>()
      {
        {
          "method",
          "flickr.photos.people.deleteCoords"
        },
        {
          "photo_id",
          photoId
        },
        {
          "user_id",
          userId
        }
      });
    }

    public void PhotosPeopleEditCoords(string photoId, string userId, int personX, int personY, int personWidth, int personHeight)
    {
      this.CheckRequiresAuthentication();
      this.GetResponseNoCache<NoResponse>(new Dictionary<string, string>()
      {
        {
          "method",
          "flickr.photos.people.editCoords"
        },
        {
          "photo_id",
          photoId
        },
        {
          "user_id",
          userId
        },
        {
          "person_x",
          personX.ToString((IFormatProvider) NumberFormatInfo.InvariantInfo)
        },
        {
          "person_y",
          personY.ToString((IFormatProvider) NumberFormatInfo.InvariantInfo)
        },
        {
          "person_w",
          personWidth.ToString((IFormatProvider) NumberFormatInfo.InvariantInfo)
        },
        {
          "person_h",
          personHeight.ToString((IFormatProvider) NumberFormatInfo.InvariantInfo)
        }
      });
    }

    public PhotoPersonCollection PhotosPeopleGetList(string photoId)
    {
      return this.GetResponseNoCache<PhotoPersonCollection>(new Dictionary<string, string>()
      {
        {
          "method",
          "flickr.photos.people.getList"
        },
        {
          "photo_id",
          photoId
        }
      });
    }

    public void PhotosPeopleAddAsync(string photoId, string userId, Action<FlickrResult<NoResponse>> callback)
    {
      this.PhotosPeopleAddAsync(photoId, userId, new int?(), new int?(), new int?(), new int?(), callback);
    }

    public void PhotosPeopleAddAsync(string photoId, string userId, int? personX, int? personY, int? personWidth, int? personHeight, Action<FlickrResult<NoResponse>> callback)
    {
      this.CheckRequiresAuthentication();
      Dictionary<string, string> parameters = new Dictionary<string, string>();
      parameters.Add("method", "flickr.photos.people.add");
      parameters.Add("photo_id", photoId);
      parameters.Add("user_id", userId);
      if (personX.HasValue)
        parameters.Add("person_x", personX.Value.ToString((IFormatProvider) NumberFormatInfo.InvariantInfo));
      if (personY.HasValue)
        parameters.Add("person_y", personY.Value.ToString((IFormatProvider) NumberFormatInfo.InvariantInfo));
      if (personWidth.HasValue)
        parameters.Add("person_w", personWidth.Value.ToString((IFormatProvider) NumberFormatInfo.InvariantInfo));
      if (personHeight.HasValue)
        parameters.Add("person_h", personHeight.Value.ToString((IFormatProvider) NumberFormatInfo.InvariantInfo));
      this.GetResponseAsync<NoResponse>(parameters, callback);
    }

    public void PhotosPeopleDeleteAsync(string photoId, string userId, Action<FlickrResult<NoResponse>> callback)
    {
      this.CheckRequiresAuthentication();
      this.GetResponseAsync<NoResponse>(new Dictionary<string, string>()
      {
        {
          "method",
          "flickr.photos.people.delete"
        },
        {
          "photo_id",
          photoId
        },
        {
          "user_id",
          userId
        }
      }, callback);
    }

    public void PhotosPeopleDeleteCoordsAsync(string photoId, string userId, Action<FlickrResult<NoResponse>> callback)
    {
      this.CheckRequiresAuthentication();
      this.GetResponseAsync<NoResponse>(new Dictionary<string, string>()
      {
        {
          "method",
          "flickr.photos.people.deleteCoords"
        },
        {
          "photo_id",
          photoId
        },
        {
          "user_id",
          userId
        }
      }, callback);
    }

    public void PhotosPeopleEditCoordsAsync(string photoId, string userId, int personX, int personY, int personWidth, int personHeight, Action<FlickrResult<NoResponse>> callback)
    {
      this.CheckRequiresAuthentication();
      this.GetResponseAsync<NoResponse>(new Dictionary<string, string>()
      {
        {
          "method",
          "flickr.photos.people.editCoords"
        },
        {
          "photo_id",
          photoId
        },
        {
          "user_id",
          userId
        },
        {
          "person_x",
          personX.ToString((IFormatProvider) NumberFormatInfo.InvariantInfo)
        },
        {
          "person_y",
          personY.ToString((IFormatProvider) NumberFormatInfo.InvariantInfo)
        },
        {
          "person_w",
          personWidth.ToString((IFormatProvider) NumberFormatInfo.InvariantInfo)
        },
        {
          "person_h",
          personHeight.ToString((IFormatProvider) NumberFormatInfo.InvariantInfo)
        }
      }, callback);
    }

    public void PhotosPeopleGetListAsync(string photoId, Action<FlickrResult<PhotoPersonCollection>> callback)
    {
      this.GetResponseAsync<PhotoPersonCollection>(new Dictionary<string, string>()
      {
        {
          "method",
          "flickr.photos.people.getList"
        },
        {
          "photo_id",
          photoId
        }
      }, callback);
    }

    public void PhotosSuggestionsApproveSuggestionAsync(string suggestionId, Action<FlickrResult<NoResponse>> callback)
    {
      this.CheckRequiresAuthentication();
      if (string.IsNullOrEmpty(suggestionId))
        throw new ArgumentNullException("suggestionId");
      this.GetResponseAsync<NoResponse>(new Dictionary<string, string>()
      {
        {
          "method",
          "flickr.photos.suggestions.approveSuggestion"
        },
        {
          "suggestion_id",
          suggestionId
        }
      }, callback);
    }

    public void PhotosSuggestionsGetListAsync(string photoId, SuggestionStatus status, Action<FlickrResult<SuggestionCollection>> callback)
    {
      this.CheckRequiresAuthentication();
      this.GetResponseAsync<SuggestionCollection>(new Dictionary<string, string>()
      {
        {
          "method",
          "flickr.photos.suggestions.getList"
        },
        {
          "photo_id",
          photoId
        },
        {
          "status_id",
          status.ToString("d")
        }
      }, callback);
    }

    public void PhotosSuggestionsRejectSuggestionAsync(string suggestionId, Action<FlickrResult<NoResponse>> callback)
    {
      this.CheckRequiresAuthentication();
      if (string.IsNullOrEmpty(suggestionId))
        throw new ArgumentNullException("suggestionId");
      this.GetResponseAsync<NoResponse>(new Dictionary<string, string>()
      {
        {
          "method",
          "flickr.photos.suggestions.rejectSuggestion"
        },
        {
          "suggestion_id",
          suggestionId
        }
      }, callback);
    }

    public void PhotosSuggestionsRemoveSuggestionAsync(string suggestionId, Action<FlickrResult<NoResponse>> callback)
    {
      this.CheckRequiresAuthentication();
      if (string.IsNullOrEmpty(suggestionId))
        throw new ArgumentNullException("suggestionId");
      this.GetResponseAsync<NoResponse>(new Dictionary<string, string>()
      {
        {
          "method",
          "flickr.photos.suggestions.removeSuggestion"
        },
        {
          "suggestion_id",
          suggestionId
        }
      }, callback);
    }

    public void PhotosSuggestionsSuggestLocationAsync(string photoId, double latitude, double longitude, GeoAccuracy accuracy, string woeId, string placeId, string note, Action<FlickrResult<NoResponse>> callback)
    {
      this.CheckRequiresAuthentication();
      this.GetResponseAsync<NoResponse>(new Dictionary<string, string>()
      {
        {
          "method",
          "flickr.photos.suggestions.suggestLocation"
        },
        {
          "photo_id",
          photoId
        },
        {
          "lat",
          latitude.ToString((IFormatProvider) NumberFormatInfo.InvariantInfo)
        },
        {
          "lon",
          longitude.ToString((IFormatProvider) NumberFormatInfo.InvariantInfo)
        },
        {
          "accuracy",
          accuracy.ToString("D")
        },
        {
          "place_id",
          placeId
        },
        {
          "woe_id",
          woeId
        },
        {
          "note",
          note
        }
      }, callback);
    }

    public void PlacesFindAsync(string query, Action<FlickrResult<PlaceCollection>> callback)
    {
      if (query == null)
        throw new ArgumentNullException("query");
      this.GetResponseAsync<PlaceCollection>(new Dictionary<string, string>()
      {
        {
          "method",
          "flickr.places.find"
        },
        {
          "query",
          query
        }
      }, callback);
    }

    public void PlacesFindByLatLonAsync(double latitude, double longitude, Action<FlickrResult<Place>> callback)
    {
      this.PlacesFindByLatLonAsync(latitude, longitude, GeoAccuracy.None, callback);
    }

    public void PlacesFindByLatLonAsync(double latitude, double longitude, GeoAccuracy accuracy, Action<FlickrResult<Place>> callback)
    {
      Dictionary<string, string> parameters = new Dictionary<string, string>();
      parameters.Add("method", "flickr.places.findByLatLon");
      parameters.Add("lat", latitude.ToString((IFormatProvider) NumberFormatInfo.InvariantInfo));
      parameters.Add("lon", longitude.ToString((IFormatProvider) NumberFormatInfo.InvariantInfo));
      if (accuracy != GeoAccuracy.None)
        parameters.Add("accuracy", ((int) accuracy).ToString((IFormatProvider) NumberFormatInfo.InvariantInfo));
      this.GetResponseAsync<PlaceCollection>(parameters, (Action<FlickrResult<PlaceCollection>>) (r =>
      {
        FlickrResult<Place> local_0 = new FlickrResult<Place>();
        local_0.Error = r.Error;
        if (!r.HasError)
          local_0.Result = r.Result[0];
        callback(local_0);
      }));
    }

    public void PlacesGetChildrenWithPhotosPublicAsync(string placeId, string woeId, Action<FlickrResult<PlaceCollection>> callback)
    {
      Dictionary<string, string> parameters = new Dictionary<string, string>();
      parameters.Add("method", "flickr.places.getChildrenWithPhotosPublic");
      if ((placeId == null || placeId.Length == 0) && (woeId == null || woeId.Length == 0))
        throw new FlickrException("Both placeId and woeId cannot be null or empty.");
      if (!string.IsNullOrEmpty(placeId))
        parameters.Add("place_id", placeId);
      if (!string.IsNullOrEmpty(woeId))
        parameters.Add("woe_id", woeId);
      this.GetResponseAsync<PlaceCollection>(parameters, callback);
    }

    public void PlacesGetInfoAsync(string placeId, string woeId, Action<FlickrResult<PlaceInfo>> callback)
    {
      Dictionary<string, string> parameters = new Dictionary<string, string>();
      parameters.Add("method", "flickr.places.getInfo");
      if (string.IsNullOrEmpty(placeId) && string.IsNullOrEmpty(woeId))
        throw new FlickrException("Both placeId and woeId cannot be null or empty.");
      if (!string.IsNullOrEmpty(placeId))
        parameters.Add("place_id", placeId);
      if (!string.IsNullOrEmpty(woeId))
        parameters.Add("woe_id", woeId);
      this.GetResponseAsync<PlaceInfo>(parameters, callback);
    }

    public void PlacesGetInfoByUrlAsync(string url, Action<FlickrResult<PlaceInfo>> callback)
    {
      this.GetResponseAsync<PlaceInfo>(new Dictionary<string, string>()
      {
        {
          "method",
          "flickr.places.getInfoByUrl"
        },
        {
          "url",
          url
        }
      }, callback);
    }

    public void PlacesGetPlaceTypesAsync(Action<FlickrResult<PlaceTypeInfoCollection>> callback)
    {
      this.GetResponseAsync<PlaceTypeInfoCollection>(new Dictionary<string, string>()
      {
        {
          "method",
          "flickr.places.getPlaceTypes"
        }
      }, callback);
    }

    public void PlacesGetShapeHistoryAsync(string placeId, string woeId, Action<FlickrResult<ShapeDataCollection>> callback)
    {
      Dictionary<string, string> parameters = new Dictionary<string, string>();
      parameters.Add("method", "flickr.places.getShapeHistory");
      if (string.IsNullOrEmpty(placeId) && string.IsNullOrEmpty(woeId))
        throw new FlickrException("Both placeId and woeId cannot be null or empty.");
      if (!string.IsNullOrEmpty(placeId))
        parameters.Add("place_id", placeId);
      if (!string.IsNullOrEmpty(woeId))
        parameters.Add("woe_id", woeId);
      this.GetResponseAsync<ShapeDataCollection>(parameters, callback);
    }

    public void PlacesGetTopPlacesListAsync(PlaceType placeType, Action<FlickrResult<PlaceCollection>> callback)
    {
      this.PlacesGetTopPlacesListAsync(placeType, DateTime.MinValue, (string) null, (string) null, callback);
    }

    public void PlacesGetTopPlacesListAsync(PlaceType placeType, string placeId, string woeId, Action<FlickrResult<PlaceCollection>> callback)
    {
      this.PlacesGetTopPlacesListAsync(placeType, DateTime.MinValue, placeId, woeId, callback);
    }

    public void PlacesGetTopPlacesListAsync(PlaceType placeType, DateTime date, Action<FlickrResult<PlaceCollection>> callback)
    {
      this.PlacesGetTopPlacesListAsync(placeType, date, (string) null, (string) null, callback);
    }

    public void PlacesGetTopPlacesListAsync(PlaceType placeType, DateTime date, string placeId, string woeId, Action<FlickrResult<PlaceCollection>> callback)
    {
      Dictionary<string, string> parameters = new Dictionary<string, string>();
      parameters.Add("method", "flickr.places.getTopPlacesList");
      parameters.Add("place_type_id", placeType.ToString("D"));
      if (date != DateTime.MinValue)
        parameters.Add("date", date.ToString("yyyy-MM-dd", (IFormatProvider) DateTimeFormatInfo.InvariantInfo));
      if (!string.IsNullOrEmpty(placeId))
        parameters.Add("place_id", placeId);
      if (!string.IsNullOrEmpty(woeId))
        parameters.Add("woe_id", woeId);
      this.GetResponseAsync<PlaceCollection>(parameters, callback);
    }

    public void PlacesPlacesForUserAsync(Action<FlickrResult<PlaceCollection>> callback)
    {
      this.PlacesPlacesForUserAsync(PlaceType.Continent, (string) null, (string) null, 0, DateTime.MinValue, DateTime.MinValue, DateTime.MinValue, DateTime.MinValue, callback);
    }

    public void PlacesPlacesForUserAsync(PlaceType placeType, string woeId, string placeId, Action<FlickrResult<PlaceCollection>> callback)
    {
      this.PlacesPlacesForUserAsync(placeType, woeId, placeId, 0, DateTime.MinValue, DateTime.MinValue, DateTime.MinValue, DateTime.MinValue, callback);
    }

    public void PlacesPlacesForUserAsync(PlaceType placeType, string woeId, string placeId, int threshold, DateTime minUploadDate, DateTime maxUploadDate, DateTime minTakenDate, DateTime maxTakenDate, Action<FlickrResult<PlaceCollection>> callback)
    {
      this.CheckRequiresAuthentication();
      Dictionary<string, string> parameters = new Dictionary<string, string>();
      parameters.Add("method", "flickr.places.placesForUser");
      parameters.Add("place_type_id", placeType.ToString("D"));
      if (!string.IsNullOrEmpty(woeId))
        parameters.Add("woe_id", woeId);
      if (!string.IsNullOrEmpty(placeId))
        parameters.Add("place_id", placeId);
      if (threshold > 0)
        parameters.Add("threshold", threshold.ToString((IFormatProvider) NumberFormatInfo.InvariantInfo));
      if (minTakenDate != DateTime.MinValue)
        parameters.Add("min_taken_date", UtilityMethods.DateToMySql(minTakenDate));
      if (maxTakenDate != DateTime.MinValue)
        parameters.Add("max_taken_date", UtilityMethods.DateToMySql(maxTakenDate));
      if (minUploadDate != DateTime.MinValue)
        parameters.Add("min_upload_date", UtilityMethods.DateToUnixTimestamp(minUploadDate));
      if (maxUploadDate != DateTime.MinValue)
        parameters.Add("max_upload_date", UtilityMethods.DateToUnixTimestamp(maxUploadDate));
      this.GetResponseAsync<PlaceCollection>(parameters, callback);
    }

    public void PlacesPlacesForTagsAsync(PlaceType placeType, string woeId, string placeId, int threshold, string[] tags, TagMode tagMode, string[] machineTags, MachineTagMode machineTagMode, DateTime minUploadDate, DateTime maxUploadDate, DateTime minTakenDate, DateTime maxTakenDate, Action<FlickrResult<PlaceCollection>> callback)
    {
      Dictionary<string, string> parameters = new Dictionary<string, string>();
      parameters.Add("method", "flickr.places.placesForTags");
      parameters.Add("place_type_id", placeType.ToString("D"));
      if (!string.IsNullOrEmpty(woeId))
        parameters.Add("woe_id", woeId);
      if (!string.IsNullOrEmpty(placeId))
        parameters.Add("place_id", placeId);
      if (threshold > 0)
        parameters.Add("threshold", threshold.ToString((IFormatProvider) NumberFormatInfo.InvariantInfo));
      if (tags != null && tags.Length > 0)
        parameters.Add("tags", string.Join(",", tags));
      if (tagMode != TagMode.None)
        parameters.Add("tag_mode", UtilityMethods.TagModeToString(tagMode));
      if (machineTags != null && machineTags.Length > 0)
        parameters.Add("machine_tags", string.Join(",", machineTags));
      if (machineTagMode != MachineTagMode.None)
        parameters.Add("machine_tag_mode", UtilityMethods.MachineTagModeToString(machineTagMode));
      if (minTakenDate != DateTime.MinValue)
        parameters.Add("min_taken_date", UtilityMethods.DateToMySql(minTakenDate));
      if (maxTakenDate != DateTime.MinValue)
        parameters.Add("max_taken_date", UtilityMethods.DateToMySql(maxTakenDate));
      if (minUploadDate != DateTime.MinValue)
        parameters.Add("min_upload_date", UtilityMethods.DateToUnixTimestamp(minUploadDate));
      if (maxUploadDate != DateTime.MinValue)
        parameters.Add("max_upload_date", UtilityMethods.DateToUnixTimestamp(maxUploadDate));
      this.GetResponseAsync<PlaceCollection>(parameters, callback);
    }

    public void PlacesPlacesForContactsAsync(PlaceType placeType, string woeId, string placeId, int threshold, ContactSearch contactType, DateTime minUploadDate, DateTime maxUploadDate, DateTime minTakenDate, DateTime maxTakenDate, Action<FlickrResult<PlaceCollection>> callback)
    {
      this.CheckRequiresAuthentication();
      Dictionary<string, string> parameters = new Dictionary<string, string>();
      parameters.Add("method", "flickr.places.placesForContacts");
      parameters.Add("place_type_id", placeType.ToString("D"));
      if (!string.IsNullOrEmpty(woeId))
        parameters.Add("woe_id", woeId);
      if (!string.IsNullOrEmpty(placeId))
        parameters.Add("place_id", placeId);
      if (threshold > 0)
        parameters.Add("threshold", threshold.ToString((IFormatProvider) NumberFormatInfo.InvariantInfo));
      if (contactType != ContactSearch.None)
        parameters.Add("contacts", contactType == ContactSearch.AllContacts ? "all" : "ff");
      if (minUploadDate != DateTime.MinValue)
        parameters.Add("min_upload_date", UtilityMethods.DateToUnixTimestamp(minUploadDate));
      if (maxUploadDate != DateTime.MinValue)
        parameters.Add("max_upload_date", UtilityMethods.DateToUnixTimestamp(maxUploadDate));
      if (minTakenDate != DateTime.MinValue)
        parameters.Add("min_taken_date", UtilityMethods.DateToMySql(minTakenDate));
      if (maxTakenDate != DateTime.MinValue)
        parameters.Add("max_taken_date", UtilityMethods.DateToMySql(maxTakenDate));
      this.GetResponseAsync<PlaceCollection>(parameters, callback);
    }

    public void PlacesPlacesForBoundingBoxAsync(PlaceType placeType, string woeId, string placeId, BoundaryBox boundaryBox, Action<FlickrResult<PlaceCollection>> callback)
    {
      Dictionary<string, string> parameters = new Dictionary<string, string>();
      parameters.Add("method", "flickr.places.placesForBoundingBox");
      parameters.Add("place_type_id", placeType.ToString("D"));
      if (!string.IsNullOrEmpty(woeId))
        parameters.Add("woe_id", woeId);
      if (!string.IsNullOrEmpty(placeId))
        parameters.Add("place_id", placeId);
      parameters.Add("bbox", boundaryBox.ToString());
      this.GetResponseAsync<PlaceCollection>(parameters, callback);
    }

    [Obsolete("This method is deprecated. Please use Flickr.PlacesGetInfoAsync instead.")]
    public void PlacesResolvePlaceIdAsync(string placeId, Action<FlickrResult<PlaceInfo>> callback)
    {
      this.PlacesGetInfoAsync(placeId, (string) null, callback);
    }

    [Obsolete("This method is deprecated. Please use Flickr.PlacesGetInfoByUrlAsync instead.")]
    public void PlacesResolvePlaceUrlAsync(string url, Action<FlickrResult<PlaceInfo>> callback)
    {
      this.PlacesGetInfoByUrlAsync(url, callback);
    }

    public void PlacesTagsForPlaceAsync(string placeId, string woeId, Action<FlickrResult<TagCollection>> callback)
    {
      this.PlacesTagsForPlaceAsync(placeId, woeId, DateTime.MinValue, DateTime.MinValue, DateTime.MinValue, DateTime.MinValue, callback);
    }

    public void PlacesTagsForPlaceAsync(string placeId, string woeId, DateTime minUploadDate, DateTime maxUploadDate, DateTime minTakenDate, DateTime maxTakenDate, Action<FlickrResult<TagCollection>> callback)
    {
      Dictionary<string, string> parameters = new Dictionary<string, string>();
      parameters.Add("method", "flickr.places.tagsForPlace");
      if (string.IsNullOrEmpty(placeId) && string.IsNullOrEmpty(woeId))
        throw new FlickrException("Both placeId and woeId cannot be null or empty.");
      if (!string.IsNullOrEmpty(woeId))
        parameters.Add("woe_id", woeId);
      if (!string.IsNullOrEmpty(placeId))
        parameters.Add("place_id", placeId);
      if (minTakenDate != DateTime.MinValue)
        parameters.Add("min_taken_date", UtilityMethods.DateToMySql(minTakenDate));
      if (maxTakenDate != DateTime.MinValue)
        parameters.Add("max_taken_date", UtilityMethods.DateToMySql(maxTakenDate));
      if (minUploadDate != DateTime.MinValue)
        parameters.Add("min_upload_date", UtilityMethods.DateToUnixTimestamp(minUploadDate));
      if (maxUploadDate != DateTime.MinValue)
        parameters.Add("max_upload_date", UtilityMethods.DateToUnixTimestamp(maxUploadDate));
      this.GetResponseAsync<TagCollection>(parameters, callback);
    }

    public void PrefsGetContentTypeAsync(Action<FlickrResult<ContentType>> callback)
    {
      this.CheckRequiresAuthentication();
      this.GetResponseAsync<UnknownResponse>(new Dictionary<string, string>()
      {
        {
          "method",
          "flickr.prefs.getContentType"
        }
      }, (Action<FlickrResult<UnknownResponse>>) (r =>
      {
        FlickrResult<ContentType> local_0 = new FlickrResult<ContentType>();
        local_0.Error = r.Error;
        if (!r.HasError)
          local_0.Result = (ContentType) int.Parse(r.Result.GetAttributeValue("*", "content_type"), (IFormatProvider) NumberFormatInfo.InvariantInfo);
        callback(local_0);
      }));
    }

    public void PrefsGetGeoPermsAsync(Action<FlickrResult<UserGeoPermissions>> callback)
    {
      this.CheckRequiresAuthentication();
      this.GetResponseAsync<UserGeoPermissions>(new Dictionary<string, string>()
      {
        {
          "method",
          "flickr.prefs.getGeoPerms"
        }
      }, callback);
    }

    public void PrefsGetHiddenAsync(Action<FlickrResult<HiddenFromSearch>> callback)
    {
      this.CheckRequiresAuthentication();
      this.GetResponseAsync<UnknownResponse>(new Dictionary<string, string>()
      {
        {
          "method",
          "flickr.prefs.getHidden"
        }
      }, (Action<FlickrResult<UnknownResponse>>) (r =>
      {
        FlickrResult<HiddenFromSearch> local_0 = new FlickrResult<HiddenFromSearch>();
        local_0.Error = r.Error;
        if (!r.HasError)
          local_0.Result = (HiddenFromSearch) int.Parse(r.Result.GetAttributeValue("*", "hidden"), (IFormatProvider) NumberFormatInfo.InvariantInfo);
        callback(local_0);
      }));
    }

    public void PrefsGetPrivacyAsync(Action<FlickrResult<PrivacyFilter>> callback)
    {
      this.CheckRequiresAuthentication();
      this.GetResponseAsync<UnknownResponse>(new Dictionary<string, string>()
      {
        {
          "method",
          "flickr.prefs.getPrivacy"
        }
      }, (Action<FlickrResult<UnknownResponse>>) (r =>
      {
        FlickrResult<PrivacyFilter> local_0 = new FlickrResult<PrivacyFilter>();
        local_0.Error = r.Error;
        if (!r.HasError)
          local_0.Result = (PrivacyFilter) int.Parse(r.Result.GetAttributeValue("*", "privacy"), (IFormatProvider) NumberFormatInfo.InvariantInfo);
        callback(local_0);
      }));
    }

    public void PrefsGetSafetyLevelAsync(Action<FlickrResult<SafetyLevel>> callback)
    {
      this.CheckRequiresAuthentication();
      this.GetResponseAsync<UnknownResponse>(new Dictionary<string, string>()
      {
        {
          "method",
          "flickr.prefs.getSafetyLevel"
        }
      }, (Action<FlickrResult<UnknownResponse>>) (r =>
      {
        FlickrResult<SafetyLevel> local_0 = new FlickrResult<SafetyLevel>();
        local_0.Error = r.Error;
        if (!r.HasError)
          local_0.Result = (SafetyLevel) int.Parse(r.Result.GetAttributeValue("*", "safety_level"), (IFormatProvider) NumberFormatInfo.InvariantInfo);
        callback(local_0);
      }));
    }

    public SubscriptionCollection PushGetSubscriptions()
    {
      this.CheckRequiresAuthentication();
      return this.GetResponseCache<SubscriptionCollection>(new Dictionary<string, string>()
      {
        {
          "method",
          "flickr.push.getSubscriptions"
        }
      });
    }

    public string[] PushGetTopics()
    {
      UnknownResponse responseCache = this.GetResponseCache<UnknownResponse>(new Dictionary<string, string>()
      {
        {
          "method",
          "flickr.push.getTopics"
        }
      });
      List<string> list = new List<string>();
      foreach (XmlNode xmlNode in responseCache.GetXmlDocument().SelectNodes("//topic/@name"))
        list.Add(xmlNode.Value);
      return list.ToArray();
    }

    public void PushSubscribe(string topic, string callback, string verify, string verifyToken, int leaseSeconds, int[] woeIds, string[] placeIds, double latitude, double longitude, int radius, RadiusUnit radiusUnits, GeoAccuracy accuracy, string[] nsids, string[] tags)
    {
      this.CheckRequiresAuthentication();
      if (string.IsNullOrEmpty(topic))
        throw new ArgumentNullException("topic");
      if (string.IsNullOrEmpty(callback))
        throw new ArgumentNullException("callback");
      if (string.IsNullOrEmpty(verify))
        throw new ArgumentNullException("verify");
      if (topic == "tags" && (tags == null || tags.Length == 0))
        throw new InvalidOperationException("Must specify at least one tag is using topic of 'tags'");
      Dictionary<string, string> parameters = new Dictionary<string, string>();
      parameters.Add("method", "flickr.push.subscribe");
      parameters.Add("topic", topic);
      parameters.Add("callback", callback);
      parameters.Add("verify", verify);
      if (!string.IsNullOrEmpty(verifyToken))
        parameters.Add("verify_token", verifyToken);
      if (leaseSeconds > 0)
        parameters.Add("lease_seconds", leaseSeconds.ToString((IFormatProvider) NumberFormatInfo.InvariantInfo));
      if (woeIds != null && woeIds.Length > 0)
      {
        List<string> list = new List<string>();
        foreach (int num in woeIds)
          list.Add(num.ToString((IFormatProvider) NumberFormatInfo.InvariantInfo));
        parameters.Add("woe_ids", string.Join(",", list.ToArray()));
      }
      if (placeIds != null && placeIds.Length > 0)
        parameters.Add("place_ids", string.Join(",", placeIds));
      if (radiusUnits != RadiusUnit.None)
        parameters.Add("radius_units", radiusUnits.ToString("d"));
      this.GetResponseNoCache<NoResponse>(parameters);
    }

    public void PushUnsubscribe(string topic, string callback, string verify, string verifyToken)
    {
      this.CheckRequiresAuthentication();
      if (string.IsNullOrEmpty(topic))
        throw new ArgumentNullException("topic");
      if (string.IsNullOrEmpty(callback))
        throw new ArgumentNullException("callback");
      if (string.IsNullOrEmpty(verify))
        throw new ArgumentNullException("verify");
      Dictionary<string, string> parameters = new Dictionary<string, string>();
      parameters.Add("method", "flickr.push.unsubscribe");
      parameters.Add("topic", topic);
      parameters.Add("callback", callback);
      parameters.Add("verify", verify);
      if (!string.IsNullOrEmpty(verifyToken))
        parameters.Add("verif_token", verifyToken);
      this.GetResponseNoCache<NoResponse>(parameters);
    }

    public void PushGetSubscriptionsAsync(Action<FlickrResult<SubscriptionCollection>> callback)
    {
      this.CheckRequiresAuthentication();
      this.GetResponseAsync<SubscriptionCollection>(new Dictionary<string, string>()
      {
        {
          "method",
          "flickr.push.getSubscriptions"
        }
      }, callback);
    }

    public void PushGetTopicsAsync(Action<FlickrResult<string[]>> callback)
    {
      this.GetResponseAsync<UnknownResponse>(new Dictionary<string, string>()
      {
        {
          "method",
          "flickr.push.getTopics"
        }
      }, (Action<FlickrResult<UnknownResponse>>) (r =>
      {
        if (callback == null)
          return;
        if (r.HasError)
        {
          callback(new FlickrResult<string[]>()
          {
            Error = r.Error
          });
        }
        else
        {
          string[] local_1 = r.Result.GetElementArray("topic", "name");
          callback(new FlickrResult<string[]>()
          {
            Result = local_1
          });
        }
      }));
    }

    public void PushSubscribeAsync(string topic, string callback, string verify, string verifyToken, int leaseSeconds, int[] woeIds, string[] placeIds, double latitude, double longitude, int radius, RadiusUnit radiusUnits, GeoAccuracy accuracy, string[] nsids, string[] tags, Action<FlickrResult<NoResponse>> callbackAction)
    {
      this.CheckRequiresAuthentication();
      if (string.IsNullOrEmpty(topic))
        throw new ArgumentNullException("topic");
      if (string.IsNullOrEmpty(callback))
        throw new ArgumentNullException("callback");
      if (string.IsNullOrEmpty(verify))
        throw new ArgumentNullException("verify");
      if (topic == "tags" && (tags == null || tags.Length == 0))
        throw new InvalidOperationException("Must specify at least one tag is using topic of 'tags'");
      Dictionary<string, string> parameters = new Dictionary<string, string>();
      parameters.Add("method", "flickr.push.subscribe");
      parameters.Add("topic", topic);
      parameters.Add("callback", callback);
      parameters.Add("verify", verify);
      if (!string.IsNullOrEmpty(verifyToken))
        parameters.Add("verify_token", verifyToken);
      if (leaseSeconds > 0)
        parameters.Add("lease_seconds", leaseSeconds.ToString((IFormatProvider) NumberFormatInfo.InvariantInfo));
      if (woeIds != null && woeIds.Length > 0)
      {
        List<string> list = new List<string>();
        foreach (int num in woeIds)
          list.Add(num.ToString((IFormatProvider) NumberFormatInfo.InvariantInfo));
        parameters.Add("woe_ids", string.Join(",", list.ToArray()));
      }
      if (placeIds != null && placeIds.Length > 0)
        parameters.Add("place_ids", string.Join(",", placeIds));
      if (radiusUnits != RadiusUnit.None)
        parameters.Add("radius_units", radiusUnits.ToString("d"));
      this.GetResponseAsync<NoResponse>(parameters, callbackAction);
    }

    public void PushUnsubscribeAsync(string topic, string callback, string verify, string verifyToken, Action<FlickrResult<NoResponse>> callbackAction)
    {
      this.CheckRequiresAuthentication();
      if (string.IsNullOrEmpty(topic))
        throw new ArgumentNullException("topic");
      if (string.IsNullOrEmpty(callback))
        throw new ArgumentNullException("callback");
      if (string.IsNullOrEmpty(verify))
        throw new ArgumentNullException("verify");
      Dictionary<string, string> parameters = new Dictionary<string, string>();
      parameters.Add("method", "flickr.push.unsubscribe");
      parameters.Add("topic", topic);
      parameters.Add("callback", callback);
      parameters.Add("verify", verify);
      if (!string.IsNullOrEmpty(verifyToken))
        parameters.Add("verif_token", verifyToken);
      this.GetResponseAsync<NoResponse>(parameters, callbackAction);
    }

    public void ReflectionGetMethodsAsync(Action<FlickrResult<MethodCollection>> callback)
    {
      this.GetResponseAsync<MethodCollection>(new Dictionary<string, string>()
      {
        {
          "method",
          "flickr.reflection.getMethods"
        }
      }, callback);
    }

    public void ReflectionGetMethodInfoAsync(string methodName, Action<FlickrResult<Method>> callback)
    {
      this.GetResponseAsync<Method>(new Dictionary<string, string>()
      {
        {
          "method",
          "flickr.reflection.getMethodInfo"
        },
        {
          "api_key",
          this.apiKey
        },
        {
          "method_name",
          methodName
        }
      }, callback);
    }

    public void StatsGetCollectionDomainsAsync(DateTime date, Action<FlickrResult<StatDomainCollection>> callback)
    {
      this.StatsGetCollectionDomainsAsync(date, (string) null, 0, 0, callback);
    }

    public void StatsGetCollectionDomainsAsync(DateTime date, string collectionId, Action<FlickrResult<StatDomainCollection>> callback)
    {
      this.StatsGetCollectionDomainsAsync(date, collectionId, 0, 0, callback);
    }

    public void StatsGetCollectionDomainsAsync(DateTime date, int page, int perPage, Action<FlickrResult<StatDomainCollection>> callback)
    {
      this.StatsGetCollectionDomainsAsync(date, (string) null, page, perPage, callback);
    }

    public void StatsGetCollectionDomainsAsync(DateTime date, string collectionId, int page, int perPage, Action<FlickrResult<StatDomainCollection>> callback)
    {
      this.CheckRequiresAuthentication();
      Dictionary<string, string> parameters = new Dictionary<string, string>();
      parameters.Add("method", "flickr.stats.getCollectionDomains");
      parameters.Add("date", UtilityMethods.DateToUnixTimestamp(date));
      if (!string.IsNullOrEmpty(collectionId))
        parameters.Add("collection_id", collectionId);
      if (page > 0)
        parameters.Add("page", page.ToString((IFormatProvider) NumberFormatInfo.InvariantInfo));
      if (perPage > 0)
        parameters.Add("per_page", perPage.ToString((IFormatProvider) NumberFormatInfo.InvariantInfo));
      this.GetResponseAsync<StatDomainCollection>(parameters, callback);
    }

    public void StatsGetCsvFilesAsync(Action<FlickrResult<CsvFileCollection>> callback)
    {
      this.CheckRequiresAuthentication();
      this.GetResponseAsync<CsvFileCollection>(new Dictionary<string, string>()
      {
        {
          "method",
          "flickr.stats.getCSVFiles"
        }
      }, callback);
    }

    public void StatsGetPhotoDomainsAsync(DateTime date, Action<FlickrResult<StatDomainCollection>> callback)
    {
      this.StatsGetPhotoDomainsAsync(date, (string) null, 0, 0, callback);
    }

    public void StatsGetPhotoDomainsAsync(DateTime date, string photoId, Action<FlickrResult<StatDomainCollection>> callback)
    {
      this.StatsGetPhotoDomainsAsync(date, photoId, 0, 0, callback);
    }

    public void StatsGetPhotoDomainsAsync(DateTime date, int page, int perPage, Action<FlickrResult<StatDomainCollection>> callback)
    {
      this.StatsGetPhotoDomainsAsync(date, (string) null, page, perPage, callback);
    }

    public void StatsGetPhotoDomainsAsync(DateTime date, string photoId, int page, int perPage, Action<FlickrResult<StatDomainCollection>> callback)
    {
      this.CheckRequiresAuthentication();
      Dictionary<string, string> parameters = new Dictionary<string, string>();
      parameters.Add("method", "flickr.stats.getPhotoDomains");
      parameters.Add("date", UtilityMethods.DateToUnixTimestamp(date));
      if (!string.IsNullOrEmpty(photoId))
        parameters.Add("photo_id", photoId);
      if (page > 0)
        parameters.Add("page", page.ToString((IFormatProvider) NumberFormatInfo.InvariantInfo));
      if (perPage > 0)
        parameters.Add("per_page", perPage.ToString((IFormatProvider) NumberFormatInfo.InvariantInfo));
      this.GetResponseAsync<StatDomainCollection>(parameters, callback);
    }

    public void StatsGetPhotostreamDomainsAsync(DateTime date, Action<FlickrResult<StatDomainCollection>> callback)
    {
      this.StatsGetPhotostreamDomainsAsync(date, 0, 0, callback);
    }

    public void StatsGetPhotostreamDomainsAsync(DateTime date, int page, int perPage, Action<FlickrResult<StatDomainCollection>> callback)
    {
      this.CheckRequiresAuthentication();
      Dictionary<string, string> parameters = new Dictionary<string, string>();
      parameters.Add("method", "flickr.stats.getPhotostreamDomains");
      parameters.Add("date", UtilityMethods.DateToUnixTimestamp(date));
      if (page > 0)
        parameters.Add("page", page.ToString((IFormatProvider) NumberFormatInfo.InvariantInfo));
      if (perPage > 0)
        parameters.Add("per_page", perPage.ToString((IFormatProvider) NumberFormatInfo.InvariantInfo));
      this.GetResponseAsync<StatDomainCollection>(parameters, callback);
    }

    public void StatsGetPhotosetDomainsAsync(DateTime date, Action<FlickrResult<StatDomainCollection>> callback)
    {
      this.StatsGetPhotosetDomainsAsync(date, (string) null, 0, 0, callback);
    }

    public void StatsGetPhotosetDomainsAsync(DateTime date, string photosetId, Action<FlickrResult<StatDomainCollection>> callback)
    {
      this.StatsGetPhotosetDomainsAsync(date, photosetId, 0, 0, callback);
    }

    public void StatsGetPhotosetDomainsAsync(DateTime date, int page, int perPage, Action<FlickrResult<StatDomainCollection>> callback)
    {
      this.StatsGetPhotosetDomainsAsync(date, (string) null, page, perPage, callback);
    }

    public void StatsGetPhotosetDomainsAsync(DateTime date, string photosetId, int page, int perPage, Action<FlickrResult<StatDomainCollection>> callback)
    {
      this.CheckRequiresAuthentication();
      Dictionary<string, string> parameters = new Dictionary<string, string>();
      parameters.Add("method", "flickr.stats.getPhotosetDomains");
      parameters.Add("date", UtilityMethods.DateToUnixTimestamp(date));
      if (!string.IsNullOrEmpty(photosetId))
        parameters.Add("photoset_id", photosetId);
      if (page > 0)
        parameters.Add("page", page.ToString((IFormatProvider) NumberFormatInfo.InvariantInfo));
      if (perPage > 0)
        parameters.Add("per_page", perPage.ToString((IFormatProvider) NumberFormatInfo.InvariantInfo));
      this.GetResponseAsync<StatDomainCollection>(parameters, callback);
    }

    public void StatsGetCollectionStatsAsync(DateTime date, string collectionId, Action<FlickrResult<Stats>> callback)
    {
      this.CheckRequiresAuthentication();
      this.GetResponseAsync<Stats>(new Dictionary<string, string>()
      {
        {
          "method",
          "flickr.stats.getCollectionStats"
        },
        {
          "date",
          UtilityMethods.DateToUnixTimestamp(date)
        },
        {
          "collection_id",
          UtilityMethods.CleanCollectionId(collectionId)
        }
      }, callback);
    }

    public void StatsGetPhotoStatsAsync(DateTime date, string photoId, Action<FlickrResult<Stats>> callback)
    {
      this.CheckRequiresAuthentication();
      this.GetResponseAsync<Stats>(new Dictionary<string, string>()
      {
        {
          "method",
          "flickr.stats.getPhotoStats"
        },
        {
          "date",
          UtilityMethods.DateToUnixTimestamp(date)
        },
        {
          "photo_id",
          photoId
        }
      }, callback);
    }

    public void StatsGetPhotostreamStatsAsync(DateTime date, Action<FlickrResult<Stats>> callback)
    {
      this.CheckRequiresAuthentication();
      this.GetResponseAsync<Stats>(new Dictionary<string, string>()
      {
        {
          "method",
          "flickr.stats.getPhotostreamStats"
        },
        {
          "date",
          UtilityMethods.DateToUnixTimestamp(date)
        }
      }, callback);
    }

    public void StatsGetPhotosetStatsAsync(DateTime date, string photosetId, Action<FlickrResult<Stats>> callback)
    {
      this.CheckRequiresAuthentication();
      this.GetResponseAsync<Stats>(new Dictionary<string, string>()
      {
        {
          "method",
          "flickr.stats.getPhotosetStats"
        },
        {
          "date",
          UtilityMethods.DateToUnixTimestamp(date)
        },
        {
          "photoset_id",
          photosetId
        }
      }, callback);
    }

    public void StatsGetPhotoReferrersAsync(DateTime date, string domain, Action<FlickrResult<StatReferrerCollection>> callback)
    {
      this.StatsGetPhotoReferrersAsync(date, domain, (string) null, 0, 0, callback);
    }

    public void StatsGetPhotoReferrersAsync(DateTime date, string domain, string photoId, Action<FlickrResult<StatReferrerCollection>> callback)
    {
      this.StatsGetPhotoReferrersAsync(date, domain, photoId, 0, 0, callback);
    }

    public void StatsGetPhotoReferrersAsync(DateTime date, string domain, int page, int perPage, Action<FlickrResult<StatReferrerCollection>> callback)
    {
      this.StatsGetPhotoReferrersAsync(date, domain, (string) null, page, perPage, callback);
    }

    public void StatsGetPhotoReferrersAsync(DateTime date, string domain, string photoId, int page, int perPage, Action<FlickrResult<StatReferrerCollection>> callback)
    {
      this.CheckRequiresAuthentication();
      Dictionary<string, string> parameters = new Dictionary<string, string>();
      parameters.Add("method", "flickr.stats.getPhotoReferrers");
      parameters.Add("date", UtilityMethods.DateToUnixTimestamp(date));
      parameters.Add("domain", domain);
      if (!string.IsNullOrEmpty(photoId))
        parameters.Add("photo_id", photoId);
      if (page > 0)
        parameters.Add("page", page.ToString((IFormatProvider) NumberFormatInfo.InvariantInfo));
      if (perPage > 0)
        parameters.Add("per_page", perPage.ToString((IFormatProvider) NumberFormatInfo.InvariantInfo));
      this.GetResponseAsync<StatReferrerCollection>(parameters, callback);
    }

    public void StatsGetPhotosetReferrersAsync(DateTime date, string domain, Action<FlickrResult<StatReferrerCollection>> callback)
    {
      this.StatsGetPhotosetReferrersAsync(date, domain, (string) null, 0, 0, callback);
    }

    public void StatsGetPhotosetReferrersAsync(DateTime date, string domain, string photosetId, Action<FlickrResult<StatReferrerCollection>> callback)
    {
      this.StatsGetPhotosetReferrersAsync(date, domain, photosetId, 0, 0, callback);
    }

    public void StatsGetPhotosetReferrersAsync(DateTime date, string domain, int page, int perPage, Action<FlickrResult<StatReferrerCollection>> callback)
    {
      this.StatsGetPhotosetReferrersAsync(date, domain, (string) null, page, perPage, callback);
    }

    public void StatsGetPhotosetReferrersAsync(DateTime date, string domain, string photosetId, int page, int perPage, Action<FlickrResult<StatReferrerCollection>> callback)
    {
      this.CheckRequiresAuthentication();
      Dictionary<string, string> parameters = new Dictionary<string, string>();
      parameters.Add("method", "flickr.stats.getPhotosetReferrers");
      parameters.Add("date", UtilityMethods.DateToUnixTimestamp(date));
      parameters.Add("domain", domain);
      if (!string.IsNullOrEmpty(photosetId))
        parameters.Add("photoset_id", photosetId);
      if (page > 0)
        parameters.Add("page", page.ToString((IFormatProvider) NumberFormatInfo.InvariantInfo));
      if (perPage > 0)
        parameters.Add("per_page", perPage.ToString((IFormatProvider) NumberFormatInfo.InvariantInfo));
      this.GetResponseAsync<StatReferrerCollection>(parameters, callback);
    }

    public void StatsGetCollectionReferrersAsync(DateTime date, string domain, Action<FlickrResult<StatReferrerCollection>> callback)
    {
      this.StatsGetCollectionReferrersAsync(date, domain, (string) null, 0, 0, callback);
    }

    public void StatsGetCollectionReferrersAsync(DateTime date, string domain, string collectionId, Action<FlickrResult<StatReferrerCollection>> callback)
    {
      this.StatsGetCollectionReferrersAsync(date, domain, collectionId, 0, 0, callback);
    }

    public void StatsGetCollectionReferrersAsync(DateTime date, string domain, int page, int perPage, Action<FlickrResult<StatReferrerCollection>> callback)
    {
      this.StatsGetCollectionReferrersAsync(date, domain, (string) null, page, perPage, callback);
    }

    public void StatsGetCollectionReferrersAsync(DateTime date, string domain, string collectionId, int page, int perPage, Action<FlickrResult<StatReferrerCollection>> callback)
    {
      this.CheckRequiresAuthentication();
      Dictionary<string, string> parameters = new Dictionary<string, string>();
      parameters.Add("method", "flickr.stats.getCollectionReferrers");
      parameters.Add("date", UtilityMethods.DateToUnixTimestamp(date));
      parameters.Add("domain", domain);
      if (!string.IsNullOrEmpty(collectionId))
        parameters.Add("collection_id", collectionId);
      if (page > 0)
        parameters.Add("page", page.ToString((IFormatProvider) NumberFormatInfo.InvariantInfo));
      if (perPage > 0)
        parameters.Add("per_page", perPage.ToString((IFormatProvider) NumberFormatInfo.InvariantInfo));
      this.GetResponseAsync<StatReferrerCollection>(parameters, callback);
    }

    public void StatsGetPhotostreamReferrersAsync(DateTime date, string domain, Action<FlickrResult<StatReferrerCollection>> callback)
    {
      this.StatsGetPhotostreamReferrersAsync(date, domain, 0, 0, callback);
    }

    public void StatsGetPhotostreamReferrersAsync(DateTime date, string domain, int page, int perPage, Action<FlickrResult<StatReferrerCollection>> callback)
    {
      this.CheckRequiresAuthentication();
      Dictionary<string, string> parameters = new Dictionary<string, string>();
      parameters.Add("method", "flickr.stats.getPhotostreamReferrers");
      parameters.Add("date", UtilityMethods.DateToUnixTimestamp(date));
      parameters.Add("domain", domain);
      if (page > 0)
        parameters.Add("page", page.ToString((IFormatProvider) NumberFormatInfo.InvariantInfo));
      if (perPage > 0)
        parameters.Add("per_page", perPage.ToString((IFormatProvider) NumberFormatInfo.InvariantInfo));
      this.GetResponseAsync<StatReferrerCollection>(parameters, callback);
    }

    public void StatsGetTotalViewsAsync(Action<FlickrResult<StatViews>> callback)
    {
      this.StatsGetTotalViewsAsync(DateTime.MinValue, callback);
    }

    public void StatsGetTotalViewsAsync(DateTime date, Action<FlickrResult<StatViews>> callback)
    {
      this.CheckRequiresAuthentication();
      Dictionary<string, string> parameters = new Dictionary<string, string>();
      parameters.Add("method", "flickr.stats.getTotalViews");
      if (date != DateTime.MinValue)
        parameters.Add("date", UtilityMethods.DateToUnixTimestamp(date));
      this.GetResponseAsync<StatViews>(parameters, callback);
    }

    public void StatsGetPopularPhotosAsync(Action<FlickrResult<PopularPhotoCollection>> callback)
    {
      this.StatsGetPopularPhotosAsync(DateTime.MinValue, PopularitySort.None, 0, 0, callback);
    }

    public void StatsGetPopularPhotosAsync(DateTime date, Action<FlickrResult<PopularPhotoCollection>> callback)
    {
      this.StatsGetPopularPhotosAsync(date, PopularitySort.None, 0, 0, callback);
    }

    public void StatsGetPopularPhotosAsync(PopularitySort sort, Action<FlickrResult<PopularPhotoCollection>> callback)
    {
      this.StatsGetPopularPhotosAsync(DateTime.MinValue, sort, 0, 0, callback);
    }

    public void StatsGetPopularPhotosAsync(PopularitySort sort, int page, int perPage, Action<FlickrResult<PopularPhotoCollection>> callback)
    {
      this.StatsGetPopularPhotosAsync(DateTime.MinValue, sort, page, perPage, callback);
    }

    public void StatsGetPopularPhotosAsync(DateTime date, int page, int perPage, Action<FlickrResult<PopularPhotoCollection>> callback)
    {
      this.StatsGetPopularPhotosAsync(date, PopularitySort.None, page, perPage, callback);
    }

    public void StatsGetPopularPhotosAsync(DateTime date, PopularitySort sort, int page, int perPage, Action<FlickrResult<PopularPhotoCollection>> callback)
    {
      this.CheckRequiresAuthentication();
      Dictionary<string, string> parameters = new Dictionary<string, string>();
      parameters.Add("method", "flickr.stats.getPopularPhotos");
      if (date != DateTime.MinValue)
        parameters.Add("date", UtilityMethods.DateToUnixTimestamp(date));
      if (sort != PopularitySort.None)
        parameters.Add("sort", UtilityMethods.SortOrderToString(sort));
      if (page > 0)
        parameters.Add("page", page.ToString((IFormatProvider) NumberFormatInfo.InvariantInfo));
      if (perPage > 0)
        parameters.Add("per_page", perPage.ToString((IFormatProvider) NumberFormatInfo.InvariantInfo));
      this.GetResponseAsync<PopularPhotoCollection>(parameters, callback);
    }

    public void TagsGetListPhotoAsync(string photoId, Action<FlickrResult<Collection<PhotoInfoTag>>> callback)
    {
      this.GetResponseAsync<PhotoInfo>(new Dictionary<string, string>()
      {
        {
          "method",
          "flickr.tags.getListPhoto"
        },
        {
          "api_key",
          this.apiKey
        },
        {
          "photo_id",
          photoId
        }
      }, (Action<FlickrResult<PhotoInfo>>) (r =>
      {
        FlickrResult<Collection<PhotoInfoTag>> local_0 = new FlickrResult<Collection<PhotoInfoTag>>();
        local_0.Error = r.Error;
        if (!r.HasError)
          local_0.Result = r.Result.Tags;
        callback(local_0);
      }));
    }

    public void TagsGetListUserAsync(Action<FlickrResult<TagCollection>> callback)
    {
      this.TagsGetListUserAsync((string) null, callback);
    }

    public void TagsGetListUserAsync(string userId, Action<FlickrResult<TagCollection>> callback)
    {
      Dictionary<string, string> parameters = new Dictionary<string, string>();
      parameters.Add("method", "flickr.tags.getListUser");
      if (userId != null && userId.Length > 0)
        parameters.Add("user_id", userId);
      this.GetResponseAsync<TagCollection>(parameters, callback);
    }

    public void TagsGetListUserPopularAsync(Action<FlickrResult<TagCollection>> callback)
    {
      this.CheckRequiresAuthentication();
      this.TagsGetListUserPopularAsync((string) null, 0, callback);
    }

    public void TagsGetListUserPopularAsync(int count, Action<FlickrResult<TagCollection>> callback)
    {
      this.CheckRequiresAuthentication();
      this.TagsGetListUserPopularAsync((string) null, count, callback);
    }

    public void TagsGetListUserPopularAsync(string userId, Action<FlickrResult<TagCollection>> callback)
    {
      this.TagsGetListUserPopularAsync(userId, 0, callback);
    }

    public void TagsGetListUserPopularAsync(string userId, int count, Action<FlickrResult<TagCollection>> callback)
    {
      Dictionary<string, string> parameters = new Dictionary<string, string>();
      parameters.Add("method", "flickr.tags.getListUserPopular");
      if (userId != null)
        parameters.Add("user_id", userId);
      if (count > 0)
        parameters.Add("count", count.ToString((IFormatProvider) NumberFormatInfo.InvariantInfo));
      this.GetResponseAsync<TagCollection>(parameters, callback);
    }

    public void TagsGetListUserRawAsync(Action<FlickrResult<RawTagCollection>> callback)
    {
      this.TagsGetListUserRawAsync((string) null, callback);
    }

    public void TagsGetListUserRawAsync(string tag, Action<FlickrResult<RawTagCollection>> callback)
    {
      this.CheckRequiresAuthentication();
      Dictionary<string, string> parameters = new Dictionary<string, string>();
      parameters.Add("method", "flickr.tags.getListUserRaw");
      if (tag != null && tag.Length > 0)
        parameters.Add("tag", tag);
      this.GetResponseAsync<RawTagCollection>(parameters, callback);
    }

    public void TagsGetMostFrequentlyUsedAsync(Action<FlickrResult<TagCollection>> callback)
    {
      this.CheckRequiresAuthentication();
      this.GetResponseAsync<TagCollection>(new Dictionary<string, string>()
      {
        {
          "method",
          "flickr.tags.getMostFrequentlyUsed"
        }
      }, callback);
    }

    public void TagsGetRelatedAsync(string tag, Action<FlickrResult<TagCollection>> callback)
    {
      this.GetResponseAsync<TagCollection>(new Dictionary<string, string>()
      {
        {
          "method",
          "flickr.tags.getRelated"
        },
        {
          "api_key",
          this.apiKey
        },
        {
          "tag",
          tag
        }
      }, callback);
    }

    public void TagsGetClustersAsync(string tag, Action<FlickrResult<ClusterCollection>> callback)
    {
      this.GetResponseAsync<ClusterCollection>(new Dictionary<string, string>()
      {
        {
          "method",
          "flickr.tags.getClusters"
        },
        {
          "tag",
          tag
        }
      }, callback);
    }

    public void TagsGetClusterPhotosAsync(Cluster cluster, Action<FlickrResult<PhotoCollection>> callback)
    {
      this.TagsGetClusterPhotosAsync(cluster.SourceTag, cluster.ClusterId, PhotoSearchExtras.None, callback);
    }

    public void TagsGetClusterPhotosAsync(Cluster cluster, PhotoSearchExtras extras, Action<FlickrResult<PhotoCollection>> callback)
    {
      this.TagsGetClusterPhotosAsync(cluster.SourceTag, cluster.ClusterId, extras, callback);
    }

    public void TagsGetClusterPhotosAsync(string tag, string clusterId, PhotoSearchExtras extras, Action<FlickrResult<PhotoCollection>> callback)
    {
      Dictionary<string, string> parameters = new Dictionary<string, string>();
      parameters.Add("method", "flickr.tags.getClusterPhotos");
      parameters.Add("tag", tag);
      parameters.Add("cluster_id", clusterId);
      if (extras != PhotoSearchExtras.None)
        parameters.Add("extras", UtilityMethods.ExtrasToString(extras));
      this.GetResponseAsync<PhotoCollection>(parameters, callback);
    }

    public void TagsGetHotListAsync(Action<FlickrResult<HotTagCollection>> callback)
    {
      this.TagsGetHotListAsync((string) null, 0, callback);
    }

    public void TagsGetHotListAsync(string period, int count, Action<FlickrResult<HotTagCollection>> callback)
    {
      if (!string.IsNullOrEmpty(period) && period != "day" && period != "week")
        throw new ArgumentException("Period must be either 'day' or 'week'.", "period");
      Dictionary<string, string> parameters = new Dictionary<string, string>();
      parameters.Add("method", "flickr.tags.getHotList");
      if (!string.IsNullOrEmpty(period))
        parameters.Add("period", period);
      if (count > 0)
        parameters.Add("count", count.ToString((IFormatProvider) NumberFormatInfo.InvariantInfo));
      this.GetResponseAsync<HotTagCollection>(parameters, callback);
    }

    public void TestGenericAsync(string method, Dictionary<string, string> parameters, Action<FlickrResult<UnknownResponse>> callback)
    {
      if (parameters == null)
        parameters = new Dictionary<string, string>();
      parameters.Add("method", method);
      this.GetResponseAsync<UnknownResponse>(parameters, callback);
    }

    public void TestLoginAsync(Action<FlickrResult<FoundUser>> callback)
    {
      this.GetResponseAsync<FoundUser>(new Dictionary<string, string>()
      {
        {
          "method",
          "flickr.test.login"
        }
      }, callback);
    }

    public void TestNullAsync(Action<FlickrResult<NoResponse>> callback)
    {
      this.GetResponseAsync<NoResponse>(new Dictionary<string, string>()
      {
        {
          "method",
          "flickr.test.null"
        }
      }, callback);
    }

    public void TestEchoAsync(Dictionary<string, string> parameters, Action<FlickrResult<EchoResponseDictionary>> callback)
    {
      parameters.Add("method", "flickr.test.echo");
      this.GetResponseAsync<EchoResponseDictionary>(parameters, callback);
    }

    public string UploadPicture(string fileName)
    {
      return this.UploadPicture(fileName, (string) null, (string) null, (string) null, true, false, false);
    }

    public string UploadPicture(string fileName, string title)
    {
      return this.UploadPicture(fileName, title, (string) null, (string) null, true, false, false);
    }

    public string UploadPicture(string fileName, string title, string description)
    {
      return this.UploadPicture(fileName, title, description, (string) null, true, false, false);
    }

    public string UploadPicture(string fileName, string title, string description, string tags)
    {
      string fileName1 = Path.GetFileName(fileName);
      using (Stream stream = (Stream) new FileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
      {
        string str = this.UploadPicture(stream, fileName1, title, description, tags, false, false, false, ContentType.None, SafetyLevel.None, HiddenFromSearch.None);
        stream.Close();
        return str;
      }
    }

    public string UploadPicture(string fileName, string title, string description, string tags, bool isPublic, bool isFamily, bool isFriend)
    {
      string fileName1 = Path.GetFileName(fileName);
      using (Stream stream = (Stream) new FileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
      {
        string str = this.UploadPicture(stream, fileName1, title, description, tags, isPublic, isFamily, isFriend, ContentType.None, SafetyLevel.None, HiddenFromSearch.None);
        stream.Close();
        return str;
      }
    }

    public string UploadPicture(Stream stream, string fileName, string title, string description, string tags, bool isPublic, bool isFamily, bool isFriend, ContentType contentType, SafetyLevel safetyLevel, HiddenFromSearch hiddenFromSearch)
    {
      this.CheckRequiresAuthentication();
      Uri uploadUri = new Uri(this.UploadUrl);
      Dictionary<string, string> parameters = new Dictionary<string, string>();
      if (!string.IsNullOrEmpty(title))
        parameters.Add("title", title);
      if (!string.IsNullOrEmpty(description))
        parameters.Add("description", description);
      if (!string.IsNullOrEmpty(tags))
        parameters.Add("tags", tags);
      parameters.Add("is_public", isPublic ? "1" : "0");
      parameters.Add("is_friend", isFriend ? "1" : "0");
      parameters.Add("is_family", isFamily ? "1" : "0");
      if (safetyLevel != SafetyLevel.None)
        parameters.Add("safety_level", safetyLevel.ToString("D"));
      if (contentType != ContentType.None)
        parameters.Add("content_type", contentType.ToString("D"));
      if (hiddenFromSearch != HiddenFromSearch.None)
        parameters.Add("hidden", hiddenFromSearch.ToString("D"));
      if (!string.IsNullOrEmpty(this.OAuthAccessToken))
      {
        this.OAuthGetBasicParameters(parameters);
        parameters.Add("oauth_token", this.OAuthAccessToken);
        string str = this.OAuthCalculateSignature("POST", uploadUri.AbsoluteUri, parameters, this.OAuthAccessTokenSecret);
        parameters.Add("oauth_signature", str);
      }
      else
      {
        parameters.Add("api_key", this.apiKey);
        parameters.Add("auth_token", this.apiToken);
      }
      XmlReader reader = XmlReader.Create((TextReader) new StringReader(this.UploadData(stream, fileName, uploadUri, parameters)), new XmlReaderSettings()
      {
        IgnoreWhitespace = true
      });
      if (!reader.ReadToDescendant("rsp"))
        throw new XmlException("Unable to find response element 'rsp' in Flickr response");
      while (reader.MoveToNextAttribute())
      {
        if (reader.LocalName == "stat" && reader.Value == "fail")
          throw ExceptionHandler.CreateResponseException(reader);
      }
      reader.MoveToElement();
      reader.Read();
      UnknownResponse unknownResponse = new UnknownResponse();
      unknownResponse.Load(reader);
      return unknownResponse.GetElementValue("photoid");
    }

    private string UploadData(Stream imageStream, string fileName, Uri uploadUri, Dictionary<string, string> parameters)
    {
      string boundary = "FLICKR_MIME_" + DateTime.Now.ToString("yyyyMMddhhmmss", (IFormatProvider) DateTimeFormatInfo.InvariantInfo);
      string str1 = FlickrResponder.OAuthCalculateAuthHeader(parameters);
      Flickr.StreamCollection uploadData = this.CreateUploadData(imageStream, fileName, parameters, boundary);
      HttpWebRequest httpWebRequest = (HttpWebRequest) WebRequest.Create(uploadUri);
      httpWebRequest.Method = "POST";
      if (this.Proxy != null)
        httpWebRequest.Proxy = (IWebProxy) this.Proxy;
      httpWebRequest.Timeout = this.HttpTimeout;
      httpWebRequest.ContentType = "multipart/form-data; boundary=" + boundary;
      if (!string.IsNullOrEmpty(str1))
        ((NameValueCollection) httpWebRequest.Headers)["Authorization"] = str1;
      httpWebRequest.ContentLength = uploadData.Length;
      using (Stream requestStream = ((WebRequest) httpWebRequest).GetRequestStream())
      {
        int bufferSize = 32768;
        if (uploadData.Length / 100L > (long) bufferSize)
          bufferSize *= 2;
        uploadData.UploadProgress += (EventHandler<UploadProgressEventArgs>) ((o, e) =>
        {
          if (this.OnUploadProgress == null)
            return;
          this.OnUploadProgress((object) this, e);
        });
        uploadData.CopyTo(requestStream, bufferSize);
        requestStream.Flush();
      }
      Stream responseStream = httpWebRequest.GetResponse().GetResponseStream();
      if (responseStream == null)
        throw new FlickrWebException("Unable to retrieve stream from web response.");
      StreamReader streamReader = new StreamReader(responseStream);
      string str2 = streamReader.ReadToEnd();
      streamReader.Close();
      return str2;
    }

    public string ReplacePicture(string fullFileName, string photoId)
    {
      FileStream fileStream = (FileStream) null;
      try
      {
        fileStream = new FileStream(fullFileName, FileMode.Open, FileAccess.Read, FileShare.Read);
        return this.ReplacePicture((Stream) fileStream, fullFileName, photoId);
      }
      finally
      {
        if (fileStream != null)
          fileStream.Close();
      }
    }

    public string ReplacePicture(Stream stream, string fileName, string photoId)
    {
      Uri uploadUri = new Uri(this.ReplaceUrl);
      Dictionary<string, string> parameters = new Dictionary<string, string>()
      {
        {
          "photo_id",
          photoId
        }
      };
      if (!string.IsNullOrEmpty(this.OAuthAccessToken))
      {
        this.OAuthGetBasicParameters(parameters);
        parameters.Add("oauth_token", this.OAuthAccessToken);
        string str = this.OAuthCalculateSignature("POST", uploadUri.AbsoluteUri, parameters, this.OAuthAccessTokenSecret);
        parameters.Add("oauth_signature", str);
      }
      else
      {
        parameters.Add("api_key", this.apiKey);
        parameters.Add("auth_token", this.apiToken);
      }
      XmlReader reader = XmlReader.Create((TextReader) new StringReader(this.UploadData(stream, fileName, uploadUri, parameters)), new XmlReaderSettings()
      {
        IgnoreWhitespace = true
      });
      if (!reader.ReadToDescendant("rsp"))
        throw new XmlException("Unable to find response element 'rsp' in Flickr response");
      while (reader.MoveToNextAttribute())
      {
        if (reader.LocalName == "stat" && reader.Value == "fail")
          throw ExceptionHandler.CreateResponseException(reader);
      }
      reader.MoveToElement();
      reader.Read();
      UnknownResponse unknownResponse = new UnknownResponse();
      unknownResponse.Load(reader);
      return unknownResponse.GetElementValue("photoid");
    }

    public void UploadPictureAsync(Stream stream, string fileName, string title, string description, string tags, bool isPublic, bool isFamily, bool isFriend, ContentType contentType, SafetyLevel safetyLevel, HiddenFromSearch hiddenFromSearch, Action<FlickrResult<string>> callback)
    {
      this.CheckRequiresAuthentication();
      Uri uploadUri = new Uri(this.UploadUrl);
      Dictionary<string, string> parameters = new Dictionary<string, string>();
      if (title != null && title.Length > 0)
        parameters.Add("title", title);
      if (description != null && description.Length > 0)
        parameters.Add("description", description);
      if (tags != null && tags.Length > 0)
        parameters.Add("tags", tags);
      parameters.Add("is_public", isPublic ? "1" : "0");
      parameters.Add("is_friend", isFriend ? "1" : "0");
      parameters.Add("is_family", isFamily ? "1" : "0");
      if (safetyLevel != SafetyLevel.None)
        parameters.Add("safety_level", safetyLevel.ToString("D"));
      if (contentType != ContentType.None)
        parameters.Add("content_type", contentType.ToString("D"));
      if (hiddenFromSearch != HiddenFromSearch.None)
        parameters.Add("hidden", hiddenFromSearch.ToString("D"));
      parameters.Add("api_key", this.apiKey);
      if (!string.IsNullOrEmpty(this.OAuthAccessToken))
      {
        parameters.Remove("api_key");
        this.OAuthGetBasicParameters(parameters);
        parameters.Add("oauth_token", this.OAuthAccessToken);
        string str = this.OAuthCalculateSignature("POST", uploadUri.AbsoluteUri, parameters, this.OAuthAccessTokenSecret);
        parameters.Add("oauth_signature", str);
      }
      else
        parameters.Add("auth_token", this.apiToken);
      this.UploadDataAsync(stream, fileName, uploadUri, parameters, callback);
    }

    public void ReplacePictureAsync(Stream stream, string fileName, string photoId, Action<FlickrResult<string>> callback)
    {
      Uri uploadUri = new Uri(this.ReplaceUrl);
      this.UploadDataAsync(stream, fileName, uploadUri, new Dictionary<string, string>()
      {
        {
          "photo_id",
          photoId
        },
        {
          "api_key",
          this.apiKey
        },
        {
          "auth_token",
          this.apiToken
        }
      }, callback);
    }

    private void UploadDataAsync(Stream imageStream, string fileName, Uri uploadUri, Dictionary<string, string> parameters, Action<FlickrResult<string>> callback)
    {
      string boundary = "FLICKR_MIME_" + DateTime.Now.ToString("yyyyMMddhhmmss", (IFormatProvider) DateTimeFormatInfo.InvariantInfo);
      string str = FlickrResponder.OAuthCalculateAuthHeader(parameters);
      Flickr.StreamCollection dataBuffer = this.CreateUploadData(imageStream, fileName, parameters, boundary);
      WebRequest req = WebRequest.Create(uploadUri);
      req.Method = "POST";
      req.ContentType = "multipart/form-data; boundary=" + boundary;
      if (!string.IsNullOrEmpty(str))
        ((NameValueCollection) req.Headers)["Authorization"] = str;
      req.BeginGetRequestStream((AsyncCallback) (r =>
      {
        using (Stream resource_0 = req.EndGetRequestStream(r))
        {
          int local_1 = 32768;
          if (dataBuffer.Length / 100L > (long) local_1)
            local_1 *= 2;
          dataBuffer.UploadProgress += (EventHandler<UploadProgressEventArgs>) ((o, e) =>
          {
            if (this.OnUploadProgress == null)
              return;
            this.OnUploadProgress((object) this, e);
          });
          dataBuffer.CopyTo(resource_0, local_1);
          resource_0.Close();
        }
        req.BeginGetResponse((AsyncCallback) (r2 =>
        {
          FlickrResult<string> local_0 = new FlickrResult<string>();
          try
          {
            StreamReader local_2 = new StreamReader(req.EndGetResponse(r2).GetResponseStream());
            string local_3 = local_2.ReadToEnd();
            local_2.Close();
            XmlReaderSettings local_4 = new XmlReaderSettings()
            {
              IgnoreWhitespace = true
            };
            XmlReader local_5 = XmlReader.Create((TextReader) new StringReader(local_3), local_4);
            if (!local_5.ReadToDescendant("rsp"))
              throw new XmlException("Unable to find response element 'rsp' in Flickr response");
            while (local_5.MoveToNextAttribute())
            {
              if (local_5.LocalName == "stat" && local_5.Value == "fail")
                throw ExceptionHandler.CreateResponseException(local_5);
            }
            local_5.MoveToElement();
            local_5.Read();
            UnknownResponse local_6 = new UnknownResponse();
            local_6.Load(local_5);
            local_0.Result = local_6.GetElementValue("photoid");
            local_0.HasError = false;
          }
          catch (Exception exception_0)
          {
            if (exception_0 is WebException)
            {
              OAuthException local_9 = new OAuthException(exception_0);
              local_0.Error = string.IsNullOrEmpty(local_9.Message) ? exception_0 : (Exception) local_9;
            }
            else
              local_0.Error = exception_0;
          }
          callback(local_0);
        }), (object) this);
      }), (object) this);
    }

    public void UrlsGetGroupAsync(string groupId, Action<FlickrResult<string>> callback)
    {
      this.GetResponseAsync<UnknownResponse>(new Dictionary<string, string>()
      {
        {
          "method",
          "flickr.urls.getGroup"
        },
        {
          "group_id",
          groupId
        }
      }, (Action<FlickrResult<UnknownResponse>>) (r =>
      {
        FlickrResult<string> local_0 = new FlickrResult<string>();
        local_0.Error = r.Error;
        if (!r.HasError)
          local_0.Result = r.Result.GetAttributeValue("*", "url");
        callback(local_0);
      }));
    }

    public void UrlsGetUserPhotosAsync(Action<FlickrResult<string>> callback)
    {
      this.CheckRequiresAuthentication();
      this.UrlsGetUserPhotosAsync((string) null, callback);
    }

    public void UrlsGetUserPhotosAsync(string userId, Action<FlickrResult<string>> callback)
    {
      Dictionary<string, string> parameters = new Dictionary<string, string>();
      parameters.Add("method", "flickr.urls.getUserPhotos");
      if (userId != null && userId.Length > 0)
        parameters.Add("user_id", userId);
      this.GetResponseAsync<UnknownResponse>(parameters, (Action<FlickrResult<UnknownResponse>>) (r =>
      {
        FlickrResult<string> local_0 = new FlickrResult<string>();
        local_0.Error = r.Error;
        if (!r.HasError)
          local_0.Result = r.Result.GetAttributeValue("*", "url");
        callback(local_0);
      }));
    }

    public void UrlsGetUserProfileAsync(Action<FlickrResult<string>> callback)
    {
      this.CheckRequiresAuthentication();
      this.UrlsGetUserProfileAsync((string) null, callback);
    }

    public void UrlsGetUserProfileAsync(string userId, Action<FlickrResult<string>> callback)
    {
      Dictionary<string, string> parameters = new Dictionary<string, string>();
      parameters.Add("method", "flickr.urls.getUserProfile");
      if (userId != null && userId.Length > 0)
        parameters.Add("user_id", userId);
      this.GetResponseAsync<UnknownResponse>(parameters, (Action<FlickrResult<UnknownResponse>>) (r =>
      {
        FlickrResult<string> local_0 = new FlickrResult<string>();
        local_0.Error = r.Error;
        if (!r.HasError)
          local_0.Result = r.Result.GetAttributeValue("*", "url");
        callback(local_0);
      }));
    }

    public void UrlsLookupGalleryAsync(string url, Action<FlickrResult<Gallery>> callback)
    {
      this.GetResponseAsync<Gallery>(new Dictionary<string, string>()
      {
        {
          "method",
          "flickr.urls.lookupGallery"
        },
        {
          "api_key",
          this.apiKey
        },
        {
          "url",
          url
        }
      }, callback);
    }

    public void UrlsLookupGroupAsync(string urlToFind, Action<FlickrResult<string>> callback)
    {
      this.GetResponseAsync<UnknownResponse>(new Dictionary<string, string>()
      {
        {
          "method",
          "flickr.urls.lookupGroup"
        },
        {
          "api_key",
          this.apiKey
        },
        {
          "url",
          urlToFind
        }
      }, (Action<FlickrResult<UnknownResponse>>) (r =>
      {
        FlickrResult<string> local_0 = new FlickrResult<string>();
        local_0.Error = r.Error;
        if (!r.HasError)
          local_0.Result = r.Result.GetAttributeValue("*", "id");
        callback(local_0);
      }));
    }

    public void UrlsLookupUserAsync(string urlToFind, Action<FlickrResult<FoundUser>> callback)
    {
      this.GetResponseAsync<FoundUser>(new Dictionary<string, string>()
      {
        {
          "method",
          "flickr.urls.lookupUser"
        },
        {
          "api_key",
          this.apiKey
        },
        {
          "url",
          urlToFind
        }
      }, callback);
    }

    public ActivityItemCollection ActivityUserPhotos()
    {
      return this.ActivityUserPhotos((string) null, 0, 0);
    }

    public ActivityItemCollection ActivityUserPhotos(int page, int perPage)
    {
      return this.ActivityUserPhotos((string) null, page, perPage);
    }

    public ActivityItemCollection ActivityUserPhotos(int timePeriod, string timeType)
    {
      return this.ActivityUserPhotos(timePeriod, timeType, 0, 0);
    }

    public ActivityItemCollection ActivityUserPhotos(int timePeriod, string timeType, int page, int perPage)
    {
      if (timePeriod == 0)
        throw new ArgumentOutOfRangeException("timePeriod", "Time Period should be greater than 0");
      if (timeType == null)
        throw new ArgumentNullException("timeType");
      if (timeType != "d" && timeType != "h")
        throw new ArgumentOutOfRangeException("timeType", "Time type must be 'd' or 'h'");
      else
        return this.ActivityUserPhotos((string) (object) timePeriod + (object) timeType, page, perPage);
    }

    private ActivityItemCollection ActivityUserPhotos(string timeframe, int page, int perPage)
    {
      this.CheckRequiresAuthentication();
      Dictionary<string, string> parameters = new Dictionary<string, string>();
      parameters.Add("method", "flickr.activity.userPhotos");
      if (!string.IsNullOrEmpty(timeframe))
        parameters.Add("timeframe", timeframe);
      if (page > 0)
        parameters.Add("page", page.ToString((IFormatProvider) NumberFormatInfo.InvariantInfo));
      if (perPage > 0)
        parameters.Add("per_page", perPage.ToString((IFormatProvider) NumberFormatInfo.InvariantInfo));
      return this.GetResponseCache<ActivityItemCollection>(parameters);
    }

    public ActivityItemCollection ActivityUserComments(int page, int perPage)
    {
      this.CheckRequiresAuthentication();
      Dictionary<string, string> parameters = new Dictionary<string, string>();
      parameters.Add("method", "flickr.activity.userComments");
      if (page > 0)
        parameters.Add("page", page.ToString((IFormatProvider) NumberFormatInfo.InvariantInfo));
      if (perPage > 0)
        parameters.Add("per_page", perPage.ToString((IFormatProvider) NumberFormatInfo.InvariantInfo));
      return this.GetResponseCache<ActivityItemCollection>(parameters);
    }

    public BlogCollection BlogsGetList()
    {
      this.CheckRequiresAuthentication();
      return this.GetResponseCache<BlogCollection>(new Dictionary<string, string>()
      {
        {
          "method",
          "flickr.blogs.getList"
        }
      });
    }

    public BlogServiceCollection BlogsGetServices()
    {
      return this.GetResponseCache<BlogServiceCollection>(new Dictionary<string, string>()
      {
        {
          "method",
          "flickr.blogs.getServices"
        }
      });
    }

    public void BlogsPostPhoto(string blogId, string photoId, string title, string description)
    {
      this.BlogsPostPhoto(blogId, photoId, title, description, (string) null);
    }

    public void BlogsPostPhoto(string blogId, string photoId, string title, string description, string blogPassword)
    {
      Dictionary<string, string> parameters = new Dictionary<string, string>();
      parameters.Add("method", "flickr.blogs.postPhoto");
      parameters.Add("blog_id", blogId);
      parameters.Add("photo_id", photoId);
      parameters.Add("title", title);
      parameters.Add("description", description);
      if (blogPassword != null)
        parameters.Add("blog_password", blogPassword);
      this.GetResponseCache<NoResponse>(parameters);
    }

    public ContactCollection ContactsGetList()
    {
      return this.ContactsGetList((string) null, 0, 0);
    }

    public ContactCollection ContactsGetList(int page, int perPage)
    {
      return this.ContactsGetList((string) null, page, perPage);
    }

    public ContactCollection ContactsGetList(string filter)
    {
      return this.ContactsGetList(filter, 0, 0);
    }

    public ContactCollection ContactsGetList(string filter, int page, int perPage)
    {
      this.CheckRequiresAuthentication();
      Dictionary<string, string> parameters = new Dictionary<string, string>();
      parameters.Add("method", "flickr.contacts.getList");
      if (!string.IsNullOrEmpty(filter))
        parameters.Add("filter", filter);
      if (page > 0)
        parameters.Add("page", page.ToString((IFormatProvider) NumberFormatInfo.InvariantInfo));
      if (perPage > 0)
        parameters.Add("per_page", perPage.ToString((IFormatProvider) NumberFormatInfo.InvariantInfo));
      return this.GetResponseCache<ContactCollection>(parameters);
    }

    public ContactCollection ContactsGetListRecentlyUploaded()
    {
      return this.ContactsGetListRecentlyUploaded(DateTime.MinValue, (string) null);
    }

    public ContactCollection ContactsGetListRecentlyUploaded(string filter)
    {
      return this.ContactsGetListRecentlyUploaded(DateTime.MinValue, filter);
    }

    public ContactCollection ContactsGetListRecentlyUploaded(DateTime dateLastUpdated)
    {
      return this.ContactsGetListRecentlyUploaded(dateLastUpdated, (string) null);
    }

    public ContactCollection ContactsGetListRecentlyUploaded(DateTime dateLastUpdated, string filter)
    {
      this.CheckRequiresAuthentication();
      Dictionary<string, string> parameters = new Dictionary<string, string>();
      parameters.Add("method", "flickr.contacts.getListRecentlyUploaded");
      if (dateLastUpdated != DateTime.MinValue)
        parameters.Add("date_lastupload", UtilityMethods.DateToUnixTimestamp(dateLastUpdated));
      if (!string.IsNullOrEmpty(filter))
        parameters.Add("filter", filter);
      return this.GetResponseNoCache<ContactCollection>(parameters);
    }

    public ContactCollection ContactsGetPublicList(string userId)
    {
      return this.ContactsGetPublicList(userId, 0, 0);
    }

    public ContactCollection ContactsGetPublicList(string userId, int page, int perPage)
    {
      Dictionary<string, string> parameters = new Dictionary<string, string>();
      parameters.Add("method", "flickr.contacts.getPublicList");
      parameters.Add("user_id", userId);
      if (page > 0)
        parameters.Add("page", page.ToString((IFormatProvider) NumberFormatInfo.InvariantInfo));
      if (perPage > 0)
        parameters.Add("per_page", perPage.ToString((IFormatProvider) NumberFormatInfo.InvariantInfo));
      return this.GetResponseCache<ContactCollection>(parameters);
    }

    public ContactCollection ContactsGetTaggingSuggestions()
    {
      return this.ContactsGetTaggingSuggestions(0, 0);
    }

    public ContactCollection ContactsGetTaggingSuggestions(int page, int perPage)
    {
      this.CheckRequiresAuthentication();
      Dictionary<string, string> parameters = new Dictionary<string, string>();
      parameters.Add("method", "flickr.contacts.getTaggingSuggestions");
      if (page > 0)
        parameters.Add("page", page.ToString((IFormatProvider) NumberFormatInfo.InvariantInfo));
      if (perPage > 0)
        parameters.Add("per_page", perPage.ToString((IFormatProvider) NumberFormatInfo.InvariantInfo));
      return this.GetResponseCache<ContactCollection>(parameters);
    }

    public void FavoritesAdd(string photoId)
    {
      this.GetResponseNoCache<NoResponse>(new Dictionary<string, string>()
      {
        {
          "method",
          "flickr.favorites.add"
        },
        {
          "photo_id",
          photoId
        }
      });
    }

    public void FavoritesRemove(string photoId)
    {
      this.GetResponseNoCache<NoResponse>(new Dictionary<string, string>()
      {
        {
          "method",
          "flickr.favorites.remove"
        },
        {
          "photo_id",
          photoId
        }
      });
    }

    public PhotoCollection FavoritesGetList()
    {
      return this.FavoritesGetList((string) null, DateTime.MinValue, DateTime.MinValue, PhotoSearchExtras.None, 0, 0);
    }

    public PhotoCollection FavoritesGetList(PhotoSearchExtras extras)
    {
      return this.FavoritesGetList((string) null, DateTime.MinValue, DateTime.MinValue, extras, 0, 0);
    }

    public PhotoCollection FavoritesGetList(int page, int perPage)
    {
      return this.FavoritesGetList((string) null, DateTime.MinValue, DateTime.MinValue, PhotoSearchExtras.None, page, perPage);
    }

    public PhotoCollection FavoritesGetList(PhotoSearchExtras extras, int page, int perPage)
    {
      return this.FavoritesGetList((string) null, DateTime.MinValue, DateTime.MinValue, extras, page, perPage);
    }

    public PhotoCollection FavoritesGetList(string userId)
    {
      return this.FavoritesGetList(userId, DateTime.MinValue, DateTime.MinValue, PhotoSearchExtras.None, 0, 0);
    }

    public PhotoCollection FavoritesGetList(string userId, PhotoSearchExtras extras)
    {
      return this.FavoritesGetList(userId, DateTime.MinValue, DateTime.MinValue, extras, 0, 0);
    }

    public PhotoCollection FavoritesGetList(string userId, int page, int perPage)
    {
      return this.FavoritesGetList(userId, DateTime.MinValue, DateTime.MinValue, PhotoSearchExtras.None, page, perPage);
    }

    public PhotoCollection FavoritesGetList(string userId, PhotoSearchExtras extras, int page, int perPage)
    {
      return this.FavoritesGetList(userId, DateTime.MinValue, DateTime.MinValue, extras, page, perPage);
    }

    public PhotoCollection FavoritesGetList(string userId, DateTime minFavoriteDate, DateTime maxFavoriteDate, PhotoSearchExtras extras, int page, int perPage)
    {
      this.CheckRequiresAuthentication();
      Dictionary<string, string> parameters = new Dictionary<string, string>()
      {
        {
          "method",
          "flickr.favorites.getList"
        }
      };
      if (userId != null)
        parameters.Add("user_id", userId);
      if (minFavoriteDate != DateTime.MinValue)
        parameters.Add("min_fave_date", UtilityMethods.DateToUnixTimestamp(minFavoriteDate));
      if (maxFavoriteDate != DateTime.MinValue)
        parameters.Add("max_fave_date", UtilityMethods.DateToUnixTimestamp(maxFavoriteDate));
      if (extras != PhotoSearchExtras.None)
        parameters.Add("extras", UtilityMethods.ExtrasToString(extras));
      if (perPage > 0)
        parameters.Add("per_page", perPage.ToString((IFormatProvider) NumberFormatInfo.InvariantInfo));
      if (page > 0)
        parameters.Add("page", page.ToString((IFormatProvider) NumberFormatInfo.InvariantInfo));
      return this.GetResponseCache<PhotoCollection>(parameters);
    }

    public PhotoCollection FavoritesGetPublicList(string userId)
    {
      return this.FavoritesGetPublicList(userId, DateTime.MinValue, DateTime.MinValue, PhotoSearchExtras.None, 0, 0);
    }

    public PhotoCollection FavoritesGetPublicList(string userId, DateTime minFavoriteDate, DateTime maxFavoriteDate, PhotoSearchExtras extras, int page, int perPage)
    {
      Dictionary<string, string> parameters = new Dictionary<string, string>()
      {
        {
          "method",
          "flickr.favorites.getPublicList"
        },
        {
          "user_id",
          userId
        }
      };
      if (minFavoriteDate != DateTime.MinValue)
        parameters.Add("min_fave_date", UtilityMethods.DateToUnixTimestamp(minFavoriteDate));
      if (maxFavoriteDate != DateTime.MinValue)
        parameters.Add("max_fave_date", UtilityMethods.DateToUnixTimestamp(maxFavoriteDate));
      if (extras != PhotoSearchExtras.None)
        parameters.Add("extras", UtilityMethods.ExtrasToString(extras));
      if (perPage > 0)
        parameters.Add("per_page", perPage.ToString((IFormatProvider) NumberFormatInfo.InvariantInfo));
      if (page > 0)
        parameters.Add("page", page.ToString((IFormatProvider) NumberFormatInfo.InvariantInfo));
      return this.GetResponseCache<PhotoCollection>(parameters);
    }

    public FavoriteContext FavoritesGetContext(string photoId, string userId)
    {
      return this.FavoritesGetContext(photoId, userId, 1, 1, PhotoSearchExtras.None);
    }

    public FavoriteContext FavoritesGetContext(string photoId, string userId, PhotoSearchExtras extras)
    {
      return this.FavoritesGetContext(photoId, userId, 1, 1, extras);
    }

    public FavoriteContext FavoritesGetContext(string photoId, string userId, int numPrevious, int numNext)
    {
      return this.FavoritesGetContext(photoId, userId, numPrevious, numNext, PhotoSearchExtras.None);
    }

    public FavoriteContext FavoritesGetContext(string photoId, string userId, int numPrevious, int numNext, PhotoSearchExtras extras)
    {
      Dictionary<string, string> parameters = new Dictionary<string, string>();
      parameters.Add("method", "flickr.favorites.getContext");
      parameters.Add("user_id", userId);
      parameters.Add("photo_id", photoId);
      parameters.Add("num_prev", Math.Max(1, numPrevious).ToString((IFormatProvider) NumberFormatInfo.InvariantInfo));
      parameters.Add("num_next", Math.Max(1, numNext).ToString((IFormatProvider) NumberFormatInfo.InvariantInfo));
      if (extras != PhotoSearchExtras.None)
        parameters.Add("extras", UtilityMethods.ExtrasToString(extras));
      return this.GetResponseCache<FavoriteContext>(parameters);
    }

    public GroupCategory GroupsBrowse()
    {
      return this.GroupsBrowse((string) null);
    }

    public GroupCategory GroupsBrowse(string catId)
    {
      Dictionary<string, string> parameters = new Dictionary<string, string>();
      parameters.Add("method", "flickr.groups.browse");
      if (!string.IsNullOrEmpty(catId))
        parameters.Add("cat_id", catId);
      return this.GetResponseCache<GroupCategory>(parameters);
    }

    public GroupSearchResultCollection GroupsSearch(string text)
    {
      return this.GroupsSearch(text, 0, 0);
    }

    public GroupSearchResultCollection GroupsSearch(string text, int page)
    {
      return this.GroupsSearch(text, page, 0);
    }

    public GroupSearchResultCollection GroupsSearch(string text, int page, int perPage)
    {
      Dictionary<string, string> parameters = new Dictionary<string, string>();
      parameters.Add("method", "flickr.groups.search");
      parameters.Add("api_key", this.apiKey);
      parameters.Add("text", text);
      if (page > 0)
        parameters.Add("page", page.ToString((IFormatProvider) NumberFormatInfo.InvariantInfo));
      if (perPage > 0)
        parameters.Add("per_page", perPage.ToString((IFormatProvider) NumberFormatInfo.InvariantInfo));
      return this.GetResponseCache<GroupSearchResultCollection>(parameters);
    }

    public GroupFullInfo GroupsGetInfo(string groupId)
    {
      return this.GetResponseCache<GroupFullInfo>(new Dictionary<string, string>()
      {
        {
          "method",
          "flickr.groups.getInfo"
        },
        {
          "api_key",
          this.apiKey
        },
        {
          "group_id",
          groupId
        }
      });
    }

    public MemberCollection GroupsMembersGetList(string groupId)
    {
      return this.GroupsMembersGetList(groupId, 0, 0, MemberTypes.None);
    }

    public MemberCollection GroupsMembersGetList(string groupId, int page, int perPage, MemberTypes memberTypes)
    {
      this.CheckRequiresAuthentication();
      Dictionary<string, string> parameters = new Dictionary<string, string>();
      parameters.Add("method", "flickr.groups.members.getList");
      parameters.Add("api_key", this.apiKey);
      if (page > 0)
        parameters.Add("page", page.ToString((IFormatProvider) NumberFormatInfo.InvariantInfo));
      if (perPage > 0)
        parameters.Add("per_page", perPage.ToString((IFormatProvider) NumberFormatInfo.InvariantInfo));
      if (memberTypes != MemberTypes.None)
        parameters.Add("membertypes", UtilityMethods.MemberTypeToString(memberTypes));
      parameters.Add("group_id", groupId);
      return this.GetResponseCache<MemberCollection>(parameters);
    }

    public void GroupsPoolsAdd(string photoId, string groupId)
    {
      this.CheckRequiresAuthentication();
      this.GetResponseCache<NoResponse>(new Dictionary<string, string>()
      {
        {
          "method",
          "flickr.groups.pools.add"
        },
        {
          "photo_id",
          photoId
        },
        {
          "group_id",
          groupId
        }
      });
    }

    public Context GroupsPoolsGetContext(string photoId, string groupId)
    {
      return this.GetResponseCache<Context>(new Dictionary<string, string>()
      {
        {
          "method",
          "flickr.groups.pools.getContext"
        },
        {
          "photo_id",
          photoId
        },
        {
          "group_id",
          groupId
        }
      });
    }

    public void GroupsPoolsRemove(string photoId, string groupId)
    {
      this.GetResponseCache<NoResponse>(new Dictionary<string, string>()
      {
        {
          "method",
          "flickr.groups.pools.remove"
        },
        {
          "photo_id",
          photoId
        },
        {
          "group_id",
          groupId
        }
      });
    }

    public MemberGroupInfoCollection GroupsPoolsGetGroups()
    {
      return this.GroupsPoolsGetGroups(0, 0);
    }

    public MemberGroupInfoCollection GroupsPoolsGetGroups(int page, int perPage)
    {
      Dictionary<string, string> parameters = new Dictionary<string, string>();
      parameters.Add("method", "flickr.groups.pools.getGroups");
      if (page > 0)
        parameters.Add("page", page.ToString((IFormatProvider) NumberFormatInfo.InvariantInfo));
      if (perPage > 0)
        parameters.Add("per_page", perPage.ToString((IFormatProvider) NumberFormatInfo.InvariantInfo));
      return this.GetResponseCache<MemberGroupInfoCollection>(parameters);
    }

    public PhotoCollection GroupsPoolsGetPhotos(string groupId)
    {
      return this.GroupsPoolsGetPhotos(groupId, (string) null, (string) null, PhotoSearchExtras.None, 0, 0);
    }

    public PhotoCollection GroupsPoolsGetPhotos(string groupId, string tags)
    {
      return this.GroupsPoolsGetPhotos(groupId, tags, (string) null, PhotoSearchExtras.None, 0, 0);
    }

    public PhotoCollection GroupsPoolsGetPhotos(string groupId, int page, int perPage)
    {
      return this.GroupsPoolsGetPhotos(groupId, (string) null, (string) null, PhotoSearchExtras.None, page, perPage);
    }

    public PhotoCollection GroupsPoolsGetPhotos(string groupId, string tags, int page, int perPage)
    {
      return this.GroupsPoolsGetPhotos(groupId, tags, (string) null, PhotoSearchExtras.None, page, perPage);
    }

    public PhotoCollection GroupsPoolsGetPhotos(string groupId, string tags, string userId, PhotoSearchExtras extras, int page, int perPage)
    {
      Dictionary<string, string> parameters = new Dictionary<string, string>();
      parameters.Add("method", "flickr.groups.pools.getPhotos");
      parameters.Add("group_id", groupId);
      if (tags != null && tags.Length > 0)
        parameters.Add("tags", tags);
      if (page > 0)
        parameters.Add("page", page.ToString((IFormatProvider) NumberFormatInfo.InvariantInfo));
      if (perPage > 0)
        parameters.Add("per_page", perPage.ToString((IFormatProvider) NumberFormatInfo.InvariantInfo));
      if (userId != null && userId.Length > 0)
        parameters.Add("user_id", userId);
      if (extras != PhotoSearchExtras.None)
        parameters.Add("extras", UtilityMethods.ExtrasToString(extras));
      return this.GetResponseCache<PhotoCollection>(parameters);
    }

    public void GroupsJoin(string groupId)
    {
      this.GroupsJoin(groupId, false);
    }

    public void GroupsJoin(string groupId, bool acceptRules)
    {
      this.CheckRequiresAuthentication();
      Dictionary<string, string> parameters = new Dictionary<string, string>();
      parameters.Add("method", "flickr.groups.join");
      parameters.Add("group_id", groupId);
      if (acceptRules)
        parameters.Add("accept_rules", "1");
      this.GetResponseNoCache<NoResponse>(parameters);
    }

    public void GroupsJoinRequest(string groupId, string message, bool acceptRules)
    {
      this.CheckRequiresAuthentication();
      Dictionary<string, string> parameters = new Dictionary<string, string>();
      parameters.Add("method", "flickr.groups.joinRequest");
      parameters.Add("group_id", groupId);
      parameters.Add("message", message);
      if (acceptRules)
        parameters.Add("accept_rules", "1");
      this.GetResponseNoCache<NoResponse>(parameters);
    }

    public void GroupsLeave(string groupId)
    {
      this.GroupsLeave(groupId, false);
    }

    public void GroupsLeave(string groupId, bool deletePhotos)
    {
      this.CheckRequiresAuthentication();
      Dictionary<string, string> parameters = new Dictionary<string, string>();
      parameters.Add("method", "flickr.groups.leave");
      parameters.Add("group_id", groupId);
      if (deletePhotos)
        parameters.Add("delete_photos", "1");
      this.GetResponseNoCache<NoResponse>(parameters);
    }

    public PhotoCollection InterestingnessGetList(PhotoSearchExtras extras, int page, int perPage)
    {
      return this.InterestingnessGetList(DateTime.MinValue, extras, page, perPage);
    }

    public PhotoCollection InterestingnessGetList(DateTime date)
    {
      return this.InterestingnessGetList(date, PhotoSearchExtras.None, 0, 0);
    }

    public PhotoCollection InterestingnessGetList()
    {
      return this.InterestingnessGetList(DateTime.MinValue, PhotoSearchExtras.None, 0, 0);
    }

    public PhotoCollection InterestingnessGetList(DateTime date, PhotoSearchExtras extras, int page, int perPage)
    {
      Dictionary<string, string> parameters = new Dictionary<string, string>();
      parameters.Add("method", "flickr.interestingness.getList");
      if (date > DateTime.MinValue)
        parameters.Add("date", date.ToString("yyyy-MM-dd", (IFormatProvider) DateTimeFormatInfo.InvariantInfo));
      if (perPage > 0)
        parameters.Add("per_page", perPage.ToString((IFormatProvider) NumberFormatInfo.InvariantInfo));
      if (page > 0)
        parameters.Add("page", page.ToString((IFormatProvider) NumberFormatInfo.InvariantInfo));
      if (extras != PhotoSearchExtras.None)
        parameters.Add("extras", UtilityMethods.ExtrasToString(extras));
      return this.GetResponseCache<PhotoCollection>(parameters);
    }

    public string PhotosNotesAdd(string photoId, int noteX, int noteY, int noteWidth, int noteHeight, string noteText)
    {
      XmlNode xmlNode = this.GetResponseCache<UnknownResponse>(new Dictionary<string, string>()
      {
        {
          "method",
          "flickr.photos.notes.add"
        },
        {
          "photo_id",
          photoId
        },
        {
          "note_x",
          noteX.ToString((IFormatProvider) NumberFormatInfo.InvariantInfo)
        },
        {
          "note_y",
          noteY.ToString((IFormatProvider) NumberFormatInfo.InvariantInfo)
        },
        {
          "note_w",
          noteWidth.ToString((IFormatProvider) NumberFormatInfo.InvariantInfo)
        },
        {
          "note_h",
          noteHeight.ToString((IFormatProvider) NumberFormatInfo.InvariantInfo)
        },
        {
          "note_text",
          noteText
        }
      }).GetXmlDocument().SelectSingleNode("*/@id");
      if (xmlNode != null)
        return xmlNode.Value;
      else
        return (string) null;
    }

    public void PhotosNotesEdit(string noteId, int noteX, int noteY, int noteWidth, int noteHeight, string noteText)
    {
      this.GetResponseCache<NoResponse>(new Dictionary<string, string>()
      {
        {
          "method",
          "flickr.photos.notes.edit"
        },
        {
          "note_id",
          noteId
        },
        {
          "note_x",
          noteX.ToString((IFormatProvider) NumberFormatInfo.InvariantInfo)
        },
        {
          "note_y",
          noteY.ToString((IFormatProvider) NumberFormatInfo.InvariantInfo)
        },
        {
          "note_w",
          noteWidth.ToString((IFormatProvider) NumberFormatInfo.InvariantInfo)
        },
        {
          "note_h",
          noteHeight.ToString((IFormatProvider) NumberFormatInfo.InvariantInfo)
        },
        {
          "note_text",
          noteText
        }
      });
    }

    public void PhotosNotesDelete(string noteId)
    {
      this.GetResponseCache<NoResponse>(new Dictionary<string, string>()
      {
        {
          "method",
          "flickr.photos.notes.delete"
        },
        {
          "note_id",
          noteId
        }
      });
    }

    public PhotoCommentCollection PhotosCommentsGetList(string photoId)
    {
      return this.GetResponseCache<PhotoCommentCollection>(new Dictionary<string, string>()
      {
        {
          "method",
          "flickr.photos.comments.getList"
        },
        {
          "photo_id",
          photoId
        }
      });
    }

    public string PhotosCommentsAddComment(string photoId, string commentText)
    {
      XmlNode xmlNode = this.GetResponseCache<UnknownResponse>(new Dictionary<string, string>()
      {
        {
          "method",
          "flickr.photos.comments.addComment"
        },
        {
          "photo_id",
          photoId
        },
        {
          "comment_text",
          commentText
        }
      }).GetXmlDocument().SelectSingleNode("*/@id");
      if (xmlNode != null)
        return xmlNode.Value;
      else
        return (string) null;
    }

    public void PhotosCommentsDeleteComment(string commentId)
    {
      this.GetResponseNoCache<NoResponse>(new Dictionary<string, string>()
      {
        {
          "method",
          "flickr.photos.comments.deleteComment"
        },
        {
          "comment_id",
          commentId
        }
      });
    }

    public void PhotosCommentsEditComment(string commentId, string commentText)
    {
      this.GetResponseNoCache<NoResponse>(new Dictionary<string, string>()
      {
        {
          "method",
          "flickr.photos.comments.editComment"
        },
        {
          "comment_id",
          commentId
        },
        {
          "comment_text",
          commentText
        }
      });
    }

    public PhotoCollection PhotosCommentsGetRecentForContacts()
    {
      return this.PhotosCommentsGetRecentForContacts(DateTime.MinValue, (string[]) null, PhotoSearchExtras.None, 0, 0);
    }

    public PhotoCollection PhotosCommentsGetRecentForContacts(int page, int perPage)
    {
      return this.PhotosCommentsGetRecentForContacts(DateTime.MinValue, (string[]) null, PhotoSearchExtras.None, page, perPage);
    }

    public PhotoCollection PhotosCommentsGetRecentForContacts(DateTime dateLastComment, PhotoSearchExtras extras, int page, int perPage)
    {
      return this.PhotosCommentsGetRecentForContacts(dateLastComment, (string[]) null, extras, page, perPage);
    }

    public PhotoCollection PhotosCommentsGetRecentForContacts(DateTime dateLastComment, string[] contactsFilter, PhotoSearchExtras extras, int page, int perPage)
    {
      this.CheckRequiresAuthentication();
      Dictionary<string, string> parameters = new Dictionary<string, string>();
      parameters.Add("method", "flickr.photos.comments.getRecentForContacts");
      if (dateLastComment != DateTime.MinValue)
        parameters.Add("date_lastcomment", UtilityMethods.DateToUnixTimestamp(dateLastComment));
      if (contactsFilter != null && contactsFilter.Length > 0)
        parameters.Add("contacts_filter", string.Join(",", contactsFilter));
      if (extras != PhotoSearchExtras.None)
        parameters.Add("extras", UtilityMethods.ExtrasToString(extras));
      if (page > 0)
        parameters.Add("page", page.ToString((IFormatProvider) NumberFormatInfo.InvariantInfo));
      if (perPage > 0)
        parameters.Add("per_page", perPage.ToString((IFormatProvider) NumberFormatInfo.InvariantInfo));
      return this.GetResponseCache<PhotoCollection>(parameters);
    }

    public void PhotosetsAddPhoto(string photosetId, string photoId)
    {
      this.CheckRequiresAuthentication();
      this.GetResponseNoCache<NoResponse>(new Dictionary<string, string>()
      {
        {
          "method",
          "flickr.photosets.addPhoto"
        },
        {
          "photoset_id",
          photosetId
        },
        {
          "photo_id",
          photoId
        }
      });
    }

    public Photoset PhotosetsCreate(string title, string primaryPhotoId)
    {
      return this.PhotosetsCreate(title, (string) null, primaryPhotoId);
    }

    public Photoset PhotosetsCreate(string title, string description, string primaryPhotoId)
    {
      this.CheckRequiresAuthentication();
      Dictionary<string, string> parameters = new Dictionary<string, string>()
      {
        {
          "method",
          "flickr.photosets.create"
        },
        {
          "primary_photo_id",
          primaryPhotoId
        }
      };
      if (!string.IsNullOrEmpty(title))
        parameters.Add("title", title);
      if (!string.IsNullOrEmpty(description))
        parameters.Add("description", description);
      return this.GetResponseNoCache<Photoset>(parameters);
    }

    public void PhotosetsDelete(string photosetId)
    {
      this.GetResponseNoCache<NoResponse>(new Dictionary<string, string>()
      {
        {
          "method",
          "flickr.photosets.delete"
        },
        {
          "photoset_id",
          photosetId
        }
      });
    }

    public void PhotosetsEditMeta(string photosetId, string title, string description)
    {
      this.GetResponseNoCache<NoResponse>(new Dictionary<string, string>()
      {
        {
          "method",
          "flickr.photosets.editMeta"
        },
        {
          "photoset_id",
          photosetId
        },
        {
          "title",
          title
        },
        {
          "description",
          description
        }
      });
    }

    public void PhotosetsEditPhotos(string photosetId, string primaryPhotoId, string[] photoIds)
    {
      this.PhotosetsEditPhotos(photosetId, primaryPhotoId, string.Join(",", photoIds));
    }

    public void PhotosetsEditPhotos(string photosetId, string primaryPhotoId, string photoIds)
    {
      this.GetResponseNoCache<NoResponse>(new Dictionary<string, string>()
      {
        {
          "method",
          "flickr.photosets.editPhotos"
        },
        {
          "photoset_id",
          photosetId
        },
        {
          "primary_photo_id",
          primaryPhotoId
        },
        {
          "photo_ids",
          photoIds
        }
      });
    }

    public Context PhotosetsGetContext(string photoId, string photosetId)
    {
      return this.GetResponseCache<Context>(new Dictionary<string, string>()
      {
        {
          "method",
          "flickr.photosets.getContext"
        },
        {
          "photo_id",
          photoId
        },
        {
          "photoset_id",
          photosetId
        }
      });
    }

    public Photoset PhotosetsGetInfo(string photosetId)
    {
      return this.GetResponseCache<Photoset>(new Dictionary<string, string>()
      {
        {
          "method",
          "flickr.photosets.getInfo"
        },
        {
          "photoset_id",
          photosetId
        }
      });
    }

    public PhotosetCollection PhotosetsGetList()
    {
      this.CheckRequiresAuthentication();
      return this.PhotosetsGetList((string) null, 0, 0);
    }

    public PhotosetCollection PhotosetsGetList(int page, int perPage)
    {
      this.CheckRequiresAuthentication();
      return this.PhotosetsGetList((string) null, page, perPage);
    }

    public PhotosetCollection PhotosetsGetList(string userId)
    {
      return this.PhotosetsGetList(userId, 0, 0);
    }

    public PhotosetCollection PhotosetsGetList(string userId, int page, int perPage)
    {
      Dictionary<string, string> parameters = new Dictionary<string, string>();
      parameters.Add("method", "flickr.photosets.getList");
      if (userId != null)
        parameters.Add("user_id", userId);
      if (page > 0)
        parameters.Add("page", page.ToString((IFormatProvider) NumberFormatInfo.InvariantInfo));
      if (perPage > 0)
        parameters.Add("per_page", perPage.ToString((IFormatProvider) NumberFormatInfo.InvariantInfo));
      PhotosetCollection responseCache = this.GetResponseCache<PhotosetCollection>(parameters);
      foreach (Photoset photoset in (Collection<Photoset>) responseCache)
        photoset.OwnerId = userId;
      return responseCache;
    }

    public PhotosetPhotoCollection PhotosetsGetPhotos(string photosetId)
    {
      return this.PhotosetsGetPhotos(photosetId, PhotoSearchExtras.None, PrivacyFilter.None, 0, 0);
    }

    public PhotosetPhotoCollection PhotosetsGetPhotos(string photosetId, int page, int perPage)
    {
      return this.PhotosetsGetPhotos(photosetId, PhotoSearchExtras.None, PrivacyFilter.None, page, perPage);
    }

    public PhotosetPhotoCollection PhotosetsGetPhotos(string photosetId, PrivacyFilter privacyFilter)
    {
      return this.PhotosetsGetPhotos(photosetId, PhotoSearchExtras.None, privacyFilter, 0, 0);
    }

    public PhotosetPhotoCollection PhotosetsGetPhotos(string photosetId, PrivacyFilter privacyFilter, int page, int perPage)
    {
      return this.PhotosetsGetPhotos(photosetId, PhotoSearchExtras.None, privacyFilter, page, perPage);
    }

    public PhotosetPhotoCollection PhotosetsGetPhotos(string photosetId, PhotoSearchExtras extras)
    {
      return this.PhotosetsGetPhotos(photosetId, extras, PrivacyFilter.None, 0, 0);
    }

    public PhotosetPhotoCollection PhotosetsGetPhotos(string photosetId, PhotoSearchExtras extras, int page, int perPage)
    {
      return this.PhotosetsGetPhotos(photosetId, extras, PrivacyFilter.None, page, perPage);
    }

    public PhotosetPhotoCollection PhotosetsGetPhotos(string photosetId, PhotoSearchExtras extras, PrivacyFilter privacyFilter)
    {
      return this.PhotosetsGetPhotos(photosetId, extras, privacyFilter, 0, 0);
    }

    public PhotosetPhotoCollection PhotosetsGetPhotos(string photosetId, PhotoSearchExtras extras, PrivacyFilter privacyFilter, int page, int perPage)
    {
      return this.PhotosetsGetPhotos(photosetId, extras, privacyFilter, page, perPage, MediaType.None);
    }

    public PhotosetPhotoCollection PhotosetsGetPhotos(string photosetId, PhotoSearchExtras extras, PrivacyFilter privacyFilter, int page, int perPage, MediaType media)
    {
      Dictionary<string, string> parameters = new Dictionary<string, string>();
      parameters.Add("method", "flickr.photosets.getPhotos");
      parameters.Add("photoset_id", photosetId);
      if (extras != PhotoSearchExtras.None)
        parameters.Add("extras", UtilityMethods.ExtrasToString(extras));
      if (privacyFilter != PrivacyFilter.None)
        parameters.Add("privacy_filter", privacyFilter.ToString("d"));
      if (page > 0)
        parameters.Add("page", page.ToString((IFormatProvider) NumberFormatInfo.InvariantInfo));
      if (perPage > 0)
        parameters.Add("per_page", perPage.ToString((IFormatProvider) NumberFormatInfo.InvariantInfo));
      if (media != MediaType.None)
        parameters.Add("media", media == MediaType.All ? "all" : (media == MediaType.Photos ? "photos" : (media == MediaType.Videos ? "videos" : string.Empty)));
      return this.GetResponseCache<PhotosetPhotoCollection>(parameters);
    }

    public void PhotosetsOrderSets(string[] photosetIds)
    {
      this.PhotosetsOrderSets(string.Join(",", photosetIds));
    }

    public void PhotosetsOrderSets(string photosetIds)
    {
      this.GetResponseNoCache<NoResponse>(new Dictionary<string, string>()
      {
        {
          "method",
          "flickr.photosets.orderSets"
        },
        {
          "photoset_ids",
          photosetIds
        }
      });
    }

    public void PhotosetsRemovePhoto(string photosetId, string photoId)
    {
      this.CheckRequiresAuthentication();
      this.GetResponseNoCache<NoResponse>(new Dictionary<string, string>()
      {
        {
          "method",
          "flickr.photosets.removePhoto"
        },
        {
          "photoset_id",
          photosetId
        },
        {
          "photo_id",
          photoId
        }
      });
    }

    public void PhotosetsRemovePhotos(string photosetId, string[] photoIds)
    {
      this.GetResponseNoCache<NoResponse>(new Dictionary<string, string>()
      {
        {
          "method",
          "flickr.photosets.removePhotos"
        },
        {
          "photoset_id",
          photosetId
        },
        {
          "photo_ids",
          string.Join(",", photoIds)
        }
      });
    }

    public void PhotosetsReorderPhotos(string photosetId, string[] photoIds)
    {
      this.GetResponseNoCache<NoResponse>(new Dictionary<string, string>()
      {
        {
          "method",
          "flickr.photosets.reorderPhotos"
        },
        {
          "photoset_id",
          photosetId
        },
        {
          "photo_ids",
          string.Join(",", photoIds)
        }
      });
    }

    public void PhotosetsSetPrimaryPhoto(string photosetId, string photoId)
    {
      this.GetResponseNoCache<NoResponse>(new Dictionary<string, string>()
      {
        {
          "method",
          "flickr.photosets.setPrimaryPhoto"
        },
        {
          "photoset_id",
          photosetId
        },
        {
          "photo_id",
          photoId
        }
      });
    }

    public PhotosetCommentCollection PhotosetsCommentsGetList(string photosetId)
    {
      return this.GetResponseCache<PhotosetCommentCollection>(new Dictionary<string, string>()
      {
        {
          "method",
          "flickr.photosets.comments.getList"
        },
        {
          "photoset_id",
          photosetId
        }
      });
    }

    public string PhotosetsCommentsAddComment(string photosetId, string commentText)
    {
      XmlNode xmlNode = this.GetResponseNoCache<UnknownResponse>(new Dictionary<string, string>()
      {
        {
          "method",
          "flickr.photosets.comments.addComment"
        },
        {
          "photoset_id",
          photosetId
        },
        {
          "comment_text",
          commentText
        }
      }).GetXmlDocument().SelectSingleNode("*/@id");
      if (xmlNode != null)
        return xmlNode.Value;
      else
        return (string) null;
    }

    public void PhotosetsCommentsDeleteComment(string commentId)
    {
      this.GetResponseNoCache<NoResponse>(new Dictionary<string, string>()
      {
        {
          "method",
          "flickr.photosets.comments.deleteComment"
        },
        {
          "comment_id",
          commentId
        }
      });
    }

    public void PhotosetsCommentsEditComment(string commentId, string commentText)
    {
      this.GetResponseNoCache<NoResponse>(new Dictionary<string, string>()
      {
        {
          "method",
          "flickr.photosets.comments.editComment"
        },
        {
          "comment_id",
          commentId
        },
        {
          "comment_text",
          commentText
        }
      });
    }

    public void PhotosGeoBatchCorrectLocation(double latitude, double longitude, GeoAccuracy accuracy, string placeId, string woeId)
    {
      this.CheckRequiresAuthentication();
      if (string.IsNullOrEmpty(placeId) && string.IsNullOrEmpty(woeId))
        throw new ArgumentException("You must pass either a placeId or a woeId");
      this.GetResponseNoCache<NoResponse>(new Dictionary<string, string>()
      {
        {
          "method",
          "flickr.photos.geo.batchCorrectLocation"
        },
        {
          "lat",
          latitude.ToString((IFormatProvider) NumberFormatInfo.InvariantInfo)
        },
        {
          "lon",
          longitude.ToString((IFormatProvider) NumberFormatInfo.InvariantInfo)
        },
        {
          "accuracy",
          accuracy.ToString("D")
        },
        {
          "place_id",
          placeId
        },
        {
          "woe_id",
          woeId
        }
      });
    }

    public void PhotosGeoCorrectLocation(string photoId, string placeId, string woeId)
    {
      this.CheckRequiresAuthentication();
      if (string.IsNullOrEmpty(placeId) && string.IsNullOrEmpty(woeId))
        throw new ArgumentException("You must supply at least one of placeId and woeId parameters.");
      Dictionary<string, string> parameters = new Dictionary<string, string>();
      parameters.Add("method", "flickr.photos.geo.correctLocation");
      parameters.Add("photo_id", photoId);
      if (!string.IsNullOrEmpty(placeId))
        parameters.Add("place_id", placeId);
      if (!string.IsNullOrEmpty(woeId))
        parameters.Add("woe_id", woeId);
      this.GetResponseNoCache<NoResponse>(parameters);
    }

    public PlaceInfo PhotosGeoGetLocation(string photoId)
    {
      Dictionary<string, string> parameters = new Dictionary<string, string>();
      parameters.Add("method", "flickr.photos.geo.getLocation");
      parameters.Add("photo_id", photoId);
      try
      {
        return this.GetResponseCache<PhotoInfo>(parameters).Location;
      }
      catch (FlickrApiException ex)
      {
        if (ex.Code == 2)
          return (PlaceInfo) null;
        throw;
      }
    }

    public void PhotosGeoSetContext(string photoId, GeoContext context)
    {
      this.GetResponseNoCache<NoResponse>(new Dictionary<string, string>()
      {
        {
          "method",
          "flickr.photos.geo.setContext"
        },
        {
          "photo_id",
          photoId
        },
        {
          "context",
          context.ToString("D")
        }
      });
    }

    public void PhotosGeoSetLocation(string photoId, double latitude, double longitude)
    {
      this.PhotosGeoSetLocation(photoId, latitude, longitude, GeoAccuracy.None, GeoContext.NotDefined);
    }

    public void PhotosGeoSetLocation(string photoId, double latitude, double longitude, GeoAccuracy accuracy)
    {
      this.PhotosGeoSetLocation(photoId, latitude, longitude, accuracy, GeoContext.NotDefined);
    }

    public void PhotosGeoSetLocation(string photoId, double latitude, double longitude, GeoAccuracy accuracy, GeoContext context)
    {
      Dictionary<string, string> parameters = new Dictionary<string, string>();
      parameters.Add("method", "flickr.photos.geo.setLocation");
      parameters.Add("photo_id", photoId);
      parameters.Add("lat", latitude.ToString((IFormatProvider) NumberFormatInfo.InvariantInfo));
      parameters.Add("lon", longitude.ToString((IFormatProvider) NumberFormatInfo.InvariantInfo));
      if (accuracy != GeoAccuracy.None)
        parameters.Add("accuracy", accuracy.ToString("D"));
      if (context != GeoContext.NotDefined)
        parameters.Add("context", context.ToString("D"));
      this.GetResponseNoCache<NoResponse>(parameters);
    }

    public PhotoCollection PhotosGeoPhotosForLocation(double latitude, double longitude, GeoAccuracy accuracy, PhotoSearchExtras extras, int perPage, int page)
    {
      this.CheckRequiresAuthentication();
      Dictionary<string, string> parameters = new Dictionary<string, string>();
      parameters.Add("method", "flickr.photos.geo.photosForLocation");
      parameters.Add("lat", latitude.ToString((IFormatProvider) NumberFormatInfo.InvariantInfo));
      parameters.Add("lon", longitude.ToString((IFormatProvider) NumberFormatInfo.InvariantInfo));
      parameters.Add("accuracy", accuracy.ToString("D"));
      if (extras != PhotoSearchExtras.None)
        parameters.Add("extras", UtilityMethods.ExtrasToString(extras));
      if (perPage > 0)
        parameters.Add("per_page", perPage.ToString((IFormatProvider) NumberFormatInfo.InvariantInfo));
      if (page > 0)
        parameters.Add("page", page.ToString((IFormatProvider) NumberFormatInfo.InvariantInfo));
      return this.GetResponseNoCache<PhotoCollection>(parameters);
    }

    public void PhotosGeoRemoveLocation(string photoId)
    {
      this.GetResponseNoCache<NoResponse>(new Dictionary<string, string>()
      {
        {
          "method",
          "flickr.photos.geo.removeLocation"
        },
        {
          "photo_id",
          photoId
        }
      });
    }

    public PhotoCollection PhotosGetWithoutGeoData()
    {
      return this.PhotosGetWithoutGeoData(new PartialSearchOptions());
    }

    public PhotoCollection PhotosGetWithoutGeoData(PartialSearchOptions options)
    {
      Dictionary<string, string> parameters = new Dictionary<string, string>();
      parameters.Add("method", "flickr.photos.getWithoutGeoData");
      UtilityMethods.PartialOptionsIntoArray(options, parameters);
      return this.GetResponseCache<PhotoCollection>(parameters);
    }

    [Obsolete("Use the PartialSearchOptions instead")]
    public PhotoCollection PhotosGetWithoutGeoData(PhotoSearchOptions options)
    {
      return this.PhotosGetWithoutGeoData(new PartialSearchOptions(options));
    }

    public PhotoCollection PhotosGetWithGeoData()
    {
      return this.PhotosGetWithGeoData(new PartialSearchOptions());
    }

    [Obsolete("Use the new PartialSearchOptions instead")]
    public PhotoCollection PhotosGetWithGeoData(PhotoSearchOptions options)
    {
      return this.PhotosGetWithGeoData(new PartialSearchOptions(options));
    }

    public PhotoCollection PhotosGetWithGeoData(PartialSearchOptions options)
    {
      Dictionary<string, string> parameters = new Dictionary<string, string>();
      parameters.Add("method", "flickr.photos.getWithGeoData");
      UtilityMethods.PartialOptionsIntoArray(options, parameters);
      return this.GetResponseCache<PhotoCollection>(parameters);
    }

    public GeoPermissions PhotosGeoGetPerms(string photoId)
    {
      return this.GetResponseCache<GeoPermissions>(new Dictionary<string, string>()
      {
        {
          "method",
          "flickr.photos.geo.getPerms"
        },
        {
          "photo_id",
          photoId
        }
      });
    }

    public void PhotosGeoSetPerms(string photoId, bool isPublic, bool isContact, bool isFamily, bool isFriend)
    {
      this.GetResponseNoCache<NoResponse>(new Dictionary<string, string>()
      {
        {
          "method",
          "flickr.photos.geo.setPerms"
        },
        {
          "photo_id",
          photoId
        },
        {
          "is_public",
          isPublic ? "1" : "0"
        },
        {
          "is_contact",
          isContact ? "1" : "0"
        },
        {
          "is_friend",
          isFriend ? "1" : "0"
        },
        {
          "is_family",
          isFamily ? "1" : "0"
        }
      });
    }

    public void PhotosTransformRotate(string photoId, int degrees)
    {
      if (photoId == null)
        throw new ArgumentNullException("photoId");
      if (degrees != 90 && degrees != 180 && degrees != 270)
        throw new ArgumentException("Must be 90, 180 or 270", "degrees");
      this.GetResponseNoCache<NoResponse>(new Dictionary<string, string>()
      {
        {
          "method",
          "flickr.photos.transform.rotate"
        },
        {
          "photo_id",
          photoId
        },
        {
          "degrees",
          degrees.ToString((IFormatProvider) NumberFormatInfo.InvariantInfo)
        }
      });
    }

    public TicketCollection PhotosUploadCheckTickets(string[] tickets)
    {
      return this.GetResponseNoCache<TicketCollection>(new Dictionary<string, string>()
      {
        {
          "method",
          "flickr.photos.upload.checkTickets"
        },
        {
          "tickets",
          string.Join(",", tickets)
        }
      });
    }

    public ContentType PrefsGetContentType()
    {
      this.CheckRequiresAuthentication();
      XmlNode xmlNode = this.GetResponseCache<UnknownResponse>(new Dictionary<string, string>()
      {
        {
          "method",
          "flickr.prefs.getContentType"
        }
      }).GetXmlDocument().SelectSingleNode("*/@content_type");
      if (xmlNode == null)
        throw new ParsingException("Unable to find content type preference in returned XML.");
      else
        return (ContentType) int.Parse(xmlNode.Value, (IFormatProvider) NumberFormatInfo.InvariantInfo);
    }

    public UserGeoPermissions PrefsGetGeoPerms()
    {
      this.CheckRequiresAuthentication();
      return this.GetResponseCache<UserGeoPermissions>(new Dictionary<string, string>()
      {
        {
          "method",
          "flickr.prefs.getGeoPerms"
        }
      });
    }

    public HiddenFromSearch PrefsGetHidden()
    {
      this.CheckRequiresAuthentication();
      XmlNode xmlNode = this.GetResponseCache<UnknownResponse>(new Dictionary<string, string>()
      {
        {
          "method",
          "flickr.prefs.getHidden"
        }
      }).GetXmlDocument().SelectSingleNode("*/@hidden");
      if (xmlNode == null)
        throw new ParsingException("Unable to find hidden preference in returned XML.");
      else
        return (HiddenFromSearch) int.Parse(xmlNode.Value, (IFormatProvider) NumberFormatInfo.InvariantInfo);
    }

    public PrivacyFilter PrefsGetPrivacy()
    {
      this.CheckRequiresAuthentication();
      XmlNode xmlNode = this.GetResponseCache<UnknownResponse>(new Dictionary<string, string>()
      {
        {
          "method",
          "flickr.prefs.getPrivacy"
        }
      }).GetXmlDocument().SelectSingleNode("*/@privacy");
      if (xmlNode == null)
        throw new ParsingException("Unable to find safety level in returned XML.");
      else
        return (PrivacyFilter) int.Parse(xmlNode.Value, (IFormatProvider) NumberFormatInfo.InvariantInfo);
    }

    public SafetyLevel PrefsGetSafetyLevel()
    {
      this.CheckRequiresAuthentication();
      XmlNode xmlNode = this.GetResponseCache<UnknownResponse>(new Dictionary<string, string>()
      {
        {
          "method",
          "flickr.prefs.getSafetyLevel"
        }
      }).GetXmlDocument().SelectSingleNode("*/@safety_level");
      if (xmlNode == null)
        throw new ParsingException("Unable to find safety level in returned XML.");
      else
        return (SafetyLevel) int.Parse(xmlNode.Value, (IFormatProvider) NumberFormatInfo.InvariantInfo);
    }

    public Collection<PhotoInfoTag> TagsGetListPhoto(string photoId)
    {
      return this.GetResponseCache<PhotoInfo>(new Dictionary<string, string>()
      {
        {
          "method",
          "flickr.tags.getListPhoto"
        },
        {
          "api_key",
          this.apiKey
        },
        {
          "photo_id",
          photoId
        }
      }).Tags;
    }

    public TagCollection TagsGetListUser()
    {
      return this.TagsGetListUser((string) null);
    }

    public TagCollection TagsGetListUser(string userId)
    {
      Dictionary<string, string> parameters = new Dictionary<string, string>();
      parameters.Add("method", "flickr.tags.getListUser");
      if (userId != null && userId.Length > 0)
        parameters.Add("user_id", userId);
      return this.GetResponseCache<TagCollection>(parameters);
    }

    public TagCollection TagsGetListUserPopular()
    {
      this.CheckRequiresAuthentication();
      return this.TagsGetListUserPopular((string) null, 0);
    }

    public TagCollection TagsGetListUserPopular(int count)
    {
      this.CheckRequiresAuthentication();
      return this.TagsGetListUserPopular((string) null, count);
    }

    public TagCollection TagsGetListUserPopular(string userId)
    {
      return this.TagsGetListUserPopular(userId, 0);
    }

    public TagCollection TagsGetListUserPopular(string userId, int count)
    {
      Dictionary<string, string> parameters = new Dictionary<string, string>();
      parameters.Add("method", "flickr.tags.getListUserPopular");
      if (userId != null)
        parameters.Add("user_id", userId);
      if (count > 0)
        parameters.Add("count", count.ToString((IFormatProvider) NumberFormatInfo.InvariantInfo));
      return this.GetResponseCache<TagCollection>(parameters);
    }

    public RawTagCollection TagsGetListUserRaw()
    {
      return this.TagsGetListUserRaw((string) null);
    }

    public RawTagCollection TagsGetListUserRaw(string tag)
    {
      this.CheckRequiresAuthentication();
      Dictionary<string, string> parameters = new Dictionary<string, string>();
      parameters.Add("method", "flickr.tags.getListUserRaw");
      if (tag != null && tag.Length > 0)
        parameters.Add("tag", tag);
      return this.GetResponseCache<RawTagCollection>(parameters);
    }

    public TagCollection TagsGetMostFrequentlyUsed()
    {
      this.CheckRequiresAuthentication();
      return this.GetResponseCache<TagCollection>(new Dictionary<string, string>()
      {
        {
          "method",
          "flickr.tags.getMostFrequentlyUsed"
        }
      });
    }

    public TagCollection TagsGetRelated(string tag)
    {
      return this.GetResponseCache<TagCollection>(new Dictionary<string, string>()
      {
        {
          "method",
          "flickr.tags.getRelated"
        },
        {
          "api_key",
          this.apiKey
        },
        {
          "tag",
          tag
        }
      });
    }

    public ClusterCollection TagsGetClusters(string tag)
    {
      return this.GetResponseCache<ClusterCollection>(new Dictionary<string, string>()
      {
        {
          "method",
          "flickr.tags.getClusters"
        },
        {
          "tag",
          tag
        }
      });
    }

    public PhotoCollection TagsGetClusterPhotos(Cluster cluster)
    {
      return this.TagsGetClusterPhotos(cluster.SourceTag, cluster.ClusterId, PhotoSearchExtras.None);
    }

    public PhotoCollection TagsGetClusterPhotos(Cluster cluster, PhotoSearchExtras extras)
    {
      return this.TagsGetClusterPhotos(cluster.SourceTag, cluster.ClusterId, extras);
    }

    public PhotoCollection TagsGetClusterPhotos(string tag, string clusterId, PhotoSearchExtras extras)
    {
      Dictionary<string, string> parameters = new Dictionary<string, string>();
      parameters.Add("method", "flickr.tags.getClusterPhotos");
      parameters.Add("tag", tag);
      parameters.Add("cluster_id", clusterId);
      if (extras != PhotoSearchExtras.None)
        parameters.Add("extras", UtilityMethods.ExtrasToString(extras));
      return this.GetResponseCache<PhotoCollection>(parameters);
    }

    public HotTagCollection TagsGetHotList()
    {
      return this.TagsGetHotList((string) null, 0);
    }

    public HotTagCollection TagsGetHotList(string period, int count)
    {
      if (!string.IsNullOrEmpty(period) && period != "day" && period != "week")
        throw new ArgumentException("Period must be either 'day' or 'week'.", "period");
      Dictionary<string, string> parameters = new Dictionary<string, string>();
      parameters.Add("method", "flickr.tags.getHotList");
      if (!string.IsNullOrEmpty(period))
        parameters.Add("period", period);
      if (count > 0)
        parameters.Add("count", count.ToString((IFormatProvider) NumberFormatInfo.InvariantInfo));
      return this.GetResponseCache<HotTagCollection>(parameters);
    }

    public string UrlsGetGroup(string groupId)
    {
      XmlNode xmlNode = this.GetResponseCache<UnknownResponse>(new Dictionary<string, string>()
      {
        {
          "method",
          "flickr.urls.getGroup"
        },
        {
          "group_id",
          groupId
        }
      }).GetXmlDocument().SelectSingleNode("*/@url");
      if (xmlNode != null)
        return xmlNode.Value.Replace("http://", "https://");
      else
        return (string) null;
    }

    public string UrlsGetUserPhotos()
    {
      this.CheckRequiresAuthentication();
      return this.UrlsGetUserPhotos((string) null);
    }

    public string UrlsGetUserPhotos(string userId)
    {
      Dictionary<string, string> parameters = new Dictionary<string, string>();
      parameters.Add("method", "flickr.urls.getUserPhotos");
      if (userId != null && userId.Length > 0)
        parameters.Add("user_id", userId);
      XmlNode xmlNode = this.GetResponseCache<UnknownResponse>(parameters).GetXmlDocument().SelectSingleNode("*/@url");
      if (xmlNode != null)
        return xmlNode.Value.Replace("http://", "https://");
      else
        return (string) null;
    }

    public string UrlsGetUserProfile()
    {
      this.CheckRequiresAuthentication();
      return this.UrlsGetUserProfile((string) null);
    }

    public string UrlsGetUserProfile(string userId)
    {
      Dictionary<string, string> parameters = new Dictionary<string, string>();
      parameters.Add("method", "flickr.urls.getUserProfile");
      if (userId != null && userId.Length > 0)
        parameters.Add("user_id", userId);
      XmlNode xmlNode = this.GetResponseCache<UnknownResponse>(parameters).GetXmlDocument().SelectSingleNode("*/@url");
      if (xmlNode != null)
        return xmlNode.Value.Replace("http://", "https://");
      else
        return (string) null;
    }

    public Gallery UrlsLookupGallery(string url)
    {
      return this.GetResponseCache<Gallery>(new Dictionary<string, string>()
      {
        {
          "method",
          "flickr.urls.lookupGallery"
        },
        {
          "api_key",
          this.apiKey
        },
        {
          "url",
          url
        }
      });
    }

    public string UrlsLookupGroup(string urlToFind)
    {
      XmlNode xmlNode = this.GetResponseCache<UnknownResponse>(new Dictionary<string, string>()
      {
        {
          "method",
          "flickr.urls.lookupGroup"
        },
        {
          "api_key",
          this.apiKey
        },
        {
          "url",
          urlToFind
        }
      }).GetXmlDocument().SelectSingleNode("*/@id");
      if (xmlNode != null)
        return xmlNode.Value.Replace("http://", "https://");
      else
        return (string) null;
    }

    public FoundUser UrlsLookupUser(string urlToFind)
    {
      return this.GetResponseCache<FoundUser>(new Dictionary<string, string>()
      {
        {
          "method",
          "flickr.urls.lookupUser"
        },
        {
          "api_key",
          this.apiKey
        },
        {
          "url",
          urlToFind
        }
      });
    }

    public void PhotosSuggestionsApproveSuggestion(string suggestionId)
    {
      this.CheckRequiresAuthentication();
      if (string.IsNullOrEmpty(suggestionId))
        throw new ArgumentNullException("suggestionId");
      this.GetResponseNoCache<NoResponse>(new Dictionary<string, string>()
      {
        {
          "method",
          "flickr.photos.suggestions.approveSuggestion"
        },
        {
          "suggestion_id",
          suggestionId
        }
      });
    }

    public SuggestionCollection PhotosSuggestionsGetList(string photoId, SuggestionStatus status)
    {
      this.CheckRequiresAuthentication();
      return this.GetResponseCache<SuggestionCollection>(new Dictionary<string, string>()
      {
        {
          "method",
          "flickr.photos.suggestions.getList"
        },
        {
          "photo_id",
          photoId
        },
        {
          "status_id",
          status.ToString("d")
        }
      });
    }

    public void PhotosSuggestionsRejectSuggestion(string suggestionId)
    {
      this.CheckRequiresAuthentication();
      if (string.IsNullOrEmpty(suggestionId))
        throw new ArgumentNullException("suggestionId");
      this.GetResponseNoCache<NoResponse>(new Dictionary<string, string>()
      {
        {
          "method",
          "flickr.photos.suggestions.rejectSuggestion"
        },
        {
          "suggestion_id",
          suggestionId
        }
      });
    }

    public void PhotosSuggestionsRemoveSuggestion(string suggestionId)
    {
      this.CheckRequiresAuthentication();
      if (string.IsNullOrEmpty(suggestionId))
        throw new ArgumentNullException("suggestionId");
      this.GetResponseNoCache<NoResponse>(new Dictionary<string, string>()
      {
        {
          "method",
          "flickr.photos.suggestions.removeSuggestion"
        },
        {
          "suggestion_id",
          suggestionId
        }
      });
    }

    public void PhotosSuggestionsSuggestLocation(string photoId, double latitude, double longitude, GeoAccuracy accuracy, string woeId, string placeId, string note)
    {
      this.CheckRequiresAuthentication();
      this.GetResponseNoCache<NoResponse>(new Dictionary<string, string>()
      {
        {
          "method",
          "flickr.photos.suggestions.suggestLocation"
        },
        {
          "photo_id",
          photoId
        },
        {
          "lat",
          latitude.ToString((IFormatProvider) NumberFormatInfo.InvariantInfo)
        },
        {
          "lon",
          longitude.ToString((IFormatProvider) NumberFormatInfo.InvariantInfo)
        },
        {
          "accuracy",
          accuracy.ToString("D")
        },
        {
          "place_id",
          placeId
        },
        {
          "woe_id",
          woeId
        },
        {
          "note",
          note
        }
      });
    }

    public static void FlushCache()
    {
      Cache.FlushCache();
    }

    public static void FlushCache(string url)
    {
      Flickr.FlushCache(new Uri(url));
    }

    public static void FlushCache(Uri url)
    {
      Cache.FlushCache(url);
    }

    private void CheckApiKey()
    {
      if (string.IsNullOrEmpty(this.ApiKey))
        throw new ApiKeyRequiredException();
    }

    private void CheckSigned()
    {
      this.CheckApiKey();
      if (string.IsNullOrEmpty(this.ApiSecret))
        throw new SignatureRequiredException();
    }

    private void CheckRequiresAuthentication()
    {
      this.CheckApiKey();
      if (!string.IsNullOrEmpty(this.OAuthAccessToken) && !string.IsNullOrEmpty(this.OAuthAccessTokenSecret))
        return;
      if (string.IsNullOrEmpty(this.ApiSecret))
        throw new SignatureRequiredException();
      if (string.IsNullOrEmpty(this.AuthToken) || string.IsNullOrEmpty(this.OAuthAccessToken) || string.IsNullOrEmpty(this.OAuthAccessTokenSecret))
        throw new AuthenticationRequiredException();
    }

    public Uri CalculateUri(Dictionary<string, string> parameters, bool includeSignature)
    {
      if (includeSignature)
      {
        string str = this.CalculateAuthSignature(parameters);
        parameters.Add("api_sig", str);
      }
      StringBuilder stringBuilder = new StringBuilder();
      stringBuilder.Append("?");
      foreach (KeyValuePair<string, string> keyValuePair in parameters)
        stringBuilder.AppendFormat((IFormatProvider) CultureInfo.InvariantCulture, "{0}={1}&", new object[2]
        {
          (object) keyValuePair.Key,
          (object) Uri.EscapeDataString(keyValuePair.Value ?? "")
        });
      return new Uri(this.BaseUri, new Uri(((object) stringBuilder).ToString(), UriKind.Relative));
    }

    private string CalculateAuthSignature(Dictionary<string, string> parameters)
    {
      SortedList<string, string> sortedList = new SortedList<string, string>();
      foreach (KeyValuePair<string, string> keyValuePair in parameters)
        sortedList.Add(keyValuePair.Key, keyValuePair.Value);
      StringBuilder stringBuilder = new StringBuilder(this.ApiSecret);
      foreach (KeyValuePair<string, string> keyValuePair in sortedList)
      {
        stringBuilder.Append(keyValuePair.Key);
        stringBuilder.Append(keyValuePair.Value);
      }
      return UtilityMethods.MD5Hash(((object) stringBuilder).ToString());
    }

    private static Stream ConvertNonSeekableStreamToByteArray(Stream nonSeekableStream)
    {
      if (nonSeekableStream.CanSeek)
      {
        nonSeekableStream.Position = 0L;
        return nonSeekableStream;
      }
      else
      {
        MemoryStream memoryStream = new MemoryStream();
        byte[] buffer = new byte[1024];
        int count;
        while ((count = nonSeekableStream.Read(buffer, 0, buffer.Length)) > 0)
          memoryStream.Write(buffer, 0, count);
        memoryStream.Position = 0L;
        return (Stream) memoryStream;
      }
    }

    private Flickr.StreamCollection CreateUploadData(Stream imageStream, string fileName, Dictionary<string, string> parameters, string boundary)
    {
      bool flag = parameters.ContainsKey("oauth_consumer_key");
      string[] array = new string[parameters.Keys.Count];
      parameters.Keys.CopyTo(array, 0);
      Array.Sort<string>(array);
      StringBuilder stringBuilder = new StringBuilder(this.sharedSecret, 2048);
      MemoryStream memoryStream1 = new MemoryStream();
      StreamWriter streamWriter1 = new StreamWriter((Stream) memoryStream1, (Encoding) new UTF8Encoding(false));
      foreach (string index in array)
      {
        if (!index.StartsWith("oauth"))
        {
          stringBuilder.Append(index);
          stringBuilder.Append(parameters[index]);
          streamWriter1.Write("--" + boundary + "\r\n");
          streamWriter1.Write("Content-Disposition: form-data; name=\"" + index + "\"\r\n");
          streamWriter1.Write("\r\n");
          streamWriter1.Write(parameters[index] + "\r\n");
        }
      }
      if (!flag)
      {
        streamWriter1.Write("--" + boundary + "\r\n");
        streamWriter1.Write("Content-Disposition: form-data; name=\"api_sig\"\r\n");
        streamWriter1.Write("\r\n");
        streamWriter1.Write(UtilityMethods.MD5Hash(((object) stringBuilder).ToString()) + "\r\n");
      }
      streamWriter1.Write("--" + boundary + "\r\n");
      streamWriter1.Write("Content-Disposition: form-data; name=\"photo\"; filename=\"" + Path.GetFileName(fileName) + "\"\r\n");
      streamWriter1.Write("Content-Type: image/jpeg\r\n");
      streamWriter1.Write("\r\n");
      ((TextWriter) streamWriter1).Flush();
      Stream stream = Flickr.ConvertNonSeekableStreamToByteArray(imageStream);
      MemoryStream memoryStream2 = new MemoryStream();
      StreamWriter streamWriter2 = new StreamWriter((Stream) memoryStream2, (Encoding) new UTF8Encoding(false));
      streamWriter2.Write("\r\n--" + boundary + "--\r\n");
      ((TextWriter) streamWriter2).Flush();
      return new Flickr.StreamCollection((IEnumerable<Stream>) new Stream[3]
      {
        (Stream) memoryStream1,
        stream,
        (Stream) memoryStream2
      });
    }

    [Obsolete("Use OAuth now.")]
    public string AuthGetFrob()
    {
      this.CheckSigned();
      return this.GetResponseNoCache<UnknownResponse>(new Dictionary<string, string>()
      {
        {
          "method",
          "flickr.auth.getFrob"
        }
      }).GetXmlDocument().SelectSingleNode("frob/text()").Value;
    }

    [Obsolete("Use OAuth now.")]
    public Auth AuthGetToken(string frob)
    {
      this.CheckSigned();
      Auth responseNoCache = this.GetResponseNoCache<Auth>(new Dictionary<string, string>()
      {
        {
          "method",
          "flickr.auth.getToken"
        },
        {
          "frob",
          frob
        }
      });
      this.AuthToken = responseNoCache.Token;
      return responseNoCache;
    }

    [Obsolete("Use OAuth now.")]
    public Auth AuthGetFullToken(string miniToken)
    {
      this.CheckSigned();
      Auth responseNoCache = this.GetResponseNoCache<Auth>(new Dictionary<string, string>()
      {
        {
          "method",
          "flickr.auth.getFullToken"
        },
        {
          "mini_token",
          miniToken.Replace("-", string.Empty)
        }
      });
      this.AuthToken = responseNoCache.Token;
      return responseNoCache;
    }

    public Auth AuthCheckToken()
    {
      this.CheckRequiresAuthentication();
      return this.AuthCheckToken(this.AuthToken);
    }

    public Auth AuthCheckToken(string token)
    {
      this.CheckSigned();
      return this.GetResponseNoCache<Auth>(new Dictionary<string, string>()
      {
        {
          "method",
          "flickr.auth.checkToken"
        },
        {
          "auth_token",
          token
        }
      });
    }

    public OAuthAccessToken AuthOAuthGetAccessToken()
    {
      this.CheckRequiresAuthentication();
      OAuthAccessToken responseNoCache = this.GetResponseNoCache<OAuthAccessToken>(new Dictionary<string, string>()
      {
        {
          "method",
          "flickr.auth.oauth.getAccessToken"
        }
      });
      this.OAuthAccessToken = responseNoCache.Token;
      this.OAuthAccessTokenSecret = responseNoCache.TokenSecret;
      this.AuthToken = (string) null;
      return responseNoCache;
    }

    public Auth AuthOAuthCheckToken()
    {
      this.CheckRequiresAuthentication();
      return this.GetResponseNoCache<Auth>(new Dictionary<string, string>()
      {
        {
          "method",
          "flickr.auth.oauth.checkToken"
        }
      });
    }

    public CollectionInfo CollectionsGetInfo(string collectionId)
    {
      this.CheckRequiresAuthentication();
      return this.GetResponseCache<CollectionInfo>(new Dictionary<string, string>()
      {
        {
          "method",
          "flickr.collections.getInfo"
        },
        {
          "collection_id",
          collectionId
        }
      });
    }

    public CollectionCollection CollectionsGetTree()
    {
      return this.CollectionsGetTree((string) null, (string) null);
    }

    public CollectionCollection CollectionsGetTree(string collectionId, string userId)
    {
      if (string.IsNullOrEmpty(userId))
        this.CheckRequiresAuthentication();
      Dictionary<string, string> parameters = new Dictionary<string, string>();
      parameters.Add("method", "flickr.collections.getTree");
      if (collectionId != null)
        parameters.Add("collection_id", collectionId);
      if (userId != null)
        parameters.Add("user_id", userId);
      return this.GetResponseCache<CollectionCollection>(parameters);
    }

    public void CollectionsEditMeta(string collectionId, string title, string description)
    {
      this.CheckRequiresAuthentication();
      this.GetResponseCache<NoResponse>(new Dictionary<string, string>()
      {
        {
          "method",
          "flickr.collections.editMeta"
        },
        {
          "collection_id",
          collectionId
        },
        {
          "title",
          title
        },
        {
          "description",
          description
        }
      });
    }

    [Obsolete("Experimental and unsupported by Flickr at this time.")]
    public void CollectionsEditSets(string collectionId, IList<string> photosetIds)
    {
      this.CheckRequiresAuthentication();
      string str = "";
      for (int index = 0; index < photosetIds.Count; ++index)
      {
        str = str + photosetIds[index];
        if (index < photosetIds.Count - 1)
          str = str + ",";
      }
      this.GetResponseNoCache<NoResponse>(new Dictionary<string, string>()
      {
        {
          "method",
          "flickr.collections.editSets"
        },
        {
          "collection_id",
          collectionId
        },
        {
          "photoset_ids",
          str
        },
        {
          "do_remove",
          "0"
        }
      });
    }

    [Obsolete("Experimental and unsupported by Flickr at this time.")]
    public void CollectionsRemoveSet(string collectionId, string photosetId)
    {
      this.CheckRequiresAuthentication();
      this.GetResponseNoCache<NoResponse>(new Dictionary<string, string>()
      {
        {
          "method",
          "flickr.collections.removeSet"
        },
        {
          "collection_id",
          collectionId
        },
        {
          "photoset_id",
          photosetId
        }
      });
    }

    private T GetResponseNoCache<T>(Dictionary<string, string> parameters) where T : IFlickrParsable, new()
    {
      return this.GetResponse<T>(parameters, TimeSpan.MinValue);
    }

    private T GetResponseCache<T>(Dictionary<string, string> parameters) where T : IFlickrParsable, new()
    {
      return this.GetResponse<T>(parameters, Cache.CacheTimeout);
    }

    private T GetResponse<T>(Dictionary<string, string> parameters, TimeSpan cacheTimeout) where T : IFlickrParsable, new()
    {
      this.CheckApiKey();
      parameters["api_key"] = this.ApiKey;
      string str1 = parameters["method"];
      if (str1.StartsWith("flickr.auth") && !str1.EndsWith("oauth.checkToken"))
      {
        if (!string.IsNullOrEmpty(this.AuthToken))
          parameters["auth_token"] = this.AuthToken;
      }
      else if (!string.IsNullOrEmpty(this.OAuthAccessToken) || string.IsNullOrEmpty(this.AuthToken))
      {
        parameters.Remove("api_key");
        this.OAuthGetBasicParameters(parameters);
        if (!string.IsNullOrEmpty(this.OAuthAccessToken))
          parameters["oauth_token"] = this.OAuthAccessToken;
      }
      else
        parameters["auth_token"] = this.AuthToken;
      Uri uri = this.CalculateUri(parameters, !string.IsNullOrEmpty(this.sharedSecret));
      this.lastRequest = uri.AbsoluteUri;
      string str2;
      if (this.InstanceCacheDisabled)
      {
        str2 = FlickrResponder.GetDataResponse(this, this.BaseUri.AbsoluteUri, parameters);
      }
      else
      {
        string absoluteUri = uri.AbsoluteUri;
        ResponseCacheItem responseCacheItem1 = (ResponseCacheItem) Cache.Responses.Get(absoluteUri, cacheTimeout, true);
        if (responseCacheItem1 != null)
        {
          str2 = responseCacheItem1.Response;
        }
        else
        {
          str2 = FlickrResponder.GetDataResponse(this, this.BaseUri.AbsoluteUri, parameters);
          ResponseCacheItem responseCacheItem2 = new ResponseCacheItem(new Uri(absoluteUri), str2, DateTime.UtcNow);
          Cache.Responses.Shrink(Math.Max(0L, Cache.CacheSizeLimit - (long) str2.Length));
          Cache.Responses[absoluteUri] = (ICacheItem) responseCacheItem2;
        }
      }
      this.lastResponse = str2;
      XmlTextReader xmlTextReader = new XmlTextReader((TextReader) new StringReader(str2))
      {
        WhitespaceHandling = WhitespaceHandling.None
      };
      if (!xmlTextReader.ReadToDescendant("rsp"))
        throw new XmlException("Unable to find response element 'rsp' in Flickr response");
      while (xmlTextReader.MoveToNextAttribute())
      {
        if (xmlTextReader.LocalName == "stat" && xmlTextReader.Value == "fail")
          throw ExceptionHandler.CreateResponseException((XmlReader) xmlTextReader);
      }
      xmlTextReader.MoveToElement();
      xmlTextReader.Read();
      T obj = new T();
      obj.Load((XmlReader) xmlTextReader);
      return obj;
    }

    public string[] PandaGetList()
    {
      UnknownResponse responseCache = this.GetResponseCache<UnknownResponse>(new Dictionary<string, string>()
      {
        {
          "method",
          "flickr.panda.getList"
        }
      });
      List<string> list = new List<string>();
      foreach (XmlNode xmlNode in responseCache.GetXmlDocument().SelectNodes("//panda/text()"))
        list.Add(xmlNode.Value);
      return list.ToArray();
    }

    public PandaPhotoCollection PandaGetPhotos(string pandaName)
    {
      return this.PandaGetPhotos(pandaName, PhotoSearchExtras.None, 0, 0);
    }

    public PandaPhotoCollection PandaGetPhotos(string pandaName, PhotoSearchExtras extras)
    {
      return this.PandaGetPhotos(pandaName, extras, 0, 0);
    }

    public PandaPhotoCollection PandaGetPhotos(string pandaName, int page, int perPage)
    {
      return this.PandaGetPhotos(pandaName, PhotoSearchExtras.None, page, perPage);
    }

    public PandaPhotoCollection PandaGetPhotos(string pandaName, PhotoSearchExtras extras, int page, int perPage)
    {
      Dictionary<string, string> parameters = new Dictionary<string, string>();
      parameters.Add("method", "flickr.panda.getPhotos");
      parameters.Add("panda_name", pandaName);
      if (extras != PhotoSearchExtras.None)
        parameters.Add("extras", UtilityMethods.ExtrasToString(extras));
      if (perPage > 0)
        parameters.Add("per_page", perPage.ToString((IFormatProvider) NumberFormatInfo.InvariantInfo));
      if (page > 0)
        parameters.Add("page", page.ToString((IFormatProvider) NumberFormatInfo.InvariantInfo));
      return this.GetResponseCache<PandaPhotoCollection>(parameters);
    }

    public FoundUser PeopleFindByEmail(string emailAddress)
    {
      return this.GetResponseCache<FoundUser>(new Dictionary<string, string>()
      {
        {
          "method",
          "flickr.people.findByEmail"
        },
        {
          "api_key",
          this.apiKey
        },
        {
          "find_email",
          emailAddress
        }
      });
    }

    public FoundUser PeopleFindByUserName(string userName)
    {
      return this.GetResponseCache<FoundUser>(new Dictionary<string, string>()
      {
        {
          "method",
          "flickr.people.findByUsername"
        },
        {
          "api_key",
          this.apiKey
        },
        {
          "username",
          userName
        }
      });
    }

    public GroupInfoCollection PeopleGetGroups(string userId)
    {
      this.CheckRequiresAuthentication();
      return this.GetResponseCache<GroupInfoCollection>(new Dictionary<string, string>()
      {
        {
          "method",
          "flickr.people.getGroups"
        },
        {
          "user_id",
          userId
        }
      });
    }

    public Person PeopleGetInfo(string userId)
    {
      return this.GetResponseCache<Person>(new Dictionary<string, string>()
      {
        {
          "method",
          "flickr.people.getInfo"
        },
        {
          "user_id",
          userId
        }
      });
    }

    public PersonLimits PeopleGetLimits()
    {
      this.CheckRequiresAuthentication();
      return this.GetResponseCache<PersonLimits>(new Dictionary<string, string>()
      {
        {
          "method",
          "flickr.people.getLimits"
        }
      });
    }

    public UserStatus PeopleGetUploadStatus()
    {
      return this.GetResponseCache<UserStatus>(new Dictionary<string, string>()
      {
        {
          "method",
          "flickr.people.getUploadStatus"
        }
      });
    }

    public GroupInfoCollection PeopleGetPublicGroups(string userId)
    {
      return this.PeopleGetPublicGroups(userId, new bool?());
    }

    public GroupInfoCollection PeopleGetPublicGroups(string userId, bool? includeInvitationOnly)
    {
      Dictionary<string, string> parameters = new Dictionary<string, string>();
      parameters.Add("method", "flickr.people.getPublicGroups");
      parameters.Add("api_key", this.apiKey);
      parameters.Add("user_id", userId);
      if (includeInvitationOnly.HasValue)
        parameters.Add("invitation_only", includeInvitationOnly.Value ? "1" : "0");
      return this.GetResponseCache<GroupInfoCollection>(parameters);
    }

    public PhotoCollection PeopleGetPublicPhotos(string userId)
    {
      return this.PeopleGetPublicPhotos(userId, 0, 0, SafetyLevel.None, PhotoSearchExtras.None);
    }

    public PhotoCollection PeopleGetPublicPhotos(string userId, int page, int perPage)
    {
      return this.PeopleGetPublicPhotos(userId, page, perPage, SafetyLevel.None, PhotoSearchExtras.None);
    }

    public PhotoCollection PeopleGetPublicPhotos(string userId, int page, int perPage, SafetyLevel safetyLevel, PhotoSearchExtras extras)
    {
      if (!this.IsAuthenticated && safetyLevel > SafetyLevel.Safe)
        throw new ArgumentException("Safety level may only be 'Safe' for unauthenticated calls", "safetyLevel");
      Dictionary<string, string> parameters = new Dictionary<string, string>();
      parameters.Add("method", "flickr.people.getPublicPhotos");
      parameters.Add("api_key", this.apiKey);
      parameters.Add("user_id", userId);
      if (perPage > 0)
        parameters.Add("per_page", perPage.ToString((IFormatProvider) NumberFormatInfo.InvariantInfo));
      if (page > 0)
        parameters.Add("page", page.ToString((IFormatProvider) NumberFormatInfo.InvariantInfo));
      if (safetyLevel != SafetyLevel.None)
        parameters.Add("safety_level", safetyLevel.ToString("D"));
      if (extras != PhotoSearchExtras.None)
        parameters.Add("extras", UtilityMethods.ExtrasToString(extras));
      return this.GetResponseCache<PhotoCollection>(parameters);
    }

    public PhotoCollection PeopleGetPhotos()
    {
      return this.PeopleGetPhotos((string) null, SafetyLevel.None, DateTime.MinValue, DateTime.MinValue, DateTime.MinValue, DateTime.MinValue, ContentTypeSearch.None, PrivacyFilter.None, PhotoSearchExtras.None, 0, 0);
    }

    public PhotoCollection PeopleGetPhotos(int page, int perPage)
    {
      return this.PeopleGetPhotos((string) null, SafetyLevel.None, DateTime.MinValue, DateTime.MinValue, DateTime.MinValue, DateTime.MinValue, ContentTypeSearch.None, PrivacyFilter.None, PhotoSearchExtras.None, page, perPage);
    }

    public PhotoCollection PeopleGetPhotos(PhotoSearchExtras extras)
    {
      return this.PeopleGetPhotos((string) null, SafetyLevel.None, DateTime.MinValue, DateTime.MinValue, DateTime.MinValue, DateTime.MinValue, ContentTypeSearch.None, PrivacyFilter.None, extras, 0, 0);
    }

    public PhotoCollection PeopleGetPhotos(PhotoSearchExtras extras, int page, int perPage)
    {
      return this.PeopleGetPhotos((string) null, SafetyLevel.None, DateTime.MinValue, DateTime.MinValue, DateTime.MinValue, DateTime.MinValue, ContentTypeSearch.None, PrivacyFilter.None, extras, page, perPage);
    }

    public PhotoCollection PeopleGetPhotos(string userId)
    {
      return this.PeopleGetPhotos(userId, SafetyLevel.None, DateTime.MinValue, DateTime.MinValue, DateTime.MinValue, DateTime.MinValue, ContentTypeSearch.None, PrivacyFilter.None, PhotoSearchExtras.None, 0, 0);
    }

    public PhotoCollection PeopleGetPhotos(string userId, int page, int perPage)
    {
      return this.PeopleGetPhotos(userId, SafetyLevel.None, DateTime.MinValue, DateTime.MinValue, DateTime.MinValue, DateTime.MinValue, ContentTypeSearch.None, PrivacyFilter.None, PhotoSearchExtras.None, page, perPage);
    }

    public PhotoCollection PeopleGetPhotos(string userId, PhotoSearchExtras extras)
    {
      return this.PeopleGetPhotos(userId, SafetyLevel.None, DateTime.MinValue, DateTime.MinValue, DateTime.MinValue, DateTime.MinValue, ContentTypeSearch.None, PrivacyFilter.None, extras, 0, 0);
    }

    public PhotoCollection PeopleGetPhotos(string userId, PhotoSearchExtras extras, int page, int perPage)
    {
      return this.PeopleGetPhotos(userId, SafetyLevel.None, DateTime.MinValue, DateTime.MinValue, DateTime.MinValue, DateTime.MinValue, ContentTypeSearch.None, PrivacyFilter.None, extras, page, perPage);
    }

    public PhotoCollection PeopleGetPhotos(string userId, SafetyLevel safeSearch, DateTime minUploadDate, DateTime maxUploadDate, DateTime minTakenDate, DateTime maxTakenDate, ContentTypeSearch contentType, PrivacyFilter privacyFilter, PhotoSearchExtras extras, int page, int perPage)
    {
      this.CheckRequiresAuthentication();
      Dictionary<string, string> parameters = new Dictionary<string, string>();
      parameters.Add("method", "flickr.people.getPhotos");
      parameters.Add("user_id", userId ?? "me");
      if (safeSearch != SafetyLevel.None)
        parameters.Add("safe_search", safeSearch.ToString("d"));
      if (minUploadDate != DateTime.MinValue)
        parameters.Add("min_upload_date", UtilityMethods.DateToUnixTimestamp(minUploadDate));
      if (maxUploadDate != DateTime.MinValue)
        parameters.Add("max_upload_date", UtilityMethods.DateToUnixTimestamp(maxUploadDate));
      if (minTakenDate != DateTime.MinValue)
        parameters.Add("min_taken_date", UtilityMethods.DateToMySql(minTakenDate));
      if (maxTakenDate != DateTime.MinValue)
        parameters.Add("max_taken_date", UtilityMethods.DateToMySql(maxTakenDate));
      if (contentType != ContentTypeSearch.None)
        parameters.Add("content_type", contentType.ToString("d"));
      if (privacyFilter != PrivacyFilter.None)
        parameters.Add("privacy_filter", privacyFilter.ToString("d"));
      if (extras != PhotoSearchExtras.None)
        parameters.Add("extras", UtilityMethods.ExtrasToString(extras));
      if (page > 0)
        parameters.Add("page", page.ToString((IFormatProvider) NumberFormatInfo.InvariantInfo));
      if (perPage > 0)
        parameters.Add("per_page", perPage.ToString((IFormatProvider) NumberFormatInfo.InvariantInfo));
      return this.GetResponseCache<PhotoCollection>(parameters);
    }

    public PeoplePhotoCollection PeopleGetPhotosOf()
    {
      this.CheckRequiresAuthentication();
      return this.PeopleGetPhotosOf("me", PhotoSearchExtras.None, 0, 0);
    }

    public PeoplePhotoCollection PeopleGetPhotosOf(string userId)
    {
      return this.PeopleGetPhotosOf(userId, PhotoSearchExtras.None, 0, 0);
    }

    public PeoplePhotoCollection PeopleGetPhotosOf(string userId, PhotoSearchExtras extras)
    {
      return this.PeopleGetPhotosOf(userId, extras, 0, 0);
    }

    public PeoplePhotoCollection PeopleGetPhotosOf(string userId, int page, int perPage)
    {
      return this.PeopleGetPhotosOf(userId, PhotoSearchExtras.None, page, perPage);
    }

    public PeoplePhotoCollection PeopleGetPhotosOf(string userId, PhotoSearchExtras extras, int page, int perPage)
    {
      Dictionary<string, string> parameters = new Dictionary<string, string>();
      parameters.Add("method", "flickr.people.getPhotosOf");
      parameters.Add("user_id", userId);
      if (extras != PhotoSearchExtras.None)
        parameters.Add("extras", UtilityMethods.ExtrasToString(extras));
      if (perPage > 0)
        parameters.Add("per_page", perPage.ToString((IFormatProvider) NumberFormatInfo.InvariantInfo));
      if (page > 0)
        parameters.Add("page", page.ToString((IFormatProvider) NumberFormatInfo.InvariantInfo));
      return this.GetResponseCache<PeoplePhotoCollection>(parameters);
    }

    public void PhotosAddTags(string photoId, string[] tags)
    {
      string tags1 = string.Join(",", tags);
      this.PhotosAddTags(photoId, tags1);
    }

    public void PhotosAddTags(string photoId, string tags)
    {
      this.GetResponseNoCache<NoResponse>(new Dictionary<string, string>()
      {
        {
          "method",
          "flickr.photos.addTags"
        },
        {
          "photo_id",
          photoId
        },
        {
          "tags",
          tags
        }
      });
    }

    public void PhotosDelete(string photoId)
    {
      this.GetResponseNoCache<NoResponse>(new Dictionary<string, string>()
      {
        {
          "method",
          "flickr.photos.delete"
        },
        {
          "photo_id",
          photoId
        }
      });
    }

    public AllContexts PhotosGetAllContexts(string photoId)
    {
      return this.GetResponseCache<AllContexts>(new Dictionary<string, string>()
      {
        {
          "method",
          "flickr.photos.getAllContexts"
        },
        {
          "photo_id",
          photoId
        }
      });
    }

    public PhotoCollection PhotosGetContactsPhotos()
    {
      return this.PhotosGetContactsPhotos(0, false, false, false, PhotoSearchExtras.None);
    }

    public PhotoCollection PhotosGetContactsPhotos(int count)
    {
      return this.PhotosGetContactsPhotos(count, false, false, false, PhotoSearchExtras.None);
    }

    public PhotoCollection PhotosGetContactsPhotos(int count, bool justFriends, bool singlePhoto, bool includeSelf, PhotoSearchExtras extras)
    {
      this.CheckRequiresAuthentication();
      if (count != 0 && (count < 10 || count > 50) && !singlePhoto)
      {
        throw new ArgumentOutOfRangeException("count", string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Count must be between 10 and 50. ({0})", new object[1]
        {
          (object) count
        }));
      }
      else
      {
        Dictionary<string, string> parameters = new Dictionary<string, string>();
        parameters.Add("method", "flickr.photos.getContactsPhotos");
        if (count > 0 && !singlePhoto)
          parameters.Add("count", count.ToString((IFormatProvider) NumberFormatInfo.InvariantInfo));
        if (justFriends)
          parameters.Add("just_friends", "1");
        if (singlePhoto)
          parameters.Add("single_photo", "1");
        if (includeSelf)
          parameters.Add("include_self", "1");
        if (extras != PhotoSearchExtras.None)
          parameters.Add("extras", UtilityMethods.ExtrasToString(extras));
        return this.GetResponseCache<PhotoCollection>(parameters);
      }
    }

    public PhotoCollection PhotosGetContactsPublicPhotos(string userId)
    {
      return this.PhotosGetContactsPublicPhotos(userId, 0, false, false, false, PhotoSearchExtras.None);
    }

    public PhotoCollection PhotosGetContactsPublicPhotos(string userId, PhotoSearchExtras extras)
    {
      return this.PhotosGetContactsPublicPhotos(userId, 0, false, false, false, extras);
    }

    public PhotoCollection PhotosGetContactsPublicPhotos(string userId, int count)
    {
      return this.PhotosGetContactsPublicPhotos(userId, count, false, false, false, PhotoSearchExtras.None);
    }

    public PhotoCollection PhotosGetContactsPublicPhotos(string userId, int count, PhotoSearchExtras extras)
    {
      return this.PhotosGetContactsPublicPhotos(userId, count, false, false, false, extras);
    }

    public PhotoCollection PhotosGetContactsPublicPhotos(string userId, int count, bool justFriends, bool singlePhoto, bool includeSelf)
    {
      return this.PhotosGetContactsPublicPhotos(userId, count, justFriends, singlePhoto, includeSelf, PhotoSearchExtras.None);
    }

    public PhotoCollection PhotosGetContactsPublicPhotos(string userId, int count, bool justFriends, bool singlePhoto, bool includeSelf, PhotoSearchExtras extras)
    {
      Dictionary<string, string> parameters = new Dictionary<string, string>();
      parameters.Add("method", "flickr.photos.getContactsPublicPhotos");
      parameters.Add("api_key", this.apiKey);
      parameters.Add("user_id", userId);
      if (count > 0)
        parameters.Add("count", count.ToString((IFormatProvider) NumberFormatInfo.InvariantInfo));
      if (justFriends)
        parameters.Add("just_friends", "1");
      if (singlePhoto)
        parameters.Add("single_photo", "1");
      if (includeSelf)
        parameters.Add("include_self", "1");
      if (extras != PhotoSearchExtras.None)
        parameters.Add("extras", UtilityMethods.ExtrasToString(extras));
      return this.GetResponseCache<PhotoCollection>(parameters);
    }

    public Context PhotosGetContext(string photoId)
    {
      return this.GetResponseCache<Context>(new Dictionary<string, string>()
      {
        {
          "method",
          "flickr.photos.getContext"
        },
        {
          "photo_id",
          photoId
        }
      });
    }

    public PhotoCountCollection PhotosGetCounts(DateTime[] dates)
    {
      return this.PhotosGetCounts(dates, false);
    }

    public PhotoCountCollection PhotosGetCounts(DateTime[] dates, bool taken)
    {
      if (taken)
        return this.PhotosGetCounts((DateTime[]) null, dates);
      else
        return this.PhotosGetCounts(dates, (DateTime[]) null);
    }

    public PhotoCountCollection PhotosGetCounts(DateTime[] dates, DateTime[] takenDates)
    {
      string str1 = (string) null;
      string str2 = (string) null;
      if (dates != null && dates.Length > 0)
      {
        Array.Sort<DateTime>(dates);
        str1 = string.Join(",", new List<DateTime>((IEnumerable<DateTime>) dates).ConvertAll<string>(new Converter<DateTime, string>(UtilityMethods.DateToUnixTimestamp)).ToArray());
      }
      if (takenDates != null && takenDates.Length > 0)
      {
        Array.Sort<DateTime>(takenDates);
        str2 = string.Join(",", new List<DateTime>((IEnumerable<DateTime>) takenDates).ConvertAll<string>(new Converter<DateTime, string>(UtilityMethods.DateToMySql)).ToArray());
      }
      Dictionary<string, string> parameters = new Dictionary<string, string>();
      parameters.Add("method", "flickr.photos.getCounts");
      if (!string.IsNullOrEmpty(str1))
        parameters.Add("dates", str1);
      if (!string.IsNullOrEmpty(str2))
        parameters.Add("taken_dates", str2);
      return this.GetResponseCache<PhotoCountCollection>(parameters);
    }

    public ExifTagCollection PhotosGetExif(string photoId)
    {
      return this.PhotosGetExif(photoId, (string) null);
    }

    public ExifTagCollection PhotosGetExif(string photoId, string secret)
    {
      Dictionary<string, string> parameters = new Dictionary<string, string>();
      parameters.Add("method", "flickr.photos.getExif");
      parameters.Add("photo_id", photoId);
      if (secret != null)
        parameters.Add("secret", secret);
      return this.GetResponseCache<ExifTagCollection>(parameters);
    }

    public PhotoInfo PhotosGetInfo(string photoId)
    {
      return this.PhotosGetInfo(photoId, (string) null);
    }

    public PhotoInfo PhotosGetInfo(string photoId, string secret)
    {
      Dictionary<string, string> parameters = new Dictionary<string, string>();
      parameters.Add("method", "flickr.photos.getInfo");
      parameters.Add("photo_id", photoId);
      if (secret != null)
        parameters.Add("secret", secret);
      return this.GetResponseCache<PhotoInfo>(parameters);
    }

    public PhotoPermissions PhotosGetPerms(string photoId)
    {
      return this.GetResponseCache<PhotoPermissions>(new Dictionary<string, string>()
      {
        {
          "method",
          "flickr.photos.getPerms"
        },
        {
          "photo_id",
          photoId
        }
      });
    }

    public PhotoCollection PhotosGetRecent()
    {
      return this.PhotosGetRecent(0, 0, PhotoSearchExtras.None);
    }

    public PhotoCollection PhotosGetRecent(PhotoSearchExtras extras)
    {
      return this.PhotosGetRecent(0, 0, extras);
    }

    public PhotoCollection PhotosGetRecent(int page, int perPage)
    {
      return this.PhotosGetRecent(page, perPage, PhotoSearchExtras.None);
    }

    public PhotoCollection PhotosGetRecent(int page, int perPage, PhotoSearchExtras extras)
    {
      Dictionary<string, string> parameters = new Dictionary<string, string>();
      parameters.Add("method", "flickr.photos.getRecent");
      parameters.Add("api_key", this.apiKey);
      if (perPage > 0)
        parameters.Add("per_page", perPage.ToString((IFormatProvider) NumberFormatInfo.InvariantInfo));
      if (page > 0)
        parameters.Add("page", page.ToString((IFormatProvider) NumberFormatInfo.InvariantInfo));
      if (extras != PhotoSearchExtras.None)
        parameters.Add("extras", UtilityMethods.ExtrasToString(extras));
      return this.GetResponseCache<PhotoCollection>(parameters);
    }

    public SizeCollection PhotosGetSizes(string photoId)
    {
      return this.GetResponseCache<SizeCollection>(new Dictionary<string, string>()
      {
        {
          "method",
          "flickr.photos.getSizes"
        },
        {
          "photo_id",
          photoId
        }
      });
    }

    public PhotoCollection PhotosGetUntagged()
    {
      return this.PhotosGetUntagged(0, 0, PhotoSearchExtras.Tags);
    }

    public PhotoCollection PhotosGetUntagged(PhotoSearchExtras extras)
    {
      return this.PhotosGetUntagged(0, 0, extras);
    }

    public PhotoCollection PhotosGetUntagged(int page, int perPage)
    {
      return this.PhotosGetUntagged(page, perPage, PhotoSearchExtras.Tags);
    }

    public PhotoCollection PhotosGetUntagged(int page, int perPage, PhotoSearchExtras extras)
    {
      return this.PhotosGetUntagged(new PartialSearchOptions()
      {
        Page = page,
        PerPage = perPage,
        Extras = extras
      });
    }

    public PhotoCollection PhotosGetUntagged(PartialSearchOptions options)
    {
      this.CheckRequiresAuthentication();
      Dictionary<string, string> parameters = new Dictionary<string, string>();
      parameters.Add("method", "flickr.photos.getUntagged");
      UtilityMethods.PartialOptionsIntoArray(options, parameters);
      return this.GetResponseCache<PhotoCollection>(parameters);
    }

    public PhotoCollection PhotosGetNotInSet()
    {
      return this.PhotosGetNotInSet(new PartialSearchOptions());
    }

    public PhotoCollection PhotosGetNotInSet(int page)
    {
      return this.PhotosGetNotInSet(page, 0, PhotoSearchExtras.None);
    }

    public PhotoCollection PhotosGetNotInSet(int page, int perPage)
    {
      return this.PhotosGetNotInSet(page, perPage, PhotoSearchExtras.None);
    }

    public PhotoCollection PhotosGetNotInSet(int page, int perPage, PhotoSearchExtras extras)
    {
      return this.PhotosGetNotInSet(new PartialSearchOptions()
      {
        PerPage = perPage,
        Page = page,
        Extras = extras
      });
    }

    public PhotoCollection PhotosGetNotInSet(PartialSearchOptions options)
    {
      this.CheckRequiresAuthentication();
      Dictionary<string, string> parameters = new Dictionary<string, string>();
      parameters.Add("method", "flickr.photos.getNotInSet");
      UtilityMethods.PartialOptionsIntoArray(options, parameters);
      return this.GetResponseCache<PhotoCollection>(parameters);
    }

    public LicenseCollection PhotosLicensesGetInfo()
    {
      return this.GetResponseCache<LicenseCollection>(new Dictionary<string, string>()
      {
        {
          "method",
          "flickr.photos.licenses.getInfo"
        },
        {
          "api_key",
          this.apiKey
        }
      });
    }

    public void PhotosLicensesSetLicense(string photoId, LicenseType license)
    {
      this.CheckRequiresAuthentication();
      this.GetResponseNoCache<NoResponse>(new Dictionary<string, string>()
      {
        {
          "method",
          "flickr.photos.licenses.setLicense"
        },
        {
          "photo_id",
          photoId
        },
        {
          "license_id",
          license.ToString("d")
        }
      });
    }

    public void PhotosRemoveTag(string tagId)
    {
      this.GetResponseCache<NoResponse>(new Dictionary<string, string>()
      {
        {
          "method",
          "flickr.photos.removeTag"
        },
        {
          "tag_id",
          tagId
        }
      });
    }

    public PhotoCollection PhotosRecentlyUpdated(DateTime minDate, PhotoSearchExtras extras)
    {
      return this.PhotosRecentlyUpdated(minDate, extras, 0, 0);
    }

    public PhotoCollection PhotosRecentlyUpdated(DateTime minDate, int page, int perPage)
    {
      return this.PhotosRecentlyUpdated(minDate, PhotoSearchExtras.None, page, perPage);
    }

    public PhotoCollection PhotosRecentlyUpdated(DateTime minDate)
    {
      return this.PhotosRecentlyUpdated(minDate, PhotoSearchExtras.None, 0, 0);
    }

    public PhotoCollection PhotosRecentlyUpdated(DateTime minDate, PhotoSearchExtras extras, int page, int perPage)
    {
      this.CheckRequiresAuthentication();
      Dictionary<string, string> parameters = new Dictionary<string, string>();
      parameters.Add("method", "flickr.photos.recentlyUpdated");
      parameters.Add("min_date", UtilityMethods.DateToUnixTimestamp(minDate));
      if (extras != PhotoSearchExtras.None)
        parameters.Add("extras", UtilityMethods.ExtrasToString(extras));
      if (perPage > 0)
        parameters.Add("per_page", perPage.ToString((IFormatProvider) NumberFormatInfo.InvariantInfo));
      if (page > 0)
        parameters.Add("page", page.ToString((IFormatProvider) NumberFormatInfo.InvariantInfo));
      return this.GetResponseCache<PhotoCollection>(parameters);
    }

    public PhotoCollection PhotosSearch(PhotoSearchOptions options)
    {
      Dictionary<string, string> parameters = new Dictionary<string, string>();
      parameters.Add("method", "flickr.photos.search");
      options.AddToDictionary(parameters);
      return this.GetResponseCache<PhotoCollection>(parameters);
    }

    public void PhotosSetDates(string photoId, DateTime dateTaken, DateGranularity granularity)
    {
      this.PhotosSetDates(photoId, DateTime.MinValue, dateTaken, granularity);
    }

    public void PhotosSetDates(string photoId, DateTime datePosted)
    {
      this.PhotosSetDates(photoId, datePosted, DateTime.MinValue, DateGranularity.FullDate);
    }

    public void PhotosSetDates(string photoId, DateTime datePosted, DateTime dateTaken, DateGranularity granularity)
    {
      Dictionary<string, string> parameters = new Dictionary<string, string>();
      parameters.Add("method", "flickr.photos.setDates");
      parameters.Add("photo_id", photoId);
      if (datePosted != DateTime.MinValue)
        parameters.Add("date_posted", ((object) UtilityMethods.DateToUnixTimestamp(datePosted)).ToString());
      if (dateTaken != DateTime.MinValue)
      {
        parameters.Add("date_taken", dateTaken.ToString("yyyy-MM-dd HH:mm:ss", (IFormatProvider) DateTimeFormatInfo.InvariantInfo));
        parameters.Add("date_taken_granularity", granularity.ToString("d"));
      }
      this.GetResponseNoCache<NoResponse>(parameters);
    }

    public void PhotosSetMeta(string photoId, string title, string description)
    {
      this.GetResponseNoCache<NoResponse>(new Dictionary<string, string>()
      {
        {
          "method",
          "flickr.photos.setMeta"
        },
        {
          "photo_id",
          photoId
        },
        {
          "title",
          title
        },
        {
          "description",
          description
        }
      });
    }

    public void PhotosSetPerms(string photoId, int isPublic, int isFriend, int isFamily, PermissionComment permComment, PermissionAddMeta permAddMeta)
    {
      this.PhotosSetPerms(photoId, isPublic == 1, isFriend == 1, isFamily == 1, permComment, permAddMeta);
    }

    public void PhotosSetPerms(string photoId, bool isPublic, bool isFriend, bool isFamily, PermissionComment permComment, PermissionAddMeta permAddMeta)
    {
      this.GetResponseNoCache<NoResponse>(new Dictionary<string, string>()
      {
        {
          "method",
          "flickr.photos.setPerms"
        },
        {
          "photo_id",
          photoId
        },
        {
          "is_public",
          isPublic ? "1" : "0"
        },
        {
          "is_friend",
          isFriend ? "1" : "0"
        },
        {
          "is_family",
          isFamily ? "1" : "0"
        },
        {
          "perm_comment",
          permComment.ToString("d")
        },
        {
          "perm_addmeta",
          permAddMeta.ToString("d")
        }
      });
    }

    public void PhotosSetTags(string photoId, string[] tags)
    {
      string tags1 = string.Join(",", tags);
      this.PhotosSetTags(photoId, tags1);
    }

    public void PhotosSetTags(string photoId, string tags)
    {
      this.GetResponseNoCache<NoResponse>(new Dictionary<string, string>()
      {
        {
          "method",
          "flickr.photos.setTags"
        },
        {
          "photo_id",
          photoId
        },
        {
          "tags",
          tags
        }
      });
    }

    public void PhotosSetContentType(string photoId, ContentType contentType)
    {
      this.CheckRequiresAuthentication();
      this.GetResponseNoCache<NoResponse>(new Dictionary<string, string>()
      {
        {
          "method",
          "flickr.photos.setContentType"
        },
        {
          "photo_id",
          photoId
        },
        {
          "content_type",
          contentType.ToString("D")
        }
      });
    }

    public void PhotosSetSafetyLevel(string photoId, HiddenFromSearch hidden)
    {
      this.PhotosSetSafetyLevel(photoId, SafetyLevel.None, hidden);
    }

    public void PhotosSetSafetyLevel(string photoId, SafetyLevel safetyLevel)
    {
      this.PhotosSetSafetyLevel(photoId, safetyLevel, HiddenFromSearch.None);
    }

    public void PhotosSetSafetyLevel(string photoId, SafetyLevel safetyLevel, HiddenFromSearch hidden)
    {
      this.CheckRequiresAuthentication();
      Dictionary<string, string> parameters = new Dictionary<string, string>();
      parameters.Add("method", "flickr.photos.setSafetyLevel");
      parameters.Add("photo_id", photoId);
      if (safetyLevel != SafetyLevel.None)
        parameters.Add("safety_level", safetyLevel.ToString("D"));
      switch (hidden)
      {
        case HiddenFromSearch.Visible:
          parameters.Add("hidden", "0");
          break;
        case HiddenFromSearch.Hidden:
          parameters.Add("hidden", "1");
          break;
      }
      this.GetResponseNoCache<NoResponse>(parameters);
    }

    public PhotoFavoriteCollection PhotosGetFavorites(string photoId)
    {
      return this.PhotosGetFavorites(photoId, 0, 0);
    }

    public PhotoFavoriteCollection PhotosGetFavorites(string photoId, int perPage, int page)
    {
      Dictionary<string, string> parameters = new Dictionary<string, string>();
      parameters.Add("method", "flickr.photos.getFavorites");
      parameters.Add("photo_id", photoId);
      if (page > 0)
        parameters.Add("page", page.ToString((IFormatProvider) NumberFormatInfo.InvariantInfo));
      if (perPage > 0)
        parameters.Add("per_page", perPage.ToString((IFormatProvider) NumberFormatInfo.InvariantInfo));
      return this.GetResponseCache<PhotoFavoriteCollection>(parameters);
    }

    public PlaceCollection PlacesFind(string query)
    {
      if (query == null)
        throw new ArgumentNullException("query");
      return this.GetResponseCache<PlaceCollection>(new Dictionary<string, string>()
      {
        {
          "method",
          "flickr.places.find"
        },
        {
          "query",
          query
        }
      });
    }

    public Place PlacesFindByLatLon(double latitude, double longitude)
    {
      return this.PlacesFindByLatLon(latitude, longitude, GeoAccuracy.None);
    }

    public Place PlacesFindByLatLon(double latitude, double longitude, GeoAccuracy accuracy)
    {
      Dictionary<string, string> parameters = new Dictionary<string, string>();
      parameters.Add("method", "flickr.places.findByLatLon");
      parameters.Add("lat", latitude.ToString((IFormatProvider) NumberFormatInfo.InvariantInfo));
      parameters.Add("lon", longitude.ToString((IFormatProvider) NumberFormatInfo.InvariantInfo));
      if (accuracy != GeoAccuracy.None)
        parameters.Add("accuracy", ((int) accuracy).ToString((IFormatProvider) NumberFormatInfo.InvariantInfo));
      return this.GetResponseCache<PlaceCollection>(parameters)[0];
    }

    public PlaceCollection PlacesGetChildrenWithPhotosPublic(string placeId, string woeId)
    {
      Dictionary<string, string> parameters = new Dictionary<string, string>();
      parameters.Add("method", "flickr.places.getChildrenWithPhotosPublic");
      if ((placeId == null || placeId.Length == 0) && (woeId == null || woeId.Length == 0))
        throw new FlickrException("Both placeId and woeId cannot be null or empty.");
      if (!string.IsNullOrEmpty(placeId))
        parameters.Add("place_id", placeId);
      if (!string.IsNullOrEmpty(woeId))
        parameters.Add("woe_id", woeId);
      return this.GetResponseCache<PlaceCollection>(parameters);
    }

    public PlaceInfo PlacesGetInfo(string placeId, string woeId)
    {
      Dictionary<string, string> parameters = new Dictionary<string, string>();
      parameters.Add("method", "flickr.places.getInfo");
      if (string.IsNullOrEmpty(placeId) && string.IsNullOrEmpty(woeId))
        throw new FlickrException("Both placeId and woeId cannot be null or empty.");
      if (!string.IsNullOrEmpty(placeId))
        parameters.Add("place_id", placeId);
      if (!string.IsNullOrEmpty(woeId))
        parameters.Add("woe_id", woeId);
      return this.GetResponseCache<PlaceInfo>(parameters);
    }

    public PlaceInfo PlacesGetInfoByUrl(string url)
    {
      return this.GetResponseCache<PlaceInfo>(new Dictionary<string, string>()
      {
        {
          "method",
          "flickr.places.getInfoByUrl"
        },
        {
          "url",
          url
        }
      });
    }

    public PlaceTypeInfoCollection PlacesGetPlaceTypes()
    {
      return this.GetResponseCache<PlaceTypeInfoCollection>(new Dictionary<string, string>()
      {
        {
          "method",
          "flickr.places.getPlaceTypes"
        }
      });
    }

    public ShapeDataCollection PlacesGetShapeHistory(string placeId, string woeId)
    {
      Dictionary<string, string> parameters = new Dictionary<string, string>();
      parameters.Add("method", "flickr.places.getShapeHistory");
      if (string.IsNullOrEmpty(placeId) && string.IsNullOrEmpty(woeId))
        throw new FlickrException("Both placeId and woeId cannot be null or empty.");
      if (!string.IsNullOrEmpty(placeId))
        parameters.Add("place_id", placeId);
      if (!string.IsNullOrEmpty(woeId))
        parameters.Add("woe_id", woeId);
      return this.GetResponseCache<ShapeDataCollection>(parameters);
    }

    public PlaceCollection PlacesGetTopPlacesList(PlaceType placeType)
    {
      return this.PlacesGetTopPlacesList(placeType, DateTime.MinValue, (string) null, (string) null);
    }

    public PlaceCollection PlacesGetTopPlacesList(PlaceType placeType, string placeId, string woeId)
    {
      return this.PlacesGetTopPlacesList(placeType, DateTime.MinValue, placeId, woeId);
    }

    public PlaceCollection PlacesGetTopPlacesList(PlaceType placeType, DateTime date)
    {
      return this.PlacesGetTopPlacesList(placeType, date, (string) null, (string) null);
    }

    public PlaceCollection PlacesGetTopPlacesList(PlaceType placeType, DateTime date, string placeId, string woeId)
    {
      Dictionary<string, string> parameters = new Dictionary<string, string>();
      parameters.Add("method", "flickr.places.getTopPlacesList");
      parameters.Add("place_type_id", placeType.ToString("D"));
      if (date != DateTime.MinValue)
        parameters.Add("date", date.ToString("yyyy-MM-dd", (IFormatProvider) DateTimeFormatInfo.InvariantInfo));
      if (!string.IsNullOrEmpty(placeId))
        parameters.Add("place_id", placeId);
      if (!string.IsNullOrEmpty(woeId))
        parameters.Add("woe_id", woeId);
      return this.GetResponseCache<PlaceCollection>(parameters);
    }

    public PlaceCollection PlacesPlacesForUser()
    {
      return this.PlacesPlacesForUser(PlaceType.Continent, (string) null, (string) null, 0, DateTime.MinValue, DateTime.MinValue, DateTime.MinValue, DateTime.MinValue);
    }

    public PlaceCollection PlacesPlacesForUser(PlaceType placeType, string woeId, string placeId)
    {
      return this.PlacesPlacesForUser(placeType, woeId, placeId, 0, DateTime.MinValue, DateTime.MinValue, DateTime.MinValue, DateTime.MinValue);
    }

    public PlaceCollection PlacesPlacesForUser(PlaceType placeType, string woeId, string placeId, int threshold, DateTime minUploadDate, DateTime maxUploadDate, DateTime minTakenDate, DateTime maxTakenDate)
    {
      this.CheckRequiresAuthentication();
      Dictionary<string, string> parameters = new Dictionary<string, string>();
      parameters.Add("method", "flickr.places.placesForUser");
      parameters.Add("place_type_id", placeType.ToString("D"));
      if (!string.IsNullOrEmpty(woeId))
        parameters.Add("woe_id", woeId);
      if (!string.IsNullOrEmpty(placeId))
        parameters.Add("place_id", placeId);
      if (threshold > 0)
        parameters.Add("threshold", threshold.ToString((IFormatProvider) NumberFormatInfo.InvariantInfo));
      if (minTakenDate != DateTime.MinValue)
        parameters.Add("min_taken_date", UtilityMethods.DateToMySql(minTakenDate));
      if (maxTakenDate != DateTime.MinValue)
        parameters.Add("max_taken_date", UtilityMethods.DateToMySql(maxTakenDate));
      if (minUploadDate != DateTime.MinValue)
        parameters.Add("min_upload_date", UtilityMethods.DateToUnixTimestamp(minUploadDate));
      if (maxUploadDate != DateTime.MinValue)
        parameters.Add("max_upload_date", UtilityMethods.DateToUnixTimestamp(maxUploadDate));
      return this.GetResponseCache<PlaceCollection>(parameters);
    }

    public PlaceCollection PlacesPlacesForTags(PlaceType placeType, string woeId, string placeId, int threshold, string[] tags, TagMode tagMode, string[] machineTags, MachineTagMode machineTagMode, DateTime minUploadDate, DateTime maxUploadDate, DateTime minTakenDate, DateTime maxTakenDate)
    {
      Dictionary<string, string> parameters = new Dictionary<string, string>();
      parameters.Add("method", "flickr.places.placesForTags");
      parameters.Add("place_type_id", placeType.ToString("D"));
      if (!string.IsNullOrEmpty(woeId))
        parameters.Add("woe_id", woeId);
      if (!string.IsNullOrEmpty(placeId))
        parameters.Add("place_id", placeId);
      if (threshold > 0)
        parameters.Add("threshold", threshold.ToString((IFormatProvider) NumberFormatInfo.InvariantInfo));
      if (tags != null && tags.Length > 0)
        parameters.Add("tags", string.Join(",", tags));
      if (tagMode != TagMode.None)
        parameters.Add("tag_mode", UtilityMethods.TagModeToString(tagMode));
      if (machineTags != null && machineTags.Length > 0)
        parameters.Add("machine_tags", string.Join(",", machineTags));
      if (machineTagMode != MachineTagMode.None)
        parameters.Add("machine_tag_mode", UtilityMethods.MachineTagModeToString(machineTagMode));
      if (minTakenDate != DateTime.MinValue)
        parameters.Add("min_taken_date", UtilityMethods.DateToMySql(minTakenDate));
      if (maxTakenDate != DateTime.MinValue)
        parameters.Add("max_taken_date", UtilityMethods.DateToMySql(maxTakenDate));
      if (minUploadDate != DateTime.MinValue)
        parameters.Add("min_upload_date", UtilityMethods.DateToUnixTimestamp(minUploadDate));
      if (maxUploadDate != DateTime.MinValue)
        parameters.Add("max_upload_date", UtilityMethods.DateToUnixTimestamp(maxUploadDate));
      return this.GetResponseCache<PlaceCollection>(parameters);
    }

    public PlaceCollection PlacesPlacesForContacts(PlaceType placeType, string woeId, string placeId, int threshold, ContactSearch contactType, DateTime minUploadDate, DateTime maxUploadDate, DateTime minTakenDate, DateTime maxTakenDate)
    {
      this.CheckRequiresAuthentication();
      Dictionary<string, string> parameters = new Dictionary<string, string>();
      parameters.Add("method", "flickr.places.placesForContacts");
      parameters.Add("place_type_id", placeType.ToString("D"));
      if (!string.IsNullOrEmpty(woeId))
        parameters.Add("woe_id", woeId);
      if (!string.IsNullOrEmpty(placeId))
        parameters.Add("place_id", placeId);
      if (threshold > 0)
        parameters.Add("threshold", threshold.ToString((IFormatProvider) NumberFormatInfo.InvariantInfo));
      if (contactType != ContactSearch.None)
        parameters.Add("contacts", contactType == ContactSearch.AllContacts ? "all" : "ff");
      if (minUploadDate != DateTime.MinValue)
        parameters.Add("min_upload_date", UtilityMethods.DateToUnixTimestamp(minUploadDate));
      if (maxUploadDate != DateTime.MinValue)
        parameters.Add("max_upload_date", UtilityMethods.DateToUnixTimestamp(maxUploadDate));
      if (minTakenDate != DateTime.MinValue)
        parameters.Add("min_taken_date", UtilityMethods.DateToMySql(minTakenDate));
      if (maxTakenDate != DateTime.MinValue)
        parameters.Add("max_taken_date", UtilityMethods.DateToMySql(maxTakenDate));
      return this.GetResponseCache<PlaceCollection>(parameters);
    }

    public PlaceCollection PlacesPlacesForBoundingBox(PlaceType placeType, string woeId, string placeId, BoundaryBox boundaryBox)
    {
      Dictionary<string, string> parameters = new Dictionary<string, string>();
      parameters.Add("method", "flickr.places.placesForBoundingBox");
      parameters.Add("place_type_id", placeType.ToString("D"));
      if (!string.IsNullOrEmpty(woeId))
        parameters.Add("woe_id", woeId);
      if (!string.IsNullOrEmpty(placeId))
        parameters.Add("place_id", placeId);
      parameters.Add("bbox", boundaryBox.ToString());
      return this.GetResponseCache<PlaceCollection>(parameters);
    }

    [Obsolete("This method is deprecated. Please use Flickr.PlacesGetInfo instead.")]
    public PlaceInfo PlacesResolvePlaceId(string placeId)
    {
      return this.PlacesGetInfo(placeId, (string) null);
    }

    [Obsolete("This method is deprecated. Please use Flickr.PlacesGetInfoByUrl instead.")]
    public PlaceInfo PlacesResolvePlaceUrl(string url)
    {
      return this.PlacesGetInfoByUrl(url);
    }

    public TagCollection PlacesTagsForPlace(string placeId, string woeId)
    {
      return this.PlacesTagsForPlace(placeId, woeId, DateTime.MinValue, DateTime.MinValue, DateTime.MinValue, DateTime.MinValue);
    }

    public TagCollection PlacesTagsForPlace(string placeId, string woeId, DateTime minUploadDate, DateTime maxUploadDate, DateTime minTakenDate, DateTime maxTakenDate)
    {
      Dictionary<string, string> parameters = new Dictionary<string, string>();
      parameters.Add("method", "flickr.places.tagsForPlace");
      if (string.IsNullOrEmpty(placeId) && string.IsNullOrEmpty(woeId))
        throw new FlickrException("Both placeId and woeId cannot be null or empty.");
      if (!string.IsNullOrEmpty(woeId))
        parameters.Add("woe_id", woeId);
      if (!string.IsNullOrEmpty(placeId))
        parameters.Add("place_id", placeId);
      if (minTakenDate != DateTime.MinValue)
        parameters.Add("min_taken_date", UtilityMethods.DateToMySql(minTakenDate));
      if (maxTakenDate != DateTime.MinValue)
        parameters.Add("max_taken_date", UtilityMethods.DateToMySql(maxTakenDate));
      if (minUploadDate != DateTime.MinValue)
        parameters.Add("min_upload_date", UtilityMethods.DateToUnixTimestamp(minUploadDate));
      if (maxUploadDate != DateTime.MinValue)
        parameters.Add("max_upload_date", UtilityMethods.DateToUnixTimestamp(maxUploadDate));
      return this.GetResponseCache<TagCollection>(parameters);
    }

    public MethodCollection ReflectionGetMethods()
    {
      return this.GetResponseNoCache<MethodCollection>(new Dictionary<string, string>()
      {
        {
          "method",
          "flickr.reflection.getMethods"
        }
      });
    }

    public Method ReflectionGetMethodInfo(string methodName)
    {
      return this.GetResponseNoCache<Method>(new Dictionary<string, string>()
      {
        {
          "method",
          "flickr.reflection.getMethodInfo"
        },
        {
          "api_key",
          this.apiKey
        },
        {
          "method_name",
          methodName
        }
      });
    }

    public StatDomainCollection StatsGetCollectionDomains(DateTime date)
    {
      return this.StatsGetCollectionDomains(date, (string) null, 0, 0);
    }

    public StatDomainCollection StatsGetCollectionDomains(DateTime date, string collectionId)
    {
      return this.StatsGetCollectionDomains(date, collectionId, 0, 0);
    }

    public StatDomainCollection StatsGetCollectionDomains(DateTime date, int page, int perPage)
    {
      return this.StatsGetCollectionDomains(date, (string) null, page, perPage);
    }

    public StatDomainCollection StatsGetCollectionDomains(DateTime date, string collectionId, int page, int perPage)
    {
      this.CheckRequiresAuthentication();
      Dictionary<string, string> parameters = new Dictionary<string, string>();
      parameters.Add("method", "flickr.stats.getCollectionDomains");
      parameters.Add("date", UtilityMethods.DateToUnixTimestamp(date));
      if (!string.IsNullOrEmpty(collectionId))
        parameters.Add("collection_id", UtilityMethods.CleanCollectionId(collectionId));
      if (page > 0)
        parameters.Add("page", page.ToString((IFormatProvider) NumberFormatInfo.InvariantInfo));
      if (perPage > 0)
        parameters.Add("per_page", perPage.ToString((IFormatProvider) NumberFormatInfo.InvariantInfo));
      return this.GetResponseCache<StatDomainCollection>(parameters);
    }

    public CsvFileCollection StatsGetCsvFiles()
    {
      this.CheckRequiresAuthentication();
      return this.GetResponseCache<CsvFileCollection>(new Dictionary<string, string>()
      {
        {
          "method",
          "flickr.stats.getCSVFiles"
        }
      });
    }

    public StatDomainCollection StatsGetPhotoDomains(DateTime date)
    {
      return this.StatsGetPhotoDomains(date, (string) null, 0, 0);
    }

    public StatDomainCollection StatsGetPhotoDomains(DateTime date, string photoId)
    {
      return this.StatsGetPhotoDomains(date, photoId, 0, 0);
    }

    public StatDomainCollection StatsGetPhotoDomains(DateTime date, int page, int perPage)
    {
      return this.StatsGetPhotoDomains(date, (string) null, page, perPage);
    }

    public StatDomainCollection StatsGetPhotoDomains(DateTime date, string photoId, int page, int perPage)
    {
      this.CheckRequiresAuthentication();
      Dictionary<string, string> parameters = new Dictionary<string, string>();
      parameters.Add("method", "flickr.stats.getPhotoDomains");
      parameters.Add("date", UtilityMethods.DateToUnixTimestamp(date));
      if (!string.IsNullOrEmpty(photoId))
        parameters.Add("photo_id", photoId);
      if (page > 0)
        parameters.Add("page", page.ToString((IFormatProvider) NumberFormatInfo.InvariantInfo));
      if (perPage > 0)
        parameters.Add("per_page", perPage.ToString((IFormatProvider) NumberFormatInfo.InvariantInfo));
      return this.GetResponseCache<StatDomainCollection>(parameters);
    }

    public StatDomainCollection StatsGetPhotostreamDomains(DateTime date)
    {
      return this.StatsGetPhotostreamDomains(date, 0, 0);
    }

    public StatDomainCollection StatsGetPhotostreamDomains(DateTime date, int page, int perPage)
    {
      this.CheckRequiresAuthentication();
      Dictionary<string, string> parameters = new Dictionary<string, string>();
      parameters.Add("method", "flickr.stats.getPhotostreamDomains");
      parameters.Add("date", UtilityMethods.DateToUnixTimestamp(date));
      if (page > 0)
        parameters.Add("page", page.ToString((IFormatProvider) NumberFormatInfo.InvariantInfo));
      if (perPage > 0)
        parameters.Add("per_page", perPage.ToString((IFormatProvider) NumberFormatInfo.InvariantInfo));
      return this.GetResponseCache<StatDomainCollection>(parameters);
    }

    public StatDomainCollection StatsGetPhotosetDomains(DateTime date)
    {
      return this.StatsGetPhotosetDomains(date, (string) null, 0, 0);
    }

    public StatDomainCollection StatsGetPhotosetDomains(DateTime date, string photosetId)
    {
      return this.StatsGetPhotosetDomains(date, photosetId, 0, 0);
    }

    public StatDomainCollection StatsGetPhotosetDomains(DateTime date, int page, int perPage)
    {
      return this.StatsGetPhotosetDomains(date, (string) null, page, perPage);
    }

    public StatDomainCollection StatsGetPhotosetDomains(DateTime date, string photosetId, int page, int perPage)
    {
      this.CheckRequiresAuthentication();
      Dictionary<string, string> parameters = new Dictionary<string, string>();
      parameters.Add("method", "flickr.stats.getPhotosetDomains");
      parameters.Add("date", UtilityMethods.DateToUnixTimestamp(date));
      if (!string.IsNullOrEmpty(photosetId))
        parameters.Add("photoset_id", photosetId);
      if (page > 0)
        parameters.Add("page", page.ToString((IFormatProvider) NumberFormatInfo.InvariantInfo));
      if (perPage > 0)
        parameters.Add("per_page", perPage.ToString((IFormatProvider) NumberFormatInfo.InvariantInfo));
      return this.GetResponseCache<StatDomainCollection>(parameters);
    }

    public Stats StatsGetCollectionStats(DateTime date, string collectionId)
    {
      this.CheckRequiresAuthentication();
      return this.GetResponseCache<Stats>(new Dictionary<string, string>()
      {
        {
          "method",
          "flickr.stats.getCollectionStats"
        },
        {
          "date",
          UtilityMethods.DateToUnixTimestamp(date)
        },
        {
          "collection_id",
          UtilityMethods.CleanCollectionId(collectionId)
        }
      });
    }

    public Stats StatsGetPhotoStats(DateTime date, string photoId)
    {
      this.CheckRequiresAuthentication();
      return this.GetResponseCache<Stats>(new Dictionary<string, string>()
      {
        {
          "method",
          "flickr.stats.getPhotoStats"
        },
        {
          "date",
          UtilityMethods.DateToUnixTimestamp(date)
        },
        {
          "photo_id",
          photoId
        }
      });
    }

    public Stats StatsGetPhotostreamStats(DateTime date)
    {
      this.CheckRequiresAuthentication();
      return this.GetResponseCache<Stats>(new Dictionary<string, string>()
      {
        {
          "method",
          "flickr.stats.getPhotostreamStats"
        },
        {
          "date",
          UtilityMethods.DateToUnixTimestamp(date)
        }
      });
    }

    public Stats StatsGetPhotosetStats(DateTime date, string photosetId)
    {
      this.CheckRequiresAuthentication();
      return this.GetResponseCache<Stats>(new Dictionary<string, string>()
      {
        {
          "method",
          "flickr.stats.getPhotosetStats"
        },
        {
          "date",
          UtilityMethods.DateToUnixTimestamp(date)
        },
        {
          "photoset_id",
          photosetId
        }
      });
    }

    public StatReferrerCollection StatsGetPhotoReferrers(DateTime date, string domain)
    {
      return this.StatsGetPhotoReferrers(date, domain, (string) null, 0, 0);
    }

    public StatReferrerCollection StatsGetPhotoReferrers(DateTime date, string domain, string photoId)
    {
      return this.StatsGetPhotoReferrers(date, domain, photoId, 0, 0);
    }

    public StatReferrerCollection StatsGetPhotoReferrers(DateTime date, string domain, int page, int perPage)
    {
      return this.StatsGetPhotoReferrers(date, domain, (string) null, page, perPage);
    }

    public StatReferrerCollection StatsGetPhotoReferrers(DateTime date, string domain, string photoId, int page, int perPage)
    {
      this.CheckRequiresAuthentication();
      Dictionary<string, string> parameters = new Dictionary<string, string>();
      parameters.Add("method", "flickr.stats.getPhotoReferrers");
      parameters.Add("date", UtilityMethods.DateToUnixTimestamp(date));
      parameters.Add("domain", domain);
      if (!string.IsNullOrEmpty(photoId))
        parameters.Add("photo_id", photoId);
      if (page > 0)
        parameters.Add("page", page.ToString((IFormatProvider) NumberFormatInfo.InvariantInfo));
      if (perPage > 0)
        parameters.Add("per_page", perPage.ToString((IFormatProvider) NumberFormatInfo.InvariantInfo));
      return this.GetResponseCache<StatReferrerCollection>(parameters);
    }

    public StatReferrerCollection StatsGetPhotosetReferrers(DateTime date, string domain)
    {
      return this.StatsGetPhotosetReferrers(date, domain, (string) null, 0, 0);
    }

    public StatReferrerCollection StatsGetPhotosetReferrers(DateTime date, string domain, string photosetId)
    {
      return this.StatsGetPhotosetReferrers(date, domain, photosetId, 0, 0);
    }

    public StatReferrerCollection StatsGetPhotosetReferrers(DateTime date, string domain, int page, int perPage)
    {
      return this.StatsGetPhotosetReferrers(date, domain, (string) null, page, perPage);
    }

    public StatReferrerCollection StatsGetPhotosetReferrers(DateTime date, string domain, string photosetId, int page, int perPage)
    {
      this.CheckRequiresAuthentication();
      Dictionary<string, string> parameters = new Dictionary<string, string>();
      parameters.Add("method", "flickr.stats.getPhotosetReferrers");
      parameters.Add("date", UtilityMethods.DateToUnixTimestamp(date));
      parameters.Add("domain", domain);
      if (!string.IsNullOrEmpty(photosetId))
        parameters.Add("photoset_id", photosetId);
      if (page > 0)
        parameters.Add("page", page.ToString((IFormatProvider) NumberFormatInfo.InvariantInfo));
      if (perPage > 0)
        parameters.Add("per_page", perPage.ToString((IFormatProvider) NumberFormatInfo.InvariantInfo));
      return this.GetResponseCache<StatReferrerCollection>(parameters);
    }

    public StatReferrerCollection StatsGetCollectionReferrers(DateTime date, string domain)
    {
      return this.StatsGetCollectionReferrers(date, domain, (string) null, 0, 0);
    }

    public StatReferrerCollection StatsGetCollectionReferrers(DateTime date, string domain, string collectionId)
    {
      return this.StatsGetCollectionReferrers(date, domain, collectionId, 0, 0);
    }

    public StatReferrerCollection StatsGetCollectionReferrers(DateTime date, string domain, int page, int perPage)
    {
      return this.StatsGetCollectionReferrers(date, domain, (string) null, page, perPage);
    }

    public StatReferrerCollection StatsGetCollectionReferrers(DateTime date, string domain, string collectionId, int page, int perPage)
    {
      this.CheckRequiresAuthentication();
      Dictionary<string, string> parameters = new Dictionary<string, string>();
      parameters.Add("method", "flickr.stats.getCollectionReferrers");
      parameters.Add("date", UtilityMethods.DateToUnixTimestamp(date));
      parameters.Add("domain", domain);
      if (!string.IsNullOrEmpty(collectionId))
        parameters.Add("collection_id", UtilityMethods.CleanCollectionId(collectionId));
      if (page > 0)
        parameters.Add("page", page.ToString((IFormatProvider) NumberFormatInfo.InvariantInfo));
      if (perPage > 0)
        parameters.Add("per_page", perPage.ToString((IFormatProvider) NumberFormatInfo.InvariantInfo));
      return this.GetResponseCache<StatReferrerCollection>(parameters);
    }

    public StatReferrerCollection StatsGetPhotostreamReferrers(DateTime date, string domain)
    {
      return this.StatsGetPhotostreamReferrers(date, domain, 0, 0);
    }

    public StatReferrerCollection StatsGetPhotostreamReferrers(DateTime date, string domain, int page, int perPage)
    {
      this.CheckRequiresAuthentication();
      Dictionary<string, string> parameters = new Dictionary<string, string>();
      parameters.Add("method", "flickr.stats.getPhotostreamReferrers");
      parameters.Add("date", UtilityMethods.DateToUnixTimestamp(date));
      parameters.Add("domain", domain);
      if (page > 0)
        parameters.Add("page", page.ToString((IFormatProvider) NumberFormatInfo.InvariantInfo));
      if (perPage > 0)
        parameters.Add("per_page", perPage.ToString((IFormatProvider) NumberFormatInfo.InvariantInfo));
      return this.GetResponseCache<StatReferrerCollection>(parameters);
    }

    public StatViews StatsGetTotalViews()
    {
      return this.StatsGetTotalViews(DateTime.MinValue);
    }

    public StatViews StatsGetTotalViews(DateTime date)
    {
      this.CheckRequiresAuthentication();
      Dictionary<string, string> parameters = new Dictionary<string, string>();
      parameters.Add("method", "flickr.stats.getTotalViews");
      if (date != DateTime.MinValue)
        parameters.Add("date", UtilityMethods.DateToUnixTimestamp(date));
      return this.GetResponseCache<StatViews>(parameters);
    }

    public PopularPhotoCollection StatsGetPopularPhotos()
    {
      return this.StatsGetPopularPhotos(DateTime.MinValue, PopularitySort.None, 0, 0);
    }

    public PopularPhotoCollection StatsGetPopularPhotos(DateTime date)
    {
      return this.StatsGetPopularPhotos(date, PopularitySort.None, 0, 0);
    }

    public PopularPhotoCollection StatsGetPopularPhotos(PopularitySort sort)
    {
      return this.StatsGetPopularPhotos(DateTime.MinValue, sort, 0, 0);
    }

    public PopularPhotoCollection StatsGetPopularPhotos(PopularitySort sort, int page, int perPage)
    {
      return this.StatsGetPopularPhotos(DateTime.MinValue, sort, page, perPage);
    }

    public PopularPhotoCollection StatsGetPopularPhotos(DateTime date, int page, int perPage)
    {
      return this.StatsGetPopularPhotos(date, PopularitySort.None, page, perPage);
    }

    public PopularPhotoCollection StatsGetPopularPhotos(DateTime date, PopularitySort sort, int page, int perPage)
    {
      this.CheckRequiresAuthentication();
      Dictionary<string, string> parameters = new Dictionary<string, string>();
      parameters.Add("method", "flickr.stats.getPopularPhotos");
      if (date != DateTime.MinValue)
        parameters.Add("date", UtilityMethods.DateToUnixTimestamp(date));
      if (sort != PopularitySort.None)
        parameters.Add("sort", UtilityMethods.SortOrderToString(sort));
      if (page > 0)
        parameters.Add("page", page.ToString((IFormatProvider) NumberFormatInfo.InvariantInfo));
      if (perPage > 0)
        parameters.Add("per_page", perPage.ToString((IFormatProvider) NumberFormatInfo.InvariantInfo));
      return this.GetResponseCache<PopularPhotoCollection>(parameters);
    }

    public UnknownResponse TestGeneric(string method, Dictionary<string, string> parameters)
    {
      if (parameters == null)
        parameters = new Dictionary<string, string>();
      parameters.Add("method", method);
      return this.GetResponseNoCache<UnknownResponse>(parameters);
    }

    public FoundUser TestLogin()
    {
      return this.GetResponseCache<FoundUser>(new Dictionary<string, string>()
      {
        {
          "method",
          "flickr.test.login"
        }
      });
    }

    public void TestNull()
    {
      this.GetResponseNoCache<NoResponse>(new Dictionary<string, string>()
      {
        {
          "method",
          "flickr.test.null"
        }
      });
    }

    public EchoResponseDictionary TestEcho(Dictionary<string, string> parameters)
    {
      parameters.Add("method", "flickr.test.echo");
      return this.GetResponseNoCache<EchoResponseDictionary>(parameters);
    }

    internal class StreamCollection : IDisposable
    {
      public EventHandler<UploadProgressEventArgs> UploadProgress;

      public List<Stream> Streams { get; private set; }

      public long Length
      {
        get
        {
          long num = 0L;
          foreach (Stream stream in this.Streams)
            num += stream.Length;
          return num;
        }
      }

      public StreamCollection(IEnumerable<Stream> streams)
      {
        this.Streams = new List<Stream>(streams);
      }

      public void ResetPosition()
      {
        this.Streams.ForEach((Action<Stream>) (s => s.Position = 0L));
      }

      public void CopyTo(Stream stream, int bufferSize = 16384)
      {
        this.ResetPosition();
        byte[] buffer = new byte[bufferSize];
        long length = this.Length;
        foreach (Stream stream1 in this.Streams)
        {
          int num = 0;
          int count;
          while (0 < (count = stream1.Read(buffer, 0, buffer.Length)))
          {
            num += count;
            stream.Write(buffer, 0, count);
            if (this.UploadProgress != null)
              this.UploadProgress((object) this, new UploadProgressEventArgs()
              {
                BytesSent = (long) num,
                TotalBytesToSend = length
              });
          }
          stream.Flush();
        }
        stream.Flush();
      }

      public void Dispose()
      {
        this.Streams.ForEach((Action<Stream>) (s =>
        {
          if (s == null)
            return;
          s.Dispose();
        }));
      }
    }
  }
}
