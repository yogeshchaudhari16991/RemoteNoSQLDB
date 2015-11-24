/////////////////////////////////////////////////////////////////////////
// ICommService.cs - Contract for WCF message-passing service          //
// Ver 1.0                                                             //
// Application: Demonstration for CSE681-SMA, Project#2                //
// Language:    C#, ver 6.0, Visual Studio 2015                        //
// Platform:    Dell Inspiron 14 5000Series, Core-i5, Windows 10       //
// Author:      Yogesh Chaudhari, Student, Syracuse University         //
//              (315) 4809210, ychaudha@syr.edu                        //
/////////////////////////////////////////////////////////////////////////
/*
 * Additions to C# Console Wizard generated code:
 * - Added reference to System.ServiceModel
 * - Added using System.ServiceModel
 * - Added reference to System.Runtime.Serialization
 * - Added using System.Runtime.Serialization
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
using System.Runtime.Serialization;

namespace Project4Starter
{
  [ServiceContract (Namespace ="Project4Starter")]
  public interface ICommService
  {
    [OperationContract(IsOneWay = true)]
    void sendMessage(Message msg);
  }

  [DataContract]
  public class Message
  {

    
    [DataMember]
    public string fromUrl { get; set; }
    [DataMember]
    public string toUrl { get; set; }
    [DataMember]
    public string content { get; set; }  // will hold XML defining message information
    
  }
}
