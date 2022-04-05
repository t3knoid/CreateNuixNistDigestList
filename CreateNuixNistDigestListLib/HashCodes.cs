using Microsoft.VisualBasic.FileIO;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CreateNuixNistDigestList
{
    /// <summary>
    /// This class is used to extract hashcodes from given
    /// NIST NSRL Files. Hashcodes are written to multiple
    /// serially named text files. Each file is limited to 
    /// a number of hashcodes specified by the value of 
    /// MaxNumHashCodes set in HashCodeParams.cs.
    /// 
    /// </summary>
    public class HashCodes
    {
        public event EventHandler<ProgressEventArgs> UpdateProgress;
        /// <summary>
        /// Provides the number of hashcodes extracted from NSRL files.
        /// </summary>
        public long Count { get { return _count; } }
        /// <summary>
        /// Provides number of serialized hashcode files.
        /// </summary>
        public int FileCount { get { return _fileCount; } set { _fileCount = value; } }

        public long OriginalCount { get { return _originalcount; } }

        private string _message;
        private long _count = 0;
        private long _originalcount = 0;
        private string _pathToNSRLFile;
        private ShowMessageCallback _showMessageCallback;
        private ShowProgressCallback _showProgressCallback;
        private int _fileCount;
        private string _filepathPrefix;

        /// <summary>
        /// Main constructor
        /// </summary>
        /// <param name="pathToNSRLFile">Path to NSRL text file</param>
        /// <param name="showProgressCallback">Method to use to show progress.</param>
        /// <param name="showMessageCallback">Method to use to show messages.</param>
        public HashCodes(string pathToNSRLFile, string filepathPrefix, ShowProgressCallback showProgressCallback, ShowMessageCallback showMessageCallback)
        {
            _pathToNSRLFile = pathToNSRLFile;
            _filepathPrefix = filepathPrefix;
            _showProgressCallback = showProgressCallback;
            _showMessageCallback = showMessageCallback;
        }

        /// <summary>
        /// Extract hashcodes from NSRL text file and copies it to chunks of serially named hash code files
        /// </summary>
        /// <param name="pathToHashCodeFile">Path to text file where hash codes are appended into.</param>
        public void CopytoHashcodeFileChunks()
        {
            UpdateProgress += UpdateProgress_EventHandler;
            _showMessageCallback($"Calculating number of lines of {_pathToNSRLFile}. This could take some time. Please wait.");
            CountLines countLines = new CountLines(_pathToNSRLFile, _showProgressCallback, _showMessageCallback);
            _originalcount = countLines.Count(); // Number of lines to parse through in the NSRL file
                                                 // Each line contains a hashcode
            _originalcount--;  // Subtract header

            long numWritten = 0; // Reset number of files written to numbered hashcode file
            string numberedpathhashcodeFile = string.Empty;

            _showMessageCallback($"Copying {_originalcount} hashcodes from {_pathToNSRLFile} start. A maximum of {HashCodesParams.MaxNumHashCodes} hashcodes will be written to each hashcode file.");
            UpdateProgress?.Invoke(this, new ProgressEventArgs((1.0f * _count) / _originalcount, ""));
            try
            {
                using (StreamReader sr = new StreamReader(new FileStream(_pathToNSRLFile, FileMode.Open)))
                {
                    string line = null;                    
                    line = sr.ReadLine(); // Skip header                    
                    do
                    {                        
                        _fileCount++; // Used to serialize numbered hashcode file
                        numWritten = 0;
                        numberedpathhashcodeFile = _filepathPrefix + $".unsorted.{_fileCount}";
                        _showMessageCallback($"Copying to {numberedpathhashcodeFile} start.");
                        long numChunkLines = 0;                        
                        // Handle existing chunk file
                        if (File.Exists(numberedpathhashcodeFile)) 
                        {
                            _showMessageCallback($"{numberedpathhashcodeFile} exists.");                            
                            numChunkLines = File.ReadAllLines(numberedpathhashcodeFile).LongLength; // Dictates how many lines to skip from the nsrl file
                            long linestoSkip = numChunkLines;
                            numWritten = linestoSkip;
                            _count += numChunkLines; // Update count with number of lines read from chunk file
                            _showMessageCallback($"{numberedpathhashcodeFile} contains {numChunkLines} hashcodes.");
                            // Skip number of lines equal to that of what is in existing chunk file                            
                            try
                            {
                                _showMessageCallback($"Skipping {numChunkLines} NSRL file entries start.");
                                linestoSkip = numChunkLines;
                                while ((line = sr.ReadLine()) != null && linestoSkip > 0)
                                {
                                    linestoSkip--;
                                    //_count++;
                                    //_message = $"{_count} of {_originalcount} skipped";
                                    //UpdateProgress?.Invoke(this, new ProgressEventArgs((1.0f * _count) / _originalcount, _message));
                                }
                                _showMessageCallback($"Skipping {numChunkLines} NSRL file entries complete.");
                                _message = $"{_count} of {_originalcount} skipped";
                                UpdateProgress?.Invoke(this, new ProgressEventArgs((1.0f * _count) / _originalcount, _message));
                            }
                            catch (Exception ex)
                            {
                                _showMessageCallback($"Skipping hashcode from {_pathToNSRLFile} failed. Chunk file had {numChunkLines} entries. There are {linestoSkip} to skip before error occured. {ex.Message}");
                            }
                        }
                        // Continue writing to existing serialized hashcode file if it has less than HashCodesParams.MaxNumHashCodes entries                        
                        using (StreamWriter sw = File.AppendText(numberedpathhashcodeFile))
                        {                            
                            if (numWritten < HashCodesParams.MaxNumHashCodes) // Make sure to fill chunk file to max
                            { 
                                _showMessageCallback($"Writing to {numberedpathhashcodeFile} start.");
                                while ((line = sr.ReadLine()) != null && (numWritten < HashCodesParams.MaxNumHashCodes))  // Stop writing if at end of input file or chunk file reach max
                                {
                                    var MD5 = GetMD5Value(line);
                                    sw.WriteLine(MD5);
                                    numWritten++; // Tracks number of hashcodes written to current numbered hashcode file
                                    _count++; // Tracks total number of hashcodes written overall
                                    if (_count % 100000 == 0)
                                    {
                                        _message = $"{_count} of {_originalcount} copied";
                                        UpdateProgress?.Invoke(this, new ProgressEventArgs((1.0f * _count) / _originalcount, _message));
                                    }
                                }
                            }
                        }
                        _showMessageCallback($"Copying {numWritten} hashcodes to {numberedpathhashcodeFile} completed.");

                    } while (line != null); // Keep writing until end of input file
                }
                _showMessageCallback($"Copying {_originalcount} hashcodes from {_pathToNSRLFile} end. Hashcodes were written to {_fileCount} files.");                
            }
            catch (Exception ex)
            {
                throw new Exception($"Copying {_originalcount} hashcodes from {_pathToNSRLFile} failed. There is a total of {_count} hashcodes successfully written. There were {numWritten} hashcodes successfully written into current hashcode chunk file, {Path.GetFileName(numberedpathhashcodeFile)},  before error occured. {ex.Message}");
            }
        }
        /// <summary>
        /// Event handler to provide progress information to the UI
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void UpdateProgress_EventHandler(object sender, ProgressEventArgs e)
        {
            _showProgressCallback((int)(e.Progress * 100f), e.Message);
        }

        /// <summary>
        /// Returns an MD5 string from a given NSRL
        /// text file line.
        /// </summary>
        /// <param name="hashFileLine">An NSRL formatted line</param>
        /// <returns></returns>
        private static string GetMD5Value(string hashFileLine)
        {
            var parser = new TextFieldParser(new StringReader(hashFileLine))
            {
                HasFieldsEnclosedInQuotes = true
            };
            parser.SetDelimiters(",");
            var dataArray = new string[] { };
            try
            {
                dataArray = parser.ReadFields();
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to parse given line. Check line provided conforms to NSRL format. {ex.Message}");
            }
            finally
            {
                if (parser != null)
                {
                    parser.Close();
                }
            }
            return dataArray[1]; // Returns MD5 value located in second column of CSV line 
        }
    }


}

