//////////////////////////////////////////////////////////////////////////////
// PersistEngine.cs - Define element for noSQL database                         //
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
 * This package includes method to Write data from DBEngine<int, DBElement<int, string> to xml file &
 * includes method to Write data from DBEngine<string, DBElement<string, list<string>> to xml file
 * This package also includes method to read data from xml file to DBEngine<int, DBElement<int, string>
 * as well as to DBEngine<string, DBElement<string, list<string>> 
 *
 */
/*
 * Maintenance:
 * ------------
 * Required Files: PersistEngine.cs, DBElement.cs, DBEngine.cs, Display, UtilityExtensions.cs
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
using System.Xml.Linq;
using System.IO;

namespace Project2
{
  public static class PersistEngine
  {
    /*
         *  It is a quirk of the XDocument class that the XML declaration,
         *  a valid element, cannot be added to the XDocument's element
         *  collection.  Instead, it must be assigned to the document's
         *  Declaration property.
         */

    /* 
    * we are creating XElements for each DBElement<int, string> and 
    * DBElement<string, List<string>> depending upon DB type in 
    * DBEngine<int, DBElement<int, string>> and 
    * DBEngine<string, DBElement<string, List<string>> respectively using XElement object
    * We are saving XMl file to ~/TestExec/bin/debug/Test_DB.xml
    */
    public static void create_xml_from_db<Key, Data>(this DBEngine<Key, DBElement<Key, Data>> db, Boolean persist, string destination = "Test_DB.xml")
    {
      Console.WriteLine("In creat_xml_from_db");
      XDocument xml = new XDocument();
      xml.Declaration = new XDeclaration("1.0", "utf-8", "yes");
      XComment comment = new XComment("Test DB data to XML");
      xml.Add(comment);
      XElement root = new XElement("noSQL");
      xml.Add(root);
      XElement keytype = new XElement("keytype", typeof(Key).Name);
      root.Add(keytype);
      string payloadTypeName = typeof(Data).Name;
      XElement payloadtype = new XElement("payloadtype", payloadTypeName);
      root.Add(payloadtype);
      foreach (var db_key in db.Keys())
      {
        DBElement<Key, Data> ele = new DBElement<Key, Data>();
        db.getValue(db_key, out ele);
        XElement key = new XElement("key", db_key.ToString());
        root.Add(key);
        XElement element = new XElement("element");
        XElement name = new XElement("name", ele.name.ToString());
        XElement descr = new XElement("descr", ele.descr.ToString());
        XElement timestamp = new XElement("timestamp", ele.timeStamp.ToString());
        XElement children = new XElement("children");
        XElement payload = new XElement("payload");
        foreach (Key x in ele.children)
        {
          XElement children_key = new XElement("key", x.ToString());
          children.Add(children_key);
        }
        addPayload(payloadTypeName, ref payload, ele);
        element.Add(name);
        element.Add(descr);
        element.Add(timestamp);
        element.Add(children);
        element.Add(payload);
        root.Add(element);
      }
      WriteLine();
      //<--------Writing to XML--------->
      "Creating XML file using XDocument and writing into Test_DB.xml".title();
      string path = Path.GetFullPath("../../../Server/bin/Debug/" + destination);
      xml.Save(path);
      display_xml(xml, persist);
      WriteLine();
    }

    private static bool addPayload<Key, Data>(string payloadTypeName, ref XElement payload, DBElement<Key, Data> ele)
    {
      if (ele.payload != null)
      {
        switch (payloadTypeName)
        {
          case "String":
            payload.Value = ele.payload as string;
            break;
          case "List`1":
            foreach (string x in ele.payload as List<string>)
            {
              XElement payload_string = new XElement("item", x.ToString());
              payload.Add(payload_string);
            }
            break;
          default: break;
        }
        return true;
      }
      return false;
    }

    /*
    *  It is a quirk of the XDocument class that the XML declaration,
    *  a valid element, cannot be added to the XDocument's element
    *  collection.  Instead, it must be assigned to the document's
    *  Declaration property.
    */
    
    // Method to display XMl file
    public static void display_xml(XDocument xml, bool persist)
    {
      if (!persist)
      {
        Console.Write("\n{0}\n", xml.Declaration.ToString());
        Console.Write(xml.ToString());
      }
      WriteLine();
    }

    /* Method to write data in DBEngine<string, DBElement<string, list<string>> from 
    * Test_Db.xml xml file
    * We are using System.Linq queries to get all elements with <element> tag from XML file 
    *
    */
    public static void create_db_from_xml(this DBEngine<int, DBElement<int, string>> db, string source = "Test_DB.xml")
    {
      string path = Path.GetFullPath("../../../Server/bin/Debug/" + source);
      if (File.Exists(path))
      {
        //<-------Read DB from XML file----------->
        "Reading and displaying saved file".title();

        XDocument newDoc = XDocument.Load(path);
        Console.Write("\n{0}", newDoc.Declaration);
        Console.Write("\n{0}", newDoc.ToString());
        WriteLine();

        //Find element by TagName
        var ele = from x in newDoc.Elements("noSQL").Elements("element")
                  select x;
        "Adding elements to db from xml file".title();
        foreach (XElement ele_tag in ele)
        {
          XElement key_ele = XElement.Parse(ele_tag.PreviousNode.ToString());
          int db_key = int.Parse(key_ele.Value);
          if (!db.Keys().Contains(db_key)) //check if key is already present in Database or not
          {
            DBElement<int, string> db_ele = new DBElement<int, string>();
            db_ele.name = ele_tag.Element("name").Value;
            db_ele.descr = ele_tag.Element("descr").Value;
            db_ele.timeStamp = DateTime.Parse(ele_tag.Element("timestamp").Value);
            db_ele.payload = ele_tag.Element("payload").Value;
            db_ele.children = new List<int> { };
            foreach (var child_key in ele_tag.Element("children").Elements())
            {
              db_ele.children.Add(int.Parse(child_key.Value));
            }
            db.insert(db_key, db_ele);
          }
          else
          {
            //discard the element
          }
        }
        "Final DB state".title();
        db.showDB();
        WriteLine();
      }
      else
      {
        Console.Write("\n\n File Doesnot Exist\n\n");
      }
    }

    /* Method to write data in DBEngine<string, DBElement<string, list<string>> from 
    * Test_enumDb.xml xml file
    * We are using System.Linq queries to get all elements with <element> tag from XML file 
    *
    */

    public static void create_enum_db_from_xml(this DBEngine<string, DBElement<string, List<string>>> enum_db, string source = "enum_db.xml")
    {
      string path = Path.GetFullPath("../../../Server/bin/Debug/" + source);
      if (File.Exists(path))
      {
        //<-------Read enum DB from XML file----------->
        "Reading and displaying saved file".title();       
        XDocument newDoc = XDocument.Load(path);
        Console.Write("\n{0}", newDoc.Declaration);
        Console.Write("\n{0}", newDoc.ToString());
        WriteLine();
        //Find element by TagName
        var ele = from x in newDoc.Elements("noSQL").Elements("element")
                  select x;
        "Adding elements to enum db from xml file".title();
        foreach (XElement ele_tag in ele)
        {
          XElement key_ele = XElement.Parse(ele_tag.PreviousNode.ToString());
          string db_key = key_ele.Value;
          if (!enum_db.Keys().Contains(db_key)) //check if key is already present in database or not
          {
            DBElement<string, List<string>> db_ele = new DBElement<string, List<string>>();
            db_ele.name = ele_tag.Element("name").Value;
            db_ele.descr = ele_tag.Element("descr").Value;
            db_ele.timeStamp = DateTime.Parse(ele_tag.Element("timestamp").Value);
            db_ele.children = new List<string> { };
            db_ele.payload = new List<string> { };
            foreach (var child_key in ele_tag.Element("children").Elements())
            {
              db_ele.children.Add(child_key.Value);
            }
            foreach (var payload_item in ele_tag.Element("payload").Elements())
            {
              db_ele.payload.Add(payload_item.Value);
            }
            enum_db.insert(db_key, db_ele);
          }
          else
          {
            //discard that element
          }
        }
        "Final DB state".title();
        enum_db.showEnumerableDB();
        WriteLine();
      }
      else
        Console.Write("\n\n File Doesnot Exist\n\n");
    }
  }

#if (TEST_PERSISTENGINE)
    class TestPersistEngine
  {
    static void Main(string[] args)
    {

      //<------Create test DB----->
      "Testing for PersistEngine package".title();
      DBEngine<int, DBElement<int, string>> db = new DBEngine<int, DBElement<int, string>>();
      DBEngine<string, DBElement<string, List<string>>> enum_db = new DBEngine<string, DBElement<string, List<string>>>();
      //PersistEngine pe = new PersistEngine();
      for (int i = 0; i < 3; i++)
      {
        DBElement<int, string> elem = new DBElement<int, string>();
        elem.name = "element";
        elem.descr = "test element";
        elem.timeStamp = DateTime.Now;
        elem.children.AddRange(new List<int> { 1, 2, 3 });
        elem.payload = "elem's payload";
        elem.showElement();
        db.insert(i, elem);
      }
      for (int i = 0; i < 3; i++)
      {
        DBElement<string, List<string>> elem = new DBElement<string, List<string>>();
        elem.name = "element";
        elem.descr = "test element";
        elem.timeStamp = DateTime.Now;
        elem.children.AddRange(new List<string> { "child1", "child2" });
        elem.payload = new List<string> { "payload 1", "payload 2" };
        elem.showEnumerableElement();
        enum_db.insert(i.ToString(), elem);
      }
      db.showDB();
      enum_db.showEnumerableDB();
      WriteLine();
      //<-------Save DB data to XML file--------->
      "Demonstrating XDocument class".title('=');
      WriteLine();
      "Creating XML string using XDocument".title();
      WriteLine();
      string destination = "Test_DB.xml"
      db.create_xml_from_db(false, destination);

      //<---Empty DBEngine<int, DBElement<int, string>>--->
      "Removing entries from Db if any".title();
      if (db != null)
      {
        db.removeAll();
      }
      "Current DB state".title();
      db.showDB();
      WriteLine();
      db.create_db_from_xml(destination);
      WriteLine();
      //<---------Create test enum DB-------->
      for (int i = 0; i < 3; i++)
      {
        DBElement<string, List<string>> elem = new DBElement<string, List<string>>();
        elem.name = "element";
        elem.descr = "test element";
        elem.timeStamp = DateTime.Now;
        elem.children.AddRange(new List<string> { "child1", "child2" });
        elem.payload = new List<string> { "payload 1", "payload 2" };
        elem.showEnumerableElement();
        enum_db.insert(i.ToString(), elem);
      }
      enum_db.showEnumerableDB();
      WriteLine();
      //<-------Save enum_DB data to XML file--------->
      "Creating XML string using XDocument".title();
      enum_db.create_xml_from_db(false, destination);
      WriteLine();

      if (enum_db != null)
      {
        enum_db.removeAll();
      }
      //<----Empty DBEngine<string, DBElement<string, List<string>>----->
      "Removing entries from enum Db if any".title();
      if (enum_db != null)
      {
        string[] key_array = enum_db.Keys().ToArray();
        foreach (string key in key_array)
        {
          enum_db.remove(key);
        }
      }
      "Cureent enum DB state".title();
      enum_db.showEnumerableDB();
      WriteLine();
      enum_db.create_enum_db_from_xml(destination);
      WriteLine();
      WriteLine();

    }
  }
#endif
}
