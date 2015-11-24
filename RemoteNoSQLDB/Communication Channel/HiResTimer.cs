///////////////////////////////////////////////////////////////////////
///  HiResTimer.cs - High Resolution Timer - Uses Win32             ///
///                  Performance Counters and .Net Interop          ///                                ///
/// Ver 1.0                                                         ///
/// Application: Demonstration for CSE681-SMA, Project#2            ///
/// Language:    C#, ver 6.0, Visual Studio 2015                    ///
/// Platform:    Dell Inspiron 14 5000Series, Core-i5, Windows 10   ///
/// Author:      Yogesh Chaudhari, Student, Syracuse University     ///
///              (315) 4809210, ychaudha@syr.edu                    ///
///////////////////////////////////////////////////////////////////////
/// Based on:                                                       ///
/// Windows Developer Magazine Column: Tech Tips, August 2002       ///
/// Author: Shawn Van Ness, shawnv@arithex.com                      ///
///////////////////////////////////////////////////////////////////////
 //*   Module Operations
 //*   -----------------
 //*   This module provides HiResTimer for performance testing in our project
 //
 //* Reference:
 //*   ----------
 //* Dr.Fawcett's CommPrototype Code
 //*
 //* Maintenance History:
 //* --------------------
 //* ver 1.0 : 19 November 2015
 //* - first release
 ///////////////////////////////////////////////////////////////////////

using System;
using System.Runtime.InteropServices; // for DllImport attribute
using System.ComponentModel; // for Win32Exception class
using System.Threading; // for Thread.Sleep method

namespace HRTimer
{
  public class HiResTimer
  {
    protected ulong a, b, f;

    public HiResTimer()
    {
      a = b = 0UL;
      if (QueryPerformanceFrequency(out f) == 0)
        throw new Win32Exception();
    }

    public ulong ElapsedTicks
    {
      get
      { return (b - a); }
    }

    public ulong ElapsedMicroseconds
    {
      get
      {
        ulong d = (b - a);
        if (d < 0x10c6f7a0b5edUL) // 2^64 / 1e6
          return (d * 1000000UL) / f;
        else
          return (d / f) * 1000000UL;
      }
    }

    public TimeSpan ElapsedTimeSpan
    {
      get
      {
        ulong t = 10UL * ElapsedMicroseconds;
        if ((t & 0x8000000000000000UL) == 0UL)
          return new TimeSpan((long)t);
        else
          return TimeSpan.MaxValue;
      }
    }

    public ulong Frequency
    {
      get
      { return f; }
    }

    public void Start()
    {
      Thread.Sleep(0);
      QueryPerformanceCounter(out a);
    }

    public ulong Stop()
    {
      QueryPerformanceCounter(out b);
      return ElapsedTicks;
    }

    // Here, C# makes calls into C language functions in Win32 API
    // through the magic of .Net Interop

    [DllImport("kernel32.dll", SetLastError = true)]
    protected static extern
       int QueryPerformanceFrequency(out ulong x);

    [DllImport("kernel32.dll")]
    protected static extern
     int QueryPerformanceCounter(out ulong x);

#if (TEST_HRTIMER)
    static void Main(string[] args)
    {
      Console.Write("{0}{1}",
        "\n  Time Parsing Operations ",
        "\n =========================\n"
      );

      HiResTimer hrt = new HiResTimer();
      hrt.Start();
      int N = 1000;
      for (int i = 0; i < N; ++i)
        Console.WriteLine("\n " + i);
      hrt.Stop();
      ulong lookUpTime = hrt.ElapsedMicroseconds;
      Console.Write("\n   {0} for loop took {1} microseconds", N, lookUpTime);
      Console.Write("\n\n");
      Console.Write("\n\n");
    }
#endif
  }
}