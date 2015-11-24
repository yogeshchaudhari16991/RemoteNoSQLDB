//////////////////////////////////////////////////////////////////////////////
// QueryEngine.cs - Define element for noSQL database                         //
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
 * This package includes method to make search key, value and pattern and also
 * specific time interval query on DBEngine<int, DBelement<int, string>>.
 * It calls queries from QueryPredicate and creates a new DBFactory object
 * for returned list of keys.
 */
/*
 * Maintenance:
 * ------------
 * Required Files: QueryEngine.cs, DBElement.cs, DBEngine.cs, DBFactory.cs, DIsplay.cs, IQuery.cs, UtilityExtensions.cs
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
    public class QueryEngine<Key, Value>
    {
        private DBEngine<Key, Value> dbEngine = new DBEngine<Key, Value>();


        public QueryEngine(DBEngine<Key, Value> db)
        {
            dbEngine = db;
        }
        public bool simpleQuery(Func<Key, string, bool> qp, string search, out IQuery<Key, Value> db)
        {

            List<Key> key_collection = new List<Key>();
            for (int i = 0; i < dbEngine.Keys().Count(); ++i)
            {
                Key key = dbEngine.Keys().ElementAt(i);
                if (qp(key, search))
                {
                    key_collection.Add(key);
                }
            }
            //Creating immutable database
            DBFactory<Key, Value> dbFactory = new DBFactory<Key, Value>(dbEngine, key_collection);
            db = dbFactory;
            if (db.Keys().Count() > 0)
            {
                "Result of queries".title();
                return true;
            }
            return false;
        }

        public bool simpleQueryDate(Func<Key, DateTime, DateTime, bool> qp, out IQuery<Key, Value> db, DateTime start, DateTime end)
        {
            DateTime temp = new DateTime();
            if (end.Equals(temp))
            {
                end = DateTime.Now;
            }
            List<Key> key_collection = new List<Key>();
            for (int i = 0; i < dbEngine.Keys().Count(); ++i)
            {
                Key key = dbEngine.Keys().ElementAt(i);
                if (qp(key, start, end))
                {
                    key_collection.Add(key);
                }
            }
            //Creating immutable database
            DBFactory<Key, Value> dbFactory = new DBFactory<Key, Value>(dbEngine, key_collection);
            db = dbFactory;
            if (db.Keys().Count() > 0)
            {
                "Result of queries".title();
                WriteLine();
                return true;
            }
            return false;
        }
    }

#if(TEST_QUERYENGINE)
    class TestQueryEngine
    {
        static void Main(string[] args)
        {

             DBEngine<int, DBElement<int, string>> db = new DBEngine<int, DBElement<int, string>>();
             DBEngine<string, DBElement<string, List<string>>> enum_db = new DBEngine<string, DBElement<string, List<string>>>();
            "Demonstrating Requirement #2".title('=');
            "Database with string as payload".title();
            DBElement<int, string> elem = new DBElement<int, string>();
            elem.name = "element";
            elem.descr = "test element";
            elem.timeStamp = DateTime.Now;
            elem.children.AddRange(new List<int> { 1, 2, 3 });
            elem.payload = "elem's payload";
            elem.showElement();
            db.insert(1, elem);
            db.showDB();
            WriteLine();
            "database with list of strings as payload".title();
            DBElement<int, string> element = new DBElement<int, string>();
            element.name = "element2";
            element.descr = "test element for list of strings as value";
            element.timeStamp = DateTime.Now;
            element.children.AddRange(new List<int> {1,2});
            element.payload = "element's payload";
            db.insert(2, element);
            enum_db.showEnumerableDB();
            WriteLine();
            "Current DB".title();
            db.showDB();
            QueryEngine<int, DBElement<int, string>> qe = new QueryEngine<int, DBElement<int, string>>(db);
            string pattern = "2";
            Func<int, string, bool> queryPredicate = (int key, string search) =>
            {
                if (!db.Keys().Contains(key))
                    return false;
                else
                {
                    DBElement<int, string> ele = new DBElement<int, string>();
                    db.getValue(key, out ele);  
                    if (ele.name.Contains(search))
                        return true;
                    else return false;
                }
            };
            IQuery<int, DBElement<int, string>> i_query = new DBEngine<int, DBElement<int, string>>();
            qe.simpleQuery(queryPredicate, pattern, out i_query);
            WriteLine();
            foreach (var key in i_query.Keys())
            {
                DBElement<int, string> temp = new DBElement<int, string>();
                WriteLine("\n value: {0}", i_query.getValue(key, out temp));
                temp.showElement();
            }

        }
    }
#endif
}
