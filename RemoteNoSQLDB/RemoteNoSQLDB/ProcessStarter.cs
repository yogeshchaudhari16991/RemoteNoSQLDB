/////////////////////////////////////////////////////////////////////
// ProcessStarter.cs - Demonstrate running a child program         //
//                                                                 //
// Ver 1.0                                                         //
// Application: Demonstration for CSE681-SMA, Project#2            //
// Language:    C#, ver 6.0, Visual Studio 2015                    //
// Platform:    Dell Inspiron 14 5000Series, Core-i5, Windows 10   //
// Author:      Yogesh Chaudhari, Student, Syracuse University     //
//              (315) 4809210, ychaudha@syr.edu                    //
/////////////////////////////////////////////////////////////////////
/*
 *   Module Operations
 *   -----------------
 *   This module starts all the processes to start our WPF client,
 *   Server, Read Clients, Write Clients.
 *   It takes command line arguments as (numberofreaders numberofwriters) to start
 *   Command Line Argument: 2 2
 *   This package using processStarterInfo package sends command line arguments
 *   to processes it starts
 *
 *   Reference:
 *   ----------
 *   Dr. Fawcett's ProcessStarter Code
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
using System.Diagnostics;
using System.IO;

namespace ProcessStarter
{
  class ProcessStarter
  {   
    //----------< class to start processes >-------------------
    //----------< method to start GUI/WPF client process >------
    public bool startProcessGUI(string process)
    {
      process = Path.GetFullPath(process);
      Console.Write("\n  fileSpec - \"{0}\"", process);
      ProcessStartInfo psi = new ProcessStartInfo
      {
        FileName = process,
        // set UseShellExecute to true to see child console, false hides console
        UseShellExecute = true
      };
      try
      {
        Process p = Process.Start(psi);
        return true;
      }
      catch(Exception ex)
      {
        Console.Write("\n  {0}", ex.Message);
        return false;
      }
    }
    //----------< method to start server process >------
    public bool startProcessServer(string process)
    {
      process = Path.GetFullPath(process);
      Console.Write("\n  fileSpec - \"{0}\"", process);
      ProcessStartInfo psi = new ProcessStartInfo
      {
        FileName = process,
        Arguments = "8080 localhost",
        // set UseShellExecute to true to see child console, false hides console
        UseShellExecute = true
      };
      try
      {
        Process p = Process.Start(psi);
        return true;
      }
      catch (Exception ex)
      {
        Console.Write("\n  {0}", ex.Message);
        return false;
      }
    }
    //----------< method to start read client process >------
    public bool startProcessReader(string process, int count)
    {
      process = Path.GetFullPath(process);
      Console.Write("\n  fileSpec - \"{0}\"", process);
      int port = (8079 - count);
      string url = "http://localhost:" + port + "/CommService";
      ProcessStartInfo psi = new ProcessStartInfo
      {
        FileName = process,
        Arguments = "/L " + url + " /R http://localhost:8080/CommService",
        // set UseShellExecute to true to see child console, false hides console
        UseShellExecute = true
      };
      try
      {
        Process p = Process.Start(psi);
        return true;
      }
      catch (Exception ex)
      {
        Console.Write("\n  {0}", ex.Message);
        return false;
      }
    }
    //----------< method to start write client process >------
    public bool startProcessWriter(string process, int count)
    {
      process = Path.GetFullPath(process);
      Console.Write("\n  fileSpec - \"{0}\"", process);
      ProcessStartInfo psi;
      int port = (8081 + count);
      string url = "http://localhost:" + port + "/CommService";
      if (count % 2 == 0)
      {
        psi = new ProcessStartInfo
        {
          FileName = process,
          Arguments = "/L " + url + " /R http://localhost:8080/CommService /LOG true",
          // set UseShellExecute to true to see child console, false hides console
          UseShellExecute = true
        };
      }
      else
      {
        psi = new ProcessStartInfo
        {
          FileName = process,
          Arguments = "/L http://localhost:8082/CommService /R http://localhost:8080/CommService /LOG false",
          // set UseShellExecute to true to see child console, false hides console
          UseShellExecute = false
        };
      }
      try
      {
        Process p = Process.Start(psi);
        return true;
      }
      catch (Exception ex)
      {
        Console.Write("\n  {0}", ex.Message);
        return false;
      }
    }
    //----------< main method to start read and write client process
    //----------  server process and WPF client process by taking user 
    //----------   inputs >--------------------------------------------------
    static void Main(string[] args)
    {
      Console.Write("\n  current directory is: \"{0}\"", Directory.GetCurrentDirectory());
      ProcessStarter ps = new ProcessStarter();
      int count = 0, read_client = 2, write_client = 2;
      if (args.Length != 0) {
        if (args[0] != null && int.TryParse(args[0], out read_client) && int.Parse(args[0]) > 0)
          read_client = int.Parse(args[0]);
        if (args[1] != null && int.TryParse(args[1], out write_client) && int.Parse(args[1]) > 0)
          write_client = int.Parse(args[1]);
      }
      if(File.Exists("../../../GUI-Client/bin/Debug/GUI-Client.exe"))
        ps.startProcessGUI("../../../GUI-Client/bin/Debug/GUI-Client.exe");
      else
        ps.startProcessGUI("GUI-Client/bin/Debug/GUI-Client.exe");
      if(File.Exists("../../../Server/bin/Debug/Server.exe"))
        ps.startProcessServer("../../../Server/bin/Debug/Server.exe");
      else
        ps.startProcessServer("Server/bin/Debug/Server.exe");
      if(File.Exists("../../../Read Client/bin/Debug/Read Client.exe"))
      for (int i = 0; i < read_client; i++)
      {
        count++;
        ps.startProcessReader("../../../Read Client/bin/Debug/Read Client.exe", count);
      }
      else
        for (int i = 0; i < read_client; i++)
        {
          count++;
          ps.startProcessReader("Read Client/bin/Debug/Read Client.exe", count);
        }
      count = 0;
      if(File.Exists("../../../Write Client/bin/Debug/Write Client.exe"))
      for (int i = 0; i < write_client; i++)
      {
        count++;
        ps.startProcessWriter("../../../Write Client/bin/Debug/Write Client.exe", count);
      }
      else
        for (int i = 0; i < write_client; i++)
        {
          count++;
          ps.startProcessWriter("Write Client/bin/Debug/Write Client.exe", count);
        }
      Console.Write("\n  press key to exit: ");
      Console.ReadKey();
    }
  }
}
