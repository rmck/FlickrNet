// Type: FlickrNet.PhotoSearchExtras
// Assembly: FlickrNet, Version=3.10.0.0, Culture=neutral, PublicKeyToken=2491df59efa5d132
// MVID: 8B8C756F-D5B2-4047-8A8B-B1C1E877A2EF
// Assembly location: D:\RMCK\vs\2014.05.24_FlickrAPI\FlickrNet.dll

using System;
using System.ComponentModel;

namespace FlickrNet
{
  [Flags]
  [Serializable]
  public enum PhotoSearchExtras
  {
    [Description("")] None = 0,
    [Description("license")] License = 1,
    [Description("date_upload")] DateUploaded = 2,
    [Description("date_taken")] DateTaken = 4,
    [Description("owner_name")] OwnerName = 8,
    [Description("icon_server")] IconServer = 16,
    [Description("original_format")] OriginalFormat = 32,
    [Description("last_update")] LastUpdated = 64,
    [Description("tags")] Tags = 128,
    [Description("geo")] Geo = 256,
    [Description("machine_tags")] MachineTags = 512,
    [Description("o_dims")] OriginalDimensions = 1024,
    [Description("views")] Views = 2048,
    [Description("media")] Media = 4096,
    [Description("path_alias")] PathAlias = 8192,
    [Description("url_sq")] SquareUrl = 16384,
    [Description("url_t")] ThumbnailUrl = 32768,
    [Description("url_s")] SmallUrl = 65536,
    [Description("url_m")] MediumUrl = 131072,
    [Description("url_l")] LargeUrl = 262144,
    [Description("url_o")] OriginalUrl = 524288,
    [Description("description")] Description = 1048576,
    [Description("usage")] Usage = 2097152,
    [Description("visibility")] Visibility = 4194304,
    [Description("url_q")] LargeSquareUrl = 8388608,
    [Description("url_n")] Small320Url = 16777216,
    [Description("rotation")] Rotation = 33554432,
    [Description("url_h")] Large1600Url = 67108864,
    [Description("url_k")] Large2048Url = 134217728,
    [Description("url_c")] Medium800Url = 268435456,
    [Description("url_z")] Medium640Url = 536870912,
    AllUrls = Medium640Url | Medium800Url | Large2048Url | Large1600Url | Small320Url | LargeSquareUrl | OriginalUrl | LargeUrl | MediumUrl | SmallUrl | ThumbnailUrl | SquareUrl,
    All = AllUrls | Rotation | Visibility | Usage | Description | PathAlias | Media | Views | OriginalDimensions | MachineTags | Geo | Tags | LastUpdated | OriginalFormat | IconServer | OwnerName | DateTaken | DateUploaded | License,
  }
}
