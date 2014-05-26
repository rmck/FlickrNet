// Type: FlickrNet.PartialSearchOptions
// Assembly: FlickrNet, Version=3.10.0.0, Culture=neutral, PublicKeyToken=2491df59efa5d132
// MVID: 8B8C756F-D5B2-4047-8A8B-B1C1E877A2EF
// Assembly location: D:\RMCK\vs\2014.05.24_FlickrAPI\FlickrNet.dll

using System;

namespace FlickrNet
{
  public class PartialSearchOptions
  {
    private DateTime minUploadDate = DateTime.MinValue;
    private DateTime maxUploadDate = DateTime.MinValue;
    private DateTime minTakenDate = DateTime.MinValue;
    private DateTime maxTakenDate = DateTime.MinValue;
    private PhotoSearchExtras extras;
    private PhotoSearchSortOrder sort;
    private int perPage;
    private int page;
    private PrivacyFilter privacyFilter;

    public DateTime MinUploadDate
    {
      get
      {
        return this.minUploadDate;
      }
      set
      {
        this.minUploadDate = value;
      }
    }

    public DateTime MaxUploadDate
    {
      get
      {
        return this.maxUploadDate;
      }
      set
      {
        this.maxUploadDate = value;
      }
    }

    public DateTime MinTakenDate
    {
      get
      {
        return this.minTakenDate;
      }
      set
      {
        this.minTakenDate = value;
      }
    }

    public DateTime MaxTakenDate
    {
      get
      {
        return this.maxTakenDate;
      }
      set
      {
        this.maxTakenDate = value;
      }
    }

    public PhotoSearchExtras Extras
    {
      get
      {
        return this.extras;
      }
      set
      {
        this.extras = value;
      }
    }

    public int PerPage
    {
      get
      {
        return this.perPage;
      }
      set
      {
        this.perPage = value;
      }
    }

    public int Page
    {
      get
      {
        return this.page;
      }
      set
      {
        if (value < 0)
          throw new ArgumentOutOfRangeException("value", "Must be greater than 0");
        this.page = value;
      }
    }

    public PhotoSearchSortOrder SortOrder
    {
      get
      {
        return this.sort;
      }
      set
      {
        this.sort = value;
      }
    }

    public PrivacyFilter PrivacyFilter
    {
      get
      {
        return this.privacyFilter;
      }
      set
      {
        this.privacyFilter = value;
      }
    }

    internal string ExtrasString
    {
      get
      {
        return UtilityMethods.ExtrasToString(this.Extras);
      }
    }

    internal string SortOrderString
    {
      get
      {
        return UtilityMethods.SortOrderToString(this.SortOrder);
      }
    }

    public PartialSearchOptions()
    {
    }

    public PartialSearchOptions(PhotoSearchExtras extras)
    {
      this.Extras = extras;
    }

    public PartialSearchOptions(int perPage, int page)
    {
      this.PerPage = perPage;
      this.Page = page;
    }

    public PartialSearchOptions(int perPage, int page, PhotoSearchExtras extras)
    {
      this.PerPage = perPage;
      this.Page = page;
      this.Extras = extras;
    }

    internal PartialSearchOptions(PhotoSearchOptions options)
    {
      this.Extras = options.Extras;
      this.MaxTakenDate = options.MaxTakenDate;
      this.MinTakenDate = options.MinTakenDate;
      this.MaxUploadDate = options.MaxUploadDate;
      this.MinUploadDate = options.MinUploadDate;
      this.Page = options.Page;
      this.PerPage = options.PerPage;
      this.PrivacyFilter = options.PrivacyFilter;
    }
  }
}
