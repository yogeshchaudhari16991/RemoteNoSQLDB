/////////////////////////////////////////////////////////////////////////
// Parser.cs - Parses Write Client Sent Messages                        //
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
 *   This module demonstrates how the XML messages are parsed
 *   by Write Client to generate request messages to send to server
 *
 * Additions to C# Console Wizard generated code:
 * - Added using Project4Starter
 * - Added reference to CommunicationChannel
 *
 *
 * Maintenance History:
 * --------------------
 * ver 1.0 : 19 November 2015
 * - first release
*
*/
using Project4Starter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Write_Client
{
  //--------< Parser class to parse content from XML and generate 
  //--------  message to be sent to server >-------------------------- 
  public class Parser
  {
    private static bool logger_flag;

    public Parser(bool flag)
    {
      logger_flag = flag;
    }

    public void parse(XDocument newDoc, ref Message msg, Sender sndr)
    {
      Console.WriteLine("Reading file: " + newDoc.ToString());
      Console.WriteLine();
      var root = newDoc.Root.Elements("DB");
      foreach (XElement Root in root)
      {
        string keytype, payloadtype = "";
        keytype = Root.Element("KeyType").Value.ToString();
        payloadtype = Root.Element("PayloadType").Value.ToString();
        msg.content = "keytype," + keytype + ",payloadtype," + payloadtype;
        var query = Root.Elements("Query");
        IEnumerator<XElement> x = query.GetEnumerator();
        string message = new String(msg.content.ToCharArray());
        while (x.MoveNext())
        {
          msg.content += ",query,";
          string querytype = "";
          querytype = x.Current.Element("QueryType").Value.ToString();
          msg.content += "querytype," + querytype;
          parseQuery(querytype, x, ref msg, sndr);
          msg.content = message;
        }
      }
    }
    private static void parseQuery(string querytype, IEnumerator<XElement> x, ref Message msg1, Sender sndr)
    {
      Message msg = new Message();
      msg.fromUrl = msg1.fromUrl;
      msg.toUrl = msg1.toUrl;
      msg.content = msg1.content.ToString();
      switch (querytype)
      {
        case "Insert Element":
          insert_element(x, ref msg, sndr);
          break;
        case "Delete Element":
          delete_element(x, ref msg, sndr);
          break;
        case "Edit Element Metadata":
          edit_element_metadata(x, ref msg, sndr);
          break;
        case "Edit Element Metadata and Add Children":
          edit_element_metadata(x, ref msg, sndr);
          break;
        case "Edit Element Metadata and Remove Children":
          edit_element_metadata(x, ref msg, sndr);
          break;
        case "Edit Element Metadata and Edit Payload":
          edit_element_metadata(x, ref msg, sndr);
          break;
        case "Persist Database":
          persist_database(x, ref msg, sndr);
          break;
        case "Restore Database":
          restore_database(x, ref msg, sndr);
          break;
      }
    }

    private static void restore_database(IEnumerator<XElement> x, ref Message msg, Sender sndr)
    {
      string source_path = x.Current.Element("Source").Value;
      msg.content += ",Source," + source_path;
      if (Client.logger_flag)
        Console.Write("\n  sending {0}", msg.content + "\n");
      if (!sndr.sendMessage(msg))
        return;
      Thread.Sleep(100);
    }


    private static void persist_database(IEnumerator<XElement> x, ref Message msg, Sender sndr)
    {
      string destination_path = x.Current.Element("Destination").Value;
      msg.content += ",Destination," + destination_path;
      if (logger_flag) Console.Write("\n  sending {0}", msg.content + "\n");
      if (!sndr.sendMessage(msg))
        return;
      Thread.Sleep(100);
    }

    private static void edit_element_metadata(IEnumerator<XElement> x, ref Message msg, Sender sndr)
    {
      int numQueries = int.Parse(x.Current.Element("NumberOfQueries").Value.ToString());
      int counter = 0;
      string message = new String(msg.content.ToCharArray());
      while (counter++ < numQueries)
      {
        string str = "";
        str = str + ",key," + x.Current.Element("Key").Value.ToString() + counter;
        ParseMetadata(ref str, x.Current.Element("Element"));
        msg.content += str;
        if (logger_flag) Console.Write("\n  sending {0}", msg.content + "\n");
        if (!sndr.sendMessage(msg))
          return;
        Thread.Sleep(100);
        msg.content = message.ToString();
      }
    }

    private static void delete_element(IEnumerator<XElement> x, ref Message msg, Sender sndr)
    {
      int numQueries = int.Parse(x.Current.Element("NumberOfQueries").Value.ToString());
      //msg.content += ",numqueries," + numQueries;
      int counter = 0;
      string message = new String(msg.content.ToCharArray());
      while (counter++ < numQueries)
      {
        string str = "";
        str = str + ",key," + x.Current.Element("Key").Value.ToString() + counter;
        msg.content += str;
        if (logger_flag) Console.Write("\n  sending {0}", msg.content + "\n");
        if (!sndr.sendMessage(msg))
          return;
        Thread.Sleep(100);
        msg.content = message.ToString();
      }
    }

    private static void insert_element(IEnumerator<XElement> x, ref Message msg, Sender sndr)
    {
      int numQueries = int.Parse(x.Current.Element("NumberOfQueries").Value.ToString());
      int counter = 0;
      string message = new String(msg.content.ToCharArray());
      while (counter++ < numQueries)
      {
        string str = "";
        str = str + ",key," + x.Current.Element("Key").Value.ToString() + counter;
        ParseMetadata(ref str, x.Current.Element("Element"));
        msg.content += str;
        if (logger_flag) Console.Write("\n  sending {0}", msg.content + "\n");
        if (!sndr.sendMessage(msg))
          return;
        Thread.Sleep(100);
        msg.content = message.ToString();
      }
    }

    public static void ParseMetadata(ref string msg, XElement ele)
    {
      string name, descr, timestamp = "";
      name = ele.Element("name").Value.ToString();
      descr = ele.Element("descr").Value.ToString();
      timestamp = ele.Element("timestamp").Value.ToString();
      msg = msg + ",name," + name + ",descr," + descr + ",timestamp," + timestamp;
      var children = ele.Element("children").Elements();
      var payload = ele.Element("payload").Elements();
      int count = 0;
      string temp = "";
      foreach (var item in children)
      {
        count++;
        temp += "," + item.Value.ToString();
      }
      msg += ",children," + count + temp;

      count = 0;
      temp = "";
      foreach (var item in payload)
      {
        count++;
        temp += "," + item.Value.ToString();
      }
      msg += ",payload," + count + temp;
    }
  }
#if (TEST_PARSER)
  class TestParser
  {
    void static main(string args[])
    {
      Sender sndr = new Sender(http://localhost:8082/CommService);  // Sender needs localUrl for start message
      Message msg = new Message();
      msg.fromUrl = "http://localhost:8082/CommService";
      msg.toUrl = "http://localhost:8080/CommService";
      Console.Write("\n  sender's url is {0}", msg.fromUrl);
      Console.Write("\n  attempting to connect to {0}\n", msg.toUrl);
      if (!sndr.Connect(msg.toUrl))
      {
        Console.Write("\n  could not connect in {0} attempts", sndr.MaxConnectAttempts);
        sndr.shutdown();
        rcvr.shutDown();
        return;
      }
      "Reading write1.xml file".title();
      string path = Path.GetFullPath("../../../Write Client/bin/Debug/write1.xml");
      XDocument newDoc = XDocument.Load(path);
      clnt.write_clinet_parse.Start();
      bool logger_flag = true;
      Parser p = new Parser(logger_flag);
      p.parse(newDoc, ref msg, sndr);
      msg.content = "done";
      sndr.sendMessage(msg);
    }
  }
#endif
}
