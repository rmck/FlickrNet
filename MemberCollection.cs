// Type: FlickrNet.MemberCollection
// Assembly: FlickrNet, Version=3.10.0.0, Culture=neutral, PublicKeyToken=2491df59efa5d132
// MVID: 8B8C756F-D5B2-4047-8A8B-B1C1E877A2EF
// Assembly location: D:\RMCK\vs\2014.05.24_FlickrAPI\FlickrNet.dll

using System;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Xml;

namespace FlickrNet
{
  public sealed class MemberCollection : Collection<Member>, IFlickrParsable
  {
    public int Page { get; set; }

    public int Pages { get; set; }

    public int Total { get; set; }

    public int PerPage { get; set; }

    void IFlickrParsable.Load(XmlReader reader)
    {
      if (reader.GetAttribute("page") != null)
        this.Page = int.Parse(reader.GetAttribute("page"), (IFormatProvider) CultureInfo.InvariantCulture);
      if (reader.GetAttribute("pages") != null)
        this.Pages = int.Parse(reader.GetAttribute("pages"), (IFormatProvider) CultureInfo.InvariantCulture);
      if (reader.GetAttribute("perpage") != null)
        this.PerPage = int.Parse(reader.GetAttribute("perpage"), (IFormatProvider) CultureInfo.InvariantCulture);
      if (reader.GetAttribute("total") != null)
        this.Total = int.Parse(reader.GetAttribute("total"), (IFormatProvider) CultureInfo.InvariantCulture);
      while (reader.Read())
      {
        if (reader.Name == "member")
        {
          Member member = new Member();
          member.Load(reader);
          this.Add(member);
        }
      }
    }
  }
}
