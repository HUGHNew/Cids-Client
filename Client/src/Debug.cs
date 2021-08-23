using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client
{
    // some debug functions
    public class Debug
    {
        public static void WriteLine(String str) {
#if DEBUG
            Console.WriteLine(str);
#endif
        }
    }
}
