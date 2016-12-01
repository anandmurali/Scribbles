using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WPFLibrary.Extensions
{
    public static class StringExtensions
    {
        public static string GetLast(this string source, int noOfCharacters)
        {
            if (source == null)
                return string.Empty;

            if (noOfCharacters >= source.Length)
                return source;
            return source.Substring(source.Length - noOfCharacters);
        }
    }
}
