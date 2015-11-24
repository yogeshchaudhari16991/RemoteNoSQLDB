/////////////////////////////////////////////////////////////////////////////
//  BlockingQueue.cs - demonstrate threads communicating via Queue         //
// Ver 1.0                                                                 //
// Application: Demonstration for CSE681-SMA, Project#2                    //
// Language:    C#, ver 6.0, Visual Studio 2015                            //
// Platform:    Dell Inspiron 14 5000Series, Core-i5, Windows 10           //
// Author:      Yogesh Chaudhari, Student, Syracuse University             //
//              (315) 4809210, ychaudha@syr.edu                            //
/////////////////////////////////////////////////////////////////////////////
/*
 *   Module Operations
 *   -----------------
 *   This module demonstrates communication between two threads using a 
 *   blocking queue.  If the queue is empty when the reader attempts to deQ
 *   an item then the reader will block until the writing thread enQs an item.
 *   Thus waiting is efficient.
 * 
 *   NOTE:
 *   This blocking queue is implemented using a Monitor and lock, which is
 *   equivalent to using a condition variable with a lock.
 * 
 *   Public Interface
 *   ----------------
 *   BlockingQueue<string> bQ = new BlockingQueue<string>();
 *   bQ.enQ(msg);
 *   string msg = bQ.deQ();
 * 
 */
/*
 *   Reference:
 *   ----------
 *   Dr. Fawcett's CommPrototype Code
 *
 *   Build Process
 *   -------------
 *   - Required files:   BlockingQueue.cs, Program.cs
 *   - Compiler command: csc BlockingQueue.cs Program.cs
 * 
 *   Maintenance History
 *   -------------------
 *   ver 1.0 : 19 November 2015
 *     - first release
 * 
 */

//
using System;
using System.Collections;
using System.Threading;

namespace SWTools
{
  public class BlockingQueue<T>
  {
    private Queue blockingQ;
    object locker_ = new object();

    //----< constructor >--------------------------------------------

    public BlockingQueue()
    {
      blockingQ = new Queue();
    }
    //----< enqueue a string >---------------------------------------

    public void enQ(T msg)
    {
      lock (locker_)  // uses Monitor
      {
        blockingQ.Enqueue(msg);
        Monitor.Pulse(locker_);
      }
    }
    //----< dequeue a T >---------------------------------------
    //
    // Note that the entire deQ operation occurs inside lock.
    // You need a Monitor (or condition variable) to do this.

    public T deQ()
    {
      T msg = default(T);
      lock(locker_)
      {
        while (this.size() == 0)
        {
          Monitor.Wait(locker_);          
        }
        msg = (T)blockingQ.Dequeue();
        return msg;
      }
    }
    //
    //----< return number of elements in queue >---------------------

    public int size()
    {
      int count;
      lock (locker_) { count = blockingQ.Count; }
      return count;
    }
    //----< purge elements from queue >------------------------------

    public void clear() 
    {
      lock(locker_) { blockingQ.Clear(); }
    }
  }
}
