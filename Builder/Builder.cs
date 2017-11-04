////////////////////////////////////////////////////////////////////////////////////
// Builder.cs - Mock Builder for Federation Message-Passing                       //
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
*
* - Builder creates a folder named TempDirectory using createDirectory function.
* - Builder accepts the given path from RepoMock through Message.
* - Builder copies BuildRequest.xml from the repoStorage folder by calling copyDeserializedFile function
* - and stores it in its TempDirectory.
* - Builder deserializes BuildRequest.xml and constructs an object using Serialization package 
* - through xmlDeserialization function.
* - Then, Builder copies the required files that have been serialized and included 
* - in BuildRequest.xml including tests, test filenames, test interfaces, and test drivers
* - from RepoMock's given path.
* - Builder uses process class to generate one dll file for each test and creates a BuildLog in clientStorage.
* - Builder creates TestRequest.xml using Serialization package via objSerialization function.
* - Builder sends path to its own tempDirectory to TestHarness through Message.
* 
* Required Files:
* ---------------
* Builder.cs, CommunicatorBase.cs, Environment.cs
*
* Build Command:
* ---------------
* csc Builder.cs ...
* 
* Maintenance History:
* --------------------
* ver 1.0 : Oct 7, 2017
* - Updated public interface documentation
* - Edited comments
* - Added Test stubs
* - Changed public string variables to private variables
* - first release
* ver 0.4 : Oct 4, 2017
* - Updated processMessage function
* - Fixed xmlDeserialization function
* - Fixed copyDeserializedFile function
* - Updated public interface documentation
* - first release
* ver 0.3 : Oct 2, 2017
* - Fixed classes that need to be serialized
* - Fixed xmlDeserialization function
* - Added copyDeserializedFile function
* - Updated public interface documentation
* ver 0.2 : Sep 21, 2017
* - Updated processMessage function
* ver 0.1 : Sep 20, 2017
* - Added classes that need to be serialized
* - Added copyFileType function
* - Added xmlDeserialization function
* - Added a prologue
* - Added public interface documentation
* - Added processMessage function
*/
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Federation_Project2
{
    /*--------< Classes that need to be deserialized >--------------*/
    [XmlRoot("BuildRequest")]
    public class BuildRequest
    {
        [XmlElement("Author")] public string Author;
        [XmlElement("Test")] public List<Test> Tests { get; set; }
    }
    public class Test
    {
        [XmlAttribute("id")] public int id;
        [XmlElement("Testfile")] public string Testfile { get; set; }
        [XmlElement("TestInterface")] public string TestInterface { get; set; }
        [XmlElement("Testdriver")] public string Testdriver { get; set; }
    }
    /*--------< A class that need to be serialized >--------------*/
    public class TestRequest
    {
        [XmlElement("TestLibrary")] public List<string> TestLibraries;
    }
    public class Builder : CommunicatorBase
    {
        private string _builderPath;
        private string _xmlFileName;
        private string _receivePath;
        private List<string> _files = new List<string>();

        public string BuilderPath
        {
            get { return _builderPath; }
            set { _builderPath = value; }
        }
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
        public Builder()
        {
            Console.WriteLine("Builder instance has been invoked\n");
            /*----< Sets a builder directory path >----*/
            _builderPath = "../../../Builder/TempDirectory";
            /*----< Sets an XML filename >----*/
            _xmlFileName = "/BuildRequest.xml";
            if (!Directory.Exists(_builderPath))
                Directory.CreateDirectory(_builderPath);
        }
        /*----< Build *.dll file >----*/
        public void Build(List<string> bF)
        {
            string buildFile = "";
            for (int i = 0; i < bF.Count; i++)
            {
                buildFile = buildFile + " " + bF[i];
            }
            Process p = new Process();
            p.StartInfo.FileName = "cmd.exe";
            p.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
            p.StartInfo.EnvironmentVariables["PATH"] = "%path%;C:/Windows/Microsoft.NET/Framework64/v4.0.30319";
            p.StartInfo.Arguments = "/Ccsc /target:library " + buildFile;
            Console.WriteLine(buildFile);

            // Specify relative path
            p.StartInfo.WorkingDirectory = _builderPath;
            p.StartInfo.RedirectStandardError = true;
            p.StartInfo.RedirectStandardOutput = true;
            p.StartInfo.UseShellExecute = false;
            p.Start();
            p.WaitForExit();
            /*----< Creates a buildLog and stores it in a temporary folder >----*/
            string buildLog = "";
            if (p.ExitCode == 0)
            {
                buildLog += "Build Result: Success";
                System.IO.File.WriteAllText("../../../Client/ClientStorage/BuildLog.txt", buildLog);
            }
            else
            {
                buildLog += "Build Result: Failed: [See error message in Buildlog.txt";
                System.IO.File.WriteAllText("../../../Client/ClientStorage/BuildLog.txt", buildLog);
            }
            string errors = p.StandardError.ReadToEnd();
            string output = p.StandardOutput.ReadToEnd();

            System.IO.File.WriteAllText("../../../Client/ClientStorage/BuildLog.txt", buildLog);
        }
        /*----< private helper function for RepoMock.getFiles >--------*/
        private void getFilesHelper(string path, string pattern)
        {
            string[] tempFiles = Directory.GetFiles(path, pattern);
            for (int i = 0; i < tempFiles.Length; ++i)
            {
                tempFiles[i] = Path.GetFullPath(tempFiles[i]);
            }
            _files.AddRange(tempFiles);

            string[] dirs = Directory.GetDirectories(path);
            foreach (string dir in dirs)
            {
                getFilesHelper(dir, pattern);
            }
        }
        /*----< find all the files in the given path >-----------*/
        /*
        *  Finds all the files, matching pattern, in the entire 
        *  directory tree rooted at repo.storagePath.
        */
        public void getFiles(string pattern)
        {
            _files.Clear();
            getFilesHelper(_receivePath, pattern);
        }
        /*----< copy file from the given path into the builderpath >----*/
        /*
        *  Will overwrite file if it exists. 
        */
        public bool sendFile(string fileSpec)
        {
            try
            {
                string fileName = Path.GetFileName(fileSpec);
                string destSpec = Path.Combine(_builderPath, fileName);
                File.Copy(fileSpec, destSpec, true);
                return true;
            }
            catch (Exception ex)
            {
                Console.Write("\n--{0}--", ex.Message);
                return false;
            }
        }
        /*--------< Deserializes an XML document, constructs objects and return them. >--------------*/
        public Dictionary<int, List<string>> xmlDeserialization<T>(string receivedPath, string _xmlFileName)
        {
            ToAndFromXml xDeserialization = new ToAndFromXml();
            BuildRequest newBrq = xDeserialization.FromXml<BuildRequest>(receivedPath, _xmlFileName); // Deserialization
            Dictionary<int, List<string>> tests = new Dictionary<int, List<string>>();

            foreach (var test in newBrq.Tests)
            {
                List<string> files = new List<string>();
                files.Add(test.Testfile);
                files.Add(test.TestInterface);
                files.Add(test.Testdriver);
                tests.Add(test.id, files);
            }
            return tests;
        }
        /*--------< Copies files of given file type from into the given path >--------------*/
        public void copyFileType(string fileType, string pathReceived)
        {
            getFiles(fileType);
            string fileName, fileSpec, filePath = string.Empty;
            for (int i = 0; i < _files.Count; i++)
            {
                fileSpec = _files[i];
                fileName = Path.GetFileName(fileSpec);
                Console.Write("\n\n Copying file from: \n \"{0}\"\n", fileSpec);
                filePath = Path.Combine(pathReceived, fileName);
                Console.Write("\n\n  sending \"{0}\" to \"{1}\"\n", fileName, _builderPath);
                sendFile(filePath);
            }
        }
        /*--------< Copies the required files that have been deserialized by serialization package
        and included in *.xml file from the given path into BuilderStorage >--------------*/
        public void copyDeserializedFile(Dictionary<int, List<string>> filetype, string pathReceived)
        {
            foreach (KeyValuePair<int, List<string>> test in filetype)
            {
                foreach (string tests in test.Value)
                {
                    getFiles(tests);
                    string fileName, fileSpec, filePath = string.Empty;
                    for (int i = 0; i < _files.Count; i++)
                    {
                        fileSpec = _files[i];
                        fileName = Path.GetFileName(fileSpec);
                        Console.Write("\n\n Copying file from: \n \"{0}\"\n", fileSpec);
                        filePath = Path.Combine(pathReceived, fileName);
                        Console.Write("\n\n  sending \"{0}\" to \"{1}\"\n", fileName, _builderPath);
                        sendFile(filePath);
                    }

                }
            }
        }

        /*--------< Serializes any objects to XML that represents test requests >--------------*/
        public void objSerialization()
        {
            TestRequest tr = new TestRequest();
            _receivePath = _builderPath;
            getFiles("*.dll");
            List<TestRequest> testRequestLists = new List<TestRequest>();
            List<string> tl = new List<string>();
            foreach (string file in _files)
            {
                Console.WriteLine("file :" + Path.GetFileName(file));
                //tr.TestLibraries = Path.GetFileName(file);
                tl.Add(Path.GetFileName(file));
                tr.TestLibraries = tl;
                testRequestLists.Add(tr);
            }
            ToAndFromXml xSerialization = new ToAndFromXml();
            xSerialization.ToXml(testRequestLists, _builderPath); // Serialization
        }
        /*--------< Parses the xml files from xmlDeserialization and builds all related files by calling a Build function>--------------*/
        public void buildAllFile()
        {
            Dictionary<int, List<string>> tests = new Dictionary<int, List<string>>();
            tests = xmlDeserialization<string>(_receivePath, _xmlFileName);
            
            foreach (KeyValuePair<int, List<string>> test in tests)
            {
                List<string> buildFiles = new List<string>();
                foreach (string t in test.Value)
                {
                    buildFiles.Add(t);
                }
                Build(buildFiles);
            }
            
        }
        /*----< Overrides a processMessage function to change the content in the message and pass it. >----*/
        public override void processMessage(Message msg)
        {
            if (msg.type == "Path" && msg.from == "RepoMock")
            {
                msg.type = "XMLfile";
                msg.to = "RepoMock";
                msg.from = "Builder";
                // send builderpath to repo
                msg.body = _builderPath;
                environ.repo.postMessage(msg);
            }
            // take buildrequest.xml from repo
            else if (msg.type == "XMLfile" && msg.from == "RepoMock")
            {
                Console.WriteLine("\nparse buildrequest.xml and take the file names\n");
                _receivePath = msg.body;
                // parse BuildRequest.xml and take the file names
                copyDeserializedFile(xmlDeserialization<string>(_receivePath, _xmlFileName), _receivePath);
                // build *.dll files
                buildAllFile();
                // create a TestRequest.xml
                objSerialization();
                Console.WriteLine("\n\n send path to test harness \n\n");
                msg.type = "Path";
                msg.to = "TestHarness";
                msg.from = "Builder";
                // builder sends its path to the testharness
                msg.body = _builderPath;
                string temp = msg.ToString();
                Console.Write("\n\n  main thread sending {0}\n", temp);
                environ.testHarness.postMessage(msg);
            }
            else
            {
                msg.type = "quit";
                msg.to = "TestHarness";
                msg.from = "Builder";
                msg.body = "";
                string temp = msg.ToString();
                Console.Write("\n  main thread sending {0}", temp);
                environ.testHarness.postMessage(msg);
            }
        }
    }
    //----< test stub >------------------------------------------------
#if (TEST_BUILDER)
    class TestBuilder
    {
        static void Main(string[] args)
        {
            Console.Write("\n  Demonstration of Builder");
            Console.Write("\n ============================");
            Message msg = new Message();
            Environment e = new Environment();
            Builder builder = new Builder();
            Console.WriteLine("\nparse buildrequest.xml and take the file names\n");
            builder.ReceivePath = msg.body;
            // parse BuildRequest.xml and take the file names
            Console.Write("\n  Demonstration of Builder using deserialization");
            Console.Write("\n ============================");
            builder.copyDeserializedFile(builder.xmlDeserialization<string>(builder.ReceivePath, builder.XmlFileName), builder.ReceivePath);
            // build *.dll files
            Console.Write("\n  Demonstration of Builder building *dll files");
            Console.Write("\n ============================");
            builder.buildAllFile();
            // create a TestRequest.xml
            Console.Write("\n  Demonstration of Builder using serialization");
            Console.Write("\n ============================");
            builder.objSerialization();
            Console.Write("\n  Demonstration of Builder constucting the message");
            Console.Write("\n ============================");
            msg.type = "Path";
            msg.to = "TestHarness";
            msg.from = "Builder";
            // builder sends its path to the testharness
            msg.body = builder.ReceivePath;
            string temp = msg.ToString();
            Console.Write("\n\n  main thread sending {0}\n", temp);
            e.testHarness.postMessage(msg);
        }
    }
#endif
}
