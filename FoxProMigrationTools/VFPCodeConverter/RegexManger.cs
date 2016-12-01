using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VFPCodeConverter
{
    public class RegexManger
    {
        public const string FirstLinePattern = @"(?m)(\A.*$)";


        public const string NoOfMethodsPattern = @"^\s*?(?<MethodsCount>PROCEDURE|FUNCTION)";


        public const string ProcedurePattern = @"^\s*?(?<MethodsCount>PROCEDURE)";

        public const string FunctionPattern = @"^\s*?(?<MethodsCount>FUNCTION)";
    }
}
