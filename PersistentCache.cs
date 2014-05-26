// Type: FlickrNet.PersistentCache
// Assembly: FlickrNet, Version=3.10.0.0, Culture=neutral, PublicKeyToken=2491df59efa5d132
// MVID: 8B8C756F-D5B2-4047-8A8B-B1C1E877A2EF
// Assembly location: D:\RMCK\vs\2014.05.24_FlickrAPI\FlickrNet.dll

using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;

namespace FlickrNet
{
  public sealed class PersistentCache : IDisposable
  {
    private Dictionary<string, ICacheItem> dataTable = new Dictionary<string, ICacheItem>();
    private readonly CacheItemPersister persister;
    private bool dirty;
    private readonly FileInfo dataFile;
    private DateTime timestamp;
    private long length;
    private readonly LockFile lockFile;

    public ICacheItem this[string key]
    {
      set
      {
        if (key == null)
          throw new ArgumentNullException("key");
        ICacheItem cacheItem;
        using (this.lockFile.Acquire())
        {
          this.Refresh();
          cacheItem = this.InternalSet(key, value);
          this.Persist();
        }
        if (cacheItem == null)
          return;
        cacheItem.OnItemFlushed();
      }
    }

    public PersistentCache(string filename, CacheItemPersister persister)
    {
      this.persister = persister;
      this.dataFile = new FileInfo(filename);
      this.lockFile = new LockFile(filename + ".lock");
    }

    public ICacheItem[] ToArray(Type valueType)
    {
      using (this.lockFile.Acquire())
      {
        this.Refresh();
        string[] keys;
        Array values;
        this.InternalGetAll(valueType, out keys, out values);
        return (ICacheItem[]) values;
      }
    }

    public ICacheItem Get(string key, TimeSpan maxAge, bool removeIfExpired)
    {
      ICacheItem cacheItem;
      bool flag;
      using (this.lockFile.Acquire())
      {
        this.Refresh();
        cacheItem = this.InternalGet(key);
        flag = cacheItem != null && PersistentCache.Expired(cacheItem.CreationTime, maxAge);
        if (flag)
        {
          if (removeIfExpired)
          {
            cacheItem = this.RemoveKey(key);
            this.Persist();
          }
          else
            cacheItem = (ICacheItem) null;
        }
      }
      if (flag && removeIfExpired)
        cacheItem.OnItemFlushed();
      if (!flag)
        return cacheItem;
      else
        return (ICacheItem) null;
    }

    public void Flush()
    {
      this.Shrink(0L);
    }

    public void Shrink(long size)
    {
      if (size < 0L)
        throw new ArgumentException("Cannot shrink to a negative size", "size");
      List<ICacheItem> list = new List<ICacheItem>();
      using (this.lockFile.Acquire())
      {
        this.Refresh();
        string[] keys;
        Array values;
        this.InternalGetAll(typeof (ICacheItem), out keys, out values);
        long num = 0L;
        foreach (ICacheItem cacheItem in values)
          num += cacheItem.FileSize;
        for (int index = 0; index < keys.Length && num > size; ++index)
        {
          ICacheItem cacheItem = (ICacheItem) values.GetValue(index);
          num -= cacheItem.FileSize;
          list.Add(this.RemoveKey(keys[index]));
        }
        this.Persist();
      }
      foreach (ICacheItem cacheItem in list)
      {
        if (cacheItem != null)
          cacheItem.OnItemFlushed();
      }
    }

    private static bool Expired(DateTime test, TimeSpan age)
    {
      if (age == TimeSpan.MinValue)
        return true;
      if (age == TimeSpan.MaxValue)
        return false;
      else
        return test < DateTime.UtcNow - age;
    }

    private void InternalGetAll(Type valueType, out string[] keys, out Array values)
    {
      if (!typeof (ICacheItem).IsAssignableFrom(valueType))
        throw new ArgumentException("Type " + valueType.FullName + " does not implement ICacheItem", "valueType");
      keys = new List<string>((IEnumerable<string>) this.dataTable.Keys).ToArray();
      values = Array.CreateInstance(valueType, keys.Length);
      for (int index = 0; index < keys.Length; ++index)
        values.SetValue((object) this.dataTable[keys[index]], index);
      Array.Sort(values, (Array) keys, (IComparer) new PersistentCache.CreationTimeComparer());
    }

    private ICacheItem InternalGet(string key)
    {
      if (key == null)
        throw new ArgumentNullException("key");
      if (this.dataTable.ContainsKey(key))
        return this.dataTable[key];
      else
        return (ICacheItem) null;
    }

    private ICacheItem InternalSet(string key, ICacheItem value)
    {
      if (key == null)
        throw new ArgumentNullException("key");
      ICacheItem cacheItem = this.RemoveKey(key);
      if (value != null)
        this.dataTable[key] = value;
      this.dirty = this.dirty || !object.ReferenceEquals((object) cacheItem, (object) value);
      return cacheItem;
    }

    private ICacheItem RemoveKey(string key)
    {
      if (!this.dataTable.ContainsKey(key))
        return (ICacheItem) null;
      ICacheItem cacheItem = this.dataTable[key];
      this.dataTable.Remove(key);
      this.dirty = true;
      return cacheItem;
    }

    private void Refresh()
    {
      DateTime dateTime = DateTime.MinValue;
      long num = -1L;
      this.dataFile.Refresh();
      if (this.dataFile.Exists)
      {
        dateTime = this.dataFile.LastWriteTime;
        num = this.dataFile.Length;
      }
      if (this.timestamp != dateTime || this.length != num)
      {
        if (!this.dataFile.Exists)
        {
          this.dataTable.Clear();
        }
        else
        {
          using (FileStream fileStream = this.dataFile.Open(FileMode.Open, FileAccess.Read, FileShare.Read))
            this.dataTable = this.Load((Stream) fileStream);
        }
      }
      this.timestamp = dateTime;
      this.length = num;
      this.dirty = false;
    }

    private void Persist()
    {
      if (!this.dirty)
        return;
      using (FileStream fileStream = this.dataFile.Open(FileMode.OpenOrCreate, FileAccess.Write, FileShare.None))
        this.Store((Stream) fileStream, this.dataTable);
      this.dataFile.Refresh();
      this.timestamp = this.dataFile.LastWriteTime;
      this.length = this.dataFile.Length;
      this.dirty = false;
    }

    private Dictionary<string, ICacheItem> Load(Stream s)
    {
      Dictionary<string, ICacheItem> dictionary = new Dictionary<string, ICacheItem>();
      int num = UtilityMethods.ReadInt32(s);
      for (int index1 = 0; index1 < num; ++index1)
      {
        try
        {
          string index2 = UtilityMethods.ReadString(s);
          ICacheItem cacheItem = this.persister.Read(s);
          if (cacheItem == null)
            return dictionary;
          dictionary[index2] = cacheItem;
        }
        catch (IOException ex)
        {
          return dictionary;
        }
      }
      return dictionary;
    }

    private void Store(Stream s, Dictionary<string, ICacheItem> table)
    {
      UtilityMethods.WriteInt32(s, table.Count);
      foreach (KeyValuePair<string, ICacheItem> keyValuePair in table)
      {
        UtilityMethods.WriteString(s, keyValuePair.Key);
        this.persister.Write(s, keyValuePair.Value);
      }
    }

    void IDisposable.Dispose()
    {
      if (this.lockFile == null)
        return;
      this.lockFile.Dispose();
    }

    private class CreationTimeComparer : IComparer
    {
      public int Compare(object x, object y)
      {
        return ((ICacheItem) x).CreationTime.CompareTo(((ICacheItem) y).CreationTime);
      }
    }
  }
}
