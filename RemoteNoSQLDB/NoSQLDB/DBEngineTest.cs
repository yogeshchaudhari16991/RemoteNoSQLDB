//////////////////////////////////////////////////////////////////////////////
// DBEngineTest.cs - Define element for noSQL database                         //
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
 * This package replaces DBEngine test stub to remove
 * circular package references.
 *
 * Now this testing depends on the class definitions in DBElement,
 * DBEngine, and the extension methods defined in DBExtensions.
 * We no longer need to define extension methods in DBEngine.
 */
/*
 * Maintenance:
 * ------------
 * Required Files: 
 *   DBEngineTest.cs,  DBElement.cs, DBEngine.cs, IQuery.cs 
 *   DBExtensions.cs, UtilityExtensions.cs
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
  #if (TEST_DBELENGINETEST)
  class Program
  {
    static void Main(string[] args)
    {
      "Testing DBEngine Package".title('=');
      WriteLine();

      Write("\n --- Test DBElement<int,string> ---");
      DBElement<int, string> elem1 = new DBElement<int, string>();
      elem1.payload = "a payload";
      Write(elem1.showElement<int, string>());
      WriteLine();

      DBElement<int, string> elem2 = new DBElement<int, string>("Darth Vader", "Evil Overlord");
      elem2.payload = "The Empire strikes back!";
      Write(elem2.showElement<int, string>());
      WriteLine();

      var elem3 = new DBElement<int, string>("Luke Skywalker", "Young HotShot");
      elem3.children.AddRange(new List<int> { 1, 5, 23 });
      elem3.payload = "X-Wing fighter in swamp - Oh oh!";
      Write(elem3.showElement<int, string>());
      WriteLine();

      Write("\n --- Test DBEngine<int,DBElement<int,string>> ---");

      int key = 0;
      Func<int> keyGen = () => { ++key; return key; };  // anonymous function to generate keys

      DBEngine<int, DBElement<int, string>> db = new DBEngine<int, DBElement<int, string>>();
      bool p1 = db.insert(keyGen(), elem1);
      bool p2 = db.insert(keyGen(), elem2);
      bool p3 = db.insert(keyGen(), elem3);
      if (p1 && p2 && p3)
        Write("\n  all inserts succeeded");
      else
        Write("\n  at least one insert failed");
      db.show<int, DBElement<int, string>, string>();
      WriteLine();
      WriteLine();

      "testing edits".title();
      db.show<int, DBElement<int, string>, string>();
      DBElement<int, string> editElement = new DBElement<int, string>();
      db.getValue(1, out editElement);
      editElement.showElement<int, string>();
      editElement.name = "editedName";
      editElement.descr = "editedDescription";
      db.show<int, DBElement<int, string>, string>();
      Write("\n\n");

      "Test for interface IQuery".title();
      IQuery<int, DBElement<int, string>> iq = db;
      foreach (int db_key in iq.Keys())
      {
           DBElement<int, string> ele = new DBElement<int, string>();
           iq.getValue(db_key, out ele);
           WriteLine("element at key {0}: {1}", db_key, ele);
      }

      int[] key_array = db.Keys().ToArray();
      WriteLine("removing element with key:{0} from database <int, DBElement<int, string>>",key_array[2]);
      WriteLine();
      db.remove(key_array[2]);
      db.show<int, DBElement<int, string>, string>();
      WriteLine();

      WriteLine("Removing all elements from database:");
      db.removeAll();
      WriteLine("DB after removal:");
      db.show<int, DBElement<int, string>, string>();


      Write("\n --- Test DBElement<string,List<string>> ---");
      DBElement<string, List<string>> newelem1 = new DBElement<string, List<string>>();
      newelem1.name = "newelem1";
      newelem1.descr = "test new type";
      newelem1.payload = new List<string> { "one", "two", "three" };
      Write(newelem1.showElement<string, List<string>>());
      WriteLine();

      Write("\n --- Test DBElement<string,List<string>> ---");
      DBElement<string, List<string>> newerelem1 = new DBElement<string, List<string>>();
      newerelem1.name = "newerelem1";
      newerelem1.descr = "better formatting";
      newerelem1.payload = new List<string> { "alpha", "beta", "gamma" };
      newerelem1.payload.Add("delta");
      newerelem1.payload.Add("epsilon");
      Write(newerelem1.showElement<string, List<string>, string>());
      WriteLine();

      DBElement<string, List<string>> newerelem2 = new DBElement<string, List<string>>();
      newerelem2.name = "newerelem2";
      newerelem2.descr = "better formatting";
      newerelem1.children.AddRange(new[] { "first", "second" });
      newerelem2.payload = new List<string> { "a", "b", "c" };
      newerelem2.payload.Add("d");
      newerelem2.payload.Add("e");
      Write(newerelem2.showElement<string, List<string>, string>());
      WriteLine();

      Write("\n --- Test DBEngine<string,DBElement<string,List<string>>> ---");

      int seed = 0;
      string skey = seed.ToString();
      Func<string> skeyGen = () => {
        ++seed;
        skey = "string" + seed.ToString();
        skey = skey.GetHashCode().ToString();
        return skey;
      };

      DBEngine<string, DBElement<string, List<string>>> newdb =
        new DBEngine<string, DBElement<string, List<string>>>();
      newdb.insert(skeyGen(), newerelem1);
      newdb.insert(skeyGen(), newerelem2);
      newdb.show<string, DBElement<string, List<string>>, List<string>, string>();
      WriteLine();
      WriteLine();
      
      string[] key_enum_array = newdb.Keys().ToArray();
      WriteLine("removing element with key:{0} from database <string, DBElement<int, List<string>>>",key_enum_array[0]);
      WriteLine();
      newdb.remove(key_enum_array[0]);
      newdb.show<string, DBElement<string, List<string>>, List<string>, string>();
      WriteLine();

      WriteLine("removing all elements from database:");
      newdb.removeAll();
      WriteLine("DB after removal:");
      newdb.show<string, DBElement<string, List<string>>, List<string>, string>(); 
    }
  }
#endif
}
