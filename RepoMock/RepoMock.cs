////////////////////////////////////////////////////////////////////////////////////
// RepoMock.cs - Mock Repository for Federation Message-Passing                   //
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
* - RepoMock accepts the path from client through Message.
* - RepoMock creates the repoStorage folder using createDirectory function.
* - RepoMock copies BuildRequest.xml from the given path into the repoStorage folder by calling copyFileType function.
* - RepoMock deserializes buildRequest.xml and constructs an object using Serialization package.
* - RepoMock copies the required files that have been serialized and included 
* - in BuildRequest.xml from client given path by calling copyDeserializedFile function.
* - Repo sends the BuildRequest.xml to the Build Server
* 
* Required Files:
* ---------------
* RepoMock.cs, CommunicatorBase.cs, Environment.cs
*
* Build Command:
* ---------------
* csc RepoMock.cs
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
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;


namespace Federation_Project2
{
    ///////////////////////////////////////////////////////////////////
    // RepoMock class
    // - begins to simulate basic Repo operations

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
    public class RepoMock : CommunicatorBase
    {
        private string _receivePath;
        private string _storagePath;
        private string _xmlFileName;
        private List<string> _files = new List<string>();

        public string ReceivePath
        {
            get { return _receivePath; }
            set { _receivePath = value; }
        }
        public string StoragePath
        {
            get { return _storagePath; }
            set { _storagePath = value; }
        }
        public string XmlFileName
        {
            get { return _xmlFileName; }
            set { _xmlFileName = value; }
        }

        public List<string> Files
        {
            get { return _files; }
            set { _files = value; }
        }

        /*----< Initialize RepoMock Storage>----*/
        public RepoMock()
        {
            Console.WriteLine("RepoMock instance has been invoked\n");
            _xmlFileName = "/BuildRequest.xml";
            _storagePath = "../../../RepoMock/RepoStorage";
            createDirectory(_storagePath);
        }
        
        public void createDirectory(string storagePath)
        {
            if (!Directory.Exists(storagePath))
                Directory.CreateDirectory(storagePath);
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
        /*----< find all the files in RepoMock.storagePath >-----------*/
        /*
        *  Finds all the files, matching pattern, in the entire 
        *  directory tree rooted at repo.storagePath.
        */
        public void getFiles(string pattern)
        {
            _files.Clear();
            getFilesHelper(_receivePath, pattern);
        }
        /*---< copy file to RepoMock.receivePath >---------------------*/
        /*
        *  Will overwrite file if it exists. 
        */
        public bool sendFile(string fileSpec)
        {
            try
            {
                string fileName = Path.GetFileName(fileSpec);
                string destSpec = Path.Combine(_storagePath, fileName);
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
        public Dictionary<int, List<string>> xmlDeserialization<T>(string receivedPath, string xmlFileName)
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
        /*--------< Copies files of given file type from the given path into RepoMockStorage >--------------*/
        // eg. copyFileType("*.cs", receivePath);
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
                Console.Write("\n\n  sending \"{0}\" to \"{1}\"\n", fileName, _storagePath);
                sendFile(filePath);
            }
        }
        /*--------< Copies the required files that have been deserialized by serialization package
        and included in *.xml file from the given path into RepoMockStorage >--------------*/
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
                        Console.Write("\n\n  sending \"{0}\" to \"{1}\"\n", fileName, _storagePath);
                        sendFile(filePath);
                    }
                }
            }
        }
        /*----< Overrides a processMessage function to change the content in the message >----*/
        public override void processMessage(Message msg)
        {
            /*--------< RepoMock accepts the client path from Client via Message. >--------------*/
            if (msg.type == "Path" && msg.from == "Client")
            {
                msg.to = "Builder";
                msg.from = "RepoMock";
                _receivePath = msg.body;
                //Console.WriteLine("Client path: {0}", receivePath);
                //createDirectory(_receivePath);
                copyFileType("*.xml", _receivePath);
                copyDeserializedFile(xmlDeserialization<string>(_storagePath, _xmlFileName), _receivePath);
                environ.builder.postMessage(msg);
            }
            /*--------< RepoMock sends Buildrequest.xml to the Builder through Message>--------------*/
            else if (msg.type == "XMLfile" && msg.from == "Builder")
            {
                msg.to = "Builder";
                msg.from = "RepoMock";
                _receivePath = msg.body;
                string temp = msg.ToString();
                Console.Write("\n\n  main thread sending {0}s\n", temp);
                // change destPath to builderpath
                string pathTemp = _receivePath;
                _receivePath = _storagePath;
                _storagePath = pathTemp;
                copyFileType("*.xml", _receivePath);
                // change destPath to builderpath
                _storagePath = _receivePath;
                msg.body = _storagePath;
                environ.builder.postMessage(msg);
            }
            else
            {
                return;
            }
        }
    }
    //----< test stub >------------------------------------------------
#if (TEST_REPO_MOCK)
    class TestRepoMock
    {
        static void Main(string[] args)
        {
            Console.Write("\n  Demonstration of Mock Repo");
            Console.Write("\n ============================");
            Message msg = new Message();
            Environment e = new Environment();
            RepoMock repo = new RepoMock();
            repo.getFiles("*.*");
            foreach (string file in repo.Files)
                Console.Write("\n  \"{0}\"", file);

            string fileSpec = repo.Files[1];
            string fileName = Path.GetFileName(fileSpec);
            Console.Write("\n  sending \"{0}\" to \"{1}\"", fileName, repo.ReceivePath);
            repo.sendFile(repo.Files[1]);
            Console.Write("\n\n");
            Console.Write("\n  Demonstration of Repo Mock constucting the message");
            Console.Write("\n ============================");
            msg.to = "Builder";
            msg.from = "RepoMock";
            repo.ReceivePath = msg.body;
            string temp = msg.ToString();
            Console.Write("\n\n  main thread sending {0}s\n", temp);
            // change destPath to builderpath
            string pathTemp = repo.ReceivePath;
            repo.ReceivePath = repo.StoragePath;
            repo.StoragePath = pathTemp;
            repo.copyFileType("*.xml", repo.ReceivePath);
            // change destPath to builderpath
            repo.StoragePath = repo.ReceivePath;
            msg.body = repo.StoragePath;
            e.builder.postMessage(msg);
        }
    }
#endif
}
