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
        EN_A = 0,
        EN_B,
        EN_C,
        IT_A,
        IT_B,
    }

    public enum TestFormat
    {
        IT_1 = 2008,
        IT_2,
    }

    public class Board
    {
        DateTime mDate;
        public string tDate { get { return mDate.ToString(DT._); } }
        TestType mType;
        public int iType { get { return (int)mType; } }
        TestFormat mFormat;
        public List<Examinee> vExaminee { get; }

        public Board()
        {
            mDate = new DateTime(1, 1, 1);
            mType = TestType.IT_A;
            mFormat = TestFormat.IT_2;
            vExaminee = new List<Examinee>();
        }

        public string Load(string filepath)
        {
            vExaminee.Clear();

            string fn = Path.GetFileNameWithoutExtension(filepath);
            //todo check fn for length

            if (DT.To_(fn.Substring(0, 10), DT._, out mDate))
                return "File name must represent test date in format yyyy-MM-dd.";
            if ((int)TestFormat.IT_1 < mDate.Year)
                mFormat = TestFormat.IT_2;
            else
                mFormat = TestFormat.IT_1;

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

                if (!TestType.TryParse(theSheet.Name.ToString().Substring(0, 4), out mType))
                    return "Sheet name must represent test type.";

                // Retrieve a reference to the worksheet part.
                WorksheetPart wsPart =
                    (WorksheetPart)(wbPart.GetPartById(theSheet.Id));

                // Use its Worksheet property to get a reference to the cell 
                // whose address matches the address you supplied.
                IEnumerable<Row> vRow = wsPart.Worksheet.Descendants<Row>();

                if (vRow.Count() < 1)
                    return "No row.";

                //birthdate or given name
                bool bName = true;
                string v = LoadExamineeAttr(vRow.First().ChildElements.ElementAt(2) as Cell, wbPart);
                foreach (char c in v.ToCharArray())
                    if ('0' <= c && c <= '9')
                    {
                        bName = false;
                        break;
                    }

                //determine the number of cells of each row
                int nCellOfRow = 5;
                if (bName)
                    ++nCellOfRow;
                if (mType < TestType.IT_A)
                    nCellOfRow += 2;
                else if (mFormat == TestFormat.IT_2)
                    ++nCellOfRow;

                int li = -1;
                foreach (Row r in vRow)
                {
                    ++li;
                    IEnumerable<Cell> cells = r.ChildElements.OfType<Cell>();
                    List<Cell> vCell = cells.ToList();
                    if ( vCell.Count < nCellOfRow)
                    {
                        msg = "Line " + li + ": The number of columns is " + vCell.Count + " < " + nCellOfRow;
                        break;
                    }

                    Examinee nee = new Examinee();
                    int i = -1;
                    //index
                    string value = LoadExamineeAttr(vCell.ElementAt(++i), wbPart);
                    if (!nee.TryParseIdx(value))
                    {
                        msg = "Line " + li + ": Attr " + i + " is error";
                        break;
                    }

                    //name
                    nee.mName = LoadExamineeAttr(vCell.ElementAt(++i), wbPart);

                    //given name
                    if(bName)
                        nee.mName += LoadExamineeAttr(vCell.ElementAt(++i), wbPart);

                    //birthdate
                    value = LoadExamineeAttr(vCell.ElementAt(++i), wbPart);
                    int val;
                    if(int.TryParse(value, out val))//birthdate is stored as number
                    {
                        if (val < 2018)//birthyear only
                            nee.mBirthdate = new DateTime(val, 1, 1, 8, 8, 8);
                        else
                            nee.mBirthdate = DateTime.FromOADate(val);
                    }
                    else if (!nee.TryParseBirdate(value))//birthdate is stored as string
                    {
                        msg = "Line " + li + ": Attr " + i + " is error";
                        break;
                    }

                    //birthplace
                    nee.mBirthplace = LoadExamineeAttr(vCell.ElementAt(++i), wbPart);

                    //grade 1
                    float grade;
                    value = LoadExamineeAttr(vCell.ElementAt(++i), wbPart);
                    if (!float.TryParse(value, out grade))
                    {
                        if (value.ToLower().Trim() == "v")
                            grade = -1.0f;
                        else
                        {
                            msg = "Line " + li + ": Attr " + i + " is error";
                            break;
                        }
                    }
                    nee.mGrade1 = grade;

                    if (mType < TestType.IT_A || mFormat == TestFormat.IT_2)
                    {
                        //grade 2
                        value = LoadExamineeAttr(vCell.ElementAt(++i), wbPart);
                        if (!float.TryParse(value, out grade))
                        {
                            if (value.ToLower().Trim() == "v")
                                grade = -1.0f;
                            else
                            {
                                msg = "Line " + li + ": Attr " + i + " is error";
                                break;
                            }
                        }
                        nee.mGrade2 = grade;
                    }

                    if(mType < TestType.IT_A)
                    {
                        //grade 3
                        value = LoadExamineeAttr(vCell.ElementAt(++i), wbPart);
                        if (!float.TryParse(value, out grade))
                        {
                            if (value.ToLower().Trim() == "v")
                                grade = -1.0f;
                            else
                            {
                                msg = "Line " + li + ": Attr " + i + " is error";
                                break;
                            }
                        }
                        nee.mGrade3 = grade;
                    }

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
