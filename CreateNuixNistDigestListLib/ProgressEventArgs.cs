using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CreateNuixNistDigestList
{
    public class ProgressEventArgs : EventArgs
    {
        private float m_progress;
        private string _message;

        public ProgressEventArgs(string message)
        {
            m_progress = 0;
            _message = message;
        }
        public ProgressEventArgs(float progress, string message="")
        {
            m_progress = progress;
            _message = message;
        }

        public float Progress => m_progress;
        public string Message => _message;
    }
}
