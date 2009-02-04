using System;
using System.Collections.Generic;

namespace Behemoth.Util
{
  public class Cons<T>
  {
    public Cons(T head, Cons<T> tail)
    {
      this.head = head;
      this.tail = tail;
    }


    public T Head { get { return head; } }


    public Cons<T> Tail { get { return tail; } }


    public override String ToString()
    {
      if (Tail == null)
      {
        return String.Format("h<{0}>", Head);
      }
      else
      {
        return String.Format("h<{0}> t<{1}>", Head, Tail);
      }
    }

    private T head;

    private Cons<T> tail;
  }
}