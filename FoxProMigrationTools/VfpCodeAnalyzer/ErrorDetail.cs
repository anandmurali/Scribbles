using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VfpCodeAnalyzer
{
    public class ErrorDetail
    {
        #region Properties

        public string FileName { get; set; }

        public string FullPath { get; set; }

        public string Error { get; set; }

        #endregion
    }
}
