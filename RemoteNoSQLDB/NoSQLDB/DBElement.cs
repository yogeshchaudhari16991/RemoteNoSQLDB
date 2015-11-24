//////////////////////////////////////////////////////////////////////////////
// DBElement.cs - Define element for noSQL database                         //
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
 * This package implements the DBElement<Key, Data> type, used by 
 * DBEngine<key, Value> where Value is DBElement<Key, Data>.
 *
 * The DBElement<Key, Data> state consists of metadata and an
 * instance of the Data type.
 *
 */
/*
 * Maintenance:
 * ------------
 * Required Files: DBElement.cs, UtilityExtensions.cs
 *
 * Build Process:  devenv Project2Starter.sln /Rebuild debug
 *                 Run from Developer Command Prompt
 *                 To find: search for developer
 *
 * Reference: Project2Starter project by Dr. Fawcett
 * ----------
 *
 * Maintenance History:
 * --------------------
 * ver 1.0 : 7 Oct 15
 * - first release
 *
 */
using System;
using System.Collections.Generic;
using static System.Console;

namespace Project2
{
  /////////////////////////////////////////////////////////////////////
  // DBElement<Key, Data> class
  // - Instances of this class are the "values" in our key/value 
  //   noSQL database.
  // - Key and Data are unspecified classes, to be supplied by the
  //   application that uses the noSQL database.
  //   See the teststub below for examples of use.

  public class DBElement<Key, Data>
  {
    public string name { get; set; }          // metadata    |
    public string descr { get; set; }         // metadata    |
    public DateTime timeStamp { get; set; }   // metadata   value
    public List<Key> children { get; set; }   // metadata    |
    public Data payload { get; set; }         // data        |

    public DBElement(string Name = "unnamed", string Descr = "undescribed")
    {
      name = Name;
      descr = Descr;
      timeStamp = DateTime.Now;
      children = new List<Key>();
    }
  }

#if (TEST_DBELEMENT)

  class TestDBElement
  {
    static void Main(string[] args)
    {
      "Testing DBElement Package".title('=');
      WriteLine();

      Write("\n  All testing of DBElement class moved to DBElementTest package.");
      Write("\n  This allow use of DBExtensions package without circular dependencies.");

      Write("\n\n");
    }
  }
#endif
}
