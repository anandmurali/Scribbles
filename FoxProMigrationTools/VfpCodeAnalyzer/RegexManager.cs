using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace VfpCodeAnalyzer
{
    public class RegexManager
    {
        #region Constants
        #endregion

        #region Public Methods
        public static Regex GetFunctionRegex()
        {
            return new Regex(@"(?mi)^(\s*)?FUNCTION(?:(?!^(\s*)?(FUNCTION|PROCEDURE))[\s\S])*");
        }

        public static Regex GetProcedureRegex()
        {
            return new Regex(@"(?mi)^(\s*)?PROCEDURE(?:(?!^(\s*)?(FUNCTION|PROCEDURE))[\s\S])*");
        }
        #endregion
    }
}
