using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace CreateNuixNistDigestList
{
    /// <summary>
    /// Provides version information
    /// </summary>
    public class Versions
    {
        public string WorkDir { get { return _workDir; } set { _workDir = value; } }
        public string BaseURL { get { return _baseURL; } set { _baseURL = value; } }

        private ShowProgressCallback _showProgressCallback;
        private ShowMessageCallback _showMessageCallback;
        private string _workDir;
        private string _baseURL;

        public Versions(string workDir, ShowProgressCallback showProgressCallback, ShowMessageCallback showMessageCallback)
        {
            _workDir = workDir;
            _showMessageCallback = showMessageCallback;
            _showProgressCallback = showProgressCallback;
        }

        /// <summary>
        /// Downloads version.txt and stores first line which contains the RDS version information.
        /// </summary>
        /// <returns></returns>
        public async Task<string> GetVersionInfo(string version = "current")
        {
            try
            {                
                Downloads downloads = new Downloads(_workDir, ShowDownloadProgress, _showMessageCallback);
                var versionFileDownloadURL = $"{_baseURL}/{version}/{Download.VERSION_FILE}";
                await downloads.Download(versionFileDownloadURL, false);
                var versionFilePath = Path.Combine(_workDir, Download.VERSION_FILE);
                var versionLine = System.IO.File.ReadLines(versionFilePath).First();
                return $"{versionLine}";
            }
            catch (Exception ex)
            {
                _showMessageCallback($"Error retrieving RDS version information.");
                _showMessageCallback(ex.Message + "\n" + ex.StackTrace);
                throw new Exception($"Error retrieving RDS version information.");
            }
        }

        private void ShowDownloadProgress(object sender, DownloadProgressChangedEventArgs e)
        {
            _showProgressCallback(e.ProgressPercentage, string.Empty);
        }
    }
}
