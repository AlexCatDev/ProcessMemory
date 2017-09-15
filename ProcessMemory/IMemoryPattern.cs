using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProcessMemory
{
    public interface IMemoryPattern
    {
        /// <summary>
        /// The function to find a match for a pattern
        /// </summary>
        /// <param name="source">The byte array to search in.</param>
        /// <returns>Returns the start index of the found pattern if pattern isn't find it should return -1.</returns>
        long FindMatch(byte[] source, long length);
        long Length { get; }
    }
}
