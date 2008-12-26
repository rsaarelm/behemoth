namespace Behemoth.Alg
{
  /// <summary>
  /// 3-dimensional generic vectors.
  /// </summary>
  /// <typeparam name="T">
  /// Type of the vector elements.
  /// </typeparam>
  public struct Vec3<T>
  {
    public T X;
    public T Y;
    public T Z;

    public Vec3(T x, T y, T z)
    {
      X = x;
      Y = y;
      Z = z;
    }
  }
}