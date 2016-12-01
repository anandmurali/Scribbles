using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using DataComparer.Common.Contracts;

namespace DataComparer.Dal
{
    public class XmlDataProvider : IXmlDataProvider
    {
        public void SaveToXmlFile<T>(string filePath, T instanceToSave) where T : class
        {
            var stringwriter = new System.IO.StringWriter();
            var serializer = new XmlSerializer(typeof(T));
            serializer.Serialize(stringwriter, instanceToSave);
            string xmlString = stringwriter.ToString();

            // Write to file
            StreamWriter streamWriter = new StreamWriter(filePath);
            streamWriter.WriteLine(xmlString);
            streamWriter.Close();
        }

        public T ReadFromXmlFile<T>(string filePath) where T : class
        {
            string xmlString = File.ReadAllText(filePath);

            var stringReader = new StringReader(xmlString);
            var serializer = new XmlSerializer(typeof(T));
            return serializer.Deserialize(stringReader) as T;
        }
    }
}
