using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VfpCodeAnalyzer
{
    public class CodeAnalyzerOptions
    {
        #region Properties

        public bool IsCaseSensitive { get; set; }

        public string SearchString { get; set; }

        #endregion

        #region Constructors

        public CodeAnalyzerOptions()
        {
            IsCaseSensitive = false;
        }
        #endregion
    }
}
