using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace CreateNuixNistDigestList
{
    /// <summary>
    /// Delegate for callback method to use to show a message
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    //public delegate void ShowProgressCallback(object sender, ProgressEventArgs e);
    public delegate void ShowProgressCallback(int progress, string message);
    /// <summary>
    /// Delegate for callback method to use to show progress
    /// </summary>
    /// <param name="message"></param>
    public delegate void ShowMessageCallback(string message);
    /// <summary>
    /// Delegate for callback method to use to show download progress
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    public delegate void ShowDownloadProgressCallback(object sender, DownloadProgressChangedEventArgs e);
}
