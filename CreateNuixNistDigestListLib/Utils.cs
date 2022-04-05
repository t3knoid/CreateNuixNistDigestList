using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace CreateNuixNistDigestList
{
    public static class Utils
    {
        /// <summary>
        /// Get folder of executable
        /// </summary>
        public static string AssemblyDirectory
        {
            get
            {
                string codeBase = Assembly.GetExecutingAssembly().CodeBase;
                UriBuilder uri = new UriBuilder(codeBase);
                string path = Uri.UnescapeDataString(uri.Path);
                return Path.GetDirectoryName(path);
            }
        }

        /// <summary>
        /// Converts given string (e.g. md5 signature) into a byte array. It loops
        /// through the string and takes each 2 byte hex number (base 16) and converts it into
        /// its 2-byte (decimal) equivalent and stores the values into a byte array.
        /// </summary>
        /// <param name="hex">hex number to convert</param>
        /// <returns>byte array</returns>
        public static byte[] PackMD5(string hex)
        {
            try
            {
                return Enumerable.Range(0, hex.Length)
                                 .Where(x => x % 2 == 0)
                                 .Select(x => Convert.ToByte(hex.Substring(x, 2), 16))
                                 .ToArray();
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to convert hash code, \"{hex}\" into a byte array. {ex.Message}");
            }
        }
        /// <summary>
        /// Remove slash at end of given URL
        /// </summary>
        /// <param name="url">A valid URL</param>
        /// <returns></returns>
        public static string RemoveSlashFromEnd(string url)
        {
            try
            {
                int lastSlash = url.LastIndexOf('/');
                string newurl = (lastSlash == (url.Length) - 1) ? url.Substring(0, lastSlash) : url; // Remove slash from end of URL if found, otherwise return original string

                return newurl;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error removing lasts slash from given URL, {url}. " + ex.Message);
            }
        }
        /// <summary>
        /// Returns a dictionary field name and its value
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static Dictionary<string, string> GetFieldValues(object obj)
        {
            return obj.GetType()
                      .GetFields(BindingFlags.Public | BindingFlags.Static)
                      .Where(f => f.FieldType == typeof(string))
                      .ToDictionary(f => f.Name,
                                    f => (string)f.GetValue(null));
        }
    }
}
