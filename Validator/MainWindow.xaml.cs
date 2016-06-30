using Microsoft.Win32;
using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Xml;
using System.Xml.Schema;
using Microsoft.Xml.XMLGen;
using System.IO;

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

        #region Обработчики событий
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
            GenerateSimpleXmL();
        }

        private void clearButton_Click(object sender, RoutedEventArgs e)
        {
            outputTextBox.Clear();
        }

        private void MenuItemSaveAs_Click(object sender, RoutedEventArgs e)
        {
            SaveFile(visualizeXSDPath, "XML Schema File\"|*.xsd|Файл \"XML\"|*.xml");
        }

        private void exitButton_Click(object sender, RoutedEventArgs e)
        {
            Environment.Exit(0);
            //Необходимо предупреждать пользователя о незаконченных потоках работы. Environment.Exit этого не делает.
            //Application.Current.Exit(ShowDialog);
        }
        #endregion

        void WriteToOutput(object message)
        {
            outputTextBox.Text += String.Format("{0} - {1}{2}", DateTime.Now, message, Environment.NewLine);
        }

        void WriteToOutput(XmlWriter xmlWriter)
        {
            outputTextBox.Text += String.Format("{0} - {1}{2}", DateTime.Now, xmlWriter.ToString(), Environment.NewLine);
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

        void SaveFile(TextBox tb, string filter)
        {
            Stream stream;
            var dialog = new SaveFileDialog();
            dialog.AddExtension = true;
            dialog.RestoreDirectory = true;
            dialog.Filter = filter;

            if (dialog.ShowDialog() == DialogResult.Value)
            {
                WriteToOutput("Файл сохранён.");
            }
        }

        bool CheckPathXSDExist()
        {
            bool result = true;
            if (String.IsNullOrEmpty(visualizeXSDPath.Text))
            {
                WriteToOutput("Не выбран файл .xsd схемы.");
                result = false;
            }

            return result;
        }

        bool CheckPathXMLExist()
        {
            bool result = true;
            if (String.IsNullOrEmpty(visualizeXMLPath.Text))
            {
                WriteToOutput("Не выбран .xml файл.");
                result = false;
            }

            return result;
        }

        void GenerateSimpleXmL()
        {
            if (CheckPathXSDExist() | CheckPathXMLExist())
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

                try
                {
                    XmlTextWriter textWriter = new XmlTextWriter("sample.xml", null);
                    textWriter.Formatting = Formatting.Indented;
                    //string result = "";
                    //XmlWriter xmlWriter = new XmlTextWriter(result, Encoding.GetEncoding("windows-1251"));
                    //xmlWriter.Formatting = Formatting.Indented;
                    XmlQualifiedName qName = new XmlQualifiedName("Файл", "http://purl.oclc.org/dsdl/schematron");
                    XmlSampleGenerator generator = new XmlSampleGenerator(schemaSetXSD, qName);
                    generator.WriteXml(textWriter);
                    WriteToOutput("Файл .xml создан и \"лежит рядом с экзешником\".");
                }
                catch (XmlException xmlException)
                {
                    WriteToOutput(xmlException.Message);
                }
            }
        }

        void ValidateXMLToXSD()
        {
            if (CheckPathXSDExist() & CheckPathXMLExist())
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
                    Task.Delay(2000).Wait();
                    progressBar.Value = progressBar.Minimum;
                }
                catch (Exception ex)
                {
                    WriteToOutput(ex.Message);
                }
            }
        }

        private void ValidationHandler(object sender, ValidationEventArgs e)
        {
            switch (e.Severity)
            {
                case XmlSeverityType.Warning: WriteToOutput("Предупреждение: " + e.Message + ". " + e.Exception); break;
                case XmlSeverityType.Error: WriteToOutput("Ошибка: " + e.Message + ". " + e.Exception); break;
            }
        }
    }
}
