using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CreateNuixNistDigestList
{
    public static class Constants
    {
        public const string HashCodeFileNamePrefix = "hashcodes";
        public const string DigestFileName = "De-NIST.HASH";
        public const char CR = '\r';
        public const char LF = '\n';
        public const char NULL = (char)0;
        public const int HashcodeCopyMessageProgressTimerInterval = 60000; 
    }
}
