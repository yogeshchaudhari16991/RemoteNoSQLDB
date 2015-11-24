/////////////////////////////////////////////////////////////////////////
// ServerParser.cs - Parses Server Received Messages                   //
//                                                                     //
// Ver 1.0                                                             //
// Application: Demonstration for CSE681-SMA, Project#2                //
// Language:    C#, ver 6.0, Visual Studio 2015                        //
// Platform:    Dell Inspiron 14 5000Series, Core-i5, Windows 10       //
// Author:      Yogesh Chaudhari, Student, Syracuse University         //
//              (315) 4809210, ychaudha@syr.edu                        //
/////////////////////////////////////////////////////////////////////////
/*
 *   Module Operations
 *   -----------------
 *   This module demonstrates how the messages received by server are
 *   parsed to make DB calls.
 *
 * Additions to C# Console Wizard generated code:
 * - Added using Project2
 * - Added reference to NoSQLDatabase
 *
 *
 * Maintenance History:
 * --------------------
 * ver 1.0 : 19 November 2015
 * - first release
*
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Project2;

namespace Project4Starter
{
  public class ServerParser
  {
    //--------< Parser class to parse content from received messages
    //-------- and generate NoSQLDatabase calls>-------------------------- 

    ///////////////////////////////////////////////////////////////////////
    // for Database with Key as string and Value as List<string>
    // 
    //

    public string parse(ref IEnumerator<string> msg_enumerator, DBEngine<string, DBElement<string, List<string>>> db)
    {
      string str = "";
      while (msg_enumerator.MoveNext())
      {
        if(msg_enumerator.Current == "querytype" && msg_enumerator.MoveNext())
        switch (msg_enumerator.Current)
        {
          case "Insert Element":
            insert_element(out str, ref msg_enumerator, db);
            break;
          case "Delete Element":
            delete_element(out str, ref msg_enumerator, db);
            break;
          case "Edit Element Metadata":
            edit_element(out str, ref msg_enumerator, db, "edit metadata");
            break;
          case "Edit Element Metadata and Add Children":
            edit_element(out str, ref msg_enumerator, db, "add children");
            break;
          case "Edit Element Metadata and Remove Children":
            edit_element(out str, ref msg_enumerator, db, "remove children");
            break;
          case "Edit Element Metadata and Edit Payload":
            edit_element(out str, ref msg_enumerator, db, "edit payload");
            break;
          case "Persist Database":
            persist_database(out str, ref msg_enumerator, db);
            break;
          case "Restore Database":
            restore_database(out str, ref msg_enumerator, db);
            break;
          case "Search Key-Value":
            search_key_value(out str, ref msg_enumerator, db);
            break;
          case "Search Children":
            search_children(out str, ref msg_enumerator, db);
            break;
          case "Pattern Matching":
            pattern_matching(out str, ref msg_enumerator, db);
            break;
          case "String in Metadata":
            string_metadata(out str, ref msg_enumerator, db);
            break;
          case "Time-Date Interval":
            time_date_inerval(out str, ref msg_enumerator, db);
            break;
        }
      }
      return str;
    }

    private void time_date_inerval(out string str, ref IEnumerator<string> msg_enumerator, DBEngine<string, DBElement<string, List<string>>> db)
    {
      IQuery<string, DBElement<string, List<string>>> i_query = new DBEngine<string, DBElement<string, List<string>>>();
      QueryEngine<string, DBElement<string, List<string>>> qe = new QueryEngine<string, DBElement<string, List<string>>>(db);
      QueryPredicate qp = new QueryPredicate();
      DateTime start = new DateTime();
      DateTime end = new DateTime();
      str = "";
      bool flag = false;
      while (msg_enumerator.MoveNext()) {
        switch(msg_enumerator.Current.ToString())
        {
          case "sdate-time":
            msg_enumerator.MoveNext();
            DateTime.TryParse(msg_enumerator.Current.ToString(), out start);
            break;
          case "edate-time":
            msg_enumerator.MoveNext();
            if (msg_enumerator.Current.ToString() != "" && DateTime.TryParse(msg_enumerator.Current.ToString(), out end)) ;
            else
              flag = true;
            break;
        }
      }
      DBElement<string, List<string>> ele = new DBElement<string, List<string>>();
      if (flag && qp.default_date_time_specific(db, out i_query, qe, start))
      {
        foreach (var key in i_query.Keys())
          if (i_query.getValue(key, out ele))
            str += " Element within timespan starting:" + start + " and ending: " + DateTime.Now + " found in Database.\n Key: " + key + "\nValue at given key: " + ele.showElement<string, List<string>, string>();
      }
      else if (qp.default_date_time_specific(db, out i_query, qe, start, end))
      {
        foreach (var key in i_query.Keys())
        {
          if (i_query.getValue(key, out ele))
          {
            str += " Element within timespan starting:" + start + " and ending: " + end + " found in Database.\n Key: " + key + "\nValue at given key: " + ele.showElement<string, List<string>, string>();
          }
        }
      } else
      {
        str = "Element within timespan: " + start + " and ending: " + end + " Not found in Database.\n";
      }
    }

    private void string_metadata(out string str, ref IEnumerator<string> msg_enumerator, DBEngine<string, DBElement<string, List<string>>> db)
    {
      IQuery<string, DBElement<string, List<string>>> i_query = new DBEngine<string, DBElement<string, List<string>>>();
      QueryEngine<string, DBElement<string, List<string>>> qe = new QueryEngine<string, DBElement<string, List<string>>>(db);
      QueryPredicate qp = new QueryPredicate();
      string search = "";
      str = "";
      msg_enumerator.MoveNext();
      msg_enumerator.MoveNext();
      search = msg_enumerator.Current.ToString();
      DBElement<string, List<string>> ele = new DBElement<string, List<string>>();
      if (qp.metadata_string(db, out i_query, qe, search))
      {
        foreach (var key in i_query.Keys())
        {
          if (i_query.getValue(key, out ele))
          {
            str += " Element with string:" + search + " found in Database.\n Key: "+ key +"\nValue at given key: " + ele.showElement<string, List<string>, string>();
          }
        }
      }
      else
      {
        str = "Element with string: " + search + " in metadata Not found in Database.\n";
      }
    }

    private void pattern_matching(out string str, ref IEnumerator<string> msg_enumerator, DBEngine<string, DBElement<string, List<string>>> db)
    {
      IQuery<string, DBElement<string, List<string>>> i_query = new DBEngine<string, DBElement<string, List<string>>>();
      QueryEngine<string, DBElement<string, List<string>>> qe = new QueryEngine<string, DBElement<string, List<string>>>(db);
      QueryPredicate qp = new QueryPredicate();
      string search = "";
      str ="";
      msg_enumerator.MoveNext();
      msg_enumerator.MoveNext();
      search = msg_enumerator.Current.ToString();
      DBElement<string, List<string>> ele = new DBElement<string, List<string>>();
      if (qp.pattern_matching(db, out i_query, qe, search))
      {
        foreach (var key in i_query.Keys())
        {
          if (i_query.getValue(key, out ele))
          {
            str += " Key: " + search + " found in Database.\nKey: "+ key +"\n Value at given key: " + ele.showElement<string, List<string>, string>();
          }
        }
      }
      else
      {
        str = "Key with Pattern: " + search + " Not found in Database.\n";
      }
    }

    private void search_children(out string str, ref IEnumerator<string> msg_enumerator, DBEngine<string, DBElement<string, List<string>>> db)
    {
      IQuery<string, DBElement<string, List<string>>> i_query = new DBEngine<string, DBElement<string, List<string>>>();
      QueryEngine<string, DBElement<string, List<string>>> qe = new QueryEngine<string, DBElement<string, List<string>>>(db);
      QueryPredicate qp = new QueryPredicate();
      string search = "";
      msg_enumerator.MoveNext();
      msg_enumerator.MoveNext();
      search = msg_enumerator.Current.ToString();
      DBElement<string, List<string>> ele = new DBElement<string, List<string>>();
      if (qp.key_children_search(db, out i_query, qe, search))
      {
        i_query.getValue(search, out ele);
        str = " Key: " + search + " found in Database.\n Value at given key: " + ele.showElement<string, List<string>, string>();
      }
      else
      {
        str = "Key: " + search + " Not found in Database.\n";
      }
    }

    private void search_key_value(out string str, ref IEnumerator<string> msg_enumerator, DBEngine<string, DBElement<string, List<string>>> db)
    {
      IQuery<string, DBElement<string, List<string>>> i_query = new DBEngine<string, DBElement<string, List<string>>>();
      QueryEngine<string, DBElement<string, List<string>>> qe = new QueryEngine<string, DBElement<string, List<string>>>(db);
      QueryPredicate qp = new QueryPredicate();
      string search = "";
      msg_enumerator.MoveNext();
      msg_enumerator.MoveNext();
      search = msg_enumerator.Current.ToString();
      DBElement<string, List<string>> ele = new DBElement<string, List<string>>();
      if (qp.key_value_search(db, out i_query, qe, search))
      {
        i_query.getValue(search, out ele);
        str = " Key: " + search + " found in Database.\n Value at given key: " + ele.showElement<string, List<string>, string>();
      }
      else
      {
        str = "Key: " + search + " Not found in Database.\n";
      }
    }

    private void restore_database(out string str, ref IEnumerator<string> msg_enumerator, DBEngine<string, DBElement<string, List<string>>> db)
    {
      str = "";
      "Database before restoring".title();
      Console.WriteLine();
      db.showEnumerableDB();
      Console.WriteLine();
      while (msg_enumerator.MoveNext())
      {
        if (msg_enumerator.Current == "Source")
        {
          msg_enumerator.MoveNext();
          Console.WriteLine(msg_enumerator.Current);
          int count = db.Keys().ToArray().Length;
          db.create_enum_db_from_xml(msg_enumerator.Current);
          if (db.Keys().ToArray().Length > count)
          {
            str += "Database Restored from " + msg_enumerator.Current + " file.";
          }
          else
          {
            str += "Database not able to restore from " + msg_enumerator.Current + "file";
          }
          return;
        }
      }
      str += "Database persistence failed.";
    }

    
    private void persist_database(out string str, ref IEnumerator<string> msg_enumerator, DBEngine<string, DBElement<string, List<string>>> db)
    {
      str = "";
      while (msg_enumerator.MoveNext())
      {
        if(msg_enumerator.Current == "Destination")
        {
          msg_enumerator.MoveNext();
          Console.WriteLine(msg_enumerator.Current);
          db.create_xml_from_db(false, msg_enumerator.Current);
          str += "Database Persisted to " + msg_enumerator.Current + " file.";
          return;
        }
      }
      str += "Database persistence failed.";
    }

    private void edit_element(out string str, ref IEnumerator<string> msg_enumerator, DBEngine<string, DBElement<string, List<string>>> db, string edit_action)
    {
      string name = "", descr = "", key1 = "";
      List<string> childList = new List<string>(), payload = null;
      DateTime timestamp = new DateTime();
      while (msg_enumerator.MoveNext())
      {
        switch (msg_enumerator.Current)
        {
          case "key":
            msg_enumerator.MoveNext();
            key1 = msg_enumerator.Current;
            break;
          case "name":
            msg_enumerator.MoveNext();
            name = msg_enumerator.Current;
            break;
          case "descr":
            msg_enumerator.MoveNext();
            descr = msg_enumerator.Current;
            break;
          case "timestamp":
            msg_enumerator.MoveNext();
            timestamp = DateTime.Parse(msg_enumerator.Current.ToString());
            break;
          case "children":
            msg_enumerator.MoveNext();
            int childcount = int.Parse(msg_enumerator.Current);
            while (childcount-- > 0)
            {
              msg_enumerator.MoveNext();
              childList.Add(msg_enumerator.Current.ToString());
            }
            break;
          case "payload":
            msg_enumerator.MoveNext();
            int payloadcount = int.Parse(msg_enumerator.Current);
            payload = addPayload(ref msg_enumerator, payloadcount);
            break;
        }
      }
      DBElement<string, List<string>> element = new DBElement<string, List<string>>();
      if (db.getValue(key1, out element))
      {
        element.editElementData(timestamp, payload, name, descr, edit_action, childList);
        str = "element at key: " + key1 + " has been edited\n";
        Console.WriteLine(element.showElement<string, List<string>, string>());
      }
      else
        str = "element not found in database";
    }

    private void delete_element(out string str, ref IEnumerator<string> msg_enumerator, DBEngine<string, DBElement<string, List<string>>> db)
    {
      string key1 = "";
      msg_enumerator.MoveNext();
      msg_enumerator.MoveNext();
      key1 = msg_enumerator.Current.ToString();
      if (db.containsKey(key1))
      {
        str = "database contains given key:" + key1 + "\n";
        str += "element at key:" + key1 + "\n";
        DBElement<string, List<string>> ele = new DBElement<string, List<string>>();
        db.getValue(key1, out ele);
        str += " with name: \"" + ele.name.ToString() + "\" is been deleted from database\n";
        db.remove(key1);
        db.showEnumerableDB();        
      } else
      {
        str = "database doesnot contain given key:" + key1;
      }
    }

    private void insert_element(out string str, ref IEnumerator<string> msg_enumerator, DBEngine<string, DBElement<string, List<string>>> db)
    {
      string key1 = "";
      List<string> childList = new List<string>(), payload = null;
      DBElement<string, List<string>> ele = new DBElement<string, List<string>>();
      while (msg_enumerator.MoveNext())
      {
        switch (msg_enumerator.Current)
        {
          case "key":
            msg_enumerator.MoveNext();
            key1 = msg_enumerator.Current;
            break;
          case "name":
            msg_enumerator.MoveNext();
            ele.name = msg_enumerator.Current;
            break;
          case "descr":
            msg_enumerator.MoveNext();
            ele.descr = msg_enumerator.Current;
            break;
          case "timestamp":
            msg_enumerator.MoveNext();
            ele.timeStamp = DateTime.Parse(msg_enumerator.Current.ToString());
            break;
          case "children":
            msg_enumerator.MoveNext();
            int childcount = int.Parse(msg_enumerator.Current);
            while (childcount-- > 0)
            {
              msg_enumerator.MoveNext();
              childList.Add(msg_enumerator.Current as string);
            }
            ele.children = childList;
            break;
          case "payload":
            msg_enumerator.MoveNext();
            int payloadcount = int.Parse(msg_enumerator.Current);
            payload = addPayload(ref msg_enumerator, payloadcount);
            ele.payload = payload;
            break;
          default:
            break;
        }
      }
      db.insert(key1, ele);
      db.showEnumerableDB();
      str = "new element added at key: " + key1;
    }

    private List<string> addPayload(ref IEnumerator<string> msg_enumerator, int payloadcount)
    {

      List<string> paylaod_list = new List<string>();
      while (payloadcount-- > 0)
      {
        msg_enumerator.MoveNext();
        paylaod_list.Add(msg_enumerator.Current);
      }
      return paylaod_list;

    }
    
    ///////////////////////////////////////////////////////////////////////
    // for Database with Key as int and Value as string
    // 
    //
    
    public string parse(ref IEnumerator<string> msg_enumerator, DBEngine<int, DBElement<int, string>> db)
    {
      string str = "";
      while (msg_enumerator.MoveNext())
      {
        if (msg_enumerator.Current == "querytype")
        {
          msg_enumerator.MoveNext();
          switch (msg_enumerator.Current)
          {
            case "Insert Element":
              insert_element(out str, ref msg_enumerator, db);
              break;
            case "Delete Element":
              delete_element(out str, ref msg_enumerator, db);
              break;
            case "Edit Element Metadata":
              edit_element(out str, ref msg_enumerator, db, msg_enumerator.Current);
              break;
            case "Persist Database":
              persist_database(out str, ref msg_enumerator, db);
              break;
            case "Restore Database":
              restore_database(out str, ref msg_enumerator, db);
              break;
          }
        }
      }
      return str;
    }

    
    private void restore_database(out string str, ref IEnumerator<string> msg_enumerator, DBEngine<int, DBElement<int, string>> db)
    {
      str = "";
      "Database before restoring".title();
      db.showDB();
      Console.WriteLine();
      while (msg_enumerator.MoveNext())
      {
        if (msg_enumerator.Current == "Source")
        {
          msg_enumerator.MoveNext();
          Console.WriteLine(msg_enumerator.Current);
          int count = db.Keys().ToArray().Length;
          db.create_db_from_xml(msg_enumerator.Current);
          if(db.Keys().ToArray().Length > count)
          {
            str += "Database Restored from " + msg_enumerator.Current + " file.";
          } else
          {
            str += "Database not able to restore from " + msg_enumerator.Current + "file";
          }
          return;
        }
      }
      str += "Database persistence failed.";
    }

    private void persist_database(out string str, ref IEnumerator<string> msg_enumerator, DBEngine<int, DBElement<int, string>> db)
    {
      str = "";
      while (msg_enumerator.MoveNext())
      {
        if (msg_enumerator.Current == "Destination")
        {
          msg_enumerator.MoveNext();
          Console.WriteLine(msg_enumerator.Current);
          db.create_xml_from_db(false, msg_enumerator.Current);
          str += "Database Persisted to " + msg_enumerator.Current + " file.";
          return;
        }
      }
      str += "Database persistence failed.";
    }

    private void edit_element(out string str, ref IEnumerator<string> msg_enumerator, DBEngine<int, DBElement<int, string>> db, string edit_action)
    {
      int key1 = 0;
      string name = "", descr = "", payload = null;
      DateTime timestamp = new DateTime();
      List<int> childList = new List<int>();
      while (msg_enumerator.MoveNext())
      {
        switch (msg_enumerator.Current)
        {
          case "key":
            msg_enumerator.MoveNext();
            key1 = int.Parse(msg_enumerator.Current);
            break;
          case "name":
            msg_enumerator.MoveNext();
            name = msg_enumerator.Current;
            break;
          case "descr":
            msg_enumerator.MoveNext();
            descr = msg_enumerator.Current;
            break;
          case "timestamp":
            msg_enumerator.MoveNext();
            timestamp = DateTime.Parse(msg_enumerator.Current.ToString());
            break;
          case "children":
            msg_enumerator.MoveNext();
            int childcount = int.Parse(msg_enumerator.Current);
            while (childcount-- > 0)
            {
              msg_enumerator.MoveNext();
              childList.Add(int.Parse(msg_enumerator.Current));
            }
            break;
          case "payload":
            msg_enumerator.MoveNext();
            int payloadcount = int.Parse(msg_enumerator.Current);
            payload = addPayload(ref msg_enumerator);
            break;
        }
      }
      DBElement<int, string> element = new DBElement<int, string>();
      if (db.getValue(key1, out element))
      {
        element.editElementData(timestamp, payload, name, descr, edit_action, childList);
        str = "element at key: " + key1 + " has been edited\n";
      }
      else
        str = "element not found in database\n";
    }

    private void delete_element(out string str, ref IEnumerator<string> msg_enumerator, DBEngine<int, DBElement<int, string>> db)
    {
      int key1 = 0;
      msg_enumerator.MoveNext();
      msg_enumerator.MoveNext();
      key1 = int.Parse(msg_enumerator.Current);
      if (db.containsKey(key1))
      {
        str = "database contains given key:" + key1 + "\n";
        str += "element at key:" + key1 + "\n";
        DBElement<int, string> ele = new DBElement<int, string>();
        db.getValue(key1, out ele);
        str += " with name: " + ele.name.ToString() + "\n";
        db.remove(key1);
        db.showDB();
      }
      else
      {
        str = "database doesnot contain given key:" + key1;
      }
    }

    
    private void insert_element(out string str, ref IEnumerator<string> msg_enumerator, DBEngine<int, DBElement<int, string>> db)
    {
      string key1 = "", payload = null;
      List<int> childList = new List<int>();
      DBElement<int, string> ele = new DBElement<int, string>();
      while (msg_enumerator.MoveNext())
      {
        switch (msg_enumerator.Current)
        {
          case "key":
            msg_enumerator.MoveNext();
            key1 = msg_enumerator.Current;
            break;
          case "name":
            msg_enumerator.MoveNext();
            ele.name = msg_enumerator.Current;
            break;
          case "descr":
            msg_enumerator.MoveNext();
            ele.descr = msg_enumerator.Current;
            break;
          case "timestamp":
            msg_enumerator.MoveNext();
            ele.timeStamp = DateTime.Parse(msg_enumerator.Current.ToString());
            break;
          case "children":
            msg_enumerator.MoveNext();
            int childcount = int.Parse(msg_enumerator.Current);
            while (childcount-- > 0)
            {
              msg_enumerator.MoveNext();
              childList.Add(int.Parse(msg_enumerator.Current));
            }
            ele.children = childList;
            break;
          case "payload":
            msg_enumerator.MoveNext();
            int payloadcount = int.Parse(msg_enumerator.Current);
            payload = addPayload(ref msg_enumerator);
            ele.payload = payload;
            break;
          default:
            break;
        }
      }
      db.insert(int.Parse(key1), ele);
      db.showDB();
      str = "new element added at key: " + key1;
      Console.WriteLine();
    }

    
    private string addPayload(ref IEnumerator<string> msg_enumerator)
    {
      string payload = "";
      msg_enumerator.MoveNext();
      payload = msg_enumerator.Current;
      return payload;
    }
  }
  #if (TEST_SERVERPARSER)
  class TestServerParser
  {
  static void main(string args[])
  {
    Util.verbose = false;
    Server srvr = new Server();
    srvr.address = "localhost";
    srvr.port = "8080";

    Console.Title = "Server: " + srvr.port.ToString();
    Console.Write(String.Format("\n  Starting CommService server listening on port {0}", srvr.port));
    Console.Write("\n ====================================================\n");

    Sender sndr = new Sender(Util.makeUrl(srvr.address, srvr.port));
    //Sender sndr = new Sender();
    Receiver rcvr = new Receiver(srvr.port, srvr.address);
    // - serviceAction defines what the server does with received messages
    // - This serviceAction just announces incoming messages and echos them
    //   back to the sender.  
    // - Note that demonstrates sender routing works if you run more than
    //   one client.
    Action serviceAction = () =>
    {
      Message msg = null;
      while (true)
      {
        msg = rcvr.getMessage();   // note use of non-service method to deQ messages          
        Console.Write("\n  Received message:\n");
        Console.Write("\n  sender is {0}\n", msg.fromUrl);
        Console.Write("\n  content is {0}\n", msg.content);
        if (msg.content == "connection start message")
        {
          continue; // don't send back start message
        }
        else if (msg.content == "done")
        {
          Console.Write("\n  client has finished\n");
          continue;
        }
        else if (msg.content == "closeServer")
        {
          Console.Write("\n\nreceived closeServer\n\n");
          break;
        }
        else
        {
          WriteLine();
          List<string> msg_list = msg.content.Split(',').ToList<string>();
          IEnumerator<string> msg_enumerator = msg_list.GetEnumerator();
          ServerParser sp = new ServerParser();
          string str = "";
          while (msg_enumerator.MoveNext())
          {
            bool flag_int = false, flag_str = false;
            switch (msg_enumerator.Current)
            {
              case "keytype":
                msg_enumerator.MoveNext();
                if (msg_enumerator.Current == "integer")
                {
                  flag_int = true;
                  flag_str = false;
                }
                if (msg_enumerator.Current == "String")
                {
                  flag_int = false;
                  flag_str = true;
                }
                break;
              case "payloadtype":
                msg_enumerator.MoveNext();
                if (msg_enumerator.Current == "String")
                {
                  flag_int = true;
                  flag_str = false;
                }
                if (msg_enumerator.Current == "List`1")
                {
                  flag_int = false;
                  flag_str = true;
                }
                if (flag_int)
                {
                  str = sp.parse(ref msg_enumerator, db);
                }
                else if (flag_str)
                {
                  str = sp.parse(ref msg_enumerator, enum_db);
                }
                break;
            }
          };
          // swap urls for outgoing message
          msg.content = str;
          Util.swapUrls(ref msg);
          /////////////////////////////////////////////////
          // Use the statement below for normal operation
          sndr.sendMessage(msg);
        }
      }
    };
    if (rcvr.StartService())
    {
      rcvr.doService(serviceAction); // This serviceAction is asynchronous,
    }
    // so the call doesn't block.
    Util.waitForUser();
  }
  }
#endif
}
