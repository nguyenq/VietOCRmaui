using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VietOCR
{
    public class Options
    {
        public string InputFolder { get; set; }
        public string OutputFolder { get; set; }
        public string OutputFormat { get; set; }
        public string SelectedLetterCase { get; set; }

        public IList<string> CurLangCodes { get; set; }

        public string CurLangCode { get { return string.Join('+', CurLangCodes); } }
    }
}
