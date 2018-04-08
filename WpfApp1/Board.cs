using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;

namespace WpfApp1
{
    public enum TestType
    {
        EN_A = 1,
        EN_B,
        EN_C,
        IT_A,
        IT_B
    }
    public class Board
    {
        public DateTime mDate { get; set; }
        public TestType mType { get; set; }
        public List<Examinee> vExaminee { get; }
        public Board()
        {
            vExaminee = new List<Examinee>();
        }
        public string LoadExaminee(string filePath)
        {
            vExaminee.Clear();
            // Open the spreadsheet document for read-only access.
            using (SpreadsheetDocument document =
                SpreadsheetDocument.Open(filePath, false))
            {
                // Retrieve a reference to the workbook part.
                WorkbookPart wbPart = document.WorkbookPart;

                // Find the sheet with the supplied name, and then use that 
                // Sheet object to retrieve a reference to the first worksheet.
                Sheet theSheet = wbPart.Workbook.Descendants<Sheet>().FirstOrDefault();

                // Throw an exception if there is no sheet.
                if (theSheet == null)
                    return "No sheet";

                // Retrieve a reference to the worksheet part.
                WorksheetPart wsPart =
                    (WorksheetPart)(wbPart.GetPartById(theSheet.Id));

                // Use its Worksheet property to get a reference to the cell 
                // whose address matches the address you supplied.
                IEnumerable<Row> vRow = wsPart.Worksheet.Descendants<Row>();

                int li = -1;
                foreach(Row r in vRow)
                {
                    ++li;
                    IEnumerable<Cell> cells = r.ChildElements.OfType<Cell>();
                    List<Cell> vCell = cells.ToList();
                    if(vCell.Count < 5)
                        return "Line " + li + ": The number of columns is " + vCell.Count;

                    Examinee nee = new Examinee();
                    int i = -1;
                    string value = LoadExamineeAttr(vCell.ElementAt(++i), wbPart);
                    if (!nee.TryParseIdx(value))//index
                        return "Line " + li + ": Attr " + i + " is error";
                    nee.mName = LoadExamineeAttr(vCell.ElementAt(++i), wbPart);//name
                    if(!nee.TryParseBirdate(LoadExamineeAttr(vCell.ElementAt(++i), wbPart)))//birthdate
                        return "Line " + li + ": Attr " + i + " is error";
                    nee.mBirthplace = LoadExamineeAttr(vCell.ElementAt(++i), wbPart);//birthplace
                    float grade;
                    if(!float.TryParse(LoadExamineeAttr(vCell.ElementAt(++i), wbPart), out grade))
                        return "Line " + li + ": Attr " + i + " error";
                    nee.mGrade1 = grade;//grade 1
                    vExaminee.Add(nee);
                }
            }

            return "ok";
        }

        string LoadExamineeAttr(Cell cell, WorkbookPart wbPart)
        {
            string value = cell.InnerText;

            // If the cell represents an integer number, you are done. 
            // For dates, this code returns the serialized value that 
            // represents the date. The code handles strings and 
            // Booleans individually. For shared strings, the code 
            // looks up the corresponding value in the shared string 
            // table. For Booleans, the code converts the value into 
            // the words TRUE or FALSE.
            if (cell.DataType != null)
            {
                switch (cell.DataType.Value)
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
            return value;
        }
    }
}
