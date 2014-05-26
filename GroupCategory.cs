// Type: FlickrNet.GroupCategory
// Assembly: FlickrNet, Version=3.10.0.0, Culture=neutral, PublicKeyToken=2491df59efa5d132
// MVID: 8B8C756F-D5B2-4047-8A8B-B1C1E877A2EF
// Assembly location: D:\RMCK\vs\2014.05.24_FlickrAPI\FlickrNet.dll

using System.Collections.ObjectModel;
using System.Xml;

namespace FlickrNet
{
  public sealed class GroupCategory : IFlickrParsable
  {
    public string CategoryName { get; set; }

    public string Path { get; set; }

    public string PathIds { get; set; }

    public Collection<Subcategory> Subcategories { get; set; }

    public Collection<Group> Groups { get; set; }

    public GroupCategory()
    {
      this.Subcategories = new Collection<Subcategory>();
      this.Groups = new Collection<Group>();
    }

    void IFlickrParsable.Load(XmlReader reader)
    {
      if (!(reader.LocalName != "category"))
        ;
      while (reader.MoveToNextAttribute())
      {
        switch (reader.LocalName)
        {
          case "name":
            this.CategoryName = reader.Value;
            continue;
          case "path":
            this.Path = reader.Value;
            continue;
          case "pathids":
            this.PathIds = reader.Value;
            continue;
          default:
            continue;
        }
      }
      reader.Read();
      while (reader.LocalName == "subcat" || reader.LocalName == "group")
      {
        if (reader.LocalName == "subcat")
        {
          Subcategory subcategory = new Subcategory();
          subcategory.Load(reader);
          this.Subcategories.Add(subcategory);
        }
        else
        {
          Group group = new Group();
          ((IFlickrParsable) group).Load(reader);
          this.Groups.Add(group);
        }
      }
      reader.Skip();
    }
  }
}
