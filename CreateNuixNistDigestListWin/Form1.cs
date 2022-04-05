using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CreateNuixNistDigestList
{
    public partial class Form1 : Form
    {
        #region Global variables
        /// <summary>
        /// Working directory
        /// </summary>
        private string workDir;
        /// <summary>
        /// Stores list of NSRL text files extracted from selected RDS
        /// </summary>
        private List<string> listofNSRLTextFilePaths;
        /// <summary>
        /// Tracks total number of hashcodes to generate from selected RDS
        /// </summary>
        private long totalnumberofHashCodes = 0;
        /// <summary>
        /// Version of RDS to download
        /// </summary>
        private string version;
        /// <summary>
        /// Version description read from README
        /// </summary>
        private string versionDescription;
        /// <summary>
        /// Base URL for download
        /// </summary>
        private string baseURL;
        /// <summary>
        /// RDS downloads initialized from entries in README
        /// </summary>
        private List<RDSFile> rdsDownloads;

        private long previousHashCodeCount = 0;
        private long previousGeneratedDigestCount = 0;

        #endregion

        public Form1()
        {
            InitializeComponent();
        }

        #region Event handlers
        private async void btGetRDS_Click(object sender, EventArgs e)
        {
            btGetRDS.Enabled = false;

            #region Initialize download

            // Initialize version information from UI entry
            version = rbCurrent.Checked ? "current" : $"rds_{tbCustomVersion.Text}";

            // Initialize base URL from UI entry
            baseURL = tbBaseURL.Text;
            baseURL = Utils.RemoveSlashFromEnd(tbBaseURL.Text);

            Downloads downloads = new Downloads(workDir, ShowDownloadProgress, ShowMessageThreadSafe);

            #endregion

            #region Download version file and display version in console            

            // This requires access to the internet
            try
            {
                ShowMessageThreadSafe($"Getting version information start.");
                var versionFile = Path.Combine(workDir, Download.VERSION_FILE); // Make sure we also get the most current version file
                Versions versions = new Versions(baseURL, ShowProgressThreadSafe, ShowMessageThreadSafe) { WorkDir = workDir, BaseURL = baseURL };
                var versionInfo = $"{ await versions.GetVersionInfo() }";
                ShowMessageThreadSafe($"{versionInfo}");
                ShowMessageThreadSafe($"Getting version information complete.");
            }
            catch (Exception ex)
            {
                ShowMessageThreadSafe($"Failed downloading Version file. Make sure you have internet access.\n\n{ex.Message}\n\n{ ex.StackTrace}\n\nContinuing.");
            }
            finally
            {
                toolStripProgressBar1.Value = 0;
                toolStripStatusLabel1.Text = string.Empty;
            }

            #endregion

            #region Download README

            // Requires internet access
            try
            {
                var url = $"{baseURL}/{version}/{Download.README_FILE}";
                ShowMessageThreadSafe($"Downloading {url} starting.");
                await downloads.Download(url);
                ShowMessageThreadSafe($"Downloading {url} complete.");
            }
            catch (Exception ex)
            {
                ShowMessageThreadSafe($"Failed downloading {Download.README_FILE} file. Make sure you have internet access.\n\n{ex.Message}");
                MessageBox.Show($"Failed downloading {Download.README_FILE} file. Make sure you have internet access.\n\n{ex.Message}", "Download Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                if (File.Exists(Path.Combine(workDir, Download.README_FILE)))
                {
                    DialogResult retval = MessageBox.Show("There is a readme file in the working directory. Use that file and continue?", "Use Existing README", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                    if (retval == DialogResult.No)
                    {
                        btGetRDS.Enabled = true;
                        return;
                    }
                }
                else 
                {
                    // Without a README file, application cannot continue, exit.
                    ShowMessageThreadSafe($"This application requires {Download.README_FILE} file. Exiting.");
                    MessageBox.Show($"This application requires {Download.README_FILE} file. Exiting.", "Readme Required", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    btGetRDS.Enabled = true;
                    return;
                }
            }
            finally
            {
                toolStripProgressBar1.Value = 0;
                toolStripStatusLabel1.Text = string.Empty;
            }

            #endregion
            
            #region Parse README
            try
            {
                //
                // Initialize RDS downloads using values from README
                // 
                ShowMessageThreadSafe($"Parsing {Download.README_FILE} start.");
                rdsDownloads = new List<RDSFile>();
                var pathtoReadme = Path.Combine(workDir, Download.README_FILE);

                if (!File.Exists(pathtoReadme))
                {
                    throw new Exception("README file missing. Application will now exit.");
                }

                using (StreamReader sr = new StreamReader(new FileStream(pathtoReadme, FileMode.Open)))
                {
                    var downloadFileTypes = Utils.GetFieldValues(new Download());

                    RDSFile rdsVersionFile = new RDSFile();

                    string line = null;
                    // Version description
                    versionDescription = sr.ReadLine(); // First line contains version information
                    rdsVersionFile.VersionDescription = versionDescription;
                    // Version.txt URL
                    sr.ReadLine(); // Skips next two empty lines
                    sr.ReadLine();
                    rdsVersionFile.Description = sr.ReadLine();
                    sr.ReadLine(); // Skips line containing dashes
                    rdsVersionFile.DownloadURL = sr.ReadLine().Split(' ').First(); // Reads line containing URL
                    rdsVersionFile.RDSFileName = System.IO.Path.GetFileName(rdsVersionFile.DownloadURL);
                    rdsVersionFile.RDSName = downloadFileTypes.FirstOrDefault(x => x.Value == rdsVersionFile.RDSFileName).Key;
                    rdsVersionFile.BaseURL = baseURL;
                    rdsVersionFile.ShowMessageCallbackMethod = ShowMessageThreadSafe;
                    rdsVersionFile.ShowProgressCallbackMethod = ShowProgressThreadSafe;
                    rdsVersionFile.WorkDir = workDir;
                    rdsVersionFile.Version = version;
                    rdsDownloads.Add(rdsVersionFile);

                    // The next lines are standard with the following lines for each subsequent downloads
                    // - Download description
                    // - Dashes (----------)
                    // - Main download URL with file size in GB
                    // - Main download sha file with file size in bytes
                    // - An empty line
                    // Loop until end of README reading the lines as previously noted
                    while ((line = sr.ReadLine()) != null)
                    {                        
                        RDSFile rdsfile = new RDSFile();
                        rdsfile.Description = sr.ReadLine();
                        sr.ReadLine();  // Skips line containing dashes
                        rdsfile.DownloadURL = sr.ReadLine().Split(' ').First(); // Reads line containing URL                        
                        rdsfile.RDSFileName = System.IO.Path.GetFileName(rdsfile.DownloadURL);
                        rdsfile.RDSName = downloadFileTypes.FirstOrDefault(x => x.Value == rdsfile.RDSFileName).Key;

                        // Get SHA                        
                        rdsfile.SHA_URL = sr.ReadLine().Split(' ').First(); // Reads line containing URL
                        await downloads.Download(rdsfile.SHA_URL); // Download SHA file
                        string[] lines = File.ReadAllLines(Path.Combine(workDir, Path.GetFileName(rdsfile.SHA_URL))); // Parse SHA value
                        rdsfile.SHA = lines[0].Split('=')[1].Trim(); // Parse SHA value

                        // Initialize common properties
                        rdsfile.BaseURL = baseURL;
                        rdsfile.ShowMessageCallbackMethod = ShowMessageThreadSafe;
                        rdsfile.ShowProgressCallbackMethod = ShowProgressThreadSafe;
                        rdsfile.WorkDir = workDir;
                        rdsfile.Version = version;                        
                        rdsDownloads.Add(rdsfile);
                    }
                }
                ShowMessageThreadSafe($"Parsing {Download.README_FILE} complete.");
            }
            catch (Exception ex)
            {
                ShowMessageThreadSafe($"Error parsing README file. {ex.Message}\n\n{ex.StackTrace}");
                MessageBox.Show($"Error parsing README file. {ex.Message}", "Error Reading README", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            finally
            {
                toolStripProgressBar1.Value = 0;
                toolStripStatusLabel1.Text = string.Empty;
                btGetRDS.Enabled = true;
            }
            #endregion

            #region Download rds files

            if (!cbSkipDownload.Checked)
            {
                try
                {
                    if (cbLegacy.Checked)
                    {
                        var rdsfile = rdsDownloads.FirstOrDefault(o => o.RDSName == RDSType.LEGACY.ToString());
                        await rdsfile.Download();
                    }
                    if (cbiOS.Checked)
                    {
                        var rdsfile = rdsDownloads.FirstOrDefault(o => o.RDSName == RDSType.IOS.ToString());
                        await rdsfile.Download();
                    }
                    if (cbAndroid.Checked)
                    {
                        var rdsfile = rdsDownloads.FirstOrDefault(o => o.RDSName == RDSType.ANDROID.ToString());
                        await rdsfile.Download();
                    }

                    if (cbModernMinimal.Checked)
                    {
                        var rdsfile = rdsDownloads.FirstOrDefault(o => o.RDSName == RDSType.MODERN_MINIMAL.ToString());
                        await rdsfile.Download();
                    }
                    if (cbModernUnique.Checked)
                    {
                        var rdsfile = rdsDownloads.FirstOrDefault(o => o.RDSName == RDSType.MODERN_UNIQUE.ToString());
                        await rdsfile.Download();
                    }
                    if (cbModern.Checked)
                    {
                        var rdsfile = rdsDownloads.FirstOrDefault(o => o.RDSName == RDSType.MODERN.ToString());
                        await rdsfile.Download();
                    }
                }
                catch (Exception ex)
                {
                    ShowMessageThreadSafe($"Download failed. {ex.Message}\n\n{ex.StackTrace}");
                    MessageBox.Show($"Download failed. {ex.Message}", "Download Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                finally
                {
                    toolStripProgressBar1.Value = 0;
                    toolStripStatusLabel1.Text = string.Empty;
                    btGetRDS.Enabled = true;
                }
            }
            else
            {
                ShowMessageThreadSafe("Download of RDS files skipped.");
            }
            #endregion

            #region Extract NSRL Text files
            
            listofNSRLTextFilePaths = new List<string>();
            try
            {
                if (cbLegacy.Checked)
                {                    
                    var rdsfile = rdsDownloads.FirstOrDefault(o => o.RDSName == RDSType.LEGACY.ToString());
                    await Task.Run(() => rdsfile.ExtractNSRLFile());      // Extract to NSRLFile text file
                    listofNSRLTextFilePaths.Add(rdsfile.PathToNSRLFile);  // Add NSRL text file path to list
                }
                if (cbiOS.Checked)
                {
                    var rdsfile = rdsDownloads.FirstOrDefault(o => o.RDSName == RDSType.IOS.ToString());
                    await Task.Run(() => rdsfile.ExtractNSRLFile());      // Extract to NSRLFile text file
                    listofNSRLTextFilePaths.Add(rdsfile.PathToNSRLFile);  // Add NSRL text file path to list
                }
                if (cbAndroid.Checked)
                {
                    var rdsfile = rdsDownloads.FirstOrDefault(o => o.RDSName == RDSType.ANDROID.ToString());
                    await Task.Run(() => rdsfile.ExtractNSRLFile());      // Extract to NSRLFile text file
                    listofNSRLTextFilePaths.Add(rdsfile.PathToNSRLFile);  // Add NSRL text file path to list
                }

                if (cbModernMinimal.Checked)
                {
                    var rdsfile = rdsDownloads.FirstOrDefault(o => o.RDSName == RDSType.MODERN_MINIMAL.ToString());
                    await Task.Run(() => rdsfile.ExtractNSRLFile());      // Extract to NSRLFile text file
                    listofNSRLTextFilePaths.Add(rdsfile.PathToNSRLFile);  // Add NSRL text file path to list
                }
                if (cbModernUnique.Checked)
                {
                    var rdsfile = rdsDownloads.FirstOrDefault(o => o.RDSName == RDSType.MODERN_UNIQUE.ToString());
                    await Task.Run(() => rdsfile.ExtractNSRLFile());      // Extract to NSRLFile text file
                    listofNSRLTextFilePaths.Add(rdsfile.PathToNSRLFile);  // Add NSRL text file path to list
                }
                if (cbModern.Checked)
                {
                    var rdsfile = rdsDownloads.FirstOrDefault(o => o.RDSName == RDSType.MODERN.ToString());
                    await Task.Run(() => rdsfile.ExtractNSRLFile());      // Extract to NSRLFile text file
                    listofNSRLTextFilePaths.Add(rdsfile.PathToNSRLFile);  // Add NSRL text file path to list
                }
            }
            catch (Exception ex)
            {
                ShowMessageThreadSafe($"Extract NSRL text file. {ex.Message}\n\n{ex.StackTrace}");
                MessageBox.Show($"Extract NSRL text file. {ex.Message}", "Extract Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            finally
            {
                toolStripProgressBar1.Value = 0;
                toolStripStatusLabel1.Text = string.Empty;
                btGetRDS.Enabled = true;
            }

            #endregion

            #region Clean up old hashcode files

            var hashcodeFilePathPrefix = Path.Combine(workDir, $"{Constants.HashCodeFileNamePrefix}");

            if (cbDeleteHashcodeFiles.Checked)
            {
                try
                {
                    // Delete existing hashcode file                
                    var existingunsortedhashcodeFiles = Directory.GetFiles(workDir, $"{Constants.HashCodeFileNamePrefix}.unsorted.*");
                    foreach (string file in existingunsortedhashcodeFiles)
                    {
                        if (File.Exists(file))
                        {
                            ShowMessageThreadSafe($"WARNING: Deleting existing unsorted hashcode file, {file}.");
                            File.Delete(file);
                        }
                    }
                    var existingsortedhashcodeFiles = Directory.GetFiles(workDir, $"{Constants.HashCodeFileNamePrefix}.sorted.*");
                    foreach (string file in existingsortedhashcodeFiles)
                    {
                        if (File.Exists(file))
                        {
                            ShowMessageThreadSafe($"WARNING: Deleting existing sorted hashcode file, {file}.");
                            File.Delete(file);
                        }
                    }

                    var mergedhashcodefile = $"{hashcodeFilePathPrefix}.txt";
                    if (File.Exists(mergedhashcodefile))
                    {
                        ShowMessageThreadSafe($"WARNING: Deleting merged hashcode file, {mergedhashcodefile}.");
                        File.Delete(mergedhashcodefile);
                    }
                }
                catch (Exception ex)
                {
                    ShowMessageThreadSafe($"Error deleting exsting hashcode files.\n\n{ex.Message}\n\n{ex.StackTrace}");
                }
            }

            #endregion

            #region Extract Hashcodes from NSRL Text files           

            try
            {
                ShowMessageThreadSafe($"Writing hashcodes to hashcode file(s) started.");
                totalnumberofHashCodes = 0;
                int numFiles = 0;
                string currentDir = Path.GetDirectoryName(hashcodeFilePathPrefix);
                string prefix = Path.GetFileName(hashcodeFilePathPrefix);

                foreach (var nsrlTextFilePath in listofNSRLTextFilePaths)
                {
                    HashCodes hashCodes = new HashCodes(nsrlTextFilePath, hashcodeFilePathPrefix, ShowProgressThreadSafe, ShowMessageThreadSafe) { FileCount = numFiles };
                    Timer showProgressTimer = new Timer();
                    showProgressTimer.Tick += (object s, EventArgs a) => CopytoHashcodeFileChunks_ShowMessageProgress(s, a, hashCodes);
                    showProgressTimer.Interval = 60000;
                    showProgressTimer.Enabled = true;
                    showProgressTimer.Start();
                    await Task.Run(() => hashCodes.CopytoHashcodeFileChunks()); // Extract hashcode from NSRL text file and add to hashcode file
                    showProgressTimer.Stop();
                    showProgressTimer.Enabled = false;
                    numFiles = hashCodes.FileCount;
                    totalnumberofHashCodes += hashCodes.Count; // Update total
                    toolStripProgressBar1.Value = 0;
                    toolStripStatusLabel1.Text = string.Empty;
                }
                ShowMessageThreadSafe($"Writing hashcodes to hashcode file (s), completed.");
            }
            catch (Exception ex)
            {
                ShowMessageThreadSafe($"Writing hashcodes to hashcode file failed. { ex.Message}\n{ex.StackTrace}");
                MessageBox.Show($"Writing hashcodes to hashcode file failed. {ex.Message}\n{ex.StackTrace}");
                btGetRDS.Enabled = true;
                return; // Dont continue processing
            }
            finally
            {
                toolStripProgressBar1.Value = 0;
                toolStripStatusLabel1.Text = string.Empty;
            }
            #endregion

            // All hashcodes are saved in hashcode file chunks unsorted
            // Sort each hashcode file and remove duplicates

            #region Sort hashcodes

            ExternalSortMerge externalSortMerge = null;
            try
            {
                externalSortMerge = new ExternalSortMerge(hashcodeFilePathPrefix, ShowMessageThreadSafe, ShowProgressThreadSafe);
                await Task.Run(() => externalSortMerge.SortUnsortedHashCodeFiles());
            }
            catch (Exception ex)
            {
                ShowMessageThreadSafe($"Sorting hashcodes failed. { ex.Message}\n{ex.StackTrace}");
                MessageBox.Show($"Sorting hashcodes failed. {ex.Message}\n{ex.StackTrace}");
                btGetRDS.Enabled = true;
                return; // Dont continue processing
            }

            // Merge hashcode files
            try
            {
                await Task.Run(() => externalSortMerge.MergeSortedHashCodeFiles());                
            }
            catch (Exception ex)
            {
                ShowMessageThreadSafe($"Merging hashcodes failed. { ex.Message}\n{ex.StackTrace}");
                MessageBox.Show($"Merging hashcodes failed. {ex.Message}\n{ex.StackTrace}");
                btGetRDS.Enabled = true;
                return; // Dont continue processing
            }
            finally
            {
                toolStripProgressBar1.Value = 0;
                toolStripStatusLabel1.Text = string.Empty;
            }

            #endregion

            #region Create Nuix digest file

            try
            {
                var totalnumberofUniqueHashCodes = externalSortMerge.Count;
                var uniqueHashCodeFilePath = externalSortMerge.MergedHashCodesFilePath;

                if (totalnumberofUniqueHashCodes > 0)
                {
                    var nuixdigestfilePath = Path.Combine(workDir, Constants.DigestFileName);
                    // Generate hashcode
                    if (File.Exists(nuixdigestfilePath))
                    {
                        ShowMessageThreadSafe($"WARNING: Overwriting existing digest file, {nuixdigestfilePath}.");
                    }

                    NuixDigestFile nuixDigestFile = new NuixDigestFile(uniqueHashCodeFilePath, ShowProgressThreadSafe, ShowMessageThreadSafe);
                    nuixDigestFile.NumberOfhashCodes = totalnumberofUniqueHashCodes;
                    Timer showProgressTimer = new Timer();
                    showProgressTimer.Tick += (object s, EventArgs a) => CreateNuixDigest_ShowMessageProgress(s, a, nuixDigestFile);
                    showProgressTimer.Interval = 60000;
                    showProgressTimer.Enabled = true;
                    showProgressTimer.Start();
                    await Task.Run(() => nuixDigestFile.Create(nuixdigestfilePath));
                    showProgressTimer.Stop();
                    showProgressTimer.Enabled = false;
                    toolStripProgressBar1.Value = 0;
                    toolStripStatusLabel1.Text = string.Empty;
                }
                else
                {
                    ShowMessageThreadSafe($"WARNING: Hashcode file, {uniqueHashCodeFilePath}, is empty. Nothing to do.");
                    MessageBox.Show($"Hashcode file, {uniqueHashCodeFilePath}, is empty. Nothing to do.", "Empty Hashcode File", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            catch (Exception ex)
            {
                ShowMessageThreadSafe($"Failed to create a Nuix digest list. {ex.Message}\n{ex.StackTrace}");
                MessageBox.Show($"Failed to create a Nuix digest list. {ex.Message}", "Error Creating Digest List", MessageBoxButtons.OK, MessageBoxIcon.Error);
                btGetRDS.Enabled = true;
                return; // Dont continue processing
            }
            finally
            {
                toolStripProgressBar1.Value = 0;
                toolStripStatusLabel1.Text = string.Empty;
            }
            #endregion

            btGetRDS.Enabled = true;
        }        
        private void tbBaseURL_Click(object sender, EventArgs e)
        {
            if (tbBaseURL.Enabled == false)
            {
                tbBaseURL.Enabled = true;
            }
            else
            {
                tbBaseURL.Enabled = false;
            }

        }
        private void tbBaseURL_DoubleClick(object sender, EventArgs e)
        {
            if (tbBaseURL.Enabled == false)
            {
                tbBaseURL.Enabled = true;
            }
            else
            {
                tbBaseURL.Enabled = false;
            }
        }
        private void Form1_Load(object sender, EventArgs e)
        {
            workDir = Path.Combine(Utils.AssemblyDirectory, "NuixDigest");
            baseURL = Utils.RemoveSlashFromEnd(tbBaseURL.Text);

            statusStrip1_Resize(this, e);

            try
            {
                Directory.CreateDirectory(workDir);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                Environment.Exit(1);
            }
        }
        private async void Form1_Shown(object sender, EventArgs e)
        {
            try
            {
                // Get current RDS version information
                var tempWorkdir = Path.GetTempPath();
                var versionFile = Path.Combine(tempWorkdir, Download.VERSION_FILE); // Make sure we also get the most current version file
                if (File.Exists(versionFile))
                {
                    File.Delete(versionFile);
                }
                baseURL = Utils.RemoveSlashFromEnd(baseURL);
                Versions versions = new Versions(baseURL, ShowProgressThreadSafe, ShowMessageThreadSafe) { WorkDir = workDir, BaseURL = baseURL };
                var versionInfo = $"{ await versions.GetVersionInfo() }";
                linkLabelCurrentVersion.Text = versionInfo;
                File.Delete(versionFile); // Clean up
            }
            catch (Exception ex)
            {
                ShowMessageThreadSafe($"Failed to get current version information. {ex.Message}");
            }
            finally
            {
                toolStripProgressBar1.Value = 0;
                toolStripStatusLabel1.Text = string.Empty;
            }
        }
        private void linkLabelCurrentVersion_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            System.Diagnostics.Process.Start("https://www.nist.gov/itl/ssd/software-quality-group/national-software-reference-library-nsrl/nsrl-download/current-rds");
        }               
        private void ShowDownloadProgress(object sender, DownloadProgressChangedEventArgs e)
        {
            ShowProgressThreadSafe(e.ProgressPercentage, $"Downloaded {e.BytesReceived / 1000 / 1000} of {e.TotalBytesToReceive / 1000 / 1000} megabytes");
        }        
        private void ShowProgressThreadSafe(int value, string message = "")
        {
            if (toolStripStatusLabel1.GetCurrentParent().InvokeRequired) // Get to check invocation of parent when checking ToolStripStatus
            {
                toolStripStatusLabel1.GetCurrentParent().Invoke(new MethodInvoker(() => toolStripStatusLabel1.Text = message));
                toolStripProgressBar1.GetCurrentParent().Invoke(new MethodInvoker(() => toolStripProgressBar1.Value = value));
            }
            else
            {
                toolStripStatusLabel1.Text = message;
                toolStripProgressBar1.Value = value;
            }
        }
        private void ShowMessageThreadSafe(string message)
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
        private void statusStrip1_Resize(object sender, EventArgs e)
        {
            toolStripStatusLabel1.Width = statusStrip1.Bounds.Right / 2;
            toolStripProgressBar1.Width = statusStrip1.Bounds.Right - toolStripStatusLabel1.Bounds.Right - 20;
        }

        private void CopytoHashcodeFileChunks_ShowMessageProgress(object sender, EventArgs e, HashCodes hashCodes)
        {
            long currentCount = hashCodes.Count - previousHashCodeCount;
            previousHashCodeCount = hashCodes.Count;
            ShowMessageThreadSafe($"Current hashcode copy rate is {currentCount} per minute.");
        }
        private void CreateNuixDigest_ShowMessageProgress(object sender, EventArgs e, NuixDigestFile digestFile)
        {
            long currentCount = digestFile.GeneratedHashCodeCount - previousGeneratedDigestCount;
            previousGeneratedDigestCount = digestFile.GeneratedHashCodeCount;
            ShowMessageThreadSafe($"Current Nuix digest file creation rate is {currentCount} per minute.");
        }

        private void createToolStripMenuItem_Click(object sender, EventArgs e)
        {
            btGetRDS.PerformClick();
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        #endregion

        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                var executingAssembly = System.Reflection.Assembly.GetExecutingAssembly();
                MessageBox.Show($"{executingAssembly.GetCustomAttribute<AssemblyTitleAttribute>().Title} v{executingAssembly.GetName().Version.ToString()}");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error trying to show version information. {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }

}

