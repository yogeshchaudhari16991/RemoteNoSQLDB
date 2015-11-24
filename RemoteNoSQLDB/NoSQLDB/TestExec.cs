//////////////////////////////////////////////////////////////////////////////
// TestExec.cs - Define element for noSQL database                         //
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
 * This package demonstrates of requirements.
 * 
 */
/*
 * Maintenance:
 * ------------
 * Required Files: 
 *   TestExec.cs,  DBElement.cs, DBEngine, Display, IQuery.cs, ItemFactory.cs, PersistEngine.cs, 
 *   QueryEngine.cs, QueryPredicate.cs, Scheduler.cs, UtilityExtensions.cs 
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
using static System.Console;

namespace Project2
{
  #if (TEST_TESTEXEC)
  public class TestExec
  {
    private DBEngine<int, DBElement<int, string>> db = new DBEngine<int, DBElement<int, string>>();
    private DBEngine<string, DBElement<string, List<string>>> enum_db = new DBEngine<string, DBElement<string, List<string>>>();

    public void TestR2()
    {
      "Demonstrating Requirement #2".title('=');
      "Database with string as payload".title();
      DBElement<int, string> elem = new DBElement<int, string>();
      elem.name = "element";
      elem.descr = "test element";
      elem.timeStamp = DateTime.Now;
      elem.children.AddRange(new List<int>{ 1, 2, 3 });
      elem.payload = "elem's payload";
      elem.showElement();
      db.insert(1, elem);
      db.showDB();
      WriteLine();
      "database with list of strings as payload".title();
      DBElement<string, List<string>> element = new DBElement<string, List<string>>();
      element.name = "element2";
      element.descr = "test element for list of strings as value";
      element.timeStamp = DateTime.Now;
      element.children.AddRange(new List<string> { "one","two"});
      element.payload = new List<string> { "element's payload", "2nd payload" };
      element.showEnumerableElement();
      enum_db.insert("enum_one", element);
      enum_db.showEnumerableDB();
      WriteLine();

    }
    void TestR3()
    {
      WriteLine();
      "Demonstrating Requirement #3".title('=');
      WriteLine();
      "Adding Key-Value pair to database".title();
      DBElement<int, string> element = new DBElement<int, string>();
      element.addElementData("element3", "test element for adding key-value pair to databse with value as string", DateTime.Now, new List<int> { 1, 2 }, "test elemet3's payload");
      "element to be added to database".title();
      element.showElement();
      db.insert(2, element);
      db.showDB();                        
      WriteLine();

      "Adding Key-Value pair to enumerable database".title();
      DBElement<string, List<string>> listelement = new DBElement<string, List<string>>();
      listelement.addElementData("element4", "test element for adding key-value pair to databse with value as list of string", DateTime.Now, new List<string> { "1", "two" }, new List<string> { "test elemet4's payload" });
      "element to be added to database".title();
      listelement.showEnumerableElement();
      enum_db.insert("enum_two", listelement);
      enum_db.showEnumerableDB();  
      WriteLine();
       
      "Deleting Key-Value pair from database".title();
      "Element with key=1 will be deleted from database".title();
      "Element with key=1:".title();
      DBElement<int, string> remove_element = new DBElement<int, string>();
      db.getValue(1, out remove_element);
      remove_element.showElement();
      db.remove(1);
      WriteLine();
      "Modified Database: ".title();
      db.showDB();
      WriteLine();

      "Deleting Key-Value pair from enumerable database".title();
      "Element with key=enum_one will be deleted from database".title();
      "Element with key=enum_one:".title();
      DBElement<string, List<string>> remove_enum_element = new DBElement<string, List<string>>();
      enum_db.getValue("enum_one", out remove_enum_element);
      remove_enum_element.showEnumerableElement();
      enum_db.remove("enum_one");
      WriteLine();
      "Modified enumerable Database: ".title();
      enum_db.showEnumerableDB();
      WriteLine();
      
    }


    void TestR4(string edit_action)
    {
      DBElement<string, List<string>> element = new DBElement<string, List<string>>();
      element.addElementData("element1", "test element for editing key-value pair to databse with value as string", DateTime.Now, new List<string> { "enum_one", "enum_two" }, new List<string> { "test element1's payload" });
      enum_db.insert("enum_one", element);
      "Demonstrating Requirement #4".title();
      DBElement<string, List<string>> edit_element = new DBElement<string, List<string>>();
      enum_db.getValue("enum_one", out edit_element);
      "Element to be edited".title();
      edit_element.showEnumerableElement();
      WriteLine();
      "Database before editing".title();
      enum_db.showEnumerableDB();
      WriteLine();
      switch (edit_action)
      {
        case "edit metadata":
          edit_element.edit_metadata(edit_action);
          goto default;
        case "add children":
          edit_element.add_children(edit_action);
          goto default;
        case "remove children":
          edit_element.remove_children(edit_action);
          goto default;
        case "edit payload":
          edit_element.edit_payload();
          goto default;
        default:
          "element after editing".title();
          edit_element.showEnumerableElement();
          "Databaser after editing".title();
          enum_db.showEnumerableDB();
          WriteLine();
          break;
      }  
    }

    void TestR5()
    {
      "Demonstrating Requirement #5".title('=');
      "Persisting to XML file".title();
      WriteLine();
      WriteLine("Saving DB with int as key and string payload to xml file ~/debug/test_DB");
      db.create_xml_from_db(false, "db.xml");
      WriteLine("Saving DB with String as key and list of strings as payload to XML file ~/debug/test_enumDB");
      enum_db.create_xml_from_db(false, "enum_db.xml");
      WriteLine("Remove previous elements from db");
      if (db != null)
      { 
         db.removeAll();
      }
      WriteLine("Current DB state");
      db.showDB();
      WriteLine();     
      WriteLine("Adding element to database");
      DBElement<int, string> element = new DBElement<int, string>();
      element.addElementData("element1", "test element for editing key-value pair to databse with value as string", DateTime.Now, new List<int> { 1, 2 }, "test element1's payload");
      db.insert(1, element);
      WriteLine("DB state after adding an element");
      db.showDB();
      WriteLine();
      WriteLine("Writing data from xml file Test_DB.xml to databse");
      WriteLine("All elements from xml which have same key as the key of element present in database is discarded.");
      db.create_db_from_xml("db.xml");
      WriteLine();
      WriteLine("Removing entries from enum Db if any");
      if (enum_db != null)
      {
                enum_db.removeAll();
      }
      WriteLine("Cureent enum DB state");
      enum_db.showEnumerableDB();
      WriteLine();
      WriteLine("Adding element to database");
      DBElement<string, List<string>> listelement = new DBElement<string, List<string>>();
      listelement.addElementData("element4", "test element for adding key-value pair to databse with value as list of string", DateTime.Now, new List<string> { "1", "two" }, new List<string> { "test elemet4's payload" });
      enum_db.insert("six",listelement);
      WriteLine("Enum database after adding an element");
      enum_db.showEnumerableDB();
      WriteLine("Writing data from xml file Test_enumDB.xml to enum databse");
      enum_db.create_enum_db_from_xml("enum_db.xml");
      WriteLine();
    }
    void TestR6()
    {
      "Demonstrating Requirement #6".title('=');
      "Persisting data to Database in XML files".title();
      //schedular(enum_db);
      Console.Write("\n\n");
      WriteLine();
    }

    public void TestR7()
    {
      "Demonstrating Requirement #7".title('=');
      WriteLine();
      "Adding elements to db".title();
      WriteLine();
      for (int i = 0; i < 3; i++)
      {
          DBElement<string, List<string>> elem = new DBElement<string, List<string>>();
          elem.name = "element";
          elem.descr = "test element";
          elem.timeStamp = new DateTime(2015,10,(2+i));
          elem.children.AddRange(new List<string> { "one", "two", "three" });
          elem.payload = new List<string>{ "elem's payload1","payload2","payload 3"};
          elem.showEnumerableElement();
          WriteLine();
          enum_db.insert((12345+i).ToString(), elem);
      }     
      "current DB status:".title();
      enum_db.showEnumerableDB();
      IQuery<string, DBElement<string, List<string>>> i_query = new DBEngine<string, DBElement<string, List<string>>>();
      QueryEngine<string, DBElement<string, List<string>>> qe = new QueryEngine<string, DBElement<string, List<string>>>(enum_db);
      QueryPredicate qp = new QueryPredicate();
      qp.key_value_search(enum_db, out i_query, qe);
      WriteLine();
      qp.key_children_search(enum_db, out i_query, qe);
      WriteLine();
      qp.pattern_matching(enum_db, out i_query, qe, "12345");
      WriteLine();
      qp.pattern_matching(enum_db, out i_query, qe);
      WriteLine();
      qp.metadata_string(enum_db, out i_query, qe);
      WriteLine();
      DateTime start = new DateTime(2015, 10, 4);
      DateTime end = new DateTime(2015, 10, 5);
      qp.date_time_specific(enum_db, out i_query, qe, start, end);
      WriteLine();
      qp.default_date_time_specific(enum_db, out i_query, qe, start);
      WriteLine();
    }
    void TestR8()
    {
      "Demonstrating Requirement #8".title();
      WriteLine();
      WriteLine("We are creating an immutable database each time we run a query on database in DBEngine.");
      WriteLine("This database creation can be seen in QueryEngine package at line 57.");
      WriteLine("Methods and structure of immutable databse is given in package DBFactory.");
      WriteLine("Here we can see that DBFactory only has a constructor and getValue() and Keys() methods, which are only reading methods and not modifying methods.");
      WriteLine("DBfactory doesnot contain any function to modify its object, hence it is an immutable database.");
      WriteLine();
    }
    static void Main(string[] args)
    {
      TestExec exec = new TestExec();
      "Demonstrating Project#2 Requirements".title('=');
      WriteLine();
      exec.TestR2();
      exec.TestR3();
      "Editing metadata of element:".title();
      exec.TestR4("edit metadata");
      "adding relationship in metadata".title();
      exec.TestR4("add children");
      "removing children relationship from metadata".title();
      exec.TestR4("remove children");
      "editing value's instance".title();
      exec.TestR4("edit payload");
      exec.TestR5();
      exec.TestR6();
      exec.TestR7();
      exec.TestR8();
      Write("\n\n");
    }
  }
#endif
}
