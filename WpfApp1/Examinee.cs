using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Globalization;

namespace WpfApp1
{
    public class Examinee
    {
        static CultureInfo sCultInfo = null;
        public const string FMT_FL = "d/M/yyyy";
        public const string FMT_DB = "yyyy-MM-dd";
        public int mIndex { get; set; }
        public string mName { get; set; }
        public DateTime mBirthdate;
        public string mBirthplace { get; set; }
        public float mGrade1 { get; set; }
        public float mGrade2 { get; set; }
        public float mGrade3 { get; set; }
        public Examinee() { }
        public bool TryParseIdx(string s)
        {
            int idx;
            bool r = int.TryParse(s.Substring(1), out idx);
            if (r)
                mIndex = idx;
            return r;
        }

        public bool TryParseBirdate(string s)
        {
            if (sCultInfo == null)
                sCultInfo = CultureInfo.CreateSpecificCulture("en-US");
            if (DateTime.TryParseExact(s.Substring(1), FMT_FL, sCultInfo, DateTimeStyles.None, out mBirthdate))
                return true;
            return false;
        }
    }
}
