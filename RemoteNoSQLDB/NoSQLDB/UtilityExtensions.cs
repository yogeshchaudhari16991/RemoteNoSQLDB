//////////////////////////////////////////////////////////////////////////////
// UtilityExtensions.cs - Define element for noSQL database                         //
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
 * This package implements utility extensions that are not specific
 * to a single package.
 */
/*
 * Maintenance:
 * ------------
 * Required Files: UtilityExtensions.cs
 *
 * Build Process:  devenv Project2Starter.sln /Rebuild debug
 *                 Run from Developer Command Prompt
 *                 To find: search for developer
 *
 *
 * Reference: Project2Starter project by Dr. Fawcett
 * ----------
 *
 * Maintenance History:
 * --------------------
 * ver 1.0 : 07 Oct 15
 * - first release
 *
 */
using System;
using static System.Console;

namespace Project2
{
  public static class UtilityExtensions
  {
    public static void title(this string aString, char underline = '-')
    {
      Console.Write("\n  {0}", aString);
      Console.Write("\n {0}", new string(underline, aString.Length + 2));
    }
  }
  #if (TEST_UTILITYEXTENSIONS)
  public class TestUtilityExtensions
  {
    static void Main(string[] args)
    {
      "Testing UtilityExtensions.title".title();
      Write("\n\n");
    }
  }
#endif
}
