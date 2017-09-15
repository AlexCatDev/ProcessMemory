using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ProcessMemory;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace Test
{
    class Program
    {
       
        static void Main(string[] args) {
            /*
            ProcessStream m = new ProcessStream(Process.GetProcessesByName("mouserate").First());
            long offset = 0;
            MEMORY_BASIC_INFORMATION i = new MEMORY_BASIC_INFORMATION();
            long maxAddress = 0x7fffffff;
            while ((long)m.Process.MainModule.BaseAddress + offset <= maxAddress) {
                int dw = VirtualQueryEx(m.Handle, (IntPtr)((long)m.Process.MainModule.BaseAddress + offset), out i, (uint)Marshal.SizeOf(i));
                offset += (long)i.RegionSize;
                AllocationProtect p = (AllocationProtect)i.AllocationProtect;
                Console.WriteLine(i.RegionSize + " : " + p);
            }
            */

            ProcessStream s = new ProcessStream(Process.GetProcessesByName("spotify").First());
            HexPattern p = new HexPattern("?? ?? 0x32 ?? 0x43");
            long address = s.PatternScan(p);
            Console.WriteLine(address);
        Console.WriteLine("Done");
            Console.ReadLine();
        }
    }
}
