// Type: FlickrNet.PointD
// Assembly: FlickrNet, Version=3.10.0.0, Culture=neutral, PublicKeyToken=2491df59efa5d132
// MVID: 8B8C756F-D5B2-4047-8A8B-B1C1E877A2EF
// Assembly location: D:\RMCK\vs\2014.05.24_FlickrAPI\FlickrNet.dll

namespace FlickrNet
{
  public struct PointD
  {
    private double x;
    private double y;

    public double X
    {
      get
      {
        return this.x;
      }
    }

    public double Y
    {
      get
      {
        return this.y;
      }
    }

    public PointD(double x, double y)
    {
      this.x = x;
      this.y = y;
    }

    public static bool operator ==(PointD point1, PointD point2)
    {
      if (point1.X == point2.X)
        return point1.Y == point2.Y;
      else
        return false;
    }

    public static bool operator !=(PointD point1, PointD point2)
    {
      if (point1.X == point2.X)
        return point1.Y != point2.Y;
      else
        return true;
    }

    public override bool Equals(object obj)
    {
      if (obj == null || !(obj is PointD))
        return false;
      PointD pointD = (PointD) obj;
      return pointD.X == this.X && pointD.Y == this.Y;
    }

    public override int GetHashCode()
    {
      return this.X.GetHashCode() + this.Y.GetHashCode();
    }

    public override string ToString()
    {
      return base.ToString();
    }
  }
}
