//////////////////////////////////////////////////////////////////////////////
// IQuery.cs - Define element for noSQL database                         //
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
 * This package provides an interface of type <Key, Value> to DBEngine and DBfactory packages
 * This interface provides DBEngine and DBFactory  getValue<Key, Value> and IEnumerable<key> Keys() methods to 
 * implement
 *
 */
/*
 * Maintenance:
 * ------------
 * Required Files: IQuery.cs
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


using System.Collections.Generic;
using static System.Console;

namespace Project2
{
    public interface IQuery<Key, Value>
    {
        //generic getValue<Key, Value>() method to be implemented by DBEngine and DBFactory package
        bool getValue(Key key, out Value val);
        //generic Keys<IEnumerable<Key>>() method to be implemented by DBEngine and DBFactory package
        IEnumerable<Key> Keys();
    }

#if (TEST_IQUERY)
    class TestIQuery
    {
        static void Main(string[] args)
        {
            WriteLine("Interface IQuery doesnot provide method definations to implement");
            WriteLine("DBFactory and DBEngine teststubs provide testing for IQuery interface.");
            WriteLine("In DBFactory at line 127.");
            WriteLine("In DBEngineTest at line 97");
        }
    }
#endif
}
