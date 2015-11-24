//////////////////////////////////////////////////////////////////////////////
// QueryPredicate.cs - Define element for noSQL database                         //
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
 * This package includes methods which define query by using lambda functions
 * and pass these lambda functions to QueryEngine for making queries on DBEngie<>
 * 
 */
/*
 * Maintenance:
 * ------------
 * Required Files: QueryPredicate.cs, DBElement.cs, DBEngine.cs, DIsplay.cs, IQuery.cs, QueryEngine.cs, UtilityExtensions.cs
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
  public class QueryPredicate
  {
    /* Defining a query using lambda function to search specific key 
    */
    public bool key_value_search(DBEngine<string, DBElement<string, List<string>>> db, out IQuery<string, DBElement<string, List<string>>> i_query, QueryEngine<string, DBElement<string, List<string>>> qe, string key_to_search = "12345")
    {
      "Query for value with specified key (key = element2):".title();
      WriteLine();

      Func<string, string, bool> keyValueSearch = (string key, string search) => //lambda function
      {
        if (!db.Keys().Contains(key))
          return false;
        else
        {
          if (key == (search))
          {
            DBElement<string, List<string>> ele = new DBElement<string, List<string>>();
            db.getValue(key, out ele);
            return true;
          }
          else { return false; }
        }
      };
      // pass query to query engine and call simpleQuery to make query on DBEngine

      if (qe.simpleQuery(keyValueSearch, key_to_search.ToString(), out i_query))
      {
        WriteLine();
        foreach (var key in i_query.Keys())
        {
          DBElement<string, List<string>> temp = new DBElement<string, List<string>>();
          i_query.getValue(key, out temp);
          WriteLine("key : {0}", key);
          temp.showEnumerableElement();
          WriteLine();
        }
        return true;
      }
      else
      {
        return false;
      }
    }

    /* Defining a query using lambda function to search children of specific element 
    */
    public bool key_children_search(DBEngine<string, DBElement<string, List<string>>> db, out IQuery<string, DBElement<string, List<string>>> i_query, QueryEngine<string, DBElement<string, List<string>>> qe, string specific_key = "element2")
    {
      
      "Query for children of specified key (key = element2):".title();
      WriteLine();
      Func<string, string, bool> childrenQuery = (string key, string search) => //lambda function
      {
        if (!db.Keys().Contains(key))
          return false;
        if (key == (search))
        {
          DBElement<string, List<string>> ele = new DBElement<string, List<string>>();
          db.getValue(key, out ele);
          return true;
        }
        else return false;
      };
      // pass query to query engine and call simpleQuery to make query on DBEngine
      if (qe.simpleQuery(childrenQuery, specific_key.ToString(), out i_query))
      {
        WriteLine();
        display_children(i_query, db);
        return true;
      }
      else
      {
        return false;
      }
    }

    private void display_children(IQuery<string, DBElement<string, List<string>>> i_query, DBEngine<string, DBElement<string, List<string>>> db)
    {
      foreach (var key in i_query.Keys())
      {
        DBElement<string, List<string>> temp = new DBElement<string, List<string>>();
        i_query.getValue(key, out temp);
        WriteLine("children of element with key {0} :", key);
        WriteLine();
        if (temp.children != null)
        {
          int i = 0;
          foreach (string child in temp.children)
          {
            WriteLine("Children {0}", ++i);
            DBElement<string, List<string>> temp_child = new DBElement<string, List<string>>();
            if (db.Keys().Contains(child))
            {
              db.getValue(child, out temp_child);
              WriteLine("key : {0}", child);
              temp_child.showEnumerableElement();
              WriteLine();
            }
            else
            {
              WriteLine("no value with key {0} is present in database", child);
              WriteLine();
            }
          }
        }
      }
    }

    /* Defining a query using lambda function to search specific key matchin a pattern 
    */
    public bool pattern_matching(DBEngine<string, DBElement<string, List<string>>> db, out IQuery<string, DBElement<string, List<string>>> i_query, QueryEngine<string, DBElement<string, List<string>>> qe, string pattern = "")
    {
      
      "Query for keys matching with specified pattern (pattern = element123):".title();
      WriteLine();
      Func<string, string, bool> keysMatchingPattern = (string key, string search) => //lambda function
      {
        if (!db.Keys().Contains(key))
          return false;
        else
        {
          if (key.Contains(search))
          {
            DBElement<string, List<string>> ele = new DBElement<string, List<string>>();
            db.getValue(key, out ele);
            return true;
          }
          else return false;
        }
      };
      // pass query to query engine and call simpleQuery to make query on DBEngine
      if (qe.simpleQuery(keysMatchingPattern, pattern, out i_query))
      {
        WriteLine();
        foreach (var key in i_query.Keys())
        {
          DBElement<string, List<string>> temp = new DBElement<string, List<string>>();
          i_query.getValue(key, out temp);
          WriteLine("key : {0}", key);
          temp.showEnumerableElement();
          WriteLine();
        }
        return true;
      }
      else
      {
        return false;
      }
    }
    /* Defining a query using lambda function to search specific key matching default pattern
    */
    public void default_pattern_matching(DBEngine<string, DBElement<string, List<string>>> db, out IQuery<string, DBElement<string, List<string>>> i_query, QueryEngine<string, DBElement<string, List<string>>> qe)
    {
      string pattern = "";
      "Query for keys matching with specified pattern (pattern = none  -> default case):".title();
      WriteLine();
      Func<string, string, bool> keysMatchingPattern = (string key, string search) => //lambda function
      {
        if (!db.Keys().Contains(key))
          return false;
        else
        {
          if (key.ToString().Contains(search))
          {
            DBElement<string, List<string>> ele = new DBElement<string, List<string>>();
            db.getValue(key, out ele);
            return true;
          }
          else return false;
        }
      };
      // pass query to query engine and call simpleQuery to make query on DBEngine
      qe.simpleQuery(keysMatchingPattern, pattern, out i_query);
      WriteLine();
      foreach (var key in i_query.Keys())
      {
        DBElement<string, List<string>> temp = new DBElement<string, List<string>>();
        i_query.getValue(key, out temp);
        WriteLine("key : {0}", key);
        temp.showEnumerableElement();
        WriteLine();
      }

    }
    /* Defining a query using lambda function to search specific string in metadata 
    */
    public bool metadata_string(DBEngine<string, DBElement<string, List<string>>> db, out IQuery<string, DBElement<string, List<string>>> i_query, QueryEngine<string, DBElement<string, List<string>>> qe, string metadata_str = "ele")
    {
      "Query for specified string in metadata: String = 'ele' ".title();
      WriteLine();
      Func<string, string, bool> stringMetadata = (string key, string search) => //lambda function
      {
        if (!db.Keys().Contains(key))
          return false;
        else
        {
          DBElement<string, List<string>> ele = new DBElement<string, List<string>>();
          db.getValue(key, out ele);
          if (ele.name.Contains(search) || ele.descr.Contains(search))
            return true;
          else return false;
        }
      };
      // pass query to query engine and call simpleQuery to make query on DBEngine
      if (qe.simpleQuery(stringMetadata, metadata_str, out i_query))
      {
        WriteLine();
        foreach (var key in i_query.Keys())
        {
          DBElement<string, List<string>> temp = new DBElement<string, List<string>>();
          i_query.getValue(key, out temp);
          WriteLine("key : {0}", key);
          temp.showEnumerableElement();
          WriteLine();
        }
        return true;
      } else
      {
        return false;
      }
    }
    /* Defining a query using lambda function to search specific elements belonging in specific  
    *  time-interval
    */
    public bool default_date_time_specific(DBEngine<string, DBElement<string, List<string>>> db, out IQuery<string, DBElement<string, List<string>>> i_query, QueryEngine<string, DBElement<string, List<string>>> qe, DateTime start, DateTime end = new DateTime())
    {
      Console.Write("\nQuery for keys with specified date time interval: start=" + start.ToString() + " end=not specified");
      WriteLine();
      Func<string, DateTime, DateTime, bool> DefaultTimeDateQuery = (string key, DateTime query_start, DateTime query_end) => //lambda function
      {
        if (!db.Keys().Contains(key))
          return false;
        else
        {
          DBElement<string, List<string>> ele = new DBElement<string, List<string>>();
          db.getValue(key, out ele);
          int start_result = DateTime.Compare(query_start, ele.timeStamp);
          int end_result = DateTime.Compare(query_end, ele.timeStamp);
          if ((start_result <= 0) && (end_result >= 0))
          {
            return true;
          }
          else return false;
        }
      };
      // pass query to query engine and call simpleQuery to make query on DBEngine
      if (qe.simpleQueryDate(DefaultTimeDateQuery, out i_query, start, end))
      {
        WriteLine();
        foreach (var key in i_query.Keys())
        {
          DBElement<string, List<string>> temp = new DBElement<string, List<string>>();
          i_query.getValue(key, out temp);
          WriteLine("key : {0}", key);
          temp.showEnumerableElement();
          WriteLine();
        }
        return true;
      }
      else
      {
        return false;
      }
    }

    /* Defining a query using lambda function to search specific elements belonging in specific  
    *  time-interval with end of time interval equal to present
    */
    public void date_time_specific(DBEngine<string, DBElement<string, List<string>>> db, out IQuery<string, DBElement<string, List<string>>> i_query, QueryEngine<string, DBElement<string, List<string>>> qe, DateTime start, DateTime end = new DateTime())
    {
      "Query for keys with specified date time interval: start=10/4/2015 end=10/5/2015".title();
      WriteLine();
      Func<string, DateTime, DateTime, bool> TimeDateQuery = (string key, DateTime query_start, DateTime query_end) => //lambda function
      {
        if (!db.Keys().Contains(key))
          return false;
        else
        {
          DBElement<string, List<string>> ele = new DBElement<string, List<string>>();
          db.getValue(key, out ele);
          int start_result = DateTime.Compare(query_start, ele.timeStamp);
          int end_result = DateTime.Compare(query_end, ele.timeStamp);
          if ((start_result <= 0) && (end_result >= 0))
          {
            return true;
          }
          else return false;
        }
      };
      // pass query to query engine and call simpleQuery to make query on DBEngine
      qe.simpleQueryDate(TimeDateQuery, out i_query, start, end);
      WriteLine();
      foreach (var key in i_query.Keys())
      {
        DBElement<string, List<string>> temp = new DBElement<string, List<string>>();
        i_query.getValue(key, out temp);
        WriteLine("key : {0}", key);
        temp.showEnumerableElement();
        WriteLine();
      }
    }
  }

#if(TEST_QUERYPREDICATE)
    class Test_QueryPredicate
    {
        static void Main(string[] args)
        {
            DBEngine<string, DBElement<string, List<string>>> db = new DBEngine<string, DBElement<string, List<string>>>();
            for (int i = 0; i < 3; i++)
            {
                DBElement<string, List<string>> elem = new DBElement<string, List<string>>();
                elem.name = "element";
                elem.descr = "test element";
                elem.timeStamp = new DateTime(2015, 10, (2 + i));
                elem.children.AddRange(new List<string> { "element1", "element2", "element3" });
                elem.payload = new List<string> { "elem's payload1", "elem's payload2" };
                elem.showEnumerableElement();
                WriteLine();
                db.insert("element"+(12345 + i).ToString(), elem);
            }
            for (int i = 0; i < 3; i++)
            {
                DBElement <string, List<string>> elem = new DBElement<string, List<string>>();
                elem.name = "db data";
                elem.descr = "db data description";
                elem.timeStamp = DateTime.Now;
                elem.children.AddRange(new List<string> { "element12345", "element12346", "element12347" });
                elem.payload = new List<string> { "elem's payload1", "elem's payload2" };
                elem.showEnumerableElement();
                WriteLine();
                db.insert("element"+(i+1).ToString(), elem);
            }

            IQuery<string, DBElement<string, List<string>>> i_query = new DBEngine<string, DBElement<string, List<string>>>();
            QueryEngine<string, DBElement<string, List<string>>> qe = new QueryEngine<string, DBElement<string, List<string>>>(db);
            //<---- creating a query predicate object and calling each query on given test database --->
            QueryPredicate qp = new QueryPredicate();
            qp.key_value_search(db, i_query, qe);
            WriteLine();
            qp.key_children_search(db, i_query, qe);
            WriteLine();
            qp.pattern_matching(db, i_query, qe);
            WriteLine();
            qp.default_pattern_matching(db, i_query, qe);
            WriteLine();
            qp.metadata_string(db, i_query, qe);
            WriteLine();
            qp.date_time_specific(db, i_query, qe);
            WriteLine();
            qp.default_date_time_specific(db, i_query, qe);
            WriteLine();
        }
    }
#endif
}