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
    class Program
    {
        #region Global variables

        private static Downloads downloads;
        private static string version = "current";
        private static string baseURL = "https://s3.amazonaws.com/rds.nsrl.nist.gov/RDS";
        private static bool skipDownload = false;
        private static bool delHashcodes = false;
        /// <summary>
        /// Working directory
        /// </summary>
        private static string workDir;
        private static bool modern = false;
        private static bool modernm = true;
        private static bool modernu = false;
        private static bool android = true;
        private static bool ios = true;
        private static bool legacy = true;
        private static List<string> hashcodeList = new List<string>();
        /// <summary>
        /// Stores list of NSRL text files extracted from selected RDS
        /// </summary>
        private static List<string> listofNSRLTextFilePaths = new List<string>();
        /// <summary>
        /// Tracks total number of hashcodes to generate from selected RDS
        /// </summary>
        private static long totalnumberofHashCodes = 0;
        /// <summary>
        /// Version description read from README
        /// </summary>
        private static string versionDescription;
        /// <summary>
        /// RDS downloads initialized from entries in README
        /// </summary>
        private static List<RDSFile> rdsDownloads;

        #endregion

        static void Main(string[] args)
        {
            workDir = Path.Combine(Utils.AssemblyDirectory, "NuixDigest");
            
            GetParams(args);

            // We will never get here if parameters are not set right

            #region Initialize environment

            try
            {
                Directory.CreateDirectory(workDir);
            }
            catch (Exception ex)
            {
                ShowMessage($"Error in creating working directory {workDir}, exiting. {ex.Message}");
                Environment.Exit(1);
            }

            #endregion

            #region Initialize download

            downloads = new Downloads(workDir, ShowDownloadProgress, ShowMessage);

            #endregion

            #region Download version file and display version in console

            // This requires access to the internet
            try
            {
                Versions versions = new Versions(baseURL, ShowProgress, ShowMessage) { WorkDir = workDir, BaseURL = baseURL };
                var versionInfo = versions.GetVersionInfo();
                ShowMessage($"{versionInfo.Result}");
            }
            catch (Exception ex)
            {
                ShowMessage($"Failed downloading Version file. Make sure you have internet access.\n\n{ex.Message}\n\n{ ex.StackTrace}\n\nContinuing.");
            }

            #endregion

            #region  Download README

            // Requires internet access
            try
            {
                var url = $"{baseURL}/{version}/{Download.README_FILE}";
                ShowMessage($"Downloading {url} starting.");
                downloads.Download(url).Wait();
                ShowMessage($"Downloading {url} complete.");
            }
            catch (Exception ex)
            {
                ShowMessage($"Failed downloading {Download.README_FILE} file. Make sure you have internet access.\n\n{ex.Message}");
                if (File.Exists(Path.Combine(workDir, Download.README_FILE)))
                {
                    ShowMessage($"Existing {Download.README_FILE} file found. Will use that file. Continuing.");
                }
                else
                {
                    // Without a README file, application cannot continue, exit.
                    ShowMessage($"This application requires {Download.README_FILE} file. Exiting.");
                    return;
                }

            }

            #endregion

            #region Parse README
            //
            // Initialize RDS downloads using values from README
            // 
            try
            {
                ShowMessage($"Parsing {Download.README_FILE} start.");
                rdsDownloads = new List<RDSFile>();
                var pathtoReadme = Path.Combine(workDir, Download.README_FILE);

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
                    rdsVersionFile.DownloadURL = sr.ReadLine().Split(' ').First(); // Reads line containing version.txt URL
                    rdsVersionFile.RDSFileName = System.IO.Path.GetFileName(rdsVersionFile.DownloadURL);
                    rdsVersionFile.RDSName = downloadFileTypes.FirstOrDefault(x => x.Value == rdsVersionFile.RDSFileName).Key;
                    rdsDownloads.Add(rdsVersionFile);
                    sr.ReadLine(); // Skips line after version.txt download URL

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
                        rdsfile.Description = line;
                        sr.ReadLine();  // Skips line containing dashes
                        rdsfile.DownloadURL = sr.ReadLine().Split(' ').First(); // Reads line containing version.txt URL
                        rdsfile.RDSFileName = System.IO.Path.GetFileName(rdsfile.DownloadURL);
                        rdsfile.RDSName = downloadFileTypes.FirstOrDefault(x => x.Value == rdsfile.RDSFileName).Key;
                        rdsDownloads.Add(rdsfile);
                        sr.ReadLine(); // Skips line after version.txt download URL
                    }
                }
                ShowMessage($"Parsing {Download.README_FILE} complete.");
            }
            catch (Exception ex)
            {
                ShowMessage($"Error parsing README file. {ex.Message}");
                return;
            }
            #endregion

            #region Download rds files
            if (!skipDownload)
            {

                try
                {
                    if (legacy)
                    {
                        var rdsfile = rdsDownloads.FirstOrDefault(o => o.RDSName == RDSType.LEGACY.ToString());
                        rdsfile.Download().Wait();
                    }
                    if (ios)
                    {
                        var rdsfile = rdsDownloads.FirstOrDefault(o => o.RDSName == RDSType.IOS.ToString());
                        rdsfile.Download().Wait();
                    }
                    if (android)
                    {
                        var rdsfile = rdsDownloads.FirstOrDefault(o => o.RDSName == RDSType.ANDROID.ToString());
                        rdsfile.Download().Wait();
                    }

                    if (modernm)
                    {
                        var rdsfile = rdsDownloads.FirstOrDefault(o => o.RDSName == RDSType.MODERN_MINIMAL.ToString());
                        rdsfile.Download().Wait();
                    }
                    if (modernu)
                    {
                        var rdsfile = rdsDownloads.FirstOrDefault(o => o.RDSName == RDSType.MODERN_UNIQUE.ToString());
                        rdsfile.Download().Wait();
                    }
                    if (modern)
                    {
                        var rdsfile = rdsDownloads.FirstOrDefault(o => o.RDSName == RDSType.MODERN.ToString());
                        rdsfile.Download().Wait();
                    }
                }
                catch (Exception ex)
                {
                    ShowMessage($"Download failed. {ex.Message}\n\n{ex.StackTrace}");
                    Environment.Exit(1);
                }
            }
            #endregion            

            #region Exttact NSRL text files

            listofNSRLTextFilePaths = new List<string>();

            try
            {
                if (legacy)
                {
                    var rdsfile = rdsDownloads.FirstOrDefault(o => o.RDSName == RDSType.LEGACY.ToString());
                    rdsfile.ExtractNSRLFile();
                    listofNSRLTextFilePaths.Add(rdsfile.PathToNSRLFile);  // Add NSRL text file path to list
                }
                if (ios)
                {
                    var rdsfile = rdsDownloads.FirstOrDefault(o => o.RDSName == RDSType.IOS.ToString());
                    rdsfile.ExtractNSRLFile();
                    listofNSRLTextFilePaths.Add(rdsfile.PathToNSRLFile);  // Add NSRL text file path to list
                }
                if (android)
                {
                    var rdsfile = rdsDownloads.FirstOrDefault(o => o.RDSName == RDSType.ANDROID.ToString());
                    rdsfile.ExtractNSRLFile();
                    listofNSRLTextFilePaths.Add(rdsfile.PathToNSRLFile);  // Add NSRL text file path to list
                }

                if (modernm)
                {
                    var rdsfile = rdsDownloads.FirstOrDefault(o => o.RDSName == RDSType.MODERN_MINIMAL.ToString());
                    rdsfile.ExtractNSRLFile();
                    listofNSRLTextFilePaths.Add(rdsfile.PathToNSRLFile);  // Add NSRL text file path to list
                }
                if (modernu)
                {
                    var rdsfile = rdsDownloads.FirstOrDefault(o => o.RDSName == RDSType.MODERN_UNIQUE.ToString());
                    rdsfile.ExtractNSRLFile();
                    listofNSRLTextFilePaths.Add(rdsfile.PathToNSRLFile);  // Add NSRL text file path to list
                }
                if (modern)
                {
                    var rdsfile = rdsDownloads.FirstOrDefault(o => o.RDSName == RDSType.MODERN.ToString());
                    rdsfile.ExtractNSRLFile();
                    listofNSRLTextFilePaths.Add(rdsfile.PathToNSRLFile);  // Add NSRL text file path to list
                }
            }
            catch (Exception ex)
            {
                ShowMessage($"Extract NSRL text file failed. {ex.Message}\n\n{ex.StackTrace}");
                Environment.Exit(1);
            }
            
            #endregion

            #region Clean up old hashcode files
            var hashcodeFilePathPrefix = Path.Combine(workDir, $"{Constants.HashCodeFileNamePrefix}");

            if (delHashcodes)
            {
                try
                {
                    // Delete existing hashcode file                
                    var existingunsortedhashcodeFiles = Directory.GetFiles(workDir, $"{Constants.HashCodeFileNamePrefix}.unsorted.*");
                    foreach (string file in existingunsortedhashcodeFiles)
                    {
                        if (File.Exists(file))
                        {
                            ShowMessage($"WARNING: Deleting existing unsorted hashcode file, {file}.");
                            File.Delete(file);
                        }
                    }
                    var existingsortedhashcodeFiles = Directory.GetFiles(workDir, $"{Constants.HashCodeFileNamePrefix}.sorted.*");
                    foreach (string file in existingunsortedhashcodeFiles)
                    {
                        if (File.Exists(file))
                        {
                            ShowMessage($"WARNING: Deleting existing sorted hashcode file, {file}.");
                            File.Delete(file);
                        }
                    }
                    var mergedhashcodefile = $"{hashcodeFilePathPrefix}.txt";
                    if (File.Exists(mergedhashcodefile))
                    {
                        ShowMessage($"WARNING: Deleting merged hashcode file, {mergedhashcodefile}.");
                        File.Delete(mergedhashcodefile);
                    }
                }
                catch (Exception ex)
                {
                    ShowMessage($"Error deleting exsting hashcode files. {ex.Message}");
                }
            }

            #endregion

            #region Extract Hashcodes from NSRL Text files           

            try
            {
                ShowMessage($"Writing hashcodes to hashcode file(s) started.");

                totalnumberofHashCodes = 0;
                int numFiles = 0;
                foreach (var nsrlTextFilePath in listofNSRLTextFilePaths)
                {
                    HashCodes hashCodes = new HashCodes(nsrlTextFilePath, hashcodeFilePathPrefix, ShowProgress, ShowMessage) { FileCount = numFiles };
                    hashCodes.CopytoHashcodeFileChunks(); // Extract hashcode from NSRL text file and add to hashcode file
                    numFiles = hashCodes.FileCount;
                    totalnumberofHashCodes += hashCodes.Count; // Update total
                }
                ShowMessage($"Writing hashcodes to hashcode file (s), completed.");
            }
            catch (Exception ex)
            {
                ShowMessage($"Writing hashcodes to hashcode file failed. { ex.Message}\n{ex.StackTrace}");
            }
            #endregion

            // All hashcodes are saved in hashcode file chunks unsorted
            // Sort each hashcode file and remove duplicates

            #region Sort hashcodes

            // Sort each hashcode file and remove duplicates
            ExternalSortMerge externalSortMerge = null;
            try
            {
                externalSortMerge = new ExternalSortMerge(hashcodeFilePathPrefix, ShowMessage, ShowProgress);
                externalSortMerge.SortUnsortedHashCodeFiles();
            }
            catch (Exception ex)
            {
                ShowMessage($"Sorting hashcodes failed. { ex.Message}\n{ex.StackTrace}");
                Environment.Exit(1); // Don't continue. Exit with error code.
            }

            // Merge hashcode files
            try
            {
                externalSortMerge.MergeSortedHashCodeFiles();
            }
            catch (Exception ex)
            {
                ShowMessage($"Merging hashcodes failed. { ex.Message}\n{ex.StackTrace}");
                Environment.Exit(1); // Don't continue. Exit with error code.
            }

            #endregion

            #region Create Nuix hashcode file

            /// Create a Nuix digest file
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
                        ShowMessage($"WARNING: Overwriting existing digest file, {nuixdigestfilePath}.");
                    }

                    NuixDigestFile nuixDigestFile = new NuixDigestFile(uniqueHashCodeFilePath, ShowProgress, ShowMessage);
                    nuixDigestFile.NumberOfhashCodes = totalnumberofUniqueHashCodes;
                    nuixDigestFile.Create(Path.Combine(workDir, nuixdigestfilePath));
                }
                else
                {
                    ShowMessage($"WARNING: Hashcode file, {uniqueHashCodeFilePath}, is empty. Nothing to do.");
                }
            }
            catch (Exception ex)
            {
                ShowMessage($"Failed to create a Nuix digest list. {ex.Message}\n{ex.StackTrace}");
            }

            #endregion
        }

        #region Get command-line parameters
        /// <summary>
        /// Parses the command-line parameters
        /// </summary>
        /// <param name="args"></param>
        private static void GetParams(string[] args)
        {
            try
            {
                var parser = new CommandLine();
                parser.Parse(args);

                if (parser.Arguments.Count > 0)
                {
                    if (parser.Arguments.ContainsKey("help"))
                    {
                        Console.WriteLine("CreateNuixDigestListDirect [-version version] [-url url] [-help] [-skipdownload]");
                        Console.WriteLine("\n  -version      Specifies the version of RDS to download.");
                        Console.WriteLine("                Uses current if not specified.");
                        Console.WriteLine("  -url          Specifies the root url to download files from.");
                        Console.WriteLine("                Uses https://s3.amazonaws.com/rds.nsrl.nist.gov/RDS if not specified.");
                        Console.WriteLine("                This parameter should never change.");
                        Console.WriteLine("  -skipdownload specifies the root url to download files from.");
                        Console.WriteLine("  -delhashcodes delete and recreated hashcode files.");
                        Console.WriteLine("  -modern       include modern RDS hash set.");
                        Console.WriteLine("  -modernm      include modern minimal RDS hash set (default).");
                        Console.WriteLine("  -modernu      include modern unique RDS hash set.");
                        Console.WriteLine("  -android      include android RDS hash set (default).");
                        Console.WriteLine("  -ios          include iOS RDS hash set (default)");
                        Console.WriteLine("  -legacy       include Legacy RDS hash set.");
                        Console.WriteLine("\n  -help         Displays this help text and current.");

                        try
                        {
                            var tempWorkdir = Path.GetTempPath();
                            var versionFile = Path.Combine(tempWorkdir, Download.VERSION_FILE); // Make sure we also get the most current version file
                            if (File.Exists(versionFile))
                            {
                                File.Delete(versionFile);
                            }
                            baseURL = Utils.RemoveSlashFromEnd(baseURL);
                            Versions versions = new Versions(baseURL, ShowProgress, ShowMessage) { WorkDir = tempWorkdir, BaseURL = baseURL };
                            var versionInfo = versions.GetVersionInfo();
                            Console.WriteLine($"\nRDS database current version information: {versionInfo.Result}");
                            File.Delete(versionFile); // Clean up
                        }
                        catch (Exception ex)
                        {
                            ShowMessage($"Failed to get current version information. {ex.Message}");
                        }

                        Environment.Exit(0);
                    }
                    if (parser.Arguments.ContainsKey("version"))
                    {
                        version = parser.Arguments["version"][0];
                    };

                    if (parser.Arguments.ContainsKey("url"))
                    {
                        baseURL = parser.Arguments["url"][0];
                    };
                    delHashcodes = parser.Arguments.ContainsKey("delhashcodes");
                    skipDownload = parser.Arguments.ContainsKey("skipdownload");
                    modern = parser.Arguments.ContainsKey("modern");
                    modernm = parser.Arguments.ContainsKey("modernm");
                    modernu = parser.Arguments.ContainsKey("modernu");
                    android = parser.Arguments.ContainsKey("android");
                    ios = parser.Arguments.ContainsKey("ios");
                    legacy = parser.Arguments.ContainsKey("legacy");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(DateTime.Now.ToString("yyymmddHHmmss.fffK") + " " + "Failed parsing parameters. Exiting. " + ex.Message);
                Environment.Exit(1);
            }
        }
        #endregion

        #region Event handler delegates

        /// <summary>
        /// Shows progress of a download
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void ShowDownloadProgress(object sender, DownloadProgressChangedEventArgs e)
        {
            ShowProgress(e.ProgressPercentage);
        }

        /// <summary>
        /// Shows progress of download.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <param name="filename"></param>
        private static void ShowProgress(int value, string message = "")
        {
            Console.Write($"\r{value}% {message}");
         }

        private static void ShowMessage(string message)
        {            
            Console.WriteLine(DateTime.Now.ToString("yyymmddHHmmss.fffK") + " " + message);
        }

        #endregion

    }
}
