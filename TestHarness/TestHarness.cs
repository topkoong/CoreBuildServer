////////////////////////////////////////////////////////////////////////////////////
// TestHarness.cs - Runs tests by loading dlls and invoking test()                //              
// ver 1.0                                                                        //
//                                                                                //
// Platform    : Windows 10 Home x64, Lenovo IdeaPad 700, Visual Studio 2017      //
// Environment : C# Class Library                                                 //
// Application : Build Server for CSE681-SMA, Fall 2017                           //  
// Author: Theerut Foongkiatcharoen, EECS Department, Syracuse University         //
//         tfoongki@syr.edu                                                       //
// Source: Ammar Salman, EECS Department, Syracuse University                     //
//         assalman@syr.edu                                                       //
////////////////////////////////////////////////////////////////////////////////////
/*
* Package Operations:
* ===================
*
* - TestHarness accepts the given path from Builder through Message.
* - TestHarness deserializes TestRequest.xml and constructs an object using Serialization package 
* - through xmlDeserialization function.
* - Then, TestHarness tests the required files that have been serialized and included 
* - in TestRequest.xml including test requests and test libraries from Builder's temp directory.
* - TestHarness creates TestLog file and sends it to Client through Message.
* 
* 
* Required Files:
* ---------------
* TestHarness.cs, CommunicatorBase.cs, Environment.cs
*
* Build Command:
* ---------------
* csc Executive.cs ...
* 
* Maintenance History:
* --------------------
* ver 1.0 : Oct 7, 2017
* - Updated public interface documentation
* - Edited comments
* - Changed public string variables to private variables
* - Added Test stubs
* - first release
* ver 0.6 : Oct 6, 2017
* - Updated processMessage function
* - Added TestAllFiles function
* - Fixed LoadAndTest function
* - Updated public interface documentation
* ver 0.5 : Oct 5, 2017
* - Added LoadAndTest function
* - Updated public interface documentation
* ver 0.4 : Oct 4, 2017
* - Updated processMessage function
* - Updated xmlDeserialization function
* - Updated public interface documentation
* - first release
* ver 0.3 : Oct 2, 2017
* - Fixed classes that need to be serialized
* - Fixed xmlDeserialization function
* - Updated public interface documentation
* ver 0.2 : Sep 21, 2017
* - Updated processMessage function
* ver 0.1 : Sep 20, 2017
* - Added classes that need to be serialized
* - Added copyFileType function
* - Added a prologue
* - Added public interface documentation
* - Added processMessage function
*/

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Federation_Project2
{
    public class TestHarness : CommunicatorBase
    {
        private StringWriter _LogBuilder;
        private bool _innerResult;
        private bool _invocationResult;

        public string Log
        {
            get { return _LogBuilder.ToString(); }
        }

        public bool InnerResult
        {
            get { return _innerResult; }
        }

        public bool InvocationResult { get { return _invocationResult; } }

        private string _xmlFileName;
        private string _receivePath;
        private List<string> _files = new List<string>();
        List<Type> testDrivers = new List<Type>();

        public string XmlFileName
        {
            get { return _xmlFileName; }
            set { _xmlFileName = value; }
        }

        public string ReceivePath
        {
            get { return _receivePath; }
            set { _receivePath = value; }
        }

        public List<string> Files
        {
            get { return _files; }
            set { _files = value; }
        }

        public TestHarness()
        {
            Console.WriteLine("TestHarness instance has been invoked");
            _xmlFileName = "/TestRequest.xml";
            _LogBuilder = new StringWriter();
        }
        /*--------< A class that needs to be deserialized >--------------*/
        [XmlRoot("TestRequest")]
        public class TestRequest
        {
            [XmlElement("TestLibrary")] public List<string> TestLibraries;
        }
        /*--------< Deserializes an XML document and constructs objects from the given path and filename >--------------*/
        public void xmlDeserialization<T>(string receivedPath, string xmlFileName)
        {
            ToAndFromXml xDeserialization = new ToAndFromXml();
            List<TestRequest> newTrq = xDeserialization.FromXml<List<TestRequest>>(receivedPath, xmlFileName);
            foreach (TestRequest trq in newTrq)
            {
                foreach (string tlb in trq.TestLibraries)
                {
                    _files.Add(tlb);
                }

            }
        }
        /*--------< Tests all dll files from the path given by calling LoadAndTest function >--------------*/
        public void TestAllFiles()
        {
            if (!Directory.Exists(_receivePath)) Console.WriteLine("Path is not existed");
            string[] files = Directory.GetFiles(_receivePath, "*.dll");
            foreach (string file in files)
            {
                LoadAndTest(file);
                Console.Write("\n\n  Log:\n{0}", Log);
            }
        }
        /*--------< Tests a dll file from the path given and logs the test result. >--------------*/
        public bool LoadAndTest(string Path)
        {
            _invocationResult = false;
            _innerResult = false;
            Console.Write("\n  loading: \"{0}\"", Path);
            // save the original output stream for Console
            TextWriter _old = Console.Out;
            // flush whatever was (if anything) in the log builder
            _LogBuilder.Flush();
            Console.Write("\n  Testing library {0}", Path);
            Console.Write("\n ==========================================================================");
            try
            {
                Console.Write("\n\n  Loading the assembly ... ");
                Assembly asm = Assembly.LoadFrom(Path);
               
                Console.Write(" Success \n  Checking Types");
                Type[] types = asm.GetTypes();
                foreach (Type type in types)
                {
                    Type itest = type.GetInterface("ITest");
                    MethodInfo testMethod = type.GetMethod("test");
                    if (type.IsClass && testMethod != null && testMethod.ReturnType == typeof(bool))
                    {
                        Console.Write("\n    Found '{1}' in {0}", type.ToString(), testMethod.ToString());
                        Console.Write("\n  Invoking Test method '{0}'",
                            testMethod.DeclaringType.FullName + "." + testMethod.Name);
                        // set the Console output to print in the LogBuilder
                        Console.SetOut(_LogBuilder);
                        // invoke the test method
                        _innerResult = (bool) testMethod.Invoke(Activator.CreateInstance(type), null);
                        if (_innerResult) Console.Write("\n\n  Test Passed.\n\n");
                        else Console.Write("\n\n  Test Failed.\n\n");
                        // set the Console output back to its original (StandardOutput that shows on the screen)
                        Console.SetOut(_old);
                        _invocationResult = true;
                    }
                }
                if (!_invocationResult)
                    Console.Write("\n\n  Could not find 'bool Test()' in the assembly.\n  Make sure it implements ITest\n  Test failed");
                return _invocationResult;
            }
            catch (Exception ex)
            {
                Console.Write("\n\n  Error: {0}", ex.Message);
                // in case of an exception while invoking test, we need to set the console output back to its original i'm setting it after the previous print so that if the invokation
                // of Test() threw an exception, it will show in the Log string
                Console.SetOut(_old);
                _invocationResult = false;
                return _invocationResult;
            }
        }
        /*----< Overrides a processMessage function to change the content in the message and pass it. >----*/
        public override void processMessage(Message msg)
        {
            if (msg.type == "Path" && msg.from == "Builder")
            {
                _receivePath = msg.body;
                // parse TestRequest.xml and take the file names from the xml file
                xmlDeserialization<string>(_receivePath, _xmlFileName);
                TestAllFiles();
                msg.body = Log;
                msg.type = "quit";
                msg.to = "Client";
                msg.from = "TestHarness";
                environ.client.postMessage(msg);
            }
            else
            {
                msg.to = "Client";
                msg.from = "TestHarness";
                msg.type = "quit";
                msg.body = "";
                string temp = msg.ToString();
                Console.Write("\n  main thread sending {0}", temp);
                environ.client.postMessage(msg);
            }
        }
    }
    //----< test stub >------------------------------------------------
#if (TEST_TESTHARNESS)
    class Test_TestHarness
    {
        static void Main(string[] args)
        {
            TestHarness th = new TestHarness();
            Message msg = new Message();
            Environment e = new Environment();
            th.ReceivePath = "../../../Builder/TempDirectory";
            // parse TestRequest.xml and take the file names from the xml file
            Console.Write("\n\n");
            Console.Write("\n  Demonstration of TestHarness deserializing an xml document");
            Console.Write("\n ============================");
            th.xmlDeserialization<string>(th.ReceivePath, th.XmlFileName);
            Console.Write("\n\n");
            Console.Write("\n  Demonstration of TestHarness testing files");
            Console.Write("\n ============================");
            th.TestAllFiles();
            Console.Write("\n\n");
            Console.Write("\n  Demonstration of TestHarness constucting the message");
            Console.Write("\n ============================");
            msg.body = th.Log;
            msg.type = "quit";
            msg.to = "Client";
            msg.from = "TestHarness";
            e.client.postMessage(msg);
        }
    }
#endif
}
