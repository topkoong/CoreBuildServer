
//////////////////////////////////////////////////////////////////////////////////////////
// CommunicatorBase.cs - base for all parts of the Federation                           //              
// ver 1.0                                                                              //
// Platform    : Windows 10 Home x64, Lenovo IdeaPad 700, Visual Studio 2017            //
// Environment : C# Class Library                                                       //
// Application : Build Server for CSE681-SMA, Fall 2017                                 //  
// Author: Theerut Foongkiatcharoen, EECS Department, Syracuse University               //
//         tfoongki@syr.edu                                                             //
// Source: Dr. Jim Fawcett, EECS Department, CST 4-187, Syracuse University             //
//         jfawcett @twcny.rr.com                                                       //
//////////////////////////////////////////////////////////////////////////////////////////
/*
* Package Operations:
* ===================
* - CommunicatorBase class is derived from ICommunicator interface (base class) so it provides
* - an implementation for all of the members that the ICommunicator interface defines.
* - CommunicatorBase class implements a postMessage method and is a base for all parts 
of the Federation.
* - postMessage function performs only calling a processMessage method, and it is a base for all parts 
of the Federation when the federation would like to communicate to each other.
* - 
* Required Files:
* ---------------
* CommunicatorBase.cs, Environment.cs
*
* Maintenance History:
* --------------------
* ver 1.0 : Sep 20, 2017
* - Added a prologue
* - Added public interface documentation
* - Added postMessage function
* - first release
*/
using System;
using System.Diagnostics;

namespace Federation_Project2
{
    public abstract class CommunicatorBase : ICommunicator
    {   
        public void postMessage(Message msg)
        {
            processMessage(msg);
        }
        /*----< A processMessage method allows itself to be overridden in a derived class >----*/
        public virtual void processMessage(Message msg) { }
        static protected Environment environ;
    }
#if (TEST_COMMUNICATOR_BASE)
    class TestCommunicatorBase
    {
        static void Main(string[] args)
        {
            Console.Write("\n  Demonstration of CommunicatorBase");
            Console.Write("\n ============================");
            Message m = new Message();
            Environment e = new Environment();
            Console.Write("\n  Demonstration of Client Mock constucting the message");
            Console.Write("\n ============================");
            m.type = "test";
            m.to = "Client";
            m.from = "CommunicatorBase";
            m.body = "Message testing";
            string temp = m.ToString();
            Console.Write("\n\n  main thread sending {0}\n", temp);
            e.client.postMessage(m);
            string log = "Test is successful";
        }
    }
#endif
}
