using System;
using System.Collections.Generic;

using Tao.Lua;

namespace Behemoth.LuaUtil
{
  /// <summary>
  /// The common Lua types, excluding weirder stuff like threads and userdata.
  /// </summary>
  public enum LuaType
  {
    Nil = Lua.LUA_TNIL,
    Bool = Lua.LUA_TBOOLEAN,
    Number = Lua.LUA_TNUMBER,
    String = Lua.LUA_TSTRING,
    Table = Lua.LUA_TTABLE,
    Function = Lua.LUA_TFUNCTION,
  }

  public class LuaState
  {
    public LuaState()
    {
      luaState = Lua.luaL_newstate();
    }


    ~LuaState()
    {
      Lua.lua_close(luaState);
    }

    /// <summary>
    /// Return the inner IntPtr state value that can be used with the Tao Lua
    /// API.
    /// </summary>
    public IntPtr Ptr { get { return luaState; } }


    /// <summary>
    /// Evaluate a string containing Lua code.
    /// </summary>
    public void DoString(string str)
    {
      int status;
      status = Lua.luaL_loadstring(luaState, str);
      Check(status);
      status = Lua.lua_pcall(luaState, 0, Lua.LUA_MULTRET, 0);
      Check(status);
    }


    /// <summary>
    /// Check for Lua error status.
    /// </summary>
    private void Check(int status)
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


    /// <summary>
    /// Dump a primitive value or a table into a C# Object. Tables are dumped
    /// using DumpTable.
    /// </summary>
    public Object Dump(int stackPos)
    {
      switch ((LuaType)Lua.lua_type(luaState, stackPos))
      {
      case LuaType.Nil:
        return null;
      case LuaType.Bool:
        return Lua.lua_toboolean(luaState, stackPos) != 0;
      case LuaType.Number:
        return Lua.lua_tonumber(luaState, stackPos);
      case LuaType.String:
        return Lua.lua_tostring(luaState, stackPos);
      case LuaType.Table:
        return DumpTable(stackPos);
      case LuaType.Function:
        throw new LuaException("Value is a function, not a primitive.");
      default:
        throw new LuaException("Can't get value of Lua type "+
                               Lua.lua_type(luaState, stackPos));
      }
    }


    /// <summary>
    /// Makes a dictionary of a Lua table.
    /// </summary>
    /// <remarks>
    /// Tables or functions as keys are not supported and their pairs will be
    /// ignored. Functions as values are not supported and their pairs will be
    /// ignored. Loops are not checked for, tables that form a referential
    /// loop will cause an infinite recursion.
    /// </remarks>
    public IDictionary<Object, Object> DumpTable(int stackPos)
    {
      var result = new Dictionary<Object, Object>();

      // Start iterating the stack. First push a nil starter key.
      Lua.lua_pushnil(luaState);
      while (Lua.lua_next(luaState, stackPos) != 0)
      {
        int keyPos = Lua.lua_gettop(luaState) - 1;
        int valPos = Lua.lua_gettop(luaState);
        if (IsValidDumpKey(keyPos) && IsValidDumpValue(valPos))
        {
          var key = Dump(keyPos);
          var val = Dump(valPos);
          result[key] = val;
        }

        // Pop the value, keep the key so that iteration may continue.
        Lua.lua_pop(luaState, 1);
      }

      return result;
    }


    public IDictionary<Object, Object> DumpGlobals()
    {
      return DumpTable(Lua.LUA_GLOBALSINDEX);
    }


    private bool IsValidDumpKey(int stackPos)
    {
      switch ((LuaType)Lua.lua_type(luaState, stackPos))
      {
      case LuaType.Bool:
        return true;
      case LuaType.Number:
        return true;
      case LuaType.String:
        return true;
      default:
        return false;
      }
    }


    private bool IsValidDumpValue(int stackPos)
    {
      switch ((LuaType)Lua.lua_type(luaState, stackPos))
      {
      case LuaType.Bool:
        return true;
      case LuaType.Number:
        return true;
      case LuaType.String:
        return true;
      case LuaType.Table:
        return true;
      default:
        return false;
      }

    }



    private IntPtr luaState;
  }


  public class LuaException : Exception {
    public LuaException(string msg) : base(msg) {}
  }
}