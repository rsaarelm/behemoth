using System;
using System.Collections.Generic;

namespace Behemoth.Alg
{
  /// <summary>
  /// Base class for components that get drawn during the game loop.
  /// </summary>
  public abstract class DrawableAppComponent : AppComponent
  {
    public virtual void Draw(double timeElapsed) {}


    /// <summary>
    /// Whether this component is currently being drawn.
    /// </summary>
    public bool Visible = true;


    /// <summary>
    /// Similar parameter as UpdateOrder for determining drawing order.
    /// </summary>
    public int DrawOrder;
  }
}