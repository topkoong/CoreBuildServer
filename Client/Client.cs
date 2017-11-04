////////////////////////////////////////////////////////////////////////////////////
// Client.cs - Mock Client for Federation Message-Passing                         //
// ver 1.0                                                                        //
//                                                                                //
// Platform    : Windows 10 Home x64, Lenovo IdeaPad 700, Visual Studio 2017      //
// Environment : C# Class Library                                                 //
// Application : Build Server for CSE681-SMA, Fall 2017                           //  
// Author: Theerut Foongkiatcharoen, EECS Department, Syracuse University         //
//         tfoongki@syr.edu                                                       //
// Source: Dr. Jim Fawcett, EECS Department, CST 4-187, Syracuse University       //
//         jfawcett @twcny.rr.com                                                 //
////////////////////////////////////////////////////////////////////////////////////
/*
* Package Operations:
* ===================
*
* ============================================================================*
* Note that we have to manually create a ClientStorage folder in Client       *
* and copy BuildRequest.xml and *.cs files that we would like to build there. *
* ============================================================================*
* 
* - Client sends the path to RepoMock through Message.
* - Client accepts the log file from TestHarness.
*
* Required Files:
* ---------------
* Client.cs, CommunicatorBase.cs, Environment.cs
* 
* Build Command:
* ---------------
* csc Client.cs ...
* 
* Maintenance History:
* --------------------
* ver 1.0 : Oct 7, 2017
* - Updated public interface documentation
* - Edited comments
* - Added Test stubs
* - Changed public string variables to private variables
* - first release
* ver 0.2 : Oct 4, 2017
* - Added testLog function
* - Updated processMessage function
* - Updated public interface documentation
* ver 0.1 : Sep 20, 2017
* - Added processMessage function
* - Added a prologue
* - Added public interface documentation
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;


namespace Federation_Project2
{
    public class Client : CommunicatorBase
    {
        private string _path;

        public string Path
        {
            get { return _path; }
            set { _path = value; }
        }
        public Client()
        {
            Console.WriteLine("Client instance has been invoked\n");
        /*----< Sets a temporary directory path >----*/
            _path = "../../../Client/ClientStorage";
        }
        /*----< Creates a testLog and stores it in a temporary folder >----*/
        public void testLog(string tLog, string storedPath)
        {
            string text = tLog;
            System.IO.File.WriteAllText(storedPath + "/TestLog.txt", text);
        }
        /*----< Overrides a processMessage function to change the content in the message and pass it. >----*/
        public override void processMessage(Message msg)
        {
            if (msg.type == "quit" && msg.from == "TestHarness")
            {
                if (!string.IsNullOrEmpty(msg.body))
                {
                    testLog(msg.body, _path);
                    Console.WriteLine("\n\nPress ENTER to exit..\n\n");
                }
                else
                {
                    Console.WriteLine("\n\nTestLog has not been created.\n\n");
                    Console.WriteLine("\n\nPress ENTER to exit..\n\n");
                }
                Console.ReadLine();
            }
            else
            {
                msg.type = "Path";
                msg.to = "RepoMock";
                msg.from = "Client";
                msg.body = _path;
                
                string temp = msg.ToString();
                Console.Write("\n\n  main thread sending {0}\n", temp);
                environ.repo.postMessage(msg);
            }
            
        }
    }
    //----< test stub >------------------------------------------------
#if (TEST_CLIENT_MOCK)
    class TestClient
    {
        static void Main(string[] args)
        {
            Console.Write("\n  Demonstration of Client Mock");
            Console.Write("\n ============================");
            Client client = new Client();
            Message m = new Message();
            Environment e = new Environment();
            Console.Write("\n  Demonstration of Client Mock constucting the message");
            Console.Write("\n ============================");
            m.type = "Path";
            m.to = "RepoMock";
            m.from = "Client";
            m.body = client.Path;
            string temp = m.ToString();
            Console.Write("\n\n  main thread sending {0}\n", temp);
            e.repo.postMessage(m);
            string log = "Test is successful";
            Console.Write("\n  Creating test log in, {0}", m.body);
            client.testLog(log, m.body);
        }
    }
#endif
}
