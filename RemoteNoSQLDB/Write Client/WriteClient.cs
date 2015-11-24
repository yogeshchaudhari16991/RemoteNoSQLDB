/////////////////////////////////////////////////////////////////////////
// WriteClient.cs - CommService client sends and receives messages         //
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
 *   This module demonstrates communication between Write Client and 
 *   Server using sender and receiver of communication channel.
 *   Write Client can change the database state by making requests to the server
 *   Using HiResTimer write client calculates time required to send all messages
 *   Write Client reads write1.xml file to generate messages to be sent to server
 *
 *
 * Additions to C# Console Wizard generated code:
 * - Added using System.Threading
 * - Added reference to Communication Channel
 *
 * Note:
 * - in this incantation the client has Sender and now has Receiver to
 *   retrieve Server echo-back messages.
 * - If you provide command line arguments they should be ordered as:
 *   /r or /R remoteAddress:remotePort
 *   /l or /L localAddress:localPort
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
using System.Threading;
using static System.Console;
using Write_Client;

namespace Project4Starter
{
  using System.IO;
  using System.Xml.Linq;
  using Util = Utilities;

  ///////////////////////////////////////////////////////////////////////
  // Client class sends and receives messages in this version
  // - commandline format: /L http://localhost:8085/CommService 
  //                       /R http://localhost:8080/CommService
  //   Either one or both may be ommitted

  class Client
  {
    string localUrl { get; set; } = "http://localhost:8082/CommService";
    string remoteUrl { get; set; } = "http://localhost:8080/CommService";
    public static bool logger_flag = true;//flag to check logger request

    //----< retrieve urls from the CommandLine if there are any >--------
    private HRTimer.HiResTimer write_clinet_parse = new HRTimer.HiResTimer();
    public void processCommandLine(string[] args)
    {
      if (args.Length == 0)
        return;
      localUrl = Util.processCommandLineForLocal(args, localUrl);
      remoteUrl = Util.processCommandLineForRemote(args, remoteUrl);
      logger_flag = Util.processLoggerFlag(args, logger_flag);
      //Console.WriteLine("\n\n\n" + logger_flag + "\n\n\n");
    }
    static void Main(string[] args)
    {
      Console.Write("\n  starting CommService client");
      Console.Write("\n =============================\n");
      Client clnt = new Client();
      clnt.processCommandLine(args);
      string localPort = Util.urlPort(clnt.localUrl);
      string localAddr = Util.urlAddress(clnt.localUrl);
      Receiver rcvr = new Receiver(localPort, localAddr);
      Console.Title = "Write Client: " + localPort;
      if (rcvr.StartService())
        rcvr.doService(doserviceAction(rcvr));
      Sender sndr = new Sender(clnt.localUrl);  // Sender needs localUrl for start message
      Message msg = new Message();
      msg.fromUrl = clnt.localUrl;
      msg.toUrl = clnt.remoteUrl;
      Console.Write("\n  sender's url is {0}", msg.fromUrl);
      Console.Write("\n  attempting to connect to {0}\n", msg.toUrl);
      if (!sndr.Connect(msg.toUrl))
      {
        Console.Write("\n  could not connect in {0} attempts", sndr.MaxConnectAttempts);
        shutdown(rcvr, sndr);
        return;
      }
      "Reading write1.xml file".title();
      string path = Path.GetFullPath("../../../Write Client/bin/Debug/write1.xml");
      XDocument newDoc = XDocument.Load(path);
      clnt.write_clinet_parse.Start();
      Parser p = new Parser(logger_flag);
      p.parse(newDoc, ref msg, sndr);
      Message msg1 = new Message();
      msg1.fromUrl = clnt.localUrl;
      msg1.toUrl = clnt.remoteUrl;
      msg1.content = "done";
      sndr.sendMessage(msg1);
      clnt.write_clinet_parse.Stop();
      Message msg2 = new Message();
      msg2.fromUrl = clnt.localUrl;
      msg2.toUrl = clnt.remoteUrl;
      msg2.content = "write-client," + clnt.write_clinet_parse.ElapsedMicroseconds;
      sndr.sendMessage(msg2);
      Console.WriteLine("\n\nWrite-Client Processing Time: " + clnt.write_clinet_parse.ElapsedMicroseconds + " microseconds\n");
      // Wait for user to press a key to quit.
      // Ensures that client has gotten all server replies.
      Util.waitForUser();
      // shut down this client's Receiver and Sender by sending close messages
      shutdown(rcvr, sndr);
      Console.Write("\n\n");
    }
    //--------< Define action to be performed on receiving message >------
    private static Action doserviceAction(Receiver rcvr)
    {
      Action serviceAction = () =>
      {
        if (Util.verbose)
          Console.Write("\n  starting Receiver.defaultServiceAction");
        Message msg1 = null;
        while (true)
        {
          msg1 = rcvr.getMessage();   // note use of non-service method to deQ messages
          Console.Write("\n  Received message:");
          Console.Write("\n  sender is {0}", msg1.fromUrl);
          Console.Write("\n  content is {0}\n", msg1.content);
          if (msg1.content == "closeReceiver")
            break;
        }
      };
      return serviceAction;
    }
    //--------< shutdown receiver and sender >---------------------------
    private static void shutdown(Receiver rcvr, Sender sndr)
    {
      sndr.shutdown();
      rcvr.shutDown();
    }
  }
}