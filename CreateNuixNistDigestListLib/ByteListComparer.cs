using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CreateNuixNistDigestList
{
    public class ByteListComparer : IComparer<IList<byte>>
    {
        public int Compare(IList<byte> x, IList<byte> y)
        {
            int result;
            for (int index = 0; index < Math.Min(x.Count, y.Count); index++)
            {
                result = x[index].CompareTo(y[index]);
                if (result != 0) return result;
            }
            return x.Count.CompareTo(y.Count);
        }
    }
}
