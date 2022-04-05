using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace CreateNuixNistDigestList
{

    public class Downloads
    {
        public string WorkFolder { get { return _workFolder; } set { _workFolder = value; } }

        private string _workFolder = null;
        private ShowDownloadProgressCallback _showProgressCallback;
        private ShowMessageCallback _showMessageCallback;
        public Downloads(string workDir, ShowDownloadProgressCallback showProgressCallback, ShowMessageCallback showMessageCallback)
        {
            _workFolder = workDir;
            _showProgressCallback = showProgressCallback;
            _showMessageCallback = showMessageCallback;
        }

        #region Public methods

        public async Task Download(string url, bool showprogress = true)
        {            
            string file = System.IO.Path.GetFileName(url);
            
            if (_workFolder.Equals(null))
            {
                throw new ArgumentNullException("Set WorkFolder property before calling " + MethodBase.GetCurrentMethod().Name + ".");
            }

            try
            {
                // start download
                await DownloadAsync(url, showprogress);
            }
            catch (Exception ex)
            {
                _showMessageCallback($"Error downloading {url}. {ex.Message}");
                throw new Exception($"Error downloading {url}. {ex.Message}");
            }
        }
        #endregion

        #region Private methods

        /// <summary>
        /// Non-blocking download
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public async Task DownloadAsync(string url, bool showprogress = true)
        {
            if (_workFolder.Equals(null))
            {
                throw new ArgumentNullException("Set WorkFolder property before calling " + MethodBase.GetCurrentMethod().Name + ".");
            }

            if (url.Equals(null))
            {
                throw new ArgumentNullException("URL not set.");
            }

            Uri uri = new Uri(url);
            string filename = System.IO.Path.GetFileName(uri.LocalPath);
            string destination = Path.Combine(_workFolder, filename);
            using (WebClient wc = new WebClient())
            {
                if (showprogress) wc.DownloadProgressChanged += new DownloadProgressChangedEventHandler(wc_DownloadProgressChanged);
                await wc.DownloadFileTaskAsync(
                    uri,
                    destination
                );
            }                        
        }        

        #endregion

        #region Event handlers
        private void wc_DownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e)
        {
            _showProgressCallback(sender, e);
        }

        #endregion


    }
}
