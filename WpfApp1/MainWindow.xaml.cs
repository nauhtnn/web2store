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
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
using System.IO;
//How to: Retrieve the values of cells in a spreadsheet document (Open XML SDK)

namespace WpfApp1
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        TextBlock txtResult;
        public MainWindow()
        {
            InitializeComponent();
            txtResult = new TextBlock();
            txtResult.MaxWidth = scrvwrResult.Width;
            scrvwrResult.Content = txtResult;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if(tbxFilePath.Text.Length == 0 || !File.Exists(tbxFilePath.Text))
            {
                txtResult.Text = "No file found!";
                return;
            }
            FileStream fs = new FileStream(tbxFilePath.Text, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
            SpreadsheetDocument doc = null;
            try
            {
                doc = SpreadsheetDocument.Open(fs, false);
            } catch(IOException ex)
            {
                txtResult.Text = ex.ToString();
                return;
            }
            WorkbookPart wbPart = doc.WorkbookPart;
            WorksheetPart wsPart = wbPart.WorksheetParts.First();
            SheetData sData = wsPart.Worksheet.Elements<SheetData>().First();
            StringBuilder sb = new StringBuilder();
            foreach (Row r in sData.Elements<Row>())
            {
                foreach(Cell c in r.Elements<Cell>())
                {
                    string value = null;
                    if (c.DataType != null)
                    {
                        value = c.InnerText;
                        switch (c.DataType.Value)
                        {
                            case CellValues.SharedString:

                                // For shared strings, look up the value in the
                                // shared strings table.
                                var stringTable =
                                    wbPart.GetPartsOfType<SharedStringTablePart>()
                                    .FirstOrDefault();

                                // If the shared string table is missing, something 
                                // is wrong. Return the index that is in
                                // the cell. Otherwise, look up the correct text in 
                                // the table.
                                if (stringTable != null)
                                {
                                    value =
                                        stringTable.SharedStringTable
                                        .ElementAt(int.Parse(value)).InnerText;
                                }
                                break;

                            case CellValues.Boolean:
                                switch (value)
                                {
                                    case "0":
                                        value = "FALSE";
                                        break;
                                    default:
                                        value = "TRUE";
                                        break;
                                }
                                break;
                        }
                    }

                    sb.Append(value);
                    sb.Append("\t|\t");
                }
                sb.Append("\n");
            }
            doc.Close();
            fs.Close();
            txtResult.Text = sb.ToString();
        }

        private void btnBrowse_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Forms.OpenFileDialog openFileDialog1 = new System.Windows.Forms.OpenFileDialog();

            //openFileDialog1.InitialDirectory = "c:\\";
            openFileDialog1.Filter = "txt files (*.txt)|*.txt|All files (*.*)|*.*";
            openFileDialog1.FilterIndex = 2;
            openFileDialog1.RestoreDirectory = true;

            if (openFileDialog1.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                tbxFilePath.Text = openFileDialog1.FileName;
            }
        }
    }
}
