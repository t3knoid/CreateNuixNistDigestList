using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CreateNuixNistDigestList
{
    /// <summary>
    /// This class provides methods to create a Nuix digest file
    /// </summary>
    public class NuixDigestFile
    {
        /// <summary>
        /// Event handler for custom progress event
        /// </summary>
        public event EventHandler<ProgressEventArgs> UpdateProgress;
        /// <summary>
        /// Path to the Nuix digest file.
        /// </summary>
        public string NuixDigestFilePath { get { return _nuixdigestfilePath; } private set { } }
        /// <summary>
        /// Hashcode count generated during creation of digest file.
        /// </summary>
        public long GeneratedHashCodeCount { get { return _generatedhashcodeCount; } private set { } }
        /// <summary>
        /// Total number of hashcodes in the given hash code file.
        /// </summary>
        public long NumberOfhashCodes { get { return _numberofhashCodes; }  set { _numberofhashCodes = value; } }

        private string _message;
        private string _nuixdigestfilePath = String.Empty;
        private long _generatedhashcodeCount;
        private long _numberofhashCodes;
        private string _pathtohashcodeFile;
        private ShowMessageCallback _showMessageCallBack;
        private ShowProgressCallback _showProgressCallBack;

        /// <summary>
        /// Main contructor to create a Nuix digest file
        /// </summary>
        /// <param name="pathToHashCodeFile">Path to a textfile containing hashcodes.</param>
        /// <param name="showProgressCallback">Method to use to show progress.</param>
        /// <param name="showMessageCallback">Method to use to show messages.</param>
        public NuixDigestFile(string pathToHashCodeFile, ShowProgressCallback showProgressCallback, ShowMessageCallback showMessageCallback)
        {
            if (new FileInfo(pathToHashCodeFile).Length == 0) throw new ArgumentException($"{pathToHashCodeFile} file is empty.");

            _pathtohashcodeFile = pathToHashCodeFile;
            _showProgressCallBack = showProgressCallback;
            _showMessageCallBack = showMessageCallback;
        }

        /// <summary>
        /// Alternate contructor to create a Nuix digest file.
        /// </summary>
        /// <param name="showProgressCallback">Method to use to show progress.</param>
        /// <param name="showMessageCallback">Method to use to show messages.</param>
        public NuixDigestFile(ShowProgressCallback showProgressCallback, ShowMessageCallback showMessageCallback)
        {
            _showProgressCallBack = showProgressCallback;
            _showMessageCallBack = showMessageCallback;
        }
        /// <summary>
        /// Creates a Nuix digest file from hash code list. This logic is adapted from 
        /// https://github.com/Nuix/Nx/blob/master/Java/src/main/java/com/nuix/nx/digest/DigestHelper.java
        /// </summary>
        /// <param name="nuixdigestfilePath">Path to nuix digest file to create</param>
        public void Create(string nuixdigestfilePath)
        {
            _nuixdigestfilePath = nuixdigestfilePath;
            Create();
        }

        /// <summary>
        /// Creates a Nuix digest file from hash code list. This logic is adapted from 
        /// https://github.com/Nuix/Nx/blob/master/Java/src/main/java/com/nuix/nx/digest/DigestHelper.java
        /// </summary>
        /// </summary>
        public void Create()
        {
            if (string.IsNullOrEmpty(_pathtohashcodeFile)) throw new ArgumentNullException($"{_pathtohashcodeFile} file is empty.");
            if (new FileInfo(_pathtohashcodeFile).Length == 0) throw new ArgumentException($"{_pathtohashcodeFile} file is empty.");

            UpdateProgress += UpdateProgress_EventHandler;

            _showMessageCallBack($"Writing {_numberofhashCodes} to digest file {_nuixdigestfilePath} start.");
            _generatedhashcodeCount = 0;

            try
            {
                using (FileStream nuixdigestfilePathStream = new FileStream(_nuixdigestfilePath, File.Exists(_nuixdigestfilePath) ? FileMode.Create : FileMode.CreateNew))
                {
                    using (BinaryWriter nuixdigestFile = new BinaryWriter(nuixdigestfilePathStream))
                    {
                        // Write header
                        byte[] bytes = WriteHeader(nuixdigestFile);
                        using (StreamReader hashCodeFileStream = new StreamReader(new FileStream(_pathtohashcodeFile, FileMode.Open)))
                        {
                            string hashcode;
                            while ((hashcode = hashCodeFileStream.ReadLine()) != null) // Read hashcode from file until end
                            {                                
                                nuixdigestFile.Write(Utils.PackMD5(hashcode));
                                _generatedhashcodeCount++;
                                _message = _generatedhashcodeCount.ToString() + " of " + _numberofhashCodes.ToString() + " written";
                                if (_generatedhashcodeCount % 100000 == 0)
                                {
                                    UpdateProgress?.Invoke(this, new ProgressEventArgs((1.0f * _generatedhashcodeCount) / _numberofhashCodes, _message));
                                }
                            }
                        }
                    }
                }
                _showMessageCallBack($"Writing {_numberofhashCodes} to digest file {_nuixdigestfilePath} complete. There were {_generatedhashcodeCount} hashcodes written out.");
            }
            catch (Exception ex)
            {
                throw new Exception($"Failure generating Nuix digest file.  There were {_generatedhashcodeCount} out {_numberofhashCodes} hashcodes written.\n{ex.Message}");
            }
        }
        /// <summary>
        /// Writes out the Nuix digest header
        /// </summary>
        /// <param name="binaryWriter"></param>
        /// <returns></returns>
        private static byte[] WriteHeader(BinaryWriter binaryWriter)
        {
            byte[] bytes;
            binaryWriter.Write(Encoding.ASCII.GetBytes("F2DL"));
            bytes = BitConverter.GetBytes((int)1);
            if (BitConverter.IsLittleEndian)
            {
                Array.Reverse(bytes);
            }

            binaryWriter.Write(bytes);

            bytes = BitConverter.GetBytes((short)3);
            if (BitConverter.IsLittleEndian)
            {
                Array.Reverse(bytes);
            }

            binaryWriter.Write(bytes);
            binaryWriter.Write(Encoding.ASCII.GetBytes("MD5"));
            return bytes;
        }

        #region Helper methods

        /// <summary>
        /// Validates digest file. Throws an exception if the file is not valid.
        /// </summary>
        public void Validate(string pathToNuixDigestList)
        {
            try
            {
                using (FileStream fileStream = new FileStream(NuixDigestFilePath, FileMode.Open)) // Read digest list file
                {
                    using (BinaryReader binaryReader = new BinaryReader(fileStream))
                    {
                        var length = (int)binaryReader.BaseStream.Length;
                        var header = binaryReader.ReadBytes(13);
                        var headertext = Encoding.ASCII.GetString(header);
                        if (!headertext.StartsWith("F2DL") || !headertext.EndsWith("MD5"))
                        {
                            throw new Exception(NuixDigestFilePath + " is not a valid Digest list file.");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        private void UpdateProgress_EventHandler(object sender, ProgressEventArgs e)
        {
            _showProgressCallBack((int)(e.Progress * 100f), e.Message);
        }

    }

}

