using Microsoft.VisualBasic.FileIO;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using DiscUtils.Udf;
using System.IO.Compression;
using System.Threading.Tasks;
using System.Net;

namespace CreateNuixNistDigestList
{
    public class RDSFile
    {
        public ShowProgressCallback ShowProgressCallbackMethod { set {_showProgressCallback = value; } }
        public ShowMessageCallback ShowMessageCallbackMethod { set { _showMessageCallback = value; } }
        public string Description { get { return _description; } set { _description = value; } }
        public string DownloadURL { get { return _downloadurl; } set { _downloadurl = value; } }
        public string PathToNSRLFile { get { return _nsrlFilePath; } set { _nsrlFilePath = value; } }
        public string Version { get { return _version; } set { _version = value; } }
        public string VersionDescription { get { return _versiondescription; } set { _versiondescription = value; } }
        public string BaseURL { get { return _baseURL; } set { _baseURL = value; } }
        public string SHA_URL { get { return _sha_URL; } set { _sha_URL = value;  } }
        public string SHA { get { return _sha;  } set { _sha = value; } }
        public string RDSFileName { get { return _rdsFileName; } set { _rdsFileName = value; } }
        public string RDSName { get { return _rdsName; } set { _rdsName = value; } }
        public string WorkDir { get { return _workDir; } set { _workDir = value; } }

        private ShowProgressCallback _showProgressCallback;
        private ShowMessageCallback _showMessageCallback;

        private string _description;
        private string _sha_URL;
        private string _sha;
        private string _downloadurl;
        private string _nsrlFilePath;
        private string _rdsFilePath;
        private string _rdsName;
        private string _rdsFileName;
        private string _workDir;
        private string _version;
        private string _versiondescription;
        private string _baseURL = null;
                
        public RDSFile()
        { }

        /// <summary>
        /// Downloads specified RDS file
        /// </summary>
        public async Task Download()
        {
            if (File.Exists(_rdsFilePath))
            {
                _showMessageCallback($"Skipped downloading {_rdsFileName}");
            }
            else
            {
                var url = $"{_baseURL}/{_version}/{_rdsFileName}";
                _showMessageCallback($"Downloading {url} starting.");                
                Downloads downloads = new Downloads(_workDir, ShowDownloadProgress, _showMessageCallback);
                await downloads.Download(url);
                _showMessageCallback($"Downloading {url} complete.");
            }
        }

        private void ShowDownloadProgress(object sender, DownloadProgressChangedEventArgs e)
        {
            // _showProgressCallback(e.ProgressPercentage, string.Empty);
            _showProgressCallback(e.ProgressPercentage, $"Downloaded {e.BytesReceived / 1000 / 1000} of {e.TotalBytesToReceive / 1000 / 1000} megabytes");
        }

        public void Validate()
        {
            string header = string.Empty;

            try
            {
                using (StreamReader streamReader = new StreamReader(_rdsFilePath))
                {
                    header = streamReader.ReadLine();
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error reading " + _rdsFilePath + " header. " + ex.Message);
            }

            // Check header
            TextFieldParser parser = null;
            try
            {
                parser = new TextFieldParser(new StringReader(header))
                {
                    HasFieldsEnclosedInQuotes = true
                };
                parser.SetDelimiters(",");
                var columns = new string[] { };
                columns = parser.ReadFields();
                if (columns.Length != 8 ||
                    !columns[0].Equals("SHA-1") ||
                    !columns[1].Equals("MD5") ||
                    !columns[2].Equals("CRC32") ||
                    !columns[3].Equals("FileName") ||
                    !columns[4].Equals("FileSize") ||
                    !columns[5].Equals("ProductCode") ||
                    !columns[6].Equals("OpSystemCode") ||
                    !columns[7].Equals("SpecialCode"))
                {
                    throw new MalformedLineException();
                }
            }
            catch (Exception ex)
            {
                throw ex; // Exits while loop 
            }
            finally
            {
                if (parser != null)
                {
                    parser.Close();
                }
            }
        }

        /// <summary>
        /// Extracts NSRLFILE.TXT of a given RDS type
        /// </summary>
        /// <param name="rdsType"></param>
        public void ExtractNSRLFile()
        {
            _nsrlFilePath = Path.Combine(_workDir, $"NSRLFile_{_rdsName}.txt");
            string extractPath = Path.Combine(Path.GetTempPath(), $"RDS_{_rdsName}");

            // Check for required parameters
            if (string.IsNullOrEmpty(_workDir))
            {
                throw new ArgumentNullException("Set working folder before calling this method.");
            }
            _showMessageCallback($"Extracting NSRL File from {_rdsFileName} start.");

            if (File.Exists(_nsrlFilePath))
            {
                // Nothing to do
                _showMessageCallback($"NSRLFile_{_rdsName}.txt exists. Nothing to do.");
                return;
            }            

            try
            {
                // Check if RDS file is an ISO file
                // If it is, ZIP file containing NSRFILE text
                // file must be extracted from ISO first
                string zipPath;

                if (Path.GetExtension(_rdsFilePath) == ".iso")
                {
                    zipPath = ExtractZipFileFromISO(extractPath);
                }
                else
                {
                    zipPath = Path.Combine(_workDir, $"{_rdsFileName}");
                }

                if (!File.Exists(_nsrlFilePath))
                {
                    Unzip(zipPath, _nsrlFilePath);
                }
                else
                {
                    _showMessageCallback($"{_nsrlFilePath} exists. Nothing to do.");
                }

            }
            catch (Exception ex)
            {
                throw new Exception($"Failed extracting NSRL file from {_rdsName} RDS file. " + ex.Message);
            }
            finally
            {
                // Clean up
                _showMessageCallback($"Cleaning up {extractPath}.");
                if (Directory.Exists(extractPath))
                {
                    Directory.Delete(extractPath, true);
                }
            }
            _showMessageCallback($"Extracting NSRL File from {_rdsFileName} completed.");
        }
        /// <summary>
        /// Extracts zip file containing NSRL text file
        /// </summary>
        /// <param name="extractPath">Path to extract zip file into.</param>
        /// <returns>Path to extracted zip file</returns>
        private string ExtractZipFileFromISO(string extractPath)
        {
            var zipPath = Path.Combine(extractPath, "NSRLFile.txt.zip");

            // Extract NSRLFILE.ZIP from ISO
            string isoPath = _rdsFilePath;
            try
            {
                _showMessageCallback($"Extracting NSRLFile.txt.zip from ISO image to {extractPath} started.");
                using (FileStream isoStream = File.Open(@isoPath, FileMode.Open))
                {
                    UdfReader cd = new UdfReader(isoStream);

                    using (Stream fileStream = cd.OpenFile(@"\\NSRLFile.txt.zip", FileMode.Open)) // Read  the NSRLFILE.ZIP from the root of CD                    
                    {
                        if (!Directory.Exists(extractPath))
                        {
                            Directory.CreateDirectory(extractPath); // Create destination folder
                        }
                        
                        using (Stream zipFile = File.Create(Path.Combine(zipPath))) // Write out NSRLFILE.ZIP 
                        {
                            using (ProgressStream inputStream = new ProgressStream(fileStream, "Extracting NSRLFile.txt.zip"))
                            {
                                inputStream.UpdateProgress += ProgressStream_UpdateProgress;
                                inputStream.CopyTo(zipFile);
                            }
                        }
                    }
                }
                _showMessageCallback($"Extracting NSRLFile.txt.zip from ISO image to {extractPath} completed.");
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to extract NSRLFile.txt.zip from {isoPath}. {ex.Message}");
            }

            return zipPath;
        }

        /// <summary>
        /// Unzips given zip file.
        /// </summary>
        /// <param name="zipPath">Path to zip file.</param>
        /// <param name="extractPath">Path to extracted file.</param>
        private void Unzip(string zipPath, string extractPath)
        {
            try
            {
                _showMessageCallback($"Unzipping {extractPath} started.");
                // Normalizes the path.
                extractPath = Path.GetFullPath(extractPath);
                using (FileStream destination = new FileStream(extractPath, FileMode.Create))
                {
                    using (ZipArchive archive = ZipFile.Open(zipPath, ZipArchiveMode.Read))
                    {
                        foreach (ZipArchiveEntry entry in archive.Entries)
                        {
                            if (entry.FullName.EndsWith("NSRLFile.txt", StringComparison.OrdinalIgnoreCase))
                            {
                                using (Stream zipArchiveEntryStream = entry.Open())
                                {
                                    using (ProgressStream inputStream = new ProgressStream(zipArchiveEntryStream, entry.Length, "Unzipping NSRLFile.txt"))
                                    {
                                        inputStream.UpdateProgress += ProgressStream_UpdateProgress;
                                        inputStream.CopyTo(destination);
                                    }
                                }
                            }
                        }
                    }
                }
                _showMessageCallback($"Unzipping {zipPath} to {extractPath} completed.");
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to unzip {zipPath}. {ex.Message}");
            }
        }

        private void ProgressStream_UpdateProgress(object sender, ProgressEventArgs e)
        {
            _showProgressCallback((int)(e.Progress * 100f), e.Message);
        }
    }
}

