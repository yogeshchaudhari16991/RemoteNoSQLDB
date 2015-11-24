/////////////////////////////////////////////////////////////////////////
// Parser.cs - Parses Read Client Sent Messages                        //
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
 *   by Read Client to generate request messages to send to server
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

namespace Read_Client
{
  //--------< Parser class to parse content from XML and generate 
  //--------  message to be sent to server >-------------------------- 
  public class Parser
  {
    public void parse(XDocument newDoc, ref Message msg, Sender sndr)
    {
      
      Console.WriteLine(newDoc.ToString());
      Console.WriteLine();
      var root = newDoc.Root.Elements("DB");
      foreach (XElement Root in root)
      {
        string keytype, payloadtype = "";
        keytype = Root.Element("KeyType").Value.ToString();
        payloadtype = Root.Element("PayloadType").Value.ToString();
        msg.content = "keytype," + keytype + ",payloadtype," + payloadtype;
        Console.WriteLine("\n" + msg.content);
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
        case "Search Key-Value":
          search_key_value(x, ref msg, sndr);
          break;
        case "Search Children":
          search_children(x, ref msg, sndr);
          break;
        case "Pattern Matching":
          pattern_matching(x, ref msg, sndr);
          break;
        case "String in Metadata":
          string_metadata(x, ref msg, sndr);
          break;
        case "Time-Date Interval":
          time_date_interval(x, ref msg, sndr);
          break;
        case "Restore":
          restore_database(x, ref msg, sndr);
          break;
      }
    }

    private static void restore_database(IEnumerator<XElement> x, ref Message msg, Sender sndr)
    {
      string source_path = x.Current.Element("Source").Value;
      msg.content += ",Source," + source_path;
      if (!sndr.sendMessage(msg))
        return;
      Thread.Sleep(100);
    }

    private static void time_date_interval(IEnumerator<XElement> x, ref Message msg, Sender sndr)
    {
      int numQueries = int.Parse(x.Current.Element("NumberOfQueries").Value.ToString());
      int counter = 0;
      string message = new String(msg.content.ToCharArray());
      while (counter++ < numQueries)
      {
        string str = "";
        DateTime date = new DateTime();
        if (DateTime.TryParse(x.Current.Element("SDateTime").Value.ToString(), out date))
        {
          str = str + ",sdate-time," + x.Current.Element("SDateTime").Value.ToString();
        }
        else
        {
          return;
        }
        if (x.Current.Element("EDateTime").Value.ToString() == "" || DateTime.TryParse(x.Current.Element("SDateTime").Value.ToString(), out date))
        {
          str = str + ",edate-time," + x.Current.Element("EDateTime").Value.ToString() + ",";
        }
        else
        {
          return;
        }
        msg.content += str;
        Console.Write("\n  sending {0}", msg.content + "\n");
        if (!sndr.sendMessage(msg))
          return;
        Thread.Sleep(100);
        msg.content = message.ToString();
      }
    }

    private static void string_metadata(IEnumerator<XElement> x, ref Message msg, Sender sndr)
    {
      int numQueries = int.Parse(x.Current.Element("NumberOfQueries").Value.ToString());
      int counter = 0;
      string message = new String(msg.content.ToCharArray());
      while (counter++ < numQueries)
      {
        string str = "";
        str = str + ",string," + x.Current.Element("String").Value.ToString();
        msg.content += str;
        Console.Write("\n  sending {0}", msg.content + "\n");
        if (!sndr.sendMessage(msg))
          return;
        Thread.Sleep(100);
        msg.content = message.ToString();
      }
    }

    private static void pattern_matching(IEnumerator<XElement> x, ref Message msg, Sender sndr)
    {
      int numQueries = int.Parse(x.Current.Element("NumberOfQueries").Value.ToString());
      int counter = 0;
      string message = new String(msg.content.ToCharArray());
      while (counter++ < numQueries)
      {
        string str = "";
        str = str + ",pattern," + x.Current.Element("Pattern").Value.ToString() + ",";
        if (x.Current.Element("Pattern").Value.ToString() != "")
        {
          str = str + counter;
        }
        msg.content += str;
        Console.Write("\n  sending {0}", msg.content + "\n");
        if (!sndr.sendMessage(msg))
          return;
        Thread.Sleep(100);
        msg.content = message.ToString();
      }
    }

    private static void search_children(IEnumerator<XElement> x, ref Message msg, Sender sndr)
    {
      int numQueries = int.Parse(x.Current.Element("NumberOfQueries").Value.ToString());
      int counter = 0;
      string message = new String(msg.content.ToCharArray());
      while (counter++ < numQueries)
      {
        string str = "";
        str = str + ",key," + x.Current.Element("Key").Value.ToString() + counter;
        msg.content += str;
        Console.Write("\n  sending {0}", msg.content + "\n");
        if (!sndr.sendMessage(msg))
          return;
        Thread.Sleep(100);
        msg.content = message.ToString();
      }
    }

    private static void search_key_value(IEnumerator<XElement> x, ref Message msg, Sender sndr)
    {
      int numQueries = int.Parse(x.Current.Element("NumberOfQueries").Value.ToString());
      int counter = 0;
      string message = new String(msg.content.ToCharArray());
      while (counter++ < numQueries)
      {
        string str = "";
        str = str + ",key," + x.Current.Element("Key").Value.ToString() + counter;
        msg.content += str;
        Console.Write("\n  sending {0}", msg.content + "\n");
        if (!sndr.sendMessage(msg))
          return;
        Thread.Sleep(100);
        msg.content = message.ToString();
      }
    }
  }
#if (TEST_PARSER)
  class TestParser
  {
    void static main(string args[])
    {
      Sender sndr = new Sender("http://localhost:8079/CommService");  // Sender needs localUrl for start message
      Message msg = new Message();
      msg.fromUrl = "http://localhost:8079/CommService";
      msg.toUrl = "http://localhost:8080/CommService";
      Console.Write("\n  sender's url is {0}", msg.fromUrl);
      Console.Write("\n  attempting to connect to {0}\n", msg.toUrl);
      if (!sndr.Connect(msg.toUrl))
      {
        Console.Write("\n  could not connect in {0} attempts", sndr.MaxConnectAttempts);
        shutdown(rcvr, sndr);
        return;
      }
      "Reading read1.xml file".title();
      string path = Path.GetFullPath("../../../Read Client/bin/Debug/read1.xml");
      XDocument newDoc = XDocument.Load(path);
      Parser p = new Parser();
      p.parse(newDoc, ref msg, sndr);
      msg.content = "done";
      sndr.sendMessage(msg);
    }
  }
#endif
}
