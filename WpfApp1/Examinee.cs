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
        public int mIndex { get; set; }
        public string mName { get; set; }
        public DateTime mBirthdate;
        public string mBirthplace { get; set; }
        public float mGrade1;
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
            if (DT.To_(s, DT.RF, out mBirthdate))
                return false;
            return true;
        }
    }
}
