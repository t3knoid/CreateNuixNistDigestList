using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Management;
using System.Text;
using System.Threading.Tasks;
using System.IO.Compression;
using Microsoft.VisualBasic.FileIO;

namespace CreateNuixNistDigestList
{
    public class ProgressStream : Stream
    {
        private Stream m_input = null;
        private long m_length = 0L;
        private long m_position = 0L;
        private string message;

        public event EventHandler<ProgressEventArgs> UpdateProgress;

        public ProgressStream(Stream input, string msg = "")
        {
            m_input = input;
            m_length = input.Length;
            message = msg;
        }

        public ProgressStream(Stream input, long length, string msg="")
        {
            m_input = input;
            m_length = length;
            message = msg;
        }

        public override void Flush()
        {
            throw new System.NotImplementedException();
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            throw new System.NotImplementedException();
        }

        public override void SetLength(long value)
        {
            throw new System.NotImplementedException();
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            int n = m_input.Read(buffer, offset, count);
            m_position += n;
            if (m_position % 10000 == 0)
            {
                UpdateProgress?.Invoke(this, new ProgressEventArgs((1.0f * m_position) / m_length, message));
            }
            return n;
        }
        /// <summary>
        /// Returns a byte from the stream
        /// </summary>
        /// <returns></returns>
        public override int ReadByte()
        {
            int n = m_input.ReadByte();
            m_position += 1; // We are only reading 1 byte
            UpdateProgress?.Invoke(this, new ProgressEventArgs((1.0f * m_position) / m_length, message + $" {m_position} of {m_length}" ));
            return n;
        }
        /// <summary>
        /// Returns the next available character but does
        /// not advance the stream
        /// </summary>
        /// <returns></returns>
        public int Peek()
        {
            int n = m_input.ReadByte();
            m_input.Position -= 1;
            return n;
        }

        public string ReadLine()
        {
            StringBuilder sb = new StringBuilder();

            int symbol = Peek();
            while (symbol != -1 && symbol != 13 && symbol != 10)
            {
                symbol = m_input.ReadByte();
                m_position = m_input.Position;
                sb.Append((char)symbol);
            }
            m_input.ReadByte(); // Account for newline character that was skipped in the above while loop
            m_position = m_input.Position;
            UpdateProgress?.Invoke(this, new ProgressEventArgs((1.0f * m_position) / m_length, message + $" {m_position} of {m_length}"));
            string line = sb.ToString();
            sb.Clear();

            return line;
        }
        public override void Write(byte[] buffer, int offset, int count)
        {
            throw new System.NotImplementedException();
        }

        public void WriteHashCodes(byte[] buffer, int offset, int count)
        {
            throw new System.NotImplementedException();
        }

        public override bool CanRead => true;
        public override bool CanSeek => false;
        public override bool CanWrite => false;
        public override long Length => m_length;
        public override long Position
        {
            get { return m_position; }
            set { throw new System.NotImplementedException(); }
        }
    }
}
