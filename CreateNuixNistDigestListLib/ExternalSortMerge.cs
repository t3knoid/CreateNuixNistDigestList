using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CreateNuixNistDigestList
{
    /// <summary>
    /// Sorts text files in chunks and merges the chunks
    /// into a single file.
    /// 
    /// The majority of the code is borrowed from here:
    /// https://www.splinter.com.au/sorting-enormous-files-using-a-c-external-mer/
    /// </summary>
    public class ExternalSortMerge
    {        
        /// <summary>
        /// Total number of unqiue hashcode count.
        /// </summary>
        public long Count { get { return _count; } set { _count = value; } }
        public HashcodeFile[] HashcodeFileArray {get {return _hashcodeFile;} }
        public string MergedHashCodesFilePath { get { return _mergedhashcodesfilePath; } }

        private long _count = 0;
        private ShowMessageCallback _showMessageCallback;
        private ShowProgressCallback _showProgressCallback;
        private string _filepathPrefix;
        private HashcodeFile[] _hashcodeFile; // Holds properties of each hashcode files
        private string _mergedhashcodesfilePath;

        public ExternalSortMerge(string filepathPrefix, ShowMessageCallback showMessageCallback, ShowProgressCallback showProgressCallback)
        {
            _filepathPrefix = filepathPrefix;
            _showMessageCallback = showMessageCallback;
            _showProgressCallback = showProgressCallback;
        }
        /// <summary>
        /// Sorts and remove duplicates of hashcodes in hashcode file chunks
        /// </summary>
        public void SortUnsortedHashCodeFiles()
        {
            string currentunsortedfilePath = string.Empty; // Using this to display current file in case there is an exception
            string[] unsortedhashcodeFilePaths;
            string currentDir = Path.GetDirectoryName(_filepathPrefix);
            string prefix = Path.GetFileName(_filepathPrefix);

            try
            {                
                _showMessageCallback($"Get a list of files of unsorted hashcodes from {currentDir}.");
                unsortedhashcodeFilePaths = Directory.GetFiles(currentDir, $"{prefix}.unsorted.*");
            }
            catch (Exception ex)
            {
                _showMessageCallback($"Failed to get hashcode files in {currentDir}. {ex.Message}");
                throw new Exception($"Failed to get hashcode files in {currentDir}. {ex.Message}");
            }

            if (unsortedhashcodeFilePaths.Length == 0)
            {
                _showMessageCallback("No unsorted hashcode files found error.");
                throw new ArgumentOutOfRangeException($"No unsorted hashcode files found error.");
            }
            
            try
            {
                _showMessageCallback($"Sorting hashcode files started.");
                _hashcodeFile = new HashcodeFile[unsortedhashcodeFilePaths.Length]; // Initialize the number of hashcodeFile array. Each entry provides properties for each hashcode file.
                int i = 0; // index ofor hashcodeFiles array

                foreach (string unsortedfilePath in unsortedhashcodeFilePaths)
                {
                    string[] hashcodes;
                    string sortedhashcodesPath = unsortedfilePath.Replace("unsorted", "sorted"); // Create the 'sorted' filename                    
                    if (File.Exists(sortedhashcodesPath))
                    {
                        _showMessageCallback($"{sortedhashcodesPath} exists, skipping sort.");
                    }
                    else
                    {
                        currentunsortedfilePath = unsortedfilePath;

                        _showMessageCallback($"Reading hashcodes from {unsortedfilePath} into memory.");
                        hashcodes = File.ReadAllLines(unsortedfilePath); // Read unsorted lines into an array                    

                        _showMessageCallback($"Sorting hashcodes");
                        Array.Sort(hashcodes); // Sort the in-memory array

                        _showMessageCallback($"Sorting duplicates.");
                        hashcodes = hashcodes.Distinct().ToArray(); // Remove Duplicates
                        
                        _showMessageCallback($"Writing sorted hashcodes into {sortedhashcodesPath} into memory.");
                        File.WriteAllLines(sortedhashcodesPath, hashcodes); // Write sorted lines into sorted file
                    }
                    hashcodes = File.ReadAllLines(sortedhashcodesPath); // Read sorted lines into an array                    
                    _hashcodeFile[i] = new HashcodeFile { FilePath = sortedhashcodesPath, Count = hashcodes.Length };
                    _count += _hashcodeFile[i].Count;
                    i++;                                                                                         
                    hashcodes = null; // Free the in-memory sorted array
                }
                _showMessageCallback($"Sorting hashcode files completed.");
            }
            catch (Exception ex)
            {
                _showMessageCallback($"Failed to sort hashcodes in {currentunsortedfilePath}. {ex.Message}");
                throw new Exception($"Failed to sort hashcodes in {currentunsortedfilePath}. {ex.Message}");
            }
        }
        /// <summary>
        /// Merges sorted hashcode files into a single file. If this file already exists, it skips the whole
        /// operation. The number of merged hashcode entries is calculated and stored in the Count property.
        /// 
        /// Opens a FIFO queue of all the sorted chunks simultaneously. It then outputs the lowest-sorted
        /// record from all queues one at a time, until all queues are empty.
        /// 
        /// See https://en.wikipedia.org/wiki/Merge_algorithm
        /// </summary>
        public void MergeSortedHashCodeFiles()
        {
            _mergedhashcodesfilePath = $"{_filepathPrefix}.txt";

            if (File.Exists(_mergedhashcodesfilePath))
            {
                _showMessageCallback($"Merged hashcode file, {_mergedhashcodesfilePath}, already exist. Nothing to do.");
            }
            else
            {
                if (_hashcodeFile.Length == 0)
                {
                    _showMessageCallback("No sorted hashcode files found error.");
                    throw new ArgumentOutOfRangeException($"No sorted hashcode files found error.");
                }

                int numberofsortedhashcodeFiles = _hashcodeFile.Length; // Number of sorted files

                int recordsize = 500; // estimated record size
                long records = HashCodesParams.MaxNumHashCodes; // estimated total # records
                int maxusage = 500000000; // max memory usage
                int buffersize = maxusage / numberofsortedhashcodeFiles; // number of hashcodes to read each time into the queue
                                                                         // avoids using too much memory
                double recordoverhead = 7.5; // The overhead of using Queue<>
                int bufferlen = (int)(buffersize / recordsize / recordoverhead); // number of records in each queue

                _showMessageCallback($"There are {numberofsortedhashcodeFiles} sorted hashcode files to merge into {_mergedhashcodesfilePath}.");
                _showMessageCallback($"Merging hashcode files start.");
                Queue<string>[] hashcodequeue = new Queue<string>[numberofsortedhashcodeFiles];  // Number of queues 
                StreamReader[] readers = new StreamReader[numberofsortedhashcodeFiles]; // Open all files
                try
                {
                    for (int i = 0; i < numberofsortedhashcodeFiles; i++)
                    {
                        readers[i] = new StreamReader(_hashcodeFile[i].FilePath);
                        hashcodequeue[i] = new Queue<string>(_hashcodeFile[i].Count);
                    }

                    // Load the queues
                    for (int i = 0; i < numberofsortedhashcodeFiles; i++)
                    {
                        LoadHashCodesIntoQueue(hashcodequeue[i], readers[i], bufferlen);
                    }

                    // Merge
                    using (StreamWriter sw = new StreamWriter(_mergedhashcodesfilePath))
                    {
                        bool done = false;
                        int lowest_index, j, progress = 0;
                        string lowest_value;
                        while (!done)
                        {
                            ++progress;
                            if (progress % 10000 == 0)
                            {
                                _showProgressCallback((int)((progress * 100f) / _count), "Merging hashcode files");
                            }

                            // Find the chunk with the lowest value
                            lowest_index = -1;
                            lowest_value = "";
                            for (j = 0; j < numberofsortedhashcodeFiles; j++)
                            {
                                if (hashcodequeue[j] != null)
                                {
                                    if (lowest_index < 0 ||
                                      String.CompareOrdinal(
                                        hashcodequeue[j].Peek(), lowest_value) < 0)
                                    {
                                        lowest_index = j;
                                        lowest_value = hashcodequeue[j].Peek();
                                    }
                                }
                            }

                            // Was nothing found in any queue? We must be done then.
                            if (lowest_index == -1) { done = true; break; }

                            // Output it
                            sw.WriteLine(lowest_value);

                            // Remove from queue
                            hashcodequeue[lowest_index].Dequeue();

                            if (hashcodequeue[lowest_index].Count == 0)  // If the queue is empty, load the next set of hashcodes into memory
                            {
                                LoadHashCodesIntoQueue(hashcodequeue[lowest_index],
                                  readers[lowest_index], bufferlen);

                                if (hashcodequeue[lowest_index].Count == 0)  // If there are no more hashcodes to read in, we are done with all hashcodes in this queue
                                {
                                    hashcodequeue[lowest_index] = null;
                                }
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception($"Failed to merge hashcode files. {ex.Message}");
                }
                finally
                {
                    // Close the files
                    for (int i = 0; i < numberofsortedhashcodeFiles; i++)
                    {
                        readers[i].Close();
                    }
                }
            }

            CountLines countLines = new CountLines(_mergedhashcodesfilePath, _showProgressCallback, _showMessageCallback);
            _count = countLines.Count();
            _showMessageCallback($"Merging hashcode files completed. There are {_count} unique hashcodes.");
            _showMessageCallback($"Sorted hashcodes are merged in {_mergedhashcodesfilePath}");
        }
        /// <summary>
        /// Initializes each queue
        /// </summary>
        /// <param name="queue"></param>
        /// <param name="file"></param>
        /// <param name="records"></param>
        private static void LoadHashCodesIntoQueue(Queue<string> queue,
          StreamReader file, int records)
        {
            for (int i = 0; i < records; i++)
            {
                if (file.Peek() < 0) break;
                queue.Enqueue(file.ReadLine());
            }
        }

        private void UpdateProgress_EventHandler(object sender, ProgressEventArgs e)
        {
            _showProgressCallback((int)(e.Progress * 100f), e.Message);
        }
    }    

}
