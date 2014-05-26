// Type: FlickrNet.UtilityMethods
// Assembly: FlickrNet, Version=3.10.0.0, Culture=neutral, PublicKeyToken=2491df59efa5d132
// MVID: 8B8C756F-D5B2-4047-8A8B-B1C1E877A2EF
// Assembly location: D:\RMCK\vs\2014.05.24_FlickrAPI\FlickrNet.dll

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;

namespace FlickrNet
{
  internal static class UtilityMethods
  {
    private static readonly DateTime UnixStartDate = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
    private const string PhotoUrlFormat = "https://farm{0}.staticflickr.com/{1}/{2}_{3}{4}.{5}";

    static UtilityMethods()
    {
    }

    public static string AuthLevelToString(AuthLevel level)
    {
      switch (level)
      {
        case AuthLevel.None:
          return "none";
        case AuthLevel.Read:
          return "read";
        case AuthLevel.Write:
          return "write";
        case AuthLevel.Delete:
          return "delete";
        default:
          return string.Empty;
      }
    }

    public static string TagModeToString(TagMode tagMode)
    {
      switch (tagMode)
      {
        case TagMode.None:
          return string.Empty;
        case TagMode.AnyTag:
          return "any";
        case TagMode.AllTags:
          return "all";
        case TagMode.Boolean:
          return "bool";
        default:
          return string.Empty;
      }
    }

    public static string MachineTagModeToString(MachineTagMode machineTagMode)
    {
      switch (machineTagMode)
      {
        case MachineTagMode.None:
          return string.Empty;
        case MachineTagMode.AnyTag:
          return "any";
        case MachineTagMode.AllTags:
          return "all";
        default:
          return string.Empty;
      }
    }

    public static string UrlEncode(string data)
    {
      return Uri.EscapeDataString(data);
    }

    public static string DateToUnixTimestamp(DateTime date)
    {
      return (date - UtilityMethods.UnixStartDate).TotalSeconds.ToString("0", (IFormatProvider) NumberFormatInfo.InvariantInfo);
    }

    public static DateTime UnixTimestampToDate(string timestamp)
    {
      if (string.IsNullOrEmpty(timestamp))
        return DateTime.MinValue;
      try
      {
        return UtilityMethods.UnixTimestampToDate(long.Parse(timestamp, NumberStyles.Any, (IFormatProvider) NumberFormatInfo.InvariantInfo));
      }
      catch (FormatException ex)
      {
        return DateTime.MinValue;
      }
    }

    public static DateTime UnixTimestampToDate(long timestamp)
    {
      return UtilityMethods.UnixStartDate.AddSeconds((double) timestamp);
    }

    public static string ExtrasToString(PhotoSearchExtras extras)
    {
      List<string> list = new List<string>();
      Type type = typeof (PhotoSearchExtras);
      foreach (PhotoSearchExtras photoSearchExtras in UtilityMethods.GetFlags((Enum) extras))
      {
        object[] customAttributes = type.GetField(photoSearchExtras.ToString("G")).GetCustomAttributes(typeof (DescriptionAttribute), false);
        if (customAttributes.Length != 0)
        {
          DescriptionAttribute descriptionAttribute = (DescriptionAttribute) customAttributes[0];
          list.Add(descriptionAttribute.Description);
        }
      }
      return string.Join(",", list.ToArray());
    }

    private static IEnumerable<Enum> GetFlags(Enum input)
    {
      long i = Convert.ToInt64((object) input);
      foreach (Enum @enum in UtilityMethods.GetValues(input))
      {
        if ((i & Convert.ToInt64((object) @enum)) != 0L)
          yield return @enum;
      }
    }

    private static IEnumerable<Enum> GetValues(Enum enumeration)
    {
      List<Enum> list = new List<Enum>();
      foreach (FieldInfo fieldInfo in enumeration.GetType().GetFields(BindingFlags.Static | BindingFlags.Public))
        list.Add((Enum) fieldInfo.GetValue((object) enumeration));
      return (IEnumerable<Enum>) list;
    }

    public static string SortOrderToString(PhotoSearchSortOrder order)
    {
      switch (order)
      {
        case PhotoSearchSortOrder.DatePostedAscending:
          return "date-posted-asc";
        case PhotoSearchSortOrder.DatePostedDescending:
          return "date-posted-desc";
        case PhotoSearchSortOrder.DateTakenAscending:
          return "date-taken-asc";
        case PhotoSearchSortOrder.DateTakenDescending:
          return "date-taken-desc";
        case PhotoSearchSortOrder.InterestingnessAscending:
          return "interestingness-asc";
        case PhotoSearchSortOrder.InterestingnessDescending:
          return "interestingness-desc";
        case PhotoSearchSortOrder.Relevance:
          return "relevance";
        default:
          return string.Empty;
      }
    }

    public static string SortOrderToString(PopularitySort sortOrder)
    {
      switch (sortOrder)
      {
        case PopularitySort.Views:
          return "views";
        case PopularitySort.Comments:
          return "comments";
        case PopularitySort.Favorites:
          return "favorites";
        default:
          return string.Empty;
      }
    }

    public static void PartialOptionsIntoArray(PartialSearchOptions options, Dictionary<string, string> parameters)
    {
      if (options == null)
        throw new ArgumentNullException("options");
      if (parameters == null)
        throw new ArgumentNullException("parameters");
      if (options.MinUploadDate != DateTime.MinValue)
        parameters.Add("min_uploaded_date", ((object) UtilityMethods.DateToUnixTimestamp(options.MinUploadDate)).ToString());
      if (options.MaxUploadDate != DateTime.MinValue)
        parameters.Add("max_uploaded_date", ((object) UtilityMethods.DateToUnixTimestamp(options.MaxUploadDate)).ToString());
      if (options.MinTakenDate != DateTime.MinValue)
        parameters.Add("min_taken_date", UtilityMethods.DateToMySql(options.MinTakenDate));
      if (options.MaxTakenDate != DateTime.MinValue)
        parameters.Add("max_taken_date", UtilityMethods.DateToMySql(options.MaxTakenDate));
      if (options.Extras != PhotoSearchExtras.None)
        parameters.Add("extras", options.ExtrasString);
      if (options.SortOrder != PhotoSearchSortOrder.None)
        parameters.Add("sort", options.SortOrderString);
      if (options.PerPage > 0)
        parameters.Add("per_page", options.PerPage.ToString((IFormatProvider) NumberFormatInfo.InvariantInfo));
      if (options.Page > 0)
        parameters.Add("page", options.Page.ToString((IFormatProvider) NumberFormatInfo.InvariantInfo));
      if (options.PrivacyFilter == PrivacyFilter.None)
        return;
      parameters.Add("privacy_filter", options.PrivacyFilter.ToString("d"));
    }

    internal static void WriteInt32(Stream s, int i)
    {
      s.WriteByte((byte) (i & (int) byte.MaxValue));
      s.WriteByte((byte) (i >> 8 & (int) byte.MaxValue));
      s.WriteByte((byte) (i >> 16 & (int) byte.MaxValue));
      s.WriteByte((byte) (i >> 24 & (int) byte.MaxValue));
    }

    internal static void WriteString(Stream s, string str)
    {
      UtilityMethods.WriteInt32(s, str.Length);
      foreach (char ch in str)
      {
        s.WriteByte((byte) ((uint) ch & (uint) byte.MaxValue));
        s.WriteByte((byte) ((int) ch >> 8 & (int) byte.MaxValue));
      }
    }

    internal static int ReadInt32(Stream s)
    {
      int num1 = 0;
      for (int index = 0; index < 4; ++index)
      {
        int num2 = s.ReadByte();
        if (num2 == -1)
          throw new IOException("Unexpected EOF encountered");
        num1 |= num2 << index * 8;
      }
      return num1;
    }

    internal static string ReadString(Stream s)
    {
      int length = UtilityMethods.ReadInt32(s);
      char[] chArray = new char[length];
      for (int index = 0; index < length; ++index)
      {
        int num1 = s.ReadByte();
        int num2 = s.ReadByte();
        if (num1 == -1 || num2 == -1)
          throw new IOException("Unexpected EOF encountered");
        chArray[index] = (char) (num1 | num2 << 8);
      }
      return new string(chArray);
    }

    internal static string UrlFormat(Photo p, string size, string extension)
    {
      if (size == "_o" || size == "original")
        return UtilityMethods.UrlFormat(p.Farm, p.Server, p.PhotoId, p.OriginalSecret, size, extension);
      else
        return UtilityMethods.UrlFormat(p.Farm, p.Server, p.PhotoId, p.Secret, size, extension);
    }

    internal static string UrlFormat(PhotoInfo p, string size, string extension)
    {
      if (size == "_o" || size == "original")
        return UtilityMethods.UrlFormat(p.Farm, p.Server, p.PhotoId, p.OriginalSecret, size, extension);
      else
        return UtilityMethods.UrlFormat(p.Farm, p.Server, p.PhotoId, p.Secret, size, extension);
    }

    internal static string UrlFormat(Photoset p, string size, string extension)
    {
      return UtilityMethods.UrlFormat(p.Farm, p.Server, p.PrimaryPhotoId, p.Secret, size, extension);
    }

    internal static string UrlFormat(string farm, string server, string photoid, string secret, string size, string extension)
    {
      string str;
      switch (size)
      {
        case "square":
          str = "_s";
          break;
        case "thumbnail":
          str = "_t";
          break;
        case "small":
          str = "_m";
          break;
        case "large":
          str = "_b";
          break;
        case "original":
          str = "_o";
          break;
        default:
          str = string.Empty;
          break;
      }
      return UtilityMethods.UrlFormat("https://farm{0}.staticflickr.com/{1}/{2}_{3}{4}.{5}", (object) farm, (object) server, (object) photoid, (object) secret, (object) str, (object) extension);
    }

    private static string UrlFormat(string format, params object[] parameters)
    {
      return string.Format((IFormatProvider) CultureInfo.InvariantCulture, format, parameters);
    }

    internal static MemberTypes ParseIdToMemberType(string memberTypeId)
    {
      switch (memberTypeId)
      {
        case "1":
          return MemberTypes.Narwhal;
        case "2":
          return MemberTypes.Member;
        case "3":
          return MemberTypes.Moderator;
        case "4":
          return MemberTypes.Admin;
        default:
          return MemberTypes.None;
      }
    }

    internal static MemberTypes ParseRoleToMemberType(string memberRole)
    {
      switch (memberRole)
      {
        case "admin":
          return MemberTypes.Admin;
        case "moderator":
          return MemberTypes.Moderator;
        case "member":
          return MemberTypes.Member;
        default:
          return MemberTypes.None;
      }
    }

    internal static string MemberTypeToString(MemberTypes memberTypes)
    {
      List<string> list = new List<string>();
      if ((memberTypes & MemberTypes.Narwhal) == MemberTypes.Narwhal)
        list.Add("1");
      if ((memberTypes & MemberTypes.Member) == MemberTypes.Member)
        list.Add("2");
      if ((memberTypes & MemberTypes.Moderator) == MemberTypes.Moderator)
        list.Add("3");
      if ((memberTypes & MemberTypes.Admin) == MemberTypes.Admin)
        list.Add("4");
      return string.Join(",", list.ToArray());
    }

    public static string MD5Hash(string data)
    {
      byte[] hash;
      using (MD5CryptoServiceProvider cryptoServiceProvider = new MD5CryptoServiceProvider())
      {
        byte[] bytes = Encoding.UTF8.GetBytes(data);
        hash = cryptoServiceProvider.ComputeHash(bytes, 0, bytes.Length);
      }
      return BitConverter.ToString(hash).Replace("-", string.Empty).ToLower(CultureInfo.InvariantCulture);
    }

    internal static DateTime MySqlToDate(string p)
    {
      string format1 = "yyyy-MM-dd";
      string format2 = "yyyy-MM-dd hh:mm:ss";
      DateTimeFormatInfo invariantInfo = DateTimeFormatInfo.InvariantInfo;
      try
      {
        return DateTime.ParseExact(p, format1, (IFormatProvider) invariantInfo, DateTimeStyles.None);
      }
      catch (FormatException ex)
      {
      }
      try
      {
        return DateTime.ParseExact(p, format2, (IFormatProvider) invariantInfo, DateTimeStyles.None);
      }
      catch (FormatException ex)
      {
      }
      return DateTime.MinValue;
    }

    public static DateTime ParseDateWithGranularity(string date)
    {
      DateTime dateTime = DateTime.MinValue;
      if (string.IsNullOrEmpty(date) || date == "0000-00-00 00:00:00")
        return dateTime;
      if (date.EndsWith("-00-01 00:00:00"))
      {
        dateTime = new DateTime(int.Parse(date.Substring(0, 4), (IFormatProvider) NumberFormatInfo.InvariantInfo), 1, 1);
        return dateTime;
      }
      else
      {
        string format = "yyyy-MM-dd HH:mm:ss";
        try
        {
          dateTime = DateTime.ParseExact(date, format, (IFormatProvider) DateTimeFormatInfo.InvariantInfo, DateTimeStyles.None);
        }
        catch (FormatException ex)
        {
        }
        return dateTime;
      }
    }

    internal static string DateToMySql(DateTime date)
    {
      return date.ToString("yyyy-MM-dd HH:mm:ss", (IFormatProvider) DateTimeFormatInfo.InvariantInfo);
    }

    public static string MediaTypeToString(MediaType mediaType)
    {
      switch (mediaType)
      {
        case MediaType.All:
          return "all";
        case MediaType.Photos:
          return "photos";
        case MediaType.Videos:
          return "videos";
        default:
          return string.Empty;
      }
    }

    [Conditional("DEBUG")]
    public static void CheckParsingException(XmlReader reader)
    {
      if (reader.NodeType == XmlNodeType.Attribute)
        throw new ParsingException("Unknown attribute: " + reader.Name + "=" + reader.Value);
      if (string.IsNullOrEmpty(reader.Value))
        throw new ParsingException("Unknown element: " + reader.Name);
      throw new ParsingException("Unknown " + ((object) reader.NodeType).ToString() + ": " + reader.Name + "=" + reader.Value);
    }

    public static string BuddyIcon(string server, string farm, string userId)
    {
      if (string.IsNullOrEmpty(server) || server == "0")
        return "https://www.flickr.com/images/buddyicon.jpg";
      return string.Format((IFormatProvider) CultureInfo.InvariantCulture, "https://farm{0}.staticflickr.com/{1}/buddyicons/{2}.jpg", (object) farm, (object) server, (object) userId);
    }

    public static Dictionary<string, string> StringToDictionary(string response)
    {
      Dictionary<string, string> dictionary = new Dictionary<string, string>();
      if (string.IsNullOrEmpty(response))
        return dictionary;
      string str1 = response;
      char[] chArray = new char[1]
      {
        '&'
      };
      foreach (string str2 in str1.Split(chArray))
      {
        char[] separator = new char[1]
        {
          '='
        };
        int count = 2;
        int num = 1;
        string[] strArray = str2.Split(separator, count, (StringSplitOptions) num);
        dictionary.Add(strArray[0], strArray.Length == 1 ? "" : Uri.UnescapeDataString(strArray[1]));
      }
      return dictionary;
    }

    public static string EscapeOAuthString(string text)
    {
      return Regex.Replace(Uri.EscapeDataString(text).Replace("+", "%20"), "(%[0-9a-f][0-9a-f])", (MatchEvaluator) (c => c.Value.ToUpper())).Replace("(", "%28").Replace(")", "%29").Replace("$", "%24").Replace("!", "%21").Replace("*", "%2A").Replace("'", "%27").Replace("%7E", "~");
    }

    internal static string CleanCollectionId(string collectionId)
    {
      if (collectionId.IndexOf("-", StringComparison.Ordinal) >= 0)
        return collectionId.Substring(collectionId.IndexOf("-", StringComparison.Ordinal) + 1);
      else
        return collectionId;
    }
  }
}
