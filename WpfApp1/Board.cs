using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
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
        DateTime mDate;
        public string tDate { get { return mDate.ToString(DT._); } }
        TestType mType;
        public int iType { get { return (int)mType; } }
        public List<Examinee> vExaminee { get; }

        public Board()
        {
            vExaminee = new List<Examinee>();
        }

        public string Load(string filepath)
        {
            vExaminee.Clear();

            if (DT.To_(Path.GetFileNameWithoutExtension(filepath), DT._, out mDate))
                return "File name must represent test date.";

            string msg = "ok";
            // Open the spreadsheet document for read-only access.
            using (SpreadsheetDocument document =
                SpreadsheetDocument.Open(filepath, false))
            {
                // Retrieve a reference to the workbook part.
                WorkbookPart wbPart = document.WorkbookPart;

                // Find the sheet with the supplied name, and then use that 
                // Sheet object to retrieve a reference to the first worksheet.
                Sheet theSheet = wbPart.Workbook.Descendants<Sheet>().FirstOrDefault();

                // Throw an exception if there is no sheet.
                if (theSheet == null)
                    return "No sheet";

                if (!TestType.TryParse(theSheet.Name, out mType))
                    return "Sheet name must represent test type.";

                // Retrieve a reference to the worksheet part.
                WorksheetPart wsPart =
                    (WorksheetPart)(wbPart.GetPartById(theSheet.Id));

                // Use its Worksheet property to get a reference to the cell 
                // whose address matches the address you supplied.
                IEnumerable<Row> vRow = wsPart.Worksheet.Descendants<Row>();

                int li = -1;
                foreach (Row r in vRow)
                {
                    ++li;
                    IEnumerable<Cell> cells = r.ChildElements.OfType<Cell>();
                    List<Cell> vCell = cells.ToList();
                    if (vCell.Count < 5)
                    {
                        msg = "Line " + li + ": The number of columns is " + vCell.Count;
                        break;
                    }

                    Examinee nee = new Examinee();
                    int i = -1;
                    string value = LoadExamineeAttr(vCell.ElementAt(++i), wbPart);
                    if (!nee.TryParseIdx(value))//index
                    {
                        msg = "Line " + li + ": Attr " + i + " is error";
                        break;
                    }
                    nee.mName = LoadExamineeAttr(vCell.ElementAt(++i), wbPart);//name
                    if (!nee.TryParseBirdate(LoadExamineeAttr(vCell.ElementAt(++i), wbPart)))//birthdate
                    {
                        msg = "Line " + li + ": Attr " + i + " is error";
                        break;
                    }
                    nee.mBirthplace = LoadExamineeAttr(vCell.ElementAt(++i), wbPart);//birthplace
                    float grade;
                    if (!float.TryParse(LoadExamineeAttr(vCell.ElementAt(++i), wbPart), out grade))
                    {
                        msg = "Line " + li + ": Attr " + i + " is error";
                        break;
                    }
                    nee.mGrade1 = grade;//grade 1
                    vExaminee.Add(nee);
                }
            }
            return msg;
        }

        public string InsertQry()
        {
            StringBuilder qry = new StringBuilder();
            qry.Append("INSERT INTO w2s_examinee(test_date,test_type_id,examinee_index,name,birth_date,birth_place,grade_1) VALUES ");
            foreach (Examinee nee in vExaminee)
                qry.Append("('" + tDate + "'," + iType +
                    "," + nee.mIndex + ",N'" + nee.mName + "','" + nee.mBirthdate.ToString(DT._) + "',N'" +
                    nee.mBirthplace + "'," + nee.mGrade1 + "),");
            qry.Remove(qry.Length - 1, 1);
            return qry.ToString();
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
