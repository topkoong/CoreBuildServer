////////////////////////////////////////////////////////////////////////////////////
// Environment.cs - Shared Code for Federation Message-Passing                    //
// ver 1.0                                                                        //
//                                                                                //
// Environment : C# Class Library                                                 //
// Platform    : Windows 10 Home x64, Lenovo IdeaPad 700, Visual Studio 2017      //
// Application : Build Server for CSE681-SMA, Fall 2017                           //  
// Author: Theerut Foongkiatcharoen, EECS Department, Syracuse University         //
//         tfoongki@syr.edu                                                       //
// Source: Dr. Jim Fawcett, EECS Department, CST 4-187, Syracuse University       //
//         jfawcett @twcny.rr.com                                                 //
////////////////////////////////////////////////////////////////////////////////////
/*
* Package Operations:
* ===================
*  The main goal of using interface is that Client, RepoMock, Builder, and TestHarness 
*   can communicate with each other, and it prevents circular dependecy from hapenning. 
* - Environment has an interface called ICommunicator that contains a definition of postMessage method.
* - Environment has a struct that contains client, repo, builder, and testHarness, which are of ICommunicator types.
* - Environment also has a Message class that constructs the message including type, to, from, and body using a makeMsg function.
* 
* Required Files:
* ---------------
* Environment.cs
*
* Build Command:
* ---------------
* csc Environment.cs
* 
* Maintenance History:
* --------------------
* ver 1.0 : Sep 20, 2017
* - Added prologue
* - Added public interface documentation
* - Added Message class
* - Added struct Environment
* - Added ICommunicator interface
* - frst release

*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Federation_Project2
{
    public interface ICommunicator
    {
        void postMessage(Message msg);
    }
    // Provide a mechanism for federation servers to contact to each other.
    public struct Environment
    {
        public ICommunicator client { get; set; }
        public ICommunicator repo { get; set; }
        public ICommunicator builder { get; set; }
        public ICommunicator testHarness { get; set; }
    }
    
    public class Message
    {

        public string type { get; set; } = "BuildRequest";
        public string to { get; set; } = "";
        public string from { get; set; } = "";
        public string body { get; set; } = "";

        /*----< Constructs the message >----*/
        public static Message makeMsg(string type, string to, string from, string body)
        {
            Message msg = new Message();
            msg.type = type;
            msg.to = to;
            msg.from = from;
            msg.body = body;
            return msg;
        }
        /*----< Overrides the Object.ToString method to provide a more suitable string representation
        of a particular type. >----*/
        public override string ToString()
        {
            string outStr = "Message - " +
                            string.Format("type: {0}, ", type) +
                            string.Format("from: {0}, ", from) +
                            string.Format("to: {0}, ", to) +
                            string.Format("body: {0} ", body);
            return outStr;
        }
    }
}
