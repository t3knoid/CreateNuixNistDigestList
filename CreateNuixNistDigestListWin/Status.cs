using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CreateNuixNistDigestList
{
    public partial class Status : Form
    {
        private long previousHashCodeCount = 0;
        private long previousGeneratedDigestCount = 0;
        private bool autoclose = false;

        /// <summary>
        /// Main constructor
        /// </summary>
        /// <param name="autoClose">Set this to true to close the dialog automatically without a prompt.</param>
        public Status(bool autoClose = false)
        {
            autoclose = autoClose;
            InitializeComponent();
        }

        private void statusStrip1_Resize(object sender, EventArgs e)
        {
            toolStripStatusLabel1.Width = statusStrip1.Bounds.Right / 2;
            toolStripProgressBar1.Width = statusStrip1.Bounds.Right - toolStripStatusLabel1.Bounds.Right - 20;
        }
        /// <summary>
        /// Shows progress download. This method is threadsafe.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void ShowDownloadProgress(object sender, DownloadProgressChangedEventArgs e)
        {
            ShowProgressThreadSafe(e.ProgressPercentage, $"Downloaded {e.BytesReceived / 1000 / 1000} of {e.TotalBytesToReceive / 1000 / 1000} megabytes");
        }
        /// <summary>
        /// Updates progress. This method is threadsafe.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="message"></param>
        public void ShowProgressThreadSafe(int value, string message = "")
        {
            if (statusStrip1.InvokeRequired) // Get to check invocation of parent when checking ToolStripStatus
            {
                statusStrip1.Invoke(new MethodInvoker(() => toolStripStatusLabel1.Text = message));
                statusStrip1.Invoke(new MethodInvoker(() => toolStripProgressBar1.Value = value));
            }
            else
            {
                toolStripStatusLabel1.Text = message;
                toolStripProgressBar1.Value = value;
            }
        }
        /// <summary>
        /// Shows a line in the console. This method is threadsafe.
        /// </summary>
        /// <param name="message"></param>
        public void ShowMessageThreadSafe(string message)
        {
            if (tbConsole.InvokeRequired) // Get to check invocation of parent when checking ToolStripStatus
            {
                tbConsole.Invoke(new MethodInvoker(() => tbConsole.AppendText(DateTime.Now.ToString("yyyyMMddHHmmss.fffK") + " " + message + Environment.NewLine)));
            }
            else
            {
                tbConsole.AppendText(DateTime.Now.ToString("yyyMMddHHmmss.fffK") + " " + message + Environment.NewLine);
            }
        }
        /// <summary>
        /// Set the progressbar to 0 and the status text to empty. This method is threadsafe.
        /// </summary>
        public void ResetProgressThreadSafe()
        {
            if (tbConsole.InvokeRequired) // Get to check invocation of parent when checking ToolStripStatus
            {
                tbConsole.Invoke(new MethodInvoker(() => toolStripProgressBar1.Value = 0));
                tbConsole.Invoke(new MethodInvoker(() => toolStripStatusLabel1.Text = string.Empty));
            }
            else
            {
                tbConsole.Invoke(new MethodInvoker(() => toolStripProgressBar1.Value = 0));
                tbConsole.Invoke(new MethodInvoker(() => toolStripStatusLabel1.Text = string.Empty));
            }
        }
        public void CopytoHashcodeFileChunks_ShowMessageProgress(object sender, EventArgs e, HashCodes hashCodes)
        {
            long currentCount = hashCodes.Count - previousHashCodeCount;
            previousHashCodeCount = hashCodes.Count;
            ShowMessageThreadSafe($"Current hashcode copy rate is {currentCount} per minute.");
        }
        public void CreateNuixDigest_ShowMessageProgress(object sender, EventArgs e, NuixDigestFile digestFile)
        {
            long currentCount = digestFile.GeneratedHashCodeCount - previousGeneratedDigestCount;
            previousGeneratedDigestCount = digestFile.GeneratedHashCodeCount;
            ShowMessageThreadSafe($"Current Nuix digest file creation rate is {currentCount} per minute.");
        }

        private void Status_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (!autoclose)
            {
                var x = MessageBox.Show("Stop current process?", "Stop Process", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (x == DialogResult.No)
                {
                    e.Cancel = true;
                }
            }
        }

        private void btCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
