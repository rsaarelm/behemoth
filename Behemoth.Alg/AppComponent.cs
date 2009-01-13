using System;
using System.Collections.Generic;

namespace Behemoth.Alg
{
  /// <summary>
  /// Base class for all game components.
  /// </summary>
  public abstract class AppComponent
  {
    /// <summary>
    /// Called when this component is initialized after all components have
    /// been attached to the App. Get services from the App here.
    /// </summary>
    public virtual void Init() {}


    public virtual void Uninit() {}


    /// <summary>
    /// Called each game cycle when this component is active, with the time in
    /// seconds since the last update call.
    /// </summary>
    public virtual void Update(double timeElapsed) {}


    /// <summary>
    /// The app this component is attached to.
    /// </summary>
    public App App;


    /// <summary>
    /// Whether this component is currently being updated.
    /// </summary>
    public bool Enabled;


    /// <summary>
    /// How should this component be ordered relative to other components when
    /// updating the components. Components are updated in the ascending order
    /// of their UpdateOrder values.
    /// </summary>
    public int UpdateOrder;


    // TODO Dispose logic
  }
}