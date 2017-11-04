//////////////////////////////////////////////////////////////////////////////////////////
// Serilization.cs - Serializes and deserializes objects into and from XML documents.   //              
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
* ToXml function performs XML Serializarion, which serializes the specified Object 
* and writes the XML document to a file using the specified TextWriter.
*
* 
* A FromXml function performs XML deserialization, which is the process of reading an instance of an XML document
* and constructing an object that is strongly typed to the XML Schema (XSD) of the document.
* 
* The data in my objects is described using programming language 
* constructs like classes, fields, properties, primitive types, lists,
* and even embedded XML in the form of XmlElement or XmlAttribute objects.
*
* Required Files:
* ---------------
* Serialization.cs
* 
* Maintenance History:
* --------------------
* ver 1.0 : Oct 4, 2017
* - Refactor the FromXml function so that it can accept the path dynamically.
* - first release
* ver 0.1 : Oct 2, 2017
*/
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;

namespace Federation_Project2
{
    public class ToAndFromXml
    {
        //----< Serialize objects to an XML document >------------------------------
        public void ToXml(object obj, string path)
        {
            XmlSerializer serializer = new XmlSerializer(obj.GetType());
            string pathStorage = path;
            using (TextWriter writer = new StreamWriter(pathStorage + "/TestRequest.xml"))
            {
                serializer.Serialize(writer, obj);
            }
        }
        //----< Deserialize an XML document to objects >------------------------------
        public T FromXml<T>(string receivedPath, string fileName)
        {
            try
            {
                XmlSerializer deserializer = new XmlSerializer(typeof(T));
                string xml = receivedPath + fileName;
                TextReader xmlFileReader = new StreamReader(xml);
                return (T)deserializer.Deserialize(xmlFileReader);
            }
            catch (Exception ex)
            {
                Console.Write("\n  deserialization failed\n  {0}", ex.Message);
                return default(T);
            }
        }
    }
#if (TEST_SERIALIZATION)
    class TestXmlSerialization
    {
        static void Main(string[] args)
        {
            ToAndFromXml testSerialization = new ToAndFromXml();
            ToAndFromXml testDeserialization = new ToAndFromXml();
            Console.WriteLine("Demonstrating XML Serialization and Deserialization");
            Console.Write("\n  attempting to serialize Widget object:");
            List<string> tl = new List<string>();
            string builderPath = "../../../Builder/TempDirectory";
            string xmlFileName = "/BuildRequest.xml";
            Console.Write("\n  attempting to serialize object:");
            testSerialization.ToXml(tl, builderPath);
            Console.Write("\n  attempting to deserialize Widget object:");
            testDeserialization.FromXml<string>(builderPath, xmlFileName);
        }
    }
#endif
}
