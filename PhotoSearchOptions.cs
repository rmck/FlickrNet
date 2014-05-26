// Type: FlickrNet.PhotoSearchOptions
// Assembly: FlickrNet, Version=3.10.0.0, Culture=neutral, PublicKeyToken=2491df59efa5d132
// MVID: 8B8C756F-D5B2-4047-8A8B-B1C1E877A2EF
// Assembly location: D:\RMCK\vs\2014.05.24_FlickrAPI\FlickrNet.dll

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Text;

namespace FlickrNet
{
  public class PhotoSearchOptions
  {
    private Collection<LicenseType> licenses = new Collection<LicenseType>();

    public string UserId { get; set; }

    public GeoContext GeoContext { get; set; }

    public string GroupId { get; set; }

    public string Tags { get; set; }

    public TagMode TagMode { get; set; }

    public string MachineTags { get; set; }

    public MachineTagMode MachineTagMode { get; set; }

    public string Text { get; set; }

    public DateTime MinUploadDate { get; set; }

    public DateTime MaxUploadDate { get; set; }

    public DateTime MinTakenDate { get; set; }

    public DateTime MaxTakenDate { get; set; }

    public MediaType MediaType { get; set; }

    public Collection<LicenseType> Licenses
    {
      get
      {
        return this.licenses;
      }
    }

    public PhotoSearchExtras Extras { get; set; }

    public int PerPage { get; set; }

    public int Page { get; set; }

    public PhotoSearchSortOrder SortOrder { get; set; }

    public PrivacyFilter PrivacyFilter { get; set; }

    public BoundaryBox BoundaryBox { get; set; }

    public GeoAccuracy Accuracy
    {
      get
      {
        if (this.BoundaryBox != null)
          return this.BoundaryBox.Accuracy;
        else
          return GeoAccuracy.None;
      }
      set
      {
        if (this.BoundaryBox == null)
          this.BoundaryBox = new BoundaryBox();
        this.BoundaryBox.Accuracy = value;
      }
    }

    public SafetyLevel SafeSearch { get; set; }

    public ContentTypeSearch ContentType { get; set; }

    public RadiusUnit RadiusUnits { get; set; }

    public float? Radius { get; set; }

    public double? Longitude { get; set; }

    public double? Latitude { get; set; }

    public bool? HasGeo { get; set; }

    public ContactSearch Contacts { get; set; }

    public string WoeId { get; set; }

    public string PlaceId { get; set; }

    public bool IsCommons { get; set; }

    public bool InGallery { get; set; }

    public bool IsGetty { get; set; }

    public bool Faves { get; set; }

    public string PersonId { get; set; }

    public string Camera { get; set; }

    public string JumpTo { get; set; }

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

    public PhotoSearchOptions()
    {
    }

    public PhotoSearchOptions(string userId)
      : this(userId, (string) null, TagMode.AllTags, (string) null)
    {
    }

    public PhotoSearchOptions(string userId, string tags)
      : this(userId, tags, TagMode.AllTags, (string) null)
    {
    }

    public PhotoSearchOptions(string userId, string tags, TagMode tagMode)
      : this(userId, tags, tagMode, (string) null)
    {
    }

    public PhotoSearchOptions(string userId, string tags, TagMode tagMode, string text)
    {
      this.UserId = userId;
      this.Tags = tags;
      this.TagMode = tagMode;
      this.Text = text;
    }

    public string CalculateSlideshowUrl()
    {
      StringBuilder stringBuilder = new StringBuilder();
      stringBuilder.Append("https://www.flickr.com/show.gne");
      stringBuilder.Append("?api_method=flickr.photos.search&method_params=");
      Dictionary<string, string> parameters = new Dictionary<string, string>();
      this.AddToDictionary(parameters);
      List<string> list = new List<string>();
      foreach (KeyValuePair<string, string> keyValuePair in parameters)
        list.Add(Uri.EscapeDataString(keyValuePair.Key) + "|" + Uri.EscapeDataString(keyValuePair.Value));
      stringBuilder.Append(string.Join(";", list.ToArray()));
      return ((object) stringBuilder).ToString();
    }

    public void AddToDictionary(Dictionary<string, string> parameters)
    {
      if (!string.IsNullOrEmpty(this.UserId))
        parameters.Add("user_id", this.UserId);
      if (!string.IsNullOrEmpty(this.GroupId))
        parameters.Add("group_id", this.GroupId);
      if (!string.IsNullOrEmpty(this.Text))
        parameters.Add("text", this.Text);
      if (!string.IsNullOrEmpty(this.Tags))
        parameters.Add("tags", this.Tags);
      if (this.TagMode != TagMode.None)
        parameters.Add("tag_mode", UtilityMethods.TagModeToString(this.TagMode));
      if (!string.IsNullOrEmpty(this.MachineTags))
        parameters.Add("machine_tags", this.MachineTags);
      if (this.MachineTagMode != MachineTagMode.None)
        parameters.Add("machine_tag_mode", UtilityMethods.MachineTagModeToString(this.MachineTagMode));
      if (this.MinUploadDate != DateTime.MinValue)
        parameters.Add("min_upload_date", ((object) UtilityMethods.DateToUnixTimestamp(this.MinUploadDate)).ToString());
      if (this.MaxUploadDate != DateTime.MinValue)
        parameters.Add("max_upload_date", ((object) UtilityMethods.DateToUnixTimestamp(this.MaxUploadDate)).ToString());
      if (this.MinTakenDate != DateTime.MinValue)
        parameters.Add("min_taken_date", UtilityMethods.DateToMySql(this.MinTakenDate));
      if (this.MaxTakenDate != DateTime.MinValue)
        parameters.Add("max_taken_date", UtilityMethods.DateToMySql(this.MaxTakenDate));
      if (this.Licenses.Count != 0)
      {
        List<string> list = new List<string>();
        foreach (LicenseType licenseType in this.Licenses)
          list.Add(licenseType.ToString("d"));
        parameters.Add("license", string.Join(",", list.ToArray()));
      }
      if (this.PerPage != 0)
        parameters.Add("per_page", this.PerPage.ToString((IFormatProvider) NumberFormatInfo.InvariantInfo));
      if (this.Page != 0)
        parameters.Add("page", this.Page.ToString((IFormatProvider) NumberFormatInfo.InvariantInfo));
      if (this.Extras != PhotoSearchExtras.None)
        parameters.Add("extras", this.ExtrasString);
      if (this.SortOrder != PhotoSearchSortOrder.None)
        parameters.Add("sort", this.SortOrderString);
      if (this.PrivacyFilter != PrivacyFilter.None)
        parameters.Add("privacy_filter", this.PrivacyFilter.ToString("d"));
      if (this.BoundaryBox != null && this.BoundaryBox.IsSet)
        parameters.Add("bbox", this.BoundaryBox.ToString());
      if (this.Accuracy != GeoAccuracy.None)
        parameters.Add("accuracy", this.Accuracy.ToString("d"));
      if (this.SafeSearch != SafetyLevel.None)
        parameters.Add("safe_search", this.SafeSearch.ToString("d"));
      if (this.ContentType != ContentTypeSearch.None)
        parameters.Add("content_type", this.ContentType.ToString("d"));
      if (this.HasGeo.HasValue)
        parameters.Add("has_geo", this.HasGeo.Value ? "1" : "0");
      if (this.Latitude.HasValue)
        parameters.Add("lat", this.Latitude.Value.ToString((IFormatProvider) NumberFormatInfo.InvariantInfo));
      if (this.Longitude.HasValue)
        parameters.Add("lon", this.Longitude.Value.ToString((IFormatProvider) NumberFormatInfo.InvariantInfo));
      if (this.Radius.HasValue)
        parameters.Add("radius", this.Radius.Value.ToString("0.00000", (IFormatProvider) NumberFormatInfo.InvariantInfo));
      if (this.RadiusUnits != RadiusUnit.None)
        parameters.Add("radius_units", this.RadiusUnits == RadiusUnit.None ? "mi" : "km");
      if (this.Contacts != ContactSearch.None)
        parameters.Add("contacts", this.Contacts == ContactSearch.AllContacts ? "all" : "ff");
      if (this.WoeId != null)
        parameters.Add("woe_id", this.WoeId);
      if (this.PlaceId != null)
        parameters.Add("place_id", this.PlaceId);
      if (this.IsCommons)
        parameters.Add("is_commons", "1");
      if (this.InGallery)
        parameters.Add("in_gallery", "1");
      if (this.IsGetty)
        parameters.Add("is_getty", "1");
      if (this.MediaType != MediaType.None)
        parameters.Add("media", UtilityMethods.MediaTypeToString(this.MediaType));
      if (this.GeoContext != GeoContext.NotDefined)
        parameters.Add("geo_context", this.GeoContext.ToString("d"));
      if (this.Faves)
        parameters.Add("faves", "1");
      if (this.PersonId != null)
        parameters.Add("person_id", this.PersonId);
      if (this.Camera != null)
        parameters.Add("camera", this.Camera);
      if (this.JumpTo == null)
        return;
      parameters.Add("jump_to", this.JumpTo);
    }
  }
}
