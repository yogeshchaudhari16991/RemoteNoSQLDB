//////////////////////////////////////////////////////////////////////////////
// DBEngine.cs - Define element for noSQL database                         //
// Ver 1.0                                                                  //
// Application: Demonstration for CSE681-SMA, Project#2                     //
// Language:    C#, ver 6.0, Visual Studio 2015                             //
// Platform:    Dell Inspiron 14 5000Series, Core-i5, Windows 10            //
// Author:      Yogesh Chaudhari, Student, Syracuse University              //
//              (315) 4809210, ychaudha@syr.edu                             //
//////////////////////////////////////////////////////////////////////////////
/*
 * Package Operations:
 * -------------------
 * This package implements DBEngine<Key, Value> where Value
 * is the DBElement<key, Data> type.
 *
 */
/*
 * Maintenance:
 * ------------
 * Required Files: DBEngine.cs, DBElement.cs, IQuery.cs and
 *                 UtilityExtensions.cs only if you enable the test stub
 *
 * Build Process:  devenv Project2Starter.sln /Rebuild debug
 *                 Run from Developer Command Prompt
 *                 To find: search for developer
 *
 *
 * Reference: Project2Starter project by Dr. Fawcett
 * ----------
 *
 * Maintenance History:
 * --------------------
 * ver 1.0 : 07 Oct 15
 * - first release
 *
 */


using System;
using System.Collections.Generic;
using System.Linq;
using static System.Console;

namespace Project2
{
  public class DBEngine<Key, Value> : IQuery<Key, Value>
  {
    private Dictionary<Key, Value> dbStore;
    public DBEngine()
    {
      dbStore = new Dictionary<Key, Value>();
    }
    public bool insert(Key key, Value val)
    {
      if (dbStore.Keys.Contains(key))
        return false;
      dbStore[key] = val;
      return true;
    }
    public bool remove(Key key)
    {    
            if (!dbStore.Keys.Contains(key))
            {
                return false;
            }     
      dbStore.Remove(key);
      return true;
    }
        /* Removes all the <key, value> pairs from database */
    public bool removeAll()
    {
            DBEngine<Key, Value> db = this;
            IEnumerable<Key> key_list = new List<Key>();
            key_list = db.Keys().ToList<Key>();
            foreach (Key key in key_list)
                db.remove(key);
            return true;
     }
        public bool getValue(Key key, out Value val)
    {
      if(dbStore.Keys.Contains(key))
      {
        val = dbStore[key];
        return true;
      }
      val = default(Value);
      return false;
    }
        public bool setValue(Key key, Value val)
    {
      if(dbStore.Keys.Contains(key))
      {
        dbStore[key] = val;
        return true;
      }
      //dbStore[key] = default(Value);
      return false;
    }
    public IEnumerable<Key> Keys()
    {
      return dbStore.Keys;
    }

    public bool containsKey(Key key)
    {
      Key temp = key;
      return dbStore.ContainsKey(temp);
    }

    public bool containsKey(string db_key)
    {
      bool b = false;
      foreach(var k in dbStore.Keys)
      {
        if ((k.ToString()).Equals(db_key))
        {
          b = true;
          return b;
        }
        else
        {
          b = false;
        } 
      }
      return b;

    }
  }

#if(TEST_DBENGINE)

  class TestDBEngine
  {
    static void Main(string[] args)
    {
      "Testing DBEngine Package".title('=');
      WriteLine();

      Write("\n  All testing of DBEngine class moved to DBEngineTest package.");
      Write("\n  This allow use of DBExtensions package without circular dependencies.");

      Write("\n\n");
    }
  }
#endif
}
