using System;
using System.Collections.Generic;

using Tao.Lua;

namespace Behemoth.LuaUtil
{
  public static class LuaUtil
  {
    public static IntPtr MakeState()
    {
      return Lua.luaL_newstate();
    }


    public static void FreeState(IntPtr luaState)
    {
      Lua.lua_close(luaState);
    }


    /// <summary>
    /// Run Lua code in a string in a Lua VM state. A replacement for the
    /// buggy dostring in Tao.
    /// </summary>
    public static void DoString(IntPtr luaState, string str)
    {
      int status;
      status = Lua.luaL_loadstring(luaState, str);
      CheckErrors(luaState, status);
      status = Lua.lua_pcall(luaState, 0, Lua.LUA_MULTRET, 0);
      CheckErrors(luaState, status);
    }


    public static void CheckErrors(IntPtr luaState, int status)
    {
      if (status != 0)
      {
        // Read the error message from the stack.
        string errMsg = Lua.lua_tostring(luaState, -1);
        // Remove error message.
        Lua.lua_pop(luaState, -1);
        throw new LuaException(errMsg);
      }
    }
  }


  public class LuaException : Exception {
    public LuaException(string msg) : base(msg) {}
  }
}