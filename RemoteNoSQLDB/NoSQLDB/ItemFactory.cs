//////////////////////////////////////////////////////////////////////////////
// ItemFactory.cs - Define element for noSQL database                         //
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
 * ItemFactory provides a method to add metadata<Key> and value instance(payload)<Data> 
 * to DBElement<key, Data> instance
 *
 */
/*
 * Maintenance:
 * ------------
 * Required Files: ItemFactory.cs, DBElement.cs, Display.cs, UtilityExtensions.cs
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
using static System.Console;

namespace Project2
{
  public static class ItemFactory
  {
    public static bool addElementData<Key, Data>(this DBElement<Key, Data> element, string name, string descr, DateTime time, List<Key> key_List, Data payload)
      where Data : class
    {
      try {
        element.name = name;
        element.descr = descr;
        element.timeStamp = time;
        element.children.AddRange(key_List);
        element.payload = payload;
        return true;
      }
      catch(Exception e)
      {
        return false;
      }
    }

    public static bool editElementData<Key, Data>(this DBElement<Key, Data> element, DateTime time, Data payload, string name = null, string descr = null, string action = null, List<Key> key_List = null)
      where Data : class
    {
      try {
        if (name != null)
        {
          element.name = name;
        }
        if (descr != null)
        {
          element.descr = descr;
        }
        element.timeStamp = time;
        if (key_List != null)
        {
          switch (action)
          {
            case "add children":
              element.children.AddRange(key_List);
              break;

            case "remove children":
              foreach (Key k in key_List) {
                element.children.Remove(k);
              }
              break;
            case "edit payload":
              element.payload = payload;
              break;
            default: break;
          }
        }
        return true;
      }
      catch(Exception e)
      {
        return false;
      }
    }
    public static bool editElementData<Key, Data, T>(this DBElement<Key, Data> element, DateTime time, Data payload, string name = null, string descr = null, string action = null, List<Key> key_List = null)
      where Data : class, IEnumerable<T>
    {
      try
      {
        if (name != null)
        {
          element.name = name;
        }
        if (descr != null)
        {
          element.descr = descr;
        }
        element.timeStamp = time;
        if (key_List != null)
        {
          switch (action)
          {
            case "add children":
              element.children.AddRange(key_List);
              break;

            case "remove children":
              foreach (Key k in key_List)
              {
                element.children.Remove(k);
              }
              break;

            default: break;
          }
        }
        element.payload = payload;
        return true;
      }
      catch (Exception e)
      {
        return false;
      }
    }

    public static bool edit_payload<Key, Data, T>(this DBElement<Key, Data> edit_element)
      where Data : class, IEnumerable<T>
    {
      List<string> str = new List<string> { "payload 1", "payload 2" };
      List<T> t = str as List<T>;
      edit_element.editElementData<Key, Data, T>(edit_element.timeStamp, t as Data);
      return true;
    }

    public static bool edit_payload<Key, Data>(this DBElement<Key, Data> edit_element)
      where Data: class
    {
      string str = "payload 1";
      edit_element.editElementData(edit_element.timeStamp, str as Data);
      return true;
    }

    public static bool edit_metadata<Key, Data>(this DBElement<Key, Data> edit_element, string edit_action)
      where Data :class
    {
      string name = "name after editing";
      string descr = "descr after editing";
      DateTime time_stamp = edit_element.timeStamp;
      edit_element.editElementData(time_stamp, edit_element.payload, name, descr, edit_action, edit_element.children);
      return true;
    }

    public static bool edit_metadata<Key, Data, T>(this DBElement<Key, Data> edit_element, string edit_action)
      where Data : class, IEnumerable<T>
    {
      string name = "name after editing";
      string descr = "descr after editing";
      DateTime time_stamp = edit_element.timeStamp;
      edit_element.editElementData(time_stamp, edit_element.payload, name, descr, edit_action, edit_element.children);
      return true;
    }

    public static void add_children<Key, Data>(this DBElement<Key, Data> edit_element, string edit_action)
      where Data: class
    {
      List<int> int_list = new List<int> { 1, 2 };
      List<Key> child_list = int_list as List<Key>;
      edit_element.editElementData(edit_element.timeStamp, edit_element.payload, edit_element.name, edit_element.descr, edit_action, child_list);
    }

    public static void add_children<Key, Data, T>(this DBElement<Key, Data> edit_element, string edit_action)
      where Data : class, IEnumerable<T>
    {
      List<string> string_list = new List<string> { "enum_one", "enum_two" };
      List<Key> child_list = string_list as List<Key>;
      edit_element.editElementData(edit_element.timeStamp, edit_element.payload, edit_element.name, edit_element.descr, edit_action, child_list);
    }

    public static void remove_children<Key, Data>(this DBElement<Key, Data> edit_element, string edit_action)
      where Data : class
    {
      List<int> int_list = new List<int> { 1, 2 };
      List<Key> child_list = int_list as List<Key>;
      edit_element.editElementData(edit_element.timeStamp, edit_element.payload, edit_element.name, edit_element.descr, "remove children", child_list);
    }
    public static void remove_children<Key, Data, T>(this DBElement<Key, Data> edit_element, string edit_action)
      where Data : class, IEnumerable<T>
    {
      List<string> string_list = new List<string> { "enum_one", "enum_two" };
      List<Key> child_list = string_list as List<Key>;
      edit_element.editElementData(edit_element.timeStamp, edit_element.payload, edit_element.name, edit_element.descr, "remove children", child_list);
    }
  }


#if (TEST_ITEMFACTORY)

  class TestItemFactory
  {
    static void Main(string[] args)
    {
      DBEngine<int, DBElement<int, string>> db = new DBEngine<int, DBElement<int, string>>();
      DBEngine<string, DBElement<string, List<string>>> enum_db = new DBEngine<string, DBElement<string, List<string>>>();
      "Testing Item Factory package".title();
      "Adding data to element with payload as string".title();
      DBElement<int, string> element = new DBElement<int, string>();
      element.addElementData("element for test", "element created to test addElementData()", DateTime.Now, new List<int> { 1, 2, 3, 4 }, "test_element payload");
      element.showElement();
      db.insert(1, element);
      WriteLine();
      DBElement<int, string> edit_element1 = new DBElement<int, string>();
      db.getValue(1, out edit_element1);
      "Editing metadata of element:".title();
      "adding relationship in metadata".title();
      edit_element1.editElementData(element.timeStamp, element.payload, "name after editing", "descr after editing", "add children", new List<int> { 3 });
      edit_element1.showElement();
      db.showDB();
      WriteLine();

      "removing relationship from metadata".title();
      edit_element1.editElementData(element.timeStamp, element.payload, element.name, element.descr, "remove children", new List<int> { 3 });
      db.showDB();
      WriteLine();

      "editing value's instance".title();
      edit_element1.editElementData(element.timeStamp, "payload 1");
      db.showDB();
      WriteLine();


      "Adding data to element with payload as list of string".title();
      DBElement<string, List<string>> enum_element = new DBElement<string, List<string>>();
      enum_element.addElementData("element for test", "element created to test addElementData()", DateTime.Now, new List<string> { "1", "two", "3", "four" }, new List<string> { "test_element's payload1", "test_element's payload2", "test_element's payload3" });
      enum_element.showEnumerableElement();
      WriteLine();
      enum_db.insert("1", enum_element);
      "editing element in db with payload as list of strings".title();
      DBElement<string, List<string>> edit_element = new DBElement<string, List<string>>();
      enum_db.getValue("1", out edit_element);
      "Editing metadata of element:".title();
      "adding relationship in metadata".title();
      edit_element.editElementData(enum_element.timeStamp, enum_element.payload, "name after editing", "descr after editing", "add children", new List<string> { "3" });
      edit_element.showEnumerableElement();
      enum_db.showEnumerableDB();
      WriteLine();

      "removing relationship from metadata".title();
      edit_element.editElementData(enum_element.timeStamp, enum_element.payload, enum_element.name, enum_element.descr, "remove children", new List<string> { "3" });
      enum_db.showEnumerableDB();
      WriteLine();

      "editing value's instance".title();
      edit_element.editElementData(element.timeStamp, new List<string> { "payload 1", "edit payload 2" });
      enum_db.showEnumerableDB();
      WriteLine();
    }
  }
#endif
}
