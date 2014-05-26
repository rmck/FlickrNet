// Type: FlickrNet.SubscriptionCollection
// Assembly: FlickrNet, Version=3.10.0.0, Culture=neutral, PublicKeyToken=2491df59efa5d132
// MVID: 8B8C756F-D5B2-4047-8A8B-B1C1E877A2EF
// Assembly location: D:\RMCK\vs\2014.05.24_FlickrAPI\FlickrNet.dll

using System;
using System.Collections.ObjectModel;
using System.Xml;

namespace FlickrNet
{
  public sealed class SubscriptionCollection : Collection<Subscription>, IFlickrParsable
  {
    void IFlickrParsable.Load(XmlReader reader)
    {
      if (reader == null)
        throw new ArgumentNullException("reader");
      if (reader.LocalName != "subscriptions")
        return;
      reader.Read();
      while (reader.LocalName == "subscription")
      {
        Subscription subscription = new Subscription();
        subscription.Load(reader);
        this.Add(subscription);
      }
      reader.Skip();
    }
  }
}
