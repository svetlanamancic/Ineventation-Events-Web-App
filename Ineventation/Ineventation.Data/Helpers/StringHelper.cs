using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ineventation.Data.Helpers
{
    public class StringHelper
    {
        public static string FirstUpperRestLower(string s)
        {
            if (s.Length > 2)
            {
                s = s.ToLower();
                return char.ToUpper(s[0]) + s.Substring(1);
            }
            else if(s!="")
            {
                return s.ToUpper();
            }
            return s;
        }

    }
}
