// Type: FlickrNet.BoundaryBox
// Assembly: FlickrNet, Version=3.10.0.0, Culture=neutral, PublicKeyToken=2491df59efa5d132
// MVID: 8B8C756F-D5B2-4047-8A8B-B1C1E877A2EF
// Assembly location: D:\RMCK\vs\2014.05.24_FlickrAPI\FlickrNet.dll

using System;
using System.Globalization;

namespace FlickrNet
{
  public class BoundaryBox
  {
    private GeoAccuracy accuracy = GeoAccuracy.Street;
    private double minimumLat = -90.0;
    private double minimumLon = -180.0;
    private double maximumLat = 90.0;
    private double maximumLon = 180.0;
    private bool isSet;

    public static BoundaryBox UK
    {
      get
      {
        return new BoundaryBox(-11.118164, 49.809632, 1.625977, 62.613562);
      }
    }

    public static BoundaryBox UKNewcastle
    {
      get
      {
        return new BoundaryBox(-1.71936, 54.935821, -1.389771, 55.145919);
      }
    }

    public static BoundaryBox Usa
    {
      get
      {
        return new BoundaryBox(-130.429687, 22.43134, -58.535156, 49.382373);
      }
    }

    public static BoundaryBox Canada
    {
      get
      {
        return new BoundaryBox(-143.085937, 41.640078, -58.535156, 73.578167);
      }
    }

    public static BoundaryBox World
    {
      get
      {
        return new BoundaryBox(-180.0, -90.0, 180.0, 90.0);
      }
    }

    public GeoAccuracy Accuracy
    {
      get
      {
        return this.accuracy;
      }
      set
      {
        this.accuracy = value;
      }
    }

    public double MinimumLatitude
    {
      get
      {
        return this.minimumLat;
      }
      set
      {
        if (value < -90.0 || value > 90.0)
          throw new ArgumentOutOfRangeException("value", "Must be between -90 and 90");
        this.isSet = true;
        this.minimumLat = value;
      }
    }

    public double MinimumLongitude
    {
      get
      {
        return this.minimumLon;
      }
      set
      {
        if (value < -180.0 || value > 180.0)
          throw new ArgumentOutOfRangeException("value", "Must be between -180 and 180");
        this.isSet = true;
        this.minimumLon = value;
      }
    }

    public double MaximumLatitude
    {
      get
      {
        return this.maximumLat;
      }
      set
      {
        if (value < -90.0 || value > 90.0)
          throw new ArgumentOutOfRangeException("value", "Must be between -90 and 90");
        this.isSet = true;
        this.maximumLat = value;
      }
    }

    public double MaximumLongitude
    {
      get
      {
        return this.maximumLon;
      }
      set
      {
        if (value < -180.0 || value > 180.0)
          throw new ArgumentOutOfRangeException("value", "Must be between -180 and 180");
        this.isSet = true;
        this.maximumLon = value;
      }
    }

    internal bool IsSet
    {
      get
      {
        return this.isSet;
      }
    }

    public BoundaryBox()
    {
    }

    public BoundaryBox(GeoAccuracy accuracy)
    {
      this.Accuracy = accuracy;
    }

    public BoundaryBox(string points, GeoAccuracy accuracy)
      : this(points)
    {
      this.Accuracy = accuracy;
    }

    public BoundaryBox(string points)
    {
      if (points == null)
        throw new ArgumentNullException("points");
      string[] strArray = points.Split(new char[1]
      {
        ','
      });
      if (strArray.Length != 4)
        throw new ArgumentException("Parameter must contain 4 values, seperated by commas.", "points");
      try
      {
        this.MinimumLongitude = double.Parse(strArray[0], (IFormatProvider) NumberFormatInfo.InvariantInfo);
        this.MinimumLatitude = double.Parse(strArray[1], (IFormatProvider) NumberFormatInfo.InvariantInfo);
        this.MaximumLongitude = double.Parse(strArray[2], (IFormatProvider) NumberFormatInfo.InvariantInfo);
        this.MaximumLatitude = double.Parse(strArray[3], (IFormatProvider) NumberFormatInfo.InvariantInfo);
      }
      catch (FormatException ex)
      {
        throw new ArgumentException("Unable to parse points as integer values", "points");
      }
    }

    public BoundaryBox(double minimumLongitude, double minimumLatitude, double maximumLongitude, double maximumLatitude, GeoAccuracy accuracy)
      : this(minimumLongitude, minimumLatitude, maximumLongitude, maximumLatitude)
    {
      this.Accuracy = accuracy;
    }

    public BoundaryBox(double minimumLongitude, double minimumLatitude, double maximumLongitude, double maximumLatitude)
    {
      this.MinimumLatitude = minimumLatitude;
      this.MinimumLongitude = minimumLongitude;
      this.MaximumLatitude = maximumLatitude;
      this.MaximumLongitude = maximumLongitude;
    }

    public override string ToString()
    {
      return string.Format((IFormatProvider) NumberFormatInfo.InvariantInfo, "{0},{1},{2},{3}", (object) this.MinimumLongitude, (object) this.MinimumLatitude, (object) this.MaximumLongitude, (object) this.MaximumLatitude);
    }

    public double DiagonalDistanceInMiles()
    {
      return this.DiagonalDistance() * 3963.191;
    }

    public double DiagonalDistanceInKM()
    {
      return this.DiagonalDistance() * 6378.137;
    }

    private double DiagonalDistance()
    {
      double num1 = this.MinimumLatitude / 180.0 * Math.PI;
      double num2 = this.MaximumLatitude / 180.0 * Math.PI;
      double num3 = this.MinimumLongitude / 180.0 * Math.PI;
      double num4 = this.MaximumLongitude / 180.0 * Math.PI;
      return Math.Acos(Math.Sin(num1) * Math.Sin(num2) + Math.Cos(num1) * Math.Cos(num2) * Math.Cos(num4 - num3));
    }
  }
}
