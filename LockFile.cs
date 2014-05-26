// Type: FlickrNet.LockFile
// Assembly: FlickrNet, Version=3.10.0.0, Culture=neutral, PublicKeyToken=2491df59efa5d132
// MVID: 8B8C756F-D5B2-4047-8A8B-B1C1E877A2EF
// Assembly location: D:\RMCK\vs\2014.05.24_FlickrAPI\FlickrNet.dll

using System;
using System.IO;
using System.Threading;

namespace FlickrNet
{
  internal class LockFile : IDisposable
  {
    private readonly string filepath;
    private readonly LockFile.DisposeHelper disposeHelper;
    private Stream stream;

    public LockFile(string filepath)
    {
      this.filepath = filepath;
      this.disposeHelper = new LockFile.DisposeHelper(this);
    }

    public IDisposable Acquire()
    {
      string directoryName = Path.GetDirectoryName(this.filepath);
      lock (this)
      {
        while (this.stream != null)
          Monitor.Wait((object) this);
        while (true)
        {
          if (!Directory.Exists(directoryName))
            Directory.CreateDirectory(directoryName);
          try
          {
            this.stream = (Stream) new FileStream(this.filepath, FileMode.OpenOrCreate, FileAccess.Read, FileShare.None);
            return (IDisposable) this.disposeHelper;
          }
          catch (IOException exception_0)
          {
            switch (SafeNativeMethods.GetErrorCode(exception_0))
            {
              case 32:
              case 33:
              case 5664:
              case 5665:
                Thread.Sleep(50);
                continue;
              default:
                throw;
            }
          }
        }
      }
    }

    internal void Release()
    {
      lock (this)
      {
        Monitor.PulseAll((object) this);
        if (this.stream == null)
          throw new InvalidOperationException("Tried to dispose a FileLock that was not owned");
        try
        {
          this.stream.Close();
          try
          {
            File.Delete(this.filepath);
          }
          catch (IOException exception_0)
          {
          }
        }
        finally
        {
          this.stream = (Stream) null;
        }
      }
    }

    public void Dispose()
    {
      if (this.disposeHelper != null)
        this.disposeHelper.Dispose();
      if (this.stream == null)
        return;
      this.stream.Dispose();
    }

    private class DisposeHelper : IDisposable
    {
      private readonly LockFile lockFile;

      public DisposeHelper(LockFile lockFile)
      {
        this.lockFile = lockFile;
      }

      public void Dispose()
      {
        this.lockFile.Release();
      }
    }
  }
}
