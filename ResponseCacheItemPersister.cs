// Type: FlickrNet.ResponseCacheItemPersister
// Assembly: FlickrNet, Version=3.10.0.0, Culture=neutral, PublicKeyToken=2491df59efa5d132
// MVID: 8B8C756F-D5B2-4047-8A8B-B1C1E877A2EF
// Assembly location: D:\RMCK\vs\2014.05.24_FlickrAPI\FlickrNet.dll

using System;
using System.Globalization;
using System.IO;
using System.Text;

namespace FlickrNet
{
  internal class ResponseCacheItemPersister : CacheItemPersister
  {
    public override ICacheItem Read(Stream inputStream)
    {
      string str = UtilityMethods.ReadString(inputStream);
      string response = UtilityMethods.ReadString(inputStream);
      string[] strArray = str.Split(new char[1]
      {
        '\n'
      });
      if (strArray.Length != 2)
        throw new IOException("Unexpected number of chunks found");
      string uriString = strArray[0];
      DateTime creationTime = new DateTime(long.Parse(strArray[1], NumberStyles.Any, (IFormatProvider) NumberFormatInfo.InvariantInfo));
      return (ICacheItem) new ResponseCacheItem(new Uri(uriString), response, creationTime);
    }

    public override void Write(Stream outputStream, ICacheItem cacheItem)
    {
      ResponseCacheItem responseCacheItem = (ResponseCacheItem) cacheItem;
      StringBuilder stringBuilder = new StringBuilder();
      stringBuilder.Append(responseCacheItem.Url.AbsoluteUri + "\n");
      stringBuilder.Append(responseCacheItem.CreationTime.Ticks.ToString((IFormatProvider) NumberFormatInfo.InvariantInfo));
      UtilityMethods.WriteString(outputStream, ((object) stringBuilder).ToString());
      UtilityMethods.WriteString(outputStream, responseCacheItem.Response);
    }
  }
}
