using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Xml;
using System.Xml.Schema;

namespace Validator
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void chooseXSDButton_Click(object sender, RoutedEventArgs e)
        {
            SelectFile(visualizeXSDPath, "XML Schema File\"|*.xsd");
        }

        private void chooseXMLButton_Click(object sender, RoutedEventArgs e)
        {
            SelectFile(visualizeXMLPath, "Файл \"XML\"|*.xml");
        }

        private void validateButton_Click(object sender, RoutedEventArgs e)
        {
            ValidateXMLToXSD();
        }

        private void generateButton_Click(object sender, RoutedEventArgs e)
        {
            //var button = (Button)sender;
        }

        private void clearButton_Click(object sender, RoutedEventArgs e)
        {
            outputTextBox.Clear();
        }

        void WriteToOutput(object message)
        {
            outputTextBox.Text += String.Format("{0} - {1}{2}", DateTime.Now, message, Environment.NewLine);
        }

        void SelectFile(TextBox tb, string filter)
        {
            var dialog = new OpenFileDialog();
            dialog.AddExtension = true;
            //dialog.DefaultExt = extension;
            dialog.Filter = filter;

            var result = dialog.ShowDialog();
            if (result.Value)
                tb.Text = dialog.FileName;
        }

        bool CheckPathExist()
        {
            bool result = true;
            if (String.IsNullOrEmpty(visualizeXSDPath.Text))
            {
                WriteToOutput("Не выбран файл .xsd схемы.");
                result = false;
            }
            if (String.IsNullOrEmpty(visualizeXMLPath.Text))
            {
                WriteToOutput("Не выбран .xml файл.");
                result = false;
            }

            return result;
        }


        void GenerateSimpleXmL()
        {
            XmlDocument documentXSD = new XmlDocument();
            XmlSchemaSet schemaSetXSD = new XmlSchemaSet();
            try
            {
                documentXSD.Load(visualizeXSDPath.Text);
                schemaSetXSD.Add(null, new XmlNodeReader(documentXSD));
            }
            catch (XmlSchemaException xmlException)
            {
                WriteToOutput(xmlException.Message);
            }

            XmlDocument documentXML = new XmlDocument();
            try
            {
                //XmlSampleGenerator s = new XmlSampleGenerator
                //documentXML.Crea
            }
            catch
            {

            }
        }

        void ValidateXMLToXSD()
        {
            if (CheckPathExist())
            {
                progressBar.Value = progressBar.Minimum;
                XmlDocument documentXSD = new XmlDocument();
                XmlSchemaSet schemaSetXSD = new XmlSchemaSet();
                try
                {
                    documentXSD.Load(visualizeXSDPath.Text);
                    schemaSetXSD.Add(null, new XmlNodeReader(documentXSD));
                }
                catch (XmlSchemaException xmlException)
                {
                    WriteToOutput(xmlException.Message);
                }

                XmlDocument documentXML = new XmlDocument();
                try
                {
                    documentXML.Load(visualizeXMLPath.Text);
                    documentXML.Schemas.Add(schemaSetXSD);
                    documentXML.Validate(new ValidationEventHandler(ValidationHandler));
                    WriteToOutput("Выбранный файл соответствует .xsd схеме.");
                    progressBar.Value = progressBar.Maximum;
                }
                catch (Exception ex)
                {
                    WriteToOutput(ex.Message);
                }
            }
        }


        void ValidateXMLToXSDNew()
        {

        }

        private void ValidationHandler(object sender, ValidationEventArgs e)
        {
            switch (e.Severity)
            {
                case XmlSeverityType.Warning: WriteToOutput("Предупреждение: " + e.Message + ".\n" + e.Exception); break;
                case XmlSeverityType.Error: WriteToOutput("Ошибка: " + e.Message + ".\n" + e.Exception); break;
            }
        }
    }
}
