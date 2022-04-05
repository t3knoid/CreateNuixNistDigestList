using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CreateNuixNistDigestList
{
    public static class RDSName
    {
        public const string ANDROID = "android";
        public const string IOS = "ios";
        public const string LEGACY = "legacy";
        public const string MODERN = "modern";
        public const string MODERN_MINIMAL = "modernm";
        public const string MODERN_UNIQUE = "modernu";
    }
    
    public class Download
    {
        public const string HASHCODE_FILE = "hashcodes.txt";
        public const string VERSION_FILE = "version.txt";
        public const string README_FILE = "README.txt";
        public const string ANDROID = "RDS_android.iso";
        public const string IOS = "RDS_ios.iso";
        public const string LEGACY = "RDS_legacy.iso";
        public const string MODERN = "RDS_modern.iso";
        public const string MODERN_MINIMAL = "rds_modernm.zip";
        public const string MODERN_UNIQUE = "rds_modernm.zip";
    }

    public enum RDSType
    {
        ANDROID,
        IOS,
        LEGACY,
        MODERN,
        MODERN_MINIMAL,
        MODERN_UNIQUE
    }

}
