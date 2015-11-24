/////////////////////////////////////////////////////////////////////////
// Server.cs - CommService server                                      //
// Ver 1.0                                                             //
// Application: Demonstration for CSE681-SMA, Project#2                //
// Language:    C#, ver 6.0, Visual Studio 2015                        //
// Platform:    Dell Inspiron 14 5000Series, Core-i5, Windows 10       //
// Author:      Yogesh Chaudhari, Student, Syracuse University         //
//              (315) 4809210, ychaudha@syr.edu                        //
/////////////////////////////////////////////////////////////////////////
/*
 *
 *   Module Operations
 *   -----------------
 *   This module acts as server for all read clients, write clients and WPF clients 
 *   It demonstartes communication between server and all other requesting clients
 *   using sender and receiver of communication channel.
 *   Using HiResTimer server calculates throughput for all messages
 *   Server passes messages to ServerParser to make NoSQLDatabase calls
 *
 *
 * Additions to C# Console Wizard generated code:
 * - Added using Project2 
 * - Added reference to Communication Channel
 *
 * Note:
 * - This server now receives and then sends back received messages.
 */
/*
 */
/*
 *   Reference:
 *   ----------
 *   Dr. Fawcett's CommPrototype Code
 *
 * Maintenance History:
 * --------------------
 * ver 1.0 : 19 November 2015
 * - first release
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Console;
using Project2;

namespace Project4Starter
{
  using Util = Utilities;

  class Server
  {
    string address { get; set; } = "localhost";
    string port { get; set; } = "8080";
    private static DBEngine<string, DBElement<string, List<string>>> enum_db = new DBEngine<string, DBElement<string, List<string>>>();
    private static DBEngine<int, DBElement<int, string>> db = new DBEngine<int, DBElement<int, string>>();
    private HRTimer.HiResTimer server_throuput = new HRTimer.HiResTimer();
    
    //----< quick way to grab ports and addresses from commandline >-----
    public void ProcessCommandLine(string[] args)
    {
      if (args.Length > 0)
      {
        port = args[0];
      }
      if (args.Length > 1)
      {
        address = args[1];
      }
    }
    static void Main(string[] args)
    {
      Util.verbose = false;
      Server srvr = new Server();
      srvr.ProcessCommandLine(args);
      
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
      
      if (rcvr.StartService())
      {
        rcvr.doService(doserviceAction(sndr, rcvr, srvr)); // This serviceAction is asynchronous,
      }
      // so the call doesn't block.
      Util.waitForUser(); 
    }
    //--------< Define action to be performed on receiving message >------
    private static Action doserviceAction(Sender sndr, Receiver rcvr, Server srvr)
    {
      int counter_write = 0;
      int counter_read = 0;
      ulong write_clnt_process_time = 0;
      ulong read_clnt_latency_time = 0;
      ulong server_throughput_time = 0;
      Action serviceAction = () =>
      {
        Message msg = null;
        while (true)
        {
          msg = rcvr.getMessage();   // note use of non-service method to deQ messages          
          Console.Write("\n  Received message:\n");
          Console.Write("\n  sender is {0}\n", msg.fromUrl);
          Console.Write("\n  content is {0}\n", msg.content);
          if (specialMessage(ref msg, srvr, sndr, rcvr, ref read_clnt_latency_time, ref write_clnt_process_time, ref server_throughput_time, ref counter_write, ref counter_read))
          {
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
              getDatabase(ref sp, ref msg_enumerator, ref str);
            };
            // swap urls for outgoing message
            msg.content = str;
            Util.swapUrls(ref msg);
            /////////////////////////////////////////////////
            // Use the statement below for normal operation
            Console.WriteLine("\n\n Sending Message: " + msg.content + "\n\n");
            sndr.sendMessage(msg);
          }
        }
      };
      return serviceAction;
    }
    //-----------< action for received special messages >------------
    private static bool specialMessage(ref Message msg, Server srvr, Sender sndr, Receiver rcvr, ref ulong read_clnt_latency_time, ref ulong write_clnt_process_time, ref ulong server_throughput_time, ref int counter_write, ref int counter_read)
    {
      if (msg.content == "connection start message")
      {
        srvr.server_throuput.Start();
        return true; // don't send back start message
      }
      else if (msg.content == "done")
      {
        //---------< maitain test performance data for server throughput >----
        srvr.server_throuput.Stop();
        server_throughput_time += srvr.server_throuput.ElapsedMicroseconds;
        Console.Write("\n  client has finished\n");
        Console.WriteLine("\nServer throughput: " + server_throughput_time + " microseconds\n");
        Util.swapUrls(ref msg);
        Console.WriteLine("\n\n Sending Message: " + msg.content + "\n\n");
        sndr.sendMessage(msg);
        return true;
      }
      else
           if (msg.content.Contains("write-client"))
      {
        write_client_processing(ref counter_write, ref write_clnt_process_time, ref msg);
        return true;
      }
      else if (msg.content.Contains("read-client"))
      {
        read_client_latency(ref counter_read, ref read_clnt_latency_time, ref msg);
        return true;
      }
      else if (msg.content == ("test-result"))
      {
        send_test_result(ref msg, ref read_clnt_latency_time, write_clnt_process_time, server_throughput_time, counter_read, counter_write, sndr);
        return true;
      }
      else
      {
        return false;
      }
    }
    //---------< maitain test performance data for write client processing >---
    private static void write_client_processing(ref int counter_write, ref ulong write_clnt_process_time, ref Message msg)
    {
      counter_write++;
      List<string> msg_list = msg.content.Split(',').ToList<string>();
      write_clnt_process_time += ulong.Parse(msg_list[1]);
    }
    //---------< maitain test performance data for read client latency >-------
    private static void read_client_latency(ref int counter_read, ref ulong read_clnt_latency_time, ref Message msg)
    {
      counter_read++;
      List<string> msg_list = msg.content.Split(',').ToList<string>();
      read_clnt_latency_time += ulong.Parse(msg_list[1]);
    }
    //---------< send test performance data to WPF client >-------
    private static void send_test_result(ref Message msg, ref ulong read_clnt_latency_time, ulong write_clnt_process_time, ulong server_throughput_time, int counter_read, int counter_write, Sender sndr)
    {
      msg.content = "test-result";
      if (counter_write > 0)
        msg.content += ", average write-client processing time is: " + (write_clnt_process_time / Convert.ToUInt64(counter_write)) + "microseconds";
      if (counter_read > 0)
        msg.content += ", average read-client latency time is: " + (read_clnt_latency_time / Convert.ToUInt64(counter_read)) + "microseconds";
      if (counter_read > 0 || counter_write > 0)
        msg.content += ", average Server Query Processing/Throughput Time is: " + (server_throughput_time / Convert.ToUInt64((counter_read + counter_write)) + "microseconds");
      else
        msg.content += ", performance tests yet to finish";
      Util.swapUrls(ref msg);
      Console.WriteLine("\n\n Sending Message: " + msg.content + "\n\n");
      sndr.sendMessage(msg);
    }
    //-----------< action for received query messages >------------
    private static void getDatabase(ref ServerParser sp, ref IEnumerator<string> msg_enumerator, ref string str)
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
    }
  }
}
