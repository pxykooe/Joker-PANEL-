using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Joker99X;
using Tomibot; 

namespace Joker_PANEL___
{
    public partial class Form2 : Form
    {
        private static TXTOMI TomiBot = new TXTOMI();
        string AimbotScan = ("FF FF 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 FF FF FF FF FF FF FF FF 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? 00 00 00 00 00 00 00 00 00 00 00 00 A5 43");
        string headoffset = ("0xAA");
        string chestoffset = ("0xA6");

        private Dictionary<long, int> OrginalValues1 = new Dictionary<long, int>();
        private Dictionary<long, int> OrginalValues2 = new Dictionary<long, int>();
        private Dictionary<long, int> OrginalValues3 = new Dictionary<long, int>();
        private Dictionary<long, int> OrginalValues4 = new Dictionary<long, int>();

        public Form2()
        {
            InitializeComponent();
        }

        private void label3_Click(object sender, EventArgs e)
        {

        }

        private async void guna2ToggleSwitch1_CheckedChanged(object sender, EventArgs e)
        {
            Process[] hdPlayers = Process.GetProcessesByName("HD-Player");

            if (hdPlayers.Length == 0)
            {
                sts.Text = "HD Player Not Running";
                sts.ForeColor = Color.Red;
                Console.Beep(1000, 400);
                return; // HD Player না চললে ফাংশন থেকে বের হয়ে যান
            }

            OrginalValues1.Clear();
            OrginalValues2.Clear();
            OrginalValues3.Clear();
            OrginalValues4.Clear();
            sts.Text = "Applying...";
            sts.ForeColor = Color.Yellow;


            Stopwatch sw = Stopwatch.StartNew();

            Int64 readoffset = Convert.ToInt64(headoffset, 16);
            Int64 writeoffset = Convert.ToInt64(chestoffset, 16);


            int proc = Process.GetProcessesByName("HD-Player")[0].Id;


            TomiBot.OpenProcess(proc);


            var result = await TomiBot.AoBScanFast(0, long.MaxValue, AimbotScan, true, true, false);
            int foundCount = result.Count();

            if (foundCount != 0)
            {
                foreach (var CurrentAddress in result)
                {
                    Int64 addressToSave = CurrentAddress + writeoffset;
                    var currentBytes = TomiBot.readMemory(addressToSave.ToString("X"), sizeof(int));
                    int currentValue = BitConverter.ToInt32(currentBytes, 0);
                    OrginalValues1[addressToSave] = currentValue;

                    Int64 addressToSave9 = CurrentAddress + readoffset;
                    var currentBytes9 = TomiBot.readMemory(addressToSave9.ToString("X"), sizeof(int));
                    int currentValue9 = BitConverter.ToInt32(currentBytes9, 0);
                    OrginalValues2[addressToSave9] = currentValue9;

                    Int64 headbytes = CurrentAddress + readoffset;
                    Int64 chestbytes = CurrentAddress + writeoffset;
                    var bytes = TomiBot.readMemory(headbytes.ToString("X"), sizeof(int));
                    int Read = BitConverter.ToInt32(bytes, 0);
                    var bytes2 = TomiBot.readMemory(chestbytes.ToString("X"), sizeof(int));
                    int Read2 = BitConverter.ToInt32(bytes2, 0);

                    TomiBot.WriteMemory(chestbytes.ToString("X"), "int", Read.ToString());
                    TomiBot.WriteMemory(headbytes.ToString("X"), "int", Read2.ToString());

                    Int64 addressToSave1 = CurrentAddress + writeoffset;
                    var currentBytes1 = TomiBot.readMemory(addressToSave9.ToString("X"), sizeof(int));
                    int currentValue1 = BitConverter.ToInt32(currentBytes1, 0);
                    OrginalValues3[addressToSave1] = currentValue1;

                    Int64 addressToSave19 = CurrentAddress + readoffset;
                    var currentBytes19 = TomiBot.readMemory(addressToSave19.ToString("X"), sizeof(int));
                    int currentValue19 = BitConverter.ToInt32(currentBytes19, 0);
                    OrginalValues4[addressToSave19] = currentValue19;
                }

                sw.Stop();
                double elapsedSec = sw.Elapsed.TotalSeconds;
                sts.Text = $"Aimbot Active  ({foundCount} found, {elapsedSec:F2} sec)";
                sts.ForeColor = Color.Green;
                Console.Beep(2000, 400);
            }
            else
            {
                sw.Stop();
                double elapsedSec = sw.Elapsed.TotalSeconds;
                sts.Text = $"Aimbot Faild  ({foundCount} found, {elapsedSec:F2} sec)";
                sts.ForeColor = Color.Red;
                Console.Beep(1000, 400);
            }
        }
        [DllImport("kernel32.dll", SetLastError = true)]
        static extern IntPtr OpenProcess(uint processAccess, bool bInheritHandle, int processId);

        [DllImport("kernel32.dll", SetLastError = true)]
        static extern IntPtr GetProcAddress(IntPtr hModule, string lpProcName);

        [DllImport("kernel32.dll", SetLastError = true)]
        static extern IntPtr GetModuleHandle(string lpModuleName);

        [DllImport("kernel32.dll", SetLastError = true)]
        static extern IntPtr VirtualAllocEx(IntPtr hProcess, IntPtr lpAddress, IntPtr dwSize, uint flAllocationType, uint flProtect);

        [DllImport("kernel32.dll", SetLastError = true)]
        static extern bool WriteProcessMemory(IntPtr hProcess, IntPtr lpBaseAddress, byte[] lpBuffer, uint nSize, out IntPtr lpNumberOfBytesWritten);

        [DllImport("kernel32.dll")]
        static extern IntPtr CreateRemoteThread(IntPtr hProcess, IntPtr lpThreadAttribute, IntPtr dwStackSize, IntPtr lpStartAddress, IntPtr lpParameter, uint dwCreationFlags, IntPtr lpThreadId);

        const uint PROCESS_CREATE_THREAD = 0x2;
        const uint PROCESS_QUERY_INFORMATION = 0x400;
        const uint PROCESS_VM_OPERATION = 0x8;
        const uint PROCESS_VM_WRITE = 0x20;
        const uint PROCESS_VM_READ = 0x10;

        const uint MEM_COMMIT = 0x1000;
        const uint PAGE_READWRITE = 4;
        private static void ExtractEmbeddedResource(string resourceName, string outputPath)
        {
            Assembly executingAssembly = Assembly.GetExecutingAssembly();

            // Get the embedded resource stream
            using (Stream resourceStream = executingAssembly.GetManifestResourceStream(resourceName))
            {
                if (resourceStream == null)
                {
                    throw new ArgumentException($"Resource '{resourceName}' not found.");
                }

                // Read the embedded resource and save it to the specified path
                using (FileStream fileStream = new FileStream(outputPath, FileMode.Create))
                {
                    byte[] buffer = new byte[resourceStream.Length];
                    resourceStream.Read(buffer, 0, buffer.Length);
                    fileStream.Write(buffer, 0, buffer.Length);
                }
            }
        }

        private void guna2ToggleSwitch2_CheckedChanged(object sender, EventArgs e)
        {
            string processName = "HD-Player"; // Specify your target process name
            string dllResourceName = "Joker99X.Joker.dll"; // Correct resource name

            // Extract the embedded msdrmi.dll to a temporary file
            string tempDllPath = Path.Combine(Path.GetTempPath(), "Joker.dll");
            ExtractEmbeddedResource(dllResourceName, tempDllPath);

            Console.WriteLine($"DLL extracted successfully to: {tempDllPath}");


            Process[] targetProcesses = Process.GetProcessesByName(processName);
            if (targetProcesses.Length == 0)
            {
                Console.WriteLine($"Waiting for {processName}.exe...");
            }
            else
            {
                Process targetProcess = targetProcesses[0];
                IntPtr hProcess = OpenProcess(PROCESS_CREATE_THREAD | PROCESS_QUERY_INFORMATION | PROCESS_VM_OPERATION | PROCESS_VM_WRITE | PROCESS_VM_READ, false, targetProcess.Id);

                IntPtr loadLibraryAddr = GetProcAddress(GetModuleHandle("kernel32.dll"), "LoadLibraryA");
                IntPtr allocMemAddress = VirtualAllocEx(hProcess, IntPtr.Zero, (IntPtr)tempDllPath.Length, MEM_COMMIT, PAGE_READWRITE);

                IntPtr bytesWritten;
                WriteProcessMemory(hProcess, allocMemAddress, System.Text.Encoding.ASCII.GetBytes(tempDllPath), (uint)tempDllPath.Length, out bytesWritten);

                CreateRemoteThread(hProcess, IntPtr.Zero, IntPtr.Zero, loadLibraryAddr, allocMemAddress, 0, IntPtr.Zero);

                Console.Beep(240, 300);
                //Type Here Chams Is Already Injected or code invaible

            }
        }

        private void guna2ToggleSwitch3_CheckedChanged(object sender, EventArgs e)
        {
            Bypass F2 = new Bypass();
            F2.Show();
        }
    }
}