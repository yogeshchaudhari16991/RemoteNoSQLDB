/////////////////////////////////////////////////////////////////////////
// CommService.cs - Implementation of WCF message-passing service      //
// Ver 1.0                                                             //
// Application: Demonstration for CSE681-SMA, Project#2                //
// Language:    C#, ver 6.0, Visual Studio 2015                        //
// Platform:    Dell Inspiron 14 5000Series, Core-i5, Windows 10       //
// Author:      Yogesh Chaudhari, Student, Syracuse University         //
//              (315) 4809210, ychaudha@syr.edu                        //
/////////////////////////////////////////////////////////////////////////
/*
 * Additions to the C# Console Wizard code:
 * - added reference to System.ServiceModel
 * - added using System.ServiceModel
 * - added reference to Project4Starter.ICommService
 * - copied BlockingQueue.cs into project folder
 * - Added BlockingQueue.cs to project
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
using System.ServiceModel;
using HRTimer;
namespace Project4Starter
{
  using SWTools;
  using Util = Utilities;

  [ServiceBehavior(InstanceContextMode=InstanceContextMode.PerSession)]
  public class CommService : ICommService
  {
    // static rcvrQueue is shared by all instances of this class

    private static SWTools.BlockingQueue<Message> rcvrQueue = 
      new SWTools.BlockingQueue<Message>();
    //----< called by clients, will only block briefly >-----------------

    public void sendMessage(Message msg)
    {
      if(Util.verbose)
        Console.Write("\n  this is CommService.sendMessage");
      rcvrQueue.enQ(msg);
    }
    //----< called by server, blocks caller while empty >----------------
    /*
     * Note: this is NOT a service method - see interface definition
     */
    public Message getMessage()
    {
      
      if(Util.verbose)
        Console.Write("\n  this is CommService.getMessage");
      return rcvrQueue.deQ();
    }
  }
}
