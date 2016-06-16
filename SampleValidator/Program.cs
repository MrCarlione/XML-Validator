using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Schema;
using System.Xml;
using System.IO;

namespace SampleValidator
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Enter path to XSD scheme!");
            string pathToXSD = Console.ReadLine();
            Console.WriteLine("Enter path to XML file!");
            string pathToXML = Console.ReadLine();
            Validate(pathToXSD, pathToXSD);
        }

        public static void Validate(string pathToXSD, string pathToXML)
        {
            XmlTextReader xmlTextReader = new XmlTextReader(pathToXSD);
            XmlSchema schema = XmlSchema.Read(xmlTextReader, ValidationCallback);
            FileStream file = new FileStream("new.xml", FileMode.Create, FileAccess.ReadWrite);
            XmlTextWriter xwriter = new XmlTextWriter(file, new UTF8Encoding());
            xwriter.Formatting = Formatting.Indented;
            schema.Write(xwriter);


        }

        static void ValidationCallback(object sender, ValidationEventArgs args)
        {
            if (args.Severity == XmlSeverityType.Warning)
                Console.Write("WARNING: ");
            else if (args.Severity == XmlSeverityType.Error)
                Console.Write("ERROR: ");

            Console.WriteLine(args.Message);
        }
    }
}
