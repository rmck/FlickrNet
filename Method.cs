// Type: FlickrNet.Method
// Assembly: FlickrNet, Version=3.10.0.0, Culture=neutral, PublicKeyToken=2491df59efa5d132
// MVID: 8B8C756F-D5B2-4047-8A8B-B1C1E877A2EF
// Assembly location: D:\RMCK\vs\2014.05.24_FlickrAPI\FlickrNet.dll

using System;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Xml;

namespace FlickrNet
{
  public sealed class Method : IFlickrParsable
  {
    public string Name { get; set; }

    public bool NeedsLogin { get; set; }

    public bool NeedsSigning { get; set; }

    public MethodPermission RequiredPermissions { get; set; }

    public string Description { get; set; }

    public string Response { get; set; }

    public string Explanation { get; set; }

    public Collection<MethodArgument> Arguments { get; set; }

    public Collection<MethodError> Errors { get; set; }

    public Method()
    {
      this.Arguments = new Collection<MethodArgument>();
      this.Errors = new Collection<MethodError>();
    }

    void IFlickrParsable.Load(XmlReader reader)
    {
      if (!(reader.LocalName != "method"))
        ;
      while (reader.MoveToNextAttribute())
      {
        switch (reader.LocalName)
        {
          case "name":
            this.Name = reader.Value;
            continue;
          case "needslogin":
            this.NeedsLogin = reader.Value == "1";
            continue;
          case "needssigning":
            this.NeedsSigning = reader.Value == "1";
            continue;
          case "requiredperms":
            this.RequiredPermissions = (MethodPermission) int.Parse(reader.Value, (IFormatProvider) NumberFormatInfo.InvariantInfo);
            continue;
          default:
            continue;
        }
      }
      reader.Read();
      while (reader.LocalName != "method")
      {
        switch (reader.LocalName)
        {
          case "description":
            this.Description = reader.ReadElementContentAsString();
            continue;
          case "response":
            this.Response = reader.ReadElementContentAsString();
            continue;
          case "explanation":
            this.Explanation = reader.ReadElementContentAsString();
            continue;
          default:
            continue;
        }
      }
      reader.ReadToFollowing("argument");
      while (reader.LocalName == "argument")
      {
        MethodArgument methodArgument = new MethodArgument();
        methodArgument.Load(reader);
        this.Arguments.Add(methodArgument);
      }
      reader.ReadToFollowing("error");
      while (reader.LocalName == "error")
      {
        MethodError methodError = new MethodError();
        methodError.Load(reader);
        this.Errors.Add(methodError);
      }
      reader.Read();
      reader.Skip();
    }
  }
}
