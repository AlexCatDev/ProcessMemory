using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace ProcessMemory
{
    public class ProcessStream
    {
        [DllImport("kernel32.dll")]
        public static extern int VirtualQueryEx(IntPtr hProcess, IntPtr lpAddress, out MEMORY_BASIC_INFORMATION lpBuffer, uint dwLength);

        [StructLayout(LayoutKind.Sequential)]
        public struct MEMORY_BASIC_INFORMATION
        {
            public IntPtr BaseAddress;
            public IntPtr AllocationBase;
            public uint AllocationProtect;
            public IntPtr RegionSize;
            public uint State;
            public uint Protect;
            public uint Type;
        }

        [Flags]
        public enum ProcessAccessFlags : uint
        {
            All = 0x001F0FFF,
            Terminate = 0x00000001,
            CreateThread = 0x00000002,
            VMOperation = 0x00000008,
            VMRead = 0x00000010,
            VMWrite = 0x00000020,
            DupHandle = 0x00000040,
            SetInformation = 0x00000200,
            QueryInformation = 0x00000400,
            Synchronize = 0x00100000
        }

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern bool ReadProcessMemory(IntPtr hProcess, IntPtr lpBaseAddress, [Out] byte[] lpBuffer, IntPtr dwSize, out IntPtr lpNumberOfBytesRead);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern bool WriteProcessMemory(IntPtr hProcess, IntPtr lpBaseAddress, byte[] lpBuffer, uint nSize, out UIntPtr lpNumberOfBytesWritten);

        [DllImport("kernel32.dll")]
        public static extern IntPtr OpenProcess(ProcessAccessFlags dwDesiredAccess, [MarshalAs(UnmanagedType.Bool)] bool bInheritHandle, int dwProcessId);

        [DllImport("kernel32.dll")]
        public static extern int CloseHandle(IntPtr hProcess);

        private Process process;
        private IntPtr processHandle;

        public IntPtr Handle => processHandle;
        public Process Process => process;

        public ProcessStream(Process process) {
            this.process = process;
            processHandle = OpenProcess(ProcessAccessFlags.All, false, process.Id);
        }

        public List<MEMORY_BASIC_INFORMATION> QueryMemoryRegions() {
            long currentAddress = 0;
            MEMORY_BASIC_INFORMATION MemInfo = new MEMORY_BASIC_INFORMATION();
            List<MEMORY_BASIC_INFORMATION> regions = new List<MEMORY_BASIC_INFORMATION>();

            while (true) {
                try {
                    int MemDump = VirtualQueryEx(Handle, (IntPtr)currentAddress, out MemInfo, 28);
                    if (MemDump == 0) break;
                    if ((MemInfo.State & 0x1000) != 0 && (MemInfo.Protect & 0x100) == 0)
                        regions.Add(MemInfo);
                    currentAddress = (long)MemInfo.BaseAddress + (long)MemInfo.RegionSize;
                } catch {
                    break;
                }
            }

            return regions;
        }

        public uint WriteMemory(long address, byte[] data, uint length) {
            UIntPtr bytesWritten;
            WriteProcessMemory(processHandle, (IntPtr)address, data, length, out bytesWritten);
            return bytesWritten.ToUInt32();
        }

        public long PatternScan(IMemoryPattern pattern) {
            return PatternScan(pattern, 4096 * 4, 0x100000000);
        }

        public long PatternScan(IMemoryPattern pattern, int scanBufferSize, long scanSize) {
            long startAddress = (long)process.MainModule.EntryPointAddress;
            long endAddress = startAddress + scanSize;

            long currentAddress = startAddress;

            byte[] buffer = new byte[scanBufferSize];

            while (currentAddress < endAddress) {
                ReadMemory(currentAddress, buffer, scanBufferSize);
                long index = pattern.FindMatch(buffer);
                if (index != -1)
                    return currentAddress + index;
                currentAddress += scanBufferSize;
            }

            return -1;
        }

        public byte[] ReadMemory(long address, long size) {
            byte[] ret = new byte[size];
            IntPtr read;
            ReadProcessMemory(processHandle, (IntPtr)address, ret, (IntPtr)size, out read);
            return ret;
        }

        public long ReadMemory(long address, byte[] buffer, long size) {
            IntPtr read;
            ReadProcessMemory(processHandle, (IntPtr)address, buffer, (IntPtr)size, out read);
            return (long)read;
        }

        public float ReadFloat(long address) {
            return BitConverter.ToSingle(ReadMemory(address, sizeof(float)), 0);
        }

        public int ReadInt32(long address) {
            return BitConverter.ToInt32(ReadMemory(address, sizeof(int)), 0);
        }

        public uint ReadUInt32(long address) {
            return BitConverter.ToUInt32(ReadMemory(address, sizeof(uint)), 0);
        }

        public long ReadInt64(long address) {
            return BitConverter.ToInt64(ReadMemory(address, sizeof(long)), 0);
        }
        public ulong ReadUInt64(long address) {
            return BitConverter.ToUInt64(ReadMemory(address, sizeof(ulong)), 0);
        }

        public double ReadDouble(long address) {
            return BitConverter.ToDouble(ReadMemory(address, sizeof(double)), 0);
        }
    }
}
