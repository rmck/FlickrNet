// Type: FlickrNet.SafeNativeMethods
// Assembly: FlickrNet, Version=3.10.0.0, Culture=neutral, PublicKeyToken=2491df59efa5d132
// MVID: 8B8C756F-D5B2-4047-8A8B-B1C1E877A2EF
// Assembly location: D:\RMCK\vs\2014.05.24_FlickrAPI\FlickrNet.dll

using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Security;
using System.Security.Permissions;

namespace FlickrNet
{
  [SuppressUnmanagedCodeSecurity]
  internal class SafeNativeMethods
  {
    private SafeNativeMethods()
    {
    }

    internal static int GetErrorCode(IOException ioe)
    {
      new SecurityPermission(SecurityPermissionFlag.UnmanagedCode).Demand();
      return Marshal.GetHRForException((Exception) ioe) & (int) ushort.MaxValue;
    }
  }
}
