////////////////////////////////////////////////////////////////////////////////////
// Executive.cs - Mock Executive for Federation Message-Passing                   //              
// ver 1.0                                                                        //
//                                                                                //
// Platform    : Windows 10 Home x64, Lenovo IdeaPad 700, Visual Studio 2017      //
// Environment:  C# Console                                                       //
// Application : Remote Build Server Prototypes for CSE681-SMA, Fall 2017         //  
// Author: Theerut Foongkiatcharoen, EECS Department, Syracuse University         //
//         tfoongki@syr.edu                                                       //
// Source: Dr. Jim Fawcett, EECS Department, CST 4-187, Syracuse University       //
//         jfawcett @twcny.rr.com                                                 //
////////////////////////////////////////////////////////////////////////////////////
/*
* Package Operations:
* ===================
*
* - Executive builds federation components by creating Client, RepoMock, Builder, and TestHarness instances.
* - Executive starts federation processing by callling doOp function.
* - A doOp function will create a message and send the message to client.
* 
* Required Files:
* ---------------
* Executive.cs, CommunicatorBase.cs, Environment.cs, Client.cs, RepoMock.cs, Builder.cs, TestHarness.cs
*
* Build Command:
* ---------------
* csc Executive.cs ...
* 
* Maintenance History:
* --------------------
* ver 1.0 : Sep 20, 2017
* - Added a prologue
* - Added public interface documentation
* - Added processMessage function
* - first release
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Federation_Project2
{
    public class Executive : CommunicatorBase
    {
        public Executive()
        {
        /*----< Creates Client, RepoMock, Builder, and TestHarness instances >----*/
            environ.client = new Client();
            environ.repo = new RepoMock();
            environ.builder = new Builder();
            environ.testHarness = new TestHarness();
        }
        /*----< Builds federation components that construct a fixed sequence of operations >----*/
        public void doOp()
        {
            Message msg = Message.makeMsg("testRequest", "Client", "Executive", "this is a message flow test");
            environ.client.postMessage(msg);
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            Console.Write("\n  Demonstrating Message Flows in Mock Federation");
            Console.Write("\n ================================================\n");
            Executive exec = new Executive();   // builds federation components
            exec.doOp(); // builds federation components
            Console.ReadLine();
        }
    }
}
