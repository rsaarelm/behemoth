using System;

namespace Behemoth.Util
{
  public struct Tuple2<T1, T2>
  {
    public Tuple2(T1 first, T2 second) {
      this.first = first;
      this.second = second;
    }


    public T1 First { get { return first; } }


    public T2 Second { get { return second; } }


    public override String ToString()
    {
      return String.Format("[{0}, {1}]", First, Second);
    }


    private T1 first;
    private T2 second;
  }


  public struct Tuple3<T1, T2, T3>
  {
    public Tuple3(T1 first, T2 second, T3 third) {
      this.first = first;
      this.second = second;
      this.third = third;
    }


    public T1 First { get { return first; } }


    public T2 Second { get { return second; } }


    public T3 Third { get { return third; } }


    private T1 first;
    private T2 second;
    private T3 third;
  }


  public struct Tuple4<T1, T2, T3, T4>
  {
    public Tuple4(T1 first, T2 second, T3 third, T4 fourth) {
      this.first = first;
      this.second = second;
      this.third = third;
      this.fourth = fourth;
    }


    public T1 First { get { return first; } }


    public T2 Second { get { return second; } }


    public T3 Third { get { return third; } }


    public T4 Fourth { get { return fourth; } }


    private T1 first;
    private T2 second;
    private T3 third;
    private T4 fourth;
  }
}