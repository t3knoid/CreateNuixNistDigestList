using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CreateNuixNistDigestList
{
    public class CountLines
    {

        private string _textFile;
        ShowProgressCallback _showProgressCallback;
        ShowMessageCallback _showMessageCallback;

        public CountLines(string textFile, ShowProgressCallback showProgressCallback, ShowMessageCallback showMessageCallback)
        {
            _textFile = textFile;
            _showProgressCallback = showProgressCallback;
            _showMessageCallback = showMessageCallback;
        }

        /// <summary>
        /// Returns number of lines in a given text stream
        /// Adapted from https://www.nimaara.com/counting-lines-of-a-text-file/
        /// </summary>
        /// <param name="textStream">A text file stream</param>
        /// <returns></returns>
        public long Count()
        {
            if (!File.Exists(_textFile))
            {
                _showMessageCallback($"Given file,  {_textFile}, does not exist.");
                throw new ArgumentNullException($"Given file,  {_textFile}, does not exist.");
            }
            var lineCount = 0L;
            using (FileStream textStream = File.OpenRead(_textFile))
            {
                if (textStream == null)
                {
                    _showMessageCallback($"Given file stream is null. ${textStream}");
                    throw new ArgumentNullException($"Given file stream is null. ${textStream}");
                }
                var byteBuffer = new byte[1024 * 1024];
                var detectedEOL = Constants.NULL;
                var currentChar = Constants.NULL;

                int bytesRead;
                using (ProgressStream inputStream = new ProgressStream(textStream, "Counting number of lines"))
                {
                    inputStream.UpdateProgress += ProgressStream_UpdateProgress;
                    while ((bytesRead = inputStream.Read(byteBuffer, 0, byteBuffer.Length)) > 0)
                    {
                        for (var i = 0; i < bytesRead; i++)
                        {
                            currentChar = (char)byteBuffer[i];

                            if (detectedEOL != Constants.NULL)
                            {
                                if (currentChar == detectedEOL)
                                {
                                    lineCount++;
                                }
                            }
                            else if (currentChar == Constants.LF || currentChar == Constants.CR)
                            {
                                detectedEOL = currentChar;
                                lineCount++;
                            }
                        }
                    }
                }
                // We had a NON-EOL character at the end without a new line
                if (currentChar != Constants.LF && currentChar != Constants.CR && currentChar != Constants.NULL)
                {
                    lineCount++;
                }
            }
            return lineCount;
        }

        private void ProgressStream_UpdateProgress(object sender, ProgressEventArgs e)
        {
            _showProgressCallback((int)(e.Progress * 100f), e.Message);
        }
    }
}
