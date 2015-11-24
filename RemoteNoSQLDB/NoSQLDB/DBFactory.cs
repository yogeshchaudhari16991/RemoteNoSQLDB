//////////////////////////////////////////////////////////////////////////////
// DBFactory.cs - Define element for noSQL database                         //
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
 * This package implements the DBFactory<Key, Data> type, used by 
 * QueryEngine<key, Value> where Value is DBElement<Key, Data>.
 *
 * DBFactory is an immutable database created from result of a query
 *
 */
/*
 * Maintenance:
 * ------------
 * Required Files: DBFactory.cs, DBEngine.cs, IQuery.cs
 * For running teststub we need DBElement.cs, DBExtensions.cs, UtilityExtensions.cs
 *
 *
 * Build Process:  devenv Project2Starter.sln /Rebuild debug
 *                 Run from Developer Command Prompt
 *                 To find: search for developer
 *
 * Maintenance History:
 * --------------------
 * ver 1.0 : 7 Oct 15
 * - first release
 *
 */

using System;
using System.Collections.Generic;
using System.Linq;
using static System.Console;

namespace Project2
{
    public class DBFactory<Key, Value> : IQuery<Key, Value>
    {
        // reference to DBEngine and List of keys for <Key, Value> present in immutable database
        private DBEngine<Key, Value> dbEngine = new DBEngine<Key, Value>();
        private List<Key> keys = new List<Key>();
        
        public DBFactory(DBEngine<Key, Value> db, List<Key> key_collection)
        {
            dbEngine = db;
            keys = key_collection;
        }

        public bool getValue(Key key, out Value val)
        {
            if (keys.Contains(key))
            {
                if (dbEngine.Keys().Contains(key))
                {
                    dbEngine.getValue(key, out val);
                    return true;
                }
            }
            val = default(Value);
            return false;
        }
        public IEnumerable<Key> Keys()
        {
            return keys;
        }
    }

#if (TEST_DBFACTORY)
    class TestDBFactory
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

            DBFactory<int, DBElement<int, string>> dbf = new DBFactory<int, DBElement<int, string>>(db, db.Keys().ToList<int>());

            "Data in DBFactory:".title();
            foreach(int db_key in dbf.Keys())
            {
                DBElement<int, string> ele = new DBElement<int, string>();
                dbf.getValue(db_key, out ele);
                WriteLine("element at key {0}: {1}", db_key, ele);
            }

            "Test for interface IQuery".title();
            IQuery<int, DBElement<int, string>> iq = dbf;
            foreach (int db_key in iq.Keys())
            {
                DBElement<int, string> ele = new DBElement<int, string>();
                iq.getValue(db_key, out ele);
                WriteLine("element at key {0}: {1}", db_key, ele);
            }

        }
    }
#endif

}

