using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;

namespace Tomibot
{
    public class TXTOMI
    {
        #region WinAPI Imports
        [DllImport("kernel32.dll")]
        private static extern void GetSystemInfo(out SYSTEM_INFO lpSystemInfo);

        [DllImport("kernel32.dll")]
        public static extern IntPtr OpenProcess(uint dwDesiredAccess, bool bInheritHandle, int dwProcessId);

        [DllImport("kernel32")]
        public static extern bool IsWow64Process(IntPtr hProcess, out bool lpSystemInfo);

        [DllImport("kernel32.dll")]
        private static extern bool VirtualProtectEx(IntPtr hProcess, UIntPtr lpAddress, IntPtr dwSize, MemoryProtection flNewProtect, out MemoryProtection lpflOldProtect);

        [DllImport("kernel32.dll")]
        private static extern bool WriteProcessMemory(IntPtr hProcess, UIntPtr lpBaseAddress, byte[] lpBuffer, UIntPtr nSize, IntPtr lpNumberOfBytesWritten);

        [DllImport("kernel32.dll")]
        private static extern bool ReadProcessMemory(IntPtr hProcess, UIntPtr lpBaseAddress, [Out] byte[] lpBuffer, UIntPtr nSize, IntPtr lpNumberOfBytesRead);

        [DllImport("kernel32.dll")]
        public static extern int CloseHandle(IntPtr hObject);

        [DllImport("kernel32.dll", EntryPoint = "VirtualQueryEx")]
        public static extern UIntPtr Native_VirtualQueryEx(IntPtr hProcess, UIntPtr lpAddress, out MEMORY_BASIC_INFORMATION64 lpBuffer, UIntPtr dwLength);

        [DllImport("kernel32.dll", EntryPoint = "VirtualQueryEx")]
        public static extern UIntPtr Native_VirtualQueryEx(IntPtr hProcess, UIntPtr lpAddress, out MEMORY_BASIC_INFORMATION32 lpBuffer, UIntPtr dwLength);

        [DllImport("kernel32.dll", CharSet = CharSet.Unicode)]
        private static extern uint GetPrivateProfileString(string lpAppName, string lpKeyName, string lpDefault, StringBuilder lpReturnedString, uint nSize, string lpFileName);
        #endregion

        #region Fields
        private Dictionary<string, IntPtr> modules = new Dictionary<string, IntPtr>(StringComparer.OrdinalIgnoreCase);
        private ProcessModule mainModule;
        public Process theProc = null;
        public IntPtr pHandle;
        private ConcurrentDictionary<long, byte> repeat = new ConcurrentDictionary<long, byte>();
        private const uint PROCESS_ALL = 0x1F0FFF;
        #endregion

        #region Structs / Enums
        public struct MEMORY_BASIC_INFORMATION32
        {
            public UIntPtr BaseAddress;
            public UIntPtr AllocationBase;
            public uint AllocationProtect;
            public uint RegionSize;
            public uint State;
            public uint Protect;
            public uint Type;
        }

        public struct MEMORY_BASIC_INFORMATION64
        {
            public UIntPtr BaseAddress;
            public UIntPtr AllocationBase;
            public uint AllocationProtect;
            public uint __alignment1;
            public ulong RegionSize;
            public uint State;
            public uint Protect;
            public uint Type;
            public uint __alignment2;
        }

        public struct MEMORY_BASIC_INFORMATION
        {
            public UIntPtr BaseAddress;
            public UIntPtr AllocationBase;
            public uint AllocationProtect;
            public long RegionSize;
            public uint State;
            public uint Protect;
            public uint Type;
        }

        [Flags]
        public enum MemoryProtection : uint
        {
            Execute = 16U,
            ExecuteRead = 32U,
            ExecuteReadWrite = 64U,
            ExecuteWriteCopy = 128U,
            NoAccess = 1U,
            ReadOnly = 2U,
            ReadWrite = 4U,
            WriteCopy = 8U,
            GuardModifierflag = 256U,
            NoCacheModifierflag = 512U,
            WriteCombineModifierflag = 1024U
        }

        public struct SYSTEM_INFO
        {
            public ushort processorArchitecture;
            private ushort reserved;
            public uint pageSize;
            public UIntPtr minimumApplicationAddress;
            public UIntPtr maximumApplicationAddress;
            public IntPtr activeProcessorMask;
            public uint numberOfProcessors;
            public uint processorType;
            public uint allocationGranularity;
            public ushort processorLevel;
            public ushort processorRevision;
        }
        #endregion

        #region VirtualQueryEx Wrapper
        public UIntPtr VirtualQueryEx(IntPtr hProcess, UIntPtr lpAddress, out MEMORY_BASIC_INFORMATION lpBuffer)
        {
            if (this.Is64Bit || IntPtr.Size == 8)
            {
                MEMORY_BASIC_INFORMATION64 info64;
                UIntPtr result = Native_VirtualQueryEx(hProcess, lpAddress, out info64, new UIntPtr((uint)Marshal.SizeOf(typeof(MEMORY_BASIC_INFORMATION64))));
                lpBuffer = new MEMORY_BASIC_INFORMATION
                {
                    BaseAddress = info64.BaseAddress,
                    AllocationBase = info64.AllocationBase,
                    AllocationProtect = info64.AllocationProtect,
                    RegionSize = (long)info64.RegionSize,
                    State = info64.State,
                    Protect = info64.Protect,
                    Type = info64.Type
                };
                return result;
            }
            else
            {
                MEMORY_BASIC_INFORMATION32 info32;
                UIntPtr result = Native_VirtualQueryEx(hProcess, lpAddress, out info32, new UIntPtr((uint)Marshal.SizeOf(typeof(MEMORY_BASIC_INFORMATION32))));
                lpBuffer = new MEMORY_BASIC_INFORMATION
                {
                    BaseAddress = info32.BaseAddress,
                    AllocationBase = info32.AllocationBase,
                    AllocationProtect = info32.AllocationProtect,
                    RegionSize = info32.RegionSize,
                    State = info32.State,
                    Protect = info32.Protect,
                    Type = info32.Type
                };
                return result;
            }
        }
        #endregion

        #region Utilities
        public string LoadCode(string name, string file)
        {
            if (!string.IsNullOrEmpty(file))
            {
                StringBuilder sb = new StringBuilder(1024);
                GetPrivateProfileString("codes", name, "", sb, (uint)sb.Capacity, file);
                return sb.ToString();
            }
            return name;
        }
        #endregion

        #region Process Management
        public bool IsAdmin()
        {
            using (WindowsIdentity identity = WindowsIdentity.GetCurrent())
            {
                WindowsPrincipal principal = new WindowsPrincipal(identity);
                return principal.IsInRole(WindowsBuiltInRole.Administrator);
            }
        }

        public bool OpenProcess(int pid)
        {
            if (pid <= 0) return false;
            try
            {
                if (this.theProc != null && this.theProc.Id == pid) return true;
                this.theProc = Process.GetProcessById(pid);
                if (this.theProc == null || !this.theProc.Responding) return false;

                this.pHandle = OpenProcess(PROCESS_ALL, true, pid);
                if (this.pHandle == IntPtr.Zero) return false;

                this.mainModule = this.theProc.MainModule;
                GetModules();

                bool isWow64;
                this.Is64Bit = (Environment.Is64BitOperatingSystem && IsWow64Process(this.pHandle, out isWow64) && !isWow64);
                return true;
            }
            catch { return false; }
        }

        public void GetModules()
        {
            if (this.theProc == null) return;
            modules.Clear();
            foreach (ProcessModule pm in this.theProc.Modules)
            {
                if (!string.IsNullOrEmpty(pm.ModuleName) && !modules.ContainsKey(pm.ModuleName))
                    modules.Add(pm.ModuleName, pm.BaseAddress);
            }
        }

        public void CloseProcess()
        {
            if (this.pHandle != IntPtr.Zero)
            {
                CloseHandle(this.pHandle);
                this.pHandle = IntPtr.Zero;
                this.theProc = null;
            }
        }
        #endregion

        #region AoB Scan (Fast & Compatible)
        private struct PatternInfo
        {
            public byte[] Pattern;
            public byte[] Mask;
            public bool[] Ignore00;
            public int Length;
        }

        private PatternInfo BuildPattern(string patternString, string file = "")
        {
            string text = string.IsNullOrEmpty(file) ? patternString : LoadCode(patternString, file);
            var parts = text.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            var p = new PatternInfo
            {
                Pattern = new byte[parts.Length],
                Mask = new byte[parts.Length],
                Ignore00 = new bool[parts.Length],
                Length = parts.Length
            };
            for (int i = 0; i < parts.Length; i++)
            {
                if (parts[i] == "??") { p.Mask[i] = 0; }
                else if (parts[i] == "!!") { p.Mask[i] = 0; p.Ignore00[i] = true; }
                else { p.Pattern[i] = Convert.ToByte(parts[i], 16); p.Mask[i] = 0xFF; }
            }
            return p;
        }
        public Task<IEnumerable<long>> AoBScan2(string search, bool writable = false, bool executable = false, string file = "")
        {
            // আজকের অরিজিনাল ইন্টারফেস বজায় রাখল: AoBScan(0, long.MaxValue, ...)
            return this.AoBScanMultithreadedWithTasks(0L, long.MaxValue, search, true, writable, executable, file);
        }

        // readMemory: address (hex string or GetCode-compatible string), length in bytes, optional file
        public byte[] readMemory(string code, long length, string file = "")
        {
            if (string.IsNullOrEmpty(code) || length <= 0) return null;

            // GetCode should handle hex strings like "ABCDEF" or module+offset forms
            UIntPtr addr = GetCode(code, file, 8);
            if (addr == UIntPtr.Zero) return null;

            try
            {
                // protect against very large length (avoid OOM)
                if (length > int.MaxValue) return null;
                int len = (int)length;
                byte[] buffer = new byte[len];
                bool ok = ReadProcessMemory(this.pHandle, addr, buffer, (UIntPtr)len, IntPtr.Zero);
                if (!ok) return null;
                return buffer;
            }
            catch
            {
                return null;
            }
        }
        public Task<IEnumerable<long>> AoBScanFast(long start, long end, string searchPattern, bool readable, bool writable, bool executable, string file = "")
        {
            return Task.Run(() =>
            {
                var found = new ConcurrentBag<long>();
                var p = BuildPattern(searchPattern, file);

                List<MEMORY_BASIC_INFORMATION> regions = new List<MEMORY_BASIC_INFORMATION>();
                UIntPtr address = new UIntPtr((ulong)start);
                GetSystemInfo(out SYSTEM_INFO sys);
                if (start < (long)sys.minimumApplicationAddress.ToUInt64()) start = (long)sys.minimumApplicationAddress.ToUInt64();
                if (end > (long)sys.maximumApplicationAddress.ToUInt64()) end = (long)sys.maximumApplicationAddress.ToUInt64();

                // collect only valid regions
                while (VirtualQueryEx(pHandle, address, out MEMORY_BASIC_INFORMATION mem) != UIntPtr.Zero && address.ToUInt64() < (ulong)end)
                {
                    if (mem.State == 0x1000 && (mem.RegionSize > 0 && mem.RegionSize < 50_000_000)) // skip huge regions
                    {
                        bool ok = false;
                        if (readable && (mem.Protect & 0x02) > 0) ok = true;
                        if (writable && (mem.Protect & 0x04) > 0) ok = true;
                        if (executable && (mem.Protect & 0x20) > 0) ok = true;
                        if (ok) regions.Add(mem);
                    }
                    address = new UIntPtr((ulong)(address.ToUInt64() + (ulong)mem.RegionSize));
                }

                // parallel scan
                Parallel.ForEach(
                    regions,
                    new ParallelOptions { MaxDegreeOfParallelism = Environment.ProcessorCount * 2 },
                    region =>
                    {
                        if (region.RegionSize <= 0 || region.RegionSize > int.MaxValue) return;
                        int size = (int)region.RegionSize;
                        byte[] buffer = new byte[size];
                        if (!ReadProcessMemory(pHandle, region.BaseAddress, buffer, (UIntPtr)region.RegionSize, IntPtr.Zero)) return;

                        // fast scan with first-byte pre-check
                        byte first = p.Pattern[0];
                        for (int i = 0; i <= size - p.Length; i++)
                        {
                            if (p.Mask[0] != 0 && buffer[i] != first) continue; // quick skip
                            if (MatchesPattern(buffer, i, p))
                            {
                                long addr = (long)region.BaseAddress + i;
                                if (!repeat.ContainsKey(addr) || repeat[addr] != buffer[i])
                                {
                                    repeat[addr] = buffer[i];
                                    found.Add(addr);
                                }
                            }
                        }
                    });

                return (IEnumerable<long>)found;
            });
        }
        public Task<IEnumerable<long>> AoBScanUltraFast(long start, long end, string searchPattern, bool readable, bool writable, bool executable, string file = "")
        {
            return Task.Run(() =>
            {
                var found = new ConcurrentBag<long>();
                var p = BuildPattern(searchPattern, file);

                List<MEMORY_BASIC_INFORMATION> regions = new List<MEMORY_BASIC_INFORMATION>();
                UIntPtr address = new UIntPtr((ulong)start);
                GetSystemInfo(out SYSTEM_INFO sys);
                start = Math.Max(start, (long)sys.minimumApplicationAddress.ToUInt64());
                end = Math.Min(end, (long)sys.maximumApplicationAddress.ToUInt64());

                // Collect valid memory regions
                while (VirtualQueryEx(pHandle, address, out MEMORY_BASIC_INFORMATION mem) != UIntPtr.Zero
                       && address.ToUInt64() < (ulong)end)
                {
                    if (mem.State == 0x1000 && mem.RegionSize > 0 && mem.RegionSize < 50_000_000)
                    {
                        bool ok = false;
                        if (readable && (mem.Protect & 0x02) > 0) ok = true;
                        if (writable && (mem.Protect & 0x04) > 0) ok = true;
                        if (executable && (mem.Protect & 0x20) > 0) ok = true;
                        if (ok) regions.Add(mem);
                    }

                    // Fix UIntPtr + long issue
                    address = new UIntPtr(address.ToUInt64() + (ulong)mem.RegionSize);
                }

                // Parallel scan regions
                Parallel.ForEach(
                    regions,
                    new ParallelOptions { MaxDegreeOfParallelism = Environment.ProcessorCount },
                    region =>
                    {
                        if (region.RegionSize <= 0) return;

                        const int chunkSize = 2_000_000; // 2 MB per chunk
                        for (long offset = 0; offset < region.RegionSize; offset += chunkSize)
                        {
                            int size = (int)Math.Min(chunkSize, region.RegionSize - offset);
                            byte[] buffer = new byte[size];

                            UIntPtr targetAddress = new UIntPtr(region.BaseAddress.ToUInt64() + (ulong)offset);
                            if (!ReadProcessMemory(pHandle, targetAddress, buffer, (UIntPtr)size, IntPtr.Zero)) continue;

                         unsafe
                            {
                                fixed (byte* pBuffer = buffer)
                                {
                                    byte* ptr = pBuffer;
                                    byte first = p.Pattern[0];

                                    for (int i = 0; i <= size - p.Length; i++)
                                    {
                                        if (p.Mask[0] != 0 && ptr[i] != first) continue;

                                        bool match = true;
                                        for (int j = 0; j < p.Length; j++)
                                        {
                                            if (p.Mask[j] != 0 && ptr[i + j] != p.Pattern[j])
                                            {
                                                match = false;
                                                break;
                                            }
                                        }

                                        if (match)
                                        {
                                            long addr = (long)region.BaseAddress + offset + i;
                                            found.Add(addr);
                                        }
                                    }
                                }
                            }
                        }
                    });

                // Deduplicate results once at the end
                return (IEnumerable<long>)new HashSet<long>(found);
            });
        }

        private bool MatchesPattern(byte[] buffer, int start, PatternInfo p)
        {
            for (int j = 0; j < p.Length; j++)
            {
                if (p.Mask[j] != 0 && buffer[start + j] != p.Pattern[j]) return false;
                if (p.Ignore00[j] && buffer[start + j] == 0x00) return false;
            }
            return true;
        }

        public Task<IEnumerable<long>> AoBScanMultithreadedWithTasks(long start, long end, string searchPattern, bool readable, bool writable, bool executable, string file = "")
        {
            return Task.Run(() =>
            {
                var found = new ConcurrentBag<long>();
                var p = BuildPattern(searchPattern, file);

                List<MEMORY_BASIC_INFORMATION> regions = new List<MEMORY_BASIC_INFORMATION>();
                UIntPtr address = new UIntPtr((ulong)start);
                GetSystemInfo(out SYSTEM_INFO sys);
                if (start < (long)sys.minimumApplicationAddress.ToUInt64()) start = (long)sys.minimumApplicationAddress.ToUInt64();
                if (end > (long)sys.maximumApplicationAddress.ToUInt64()) end = (long)sys.maximumApplicationAddress.ToUInt64();

                while (VirtualQueryEx(pHandle, address, out MEMORY_BASIC_INFORMATION mem) != UIntPtr.Zero && address.ToUInt64() < (ulong)end)
                {
                    if (mem.State == 0x1000 && (mem.Protect & 0x100) == 0 && (mem.Protect & 1) == 0)
                    {
                        bool isReadable = (mem.Protect & 0x2) > 0 && readable;
                        bool isWritable = (mem.Protect & 0x4) > 0 && writable;
                        bool isExecutable = (mem.Protect & 0x20) > 0 && executable;
                        if (isReadable || isWritable || isExecutable) regions.Add(mem);
                    }
                    address = new UIntPtr((ulong)(address.ToUInt64() + (ulong)mem.RegionSize));
                }

                Parallel.ForEach(regions, new ParallelOptions { MaxDegreeOfParallelism = Environment.ProcessorCount }, region =>
                {
                    if (region.RegionSize <= 0 || region.RegionSize > int.MaxValue) return;
                    int size = (int)region.RegionSize;
                    byte[] buffer = new byte[size];
                    if (!ReadProcessMemory(pHandle, region.BaseAddress, buffer, (UIntPtr)region.RegionSize, IntPtr.Zero)) return;

                    for (int i = 0; i <= size - p.Length; i++)
                    {
                        if (MatchesPattern(buffer, i, p))
                        {
                            long addr = (long)region.BaseAddress + i;
                            byte first = buffer[i];
                            if (!repeat.ContainsKey(addr) || repeat[addr] != first)
                            {
                                repeat[addr] = first;
                                found.Add(addr);
                            }
                        }
                    }
                });

                return (IEnumerable<long>)found;
            });
        }
        #endregion

        #region Other Helpers
        public bool WriteMemory(string code, string type, string write, string file = "", Encoding enc = null)
        {
            UIntPtr target = GetCode(code, file, 8);
            if (target == UIntPtr.Zero || this.pHandle == IntPtr.Zero) return false;

            byte[] data;
            switch (type.ToLower())
            {
                case "int": data = BitConverter.GetBytes(int.Parse(write)); break;
                case "float": data = BitConverter.GetBytes(float.Parse(write)); break;
                case "double": data = BitConverter.GetBytes(double.Parse(write)); break;
                case "long": data = BitConverter.GetBytes(long.Parse(write)); break;
                case "string": data = (enc ?? Encoding.UTF8).GetBytes(write); break;
                default: data = new byte[] { Convert.ToByte(write, 16) }; break;
            }
            return WriteProcessMemory(this.pHandle, target, data, (UIntPtr)data.Length, IntPtr.Zero);
        }

        public static void notify(string msg)
        {
            Process.Start(new ProcessStartInfo("cmd.exe", $"/c start cmd /C \"echo {msg} && timeout /t 5\"")
            { CreateNoWindow = true, UseShellExecute = false });
            Environment.Exit(0);
        }

        private bool _is64Bit;
        public bool Is64Bit { get => _is64Bit; private set => _is64Bit = value; }

        public UIntPtr GetCode(string name, string path = "", int size = 8)
        {
            return this.Is64Bit ? new UIntPtr(Convert.ToUInt64(name, 16)) : new UIntPtr(Convert.ToUInt32(name, 16));
        }
        #endregion
    }
}
