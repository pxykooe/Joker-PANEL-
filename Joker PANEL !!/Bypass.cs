using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Memory;

namespace Joker99X
{
    public partial class Bypass : Form
    {
        Mem memory = new Mem();

        public Bypass()
        {
            InitializeComponent();
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }


        private async void guna2GradientButton1_Click(object sender, EventArgs e)
        {
            if (Process.GetProcessesByName("HD-Player").Length == 0)
            {
                //Type Here Emulator not found Status
                Console.Beep(240, 300);
            }
            else
            {
                //Type Here Waiting Status

                string search = "01 00 A0 E3 1C 00 85 E5 00 70 94 E5 C0 50 96 E5 00 00 57 E3 01 00 00 1A 00 00 A0 E3 57 7D CD EB FC 50 87 E5 00 70 94 E5 C4 50 96 E5 00 00 57 E3 01 00 00 1A 00 00 A0 E3 50 7D CD EB 00 51 87 E5 00 70 94 E5 C8 50 96 E5 00 00 57 E3 01 00 00 1A 00 00 A0 E3 49 7D CD EB 98 03 9F E5 30 51 87 E5 00 00 9F E7 00 00 90 E5 BF 10 D0 E5 02 00 11 E3 06 00 00 0A 70 10 90 E5 00 00 51 E3 03 00 00 1A 72 22 CD EB 70 03 9F E5 00 00 9F E7 00 00 90 E5 5C 00 90 E5 B9 10 01 E3 01 00 D0 E7 00 00 50 E3 49 00 00 0A 54 03 9F E5 00 00 9F E7 00 00 90 E5 BF 10 D0 E5 02 00 11 E3 03 00 00 0A 70 10 90 E5 00 00 51 E3 00 00 00 1A 60 22 CD EB 00 00 A0 E3 7B D6 0A EB 00 50 A0 E1 00 00 55 E3 01 00 00 1A 00 00 A0 E3 25 7D CD EB 05 00 A0 E1 00 10 A0 E3 D3 D6 0A EB 00 50 94 E5 00 70 A0 E1 00 00 55 E3 01 00 00 1A 00 00 A0 E3 1C 7D CD EB 58 50 95 E5 00 00 55 E3 01 00 00 1A 00 00 A0 E3 17 7D CD EB 00 00 95 E5 BA 10 D0 E5 00 00 51 E3 BF 10 D0 15 40 00 11 13 01 00 00 0A B0 95 CC EB 00 00 95 E5 D8 20 90 E5 DC 10 90 E5 05 00 A0 E1 32 FF 2F E1 00 50 A0 E1 A8 02 9F E5 00 00 9F E7 00 00 90 E5 BF 10 D0 E5 02 00 11 E3 03 00 00 0A 70 10 90 E5 00 00 51 E3 00 00 00 1A 34 22 CD EB 07 00 A0 E1 05 10 A0 E1 00 20 A0 E3 C5 CD 0A EB 00 50 A0 E1 70 02 9F E5 00 00 9F E7 00 00 90 E5 BF 10 D0 E5 02 00 11 E3 03 00 00 0A 70 10 90 E5 00 00 51 E3 00 00 00 1A 25 22 CD EB 05 00 A0 E1 00 10 A0 E3 05 D7 0A EB 00 70 94 E5 13 00 00 EA 00 70 94 E5 00 00 57 E3 01 00 00 1A 00 00 A0 E3 E7 7C CD EB 38 00 87 E2 00 10 A0 E3 64 C8 0A EB 00 50 A0 E1 14 02 9F E5 00 00 9F E7 00 00 90 E5 BF 10 D0 E5 02 00 11 E3 03 00 00 0A 70 10 90 E5 00 00 51 E3 00 00 00 1A 0D 22 CD EB 05 00 A0 E1 00 10 A0 E3 F0 D6 0A EB 00 00 57 E3 5D 00 00 0A DC 11 9F E5 54 01 87 E5 00 70 94 E5 01 10 9F E7 00 00 91 E5 BF 10 D0 E5 02 00 11 E3 03 00 00 0A 70 10 90 E5 00 00 51 E3 00 00 00 1A FC 21 CD EB 00 00 A0 E3 3E D6 0A EB 00 50 A0 E1 00 00 55 E3 01 00 00 1A 00 00 A0 E3 C1 7C CD EB 05 00 A0 E1 00 10 A0 E3 4B D6 0A EB 00 50 A0 E1 00 00 57 E3 01 00 00 1A 00 00 A0 E3 B9 7C CD EB 24 51 87 E5 00 70 94 E5 20 50 98 E5 00 00 57 E3 01 00 00 1A 00 00 A0 E3 B2 7C CD EB 28 51 87 E5 00 70 94 E5 24 50 98 E5 00 00 57 E3 01 00 00 1A 00 00 A0 E3 AB 7C CD EB 08 00 A0 E1 2C 51 87 E5 11 64 00 EB 00 50 A0 E1 00 00 55 E3 0E 00 00 1A 24 01 9F E5 00 00 9F E7 00 00 90 E5 BF 10 D0 E5 02 00 11 E3 03 00 00 0A 70 10 90 E5 00 00 51 E3 00 00 00 1A CF 21 CD EB 00 00 A0 E3 B6 D6 0A EB 00 50 A0 E1 00 00 55 E3 05 00 00 0A 00 70 94 E5 00 00 57 E3 01 00 00 1A 00 00 A0 E3 91 7C CD EB 38 51 87 E5 00 70 94 E5 CC 50 96 E5";
                string replace = "0F 00 A0 E1 1C 00 85 E5 00 70 94 E5 C0 50 96 E5 00 00 57 E3 01 00 00 1A 00 00 A0 E3 57 7D CD EB FC 50 87 E5 00 70 94 E5 C4 50 96 E5 00 00 57 E3 01 00 00 1A 00 00 A0 E3 50 7D CD EB 00 51 87 E5 00 70 94 E5 C8 50 96 E5 00 00 57 E3 01 00 00 1A 00 00 A0 E3 49 7D CD EB 98 03 9F E5 30 51 87 E5 00 00 9F E7 00 00 90 E5 BF 10 D0 E5 02 00 11 E3 06 00 00 0A 70 10 90 E5 00 00 51 E3 03 00 00 1A 72 22 CD EB 70 03 9F E5 00 00 9F E7 00 00 90 E5 5C 00 90 E5 B9 10 01 E3 01 00 D0 E7 00 00 50 E3 49 00 00 0A 54 03 9F E5 00 00 9F E7 00 00 90 E5 BF 10 D0 E5 02 00 11 E3 03 00 00 0A 70 10 90 E5 00 00 51 E3 00 00 00 1A 60 22 CD EB 00 00 A0 E3 7B D6 0A EB 00 50 A0 E1 00 00 55 E3 01 00 00 1A 00 00 A0 E3 25 7D CD EB 05 00 A0 E1 00 10 A0 E3 D3 D6 0A EB 00 50 94 E5 00 70 A0 E1 00 00 55 E3 01 00 00 1A 00 00 A0 E3 1C 7D CD EB 58 50 95 E5 00 00 55 E3 01 00 00 1A 00 00 A0 E3 17 7D CD EB 00 00 95 E5 BA 10 D0 E5 00 00 51 E3 BF 10 D0 15 40 00 11 13 01 00 00 0A B0 95 CC EB 00 00 95 E5 D8 20 90 E5 DC 10 90 E5 05 00 A0 E1 32 FF 2F E1 00 50 A0 E1 A8 02 9F E5 00 00 9F E7 00 00 90 E5 BF 10 D0 E5 02 00 11 E3 03 00 00 0A 70 10 90 E5 00 00 51 E3 00 00 00 1A 34 22 CD EB 07 00 A0 E1 05 10 A0 E1 00 20 A0 E3 C5 CD 0A EB 00 50 A0 E1 70 02 9F E5 00 00 9F E7 00 00 90 E5 BF 10 D0 E5 02 00 11 E3 03 00 00 0A 70 10 90 E5 00 00 51 E3 00 00 00 1A 25 22 CD EB 05 00 A0 E1 00 10 A0 E3 05 D7 0A EB 00 70 94 E5 13 00 00 EA 00 70 94 E5 00 00 57 E3 01 00 00 1A 00 00 A0 E3 E7 7C CD EB 38 00 87 E2 00 10 A0 E3 64 C8 0A EB 00 50 A0 E1 14 02 9F E5 00 00 9F E7 00 00 90 E5 BF 10 D0 E5 02 00 11 E3 03 00 00 0A 70 10 90 E5 00 00 51 E3 00 00 00 1A 0D 22 CD EB 05 00 A0 E1 00 10 A0 E3 F0 D6 0A EB 00 00 57 E3 5D 00 00 0A DC 11 9F E5 24 01 87 E5 00 70 94 E5 01 10 9F E7 00 00 91 E5 BF 10 D0 E5 02 00 11 E3 03 00 00 0A 70 10 90 E5 00 00 51 E3 00 00 00 1A FC 21 CD EB 00 00 A0 E3 3E D6 0A EB 00 50 A0 E1 00 00 55 E3 01 00 00 1A 00 00 A0 E3 C1 7C CD EB 05 00 A0 E1 00 10 A0 E3 4B D6 0A EB 00 50 A0 E1 00 00 57 E3 01 00 00 1A 00 00 A0 E3 B9 7C CD EB 24 51 87 E5 00 70 94 E5 20 50 98 E5 00 00 57 E3 01 00 00 1A 00 00 A0 E3 B2 7C CD EB 28 51 87 E5 00 70 94 E5 24 50 98 E5 00 00 57 E3 01 00 00 1A 00 00 A0 E3 AB 7C CD EB 08 00 A0 E1 2C 51 87 E5 11 64 00 EB 00 50 A0 E1 00 00 55 E3 0E 00 00 1A 24 01 9F E5 00 00 9F E7 00 00 90 E5 BF 10 D0 E5 02 00 11 E3 03 00 00 0A 70 10 90 E5 00 00 51 E3 00 00 00 1A CF 21 CD EB 00 00 A0 E3 B6 D6 0A EB 00 50 A0 E1 00 00 55 E3 05 00 00 0A 00 70 94 E5 00 00 57 E3 01 00 00 1A 00 00 A0 E3 91 7C CD EB 38 51 87 05 00 70 94 E5 CC 50 96 E5";

                bool k = false;
                memory.OpenProcess("HD-Player");

                int i2 = 22000000;
                IEnumerable<long> wl = await memory.AoBScan(search, writable: true, true);
                string u = "0x" + wl.FirstOrDefault().ToString("X");
                if (wl.Count() != 0)
                {
                    for (int i = 0; i < wl.Count(); i++)
                    {
                        i2++;
                        memory.WriteMemory(wl.ElementAt(i).ToString("X"), "bytes", replace);
                    }
                    k = true;
                }

                if (k == true)
                {
                    Console.Beep(400, 300);
                    //Type Here Code Inject Success Status
                    notificationLabel.Text = "Inject";

                }
                else
                {
                    //Type Here Code Inject Faild Status
                    Console.Beep(240, 300);
                    notificationLabel.Text = "Failed";
                }
            }

        }

        public void ExecuteCommand(string command)
        {
            ProcessStartInfo psi = new ProcessStartInfo("cmd.exe")
            {
                RedirectStandardInput = true,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };

            using (Process process = new Process { StartInfo = psi })
            {
                process.Start();

                // Write the command to the command prompt
                process.StandardInput.WriteLine(command);
                process.StandardInput.Flush();
                process.StandardInput.Close();

                // Wait for the command to finish
                process.WaitForExit();
            }
        }
        private void guna2GradientButton2_Click(object sender, EventArgs e)
        {
            this.ExecuteCommand("netsh advfirewall firewall add rule name=\"TemporaryBlock2\" dir=in action=block profile=any program=\"C:\\Program Files\\BlueStacks_nxt\\HD-Player.exe");
            this.ExecuteCommand("netsh advfirewall firewall add rule name=\"TemporaryBlock2\" dir=out action=block profile=any program=\"C:\\Program Files\\BlueStacks_nxt\\HD-Player.exe");
            this.ExecuteCommand("netsh advfirewall firewall add rule name=\"TemporaryBlock2\" dir=in action=block profile=any program=\"C:\\Program Files\\BlueStacks\\HD-Player.exe");
            this.ExecuteCommand("netsh advfirewall firewall add rule name=\"TemporaryBlock2\" dir=out action=block profile=any program=\"C:\\Program Files\\BlueStacks\\HD-Player.exe");
            this.ExecuteCommand("netsh advfirewall firewall add rule name=\"TemporaryBlock2\" dir=in action=block profile=any program=\"C:\\Program Files\\BlueStacks_msi2\\HD-Player.exe");
            this.ExecuteCommand("netsh advfirewall firewall add rule name=\"TemporaryBlock2\" dir=out action=block profile=any program=\"C:\\Program Files\\BlueStacks_msi2\\HD-Player.exe");
            this.ExecuteCommand("netsh advfirewall firewall add rule name=\"TemporaryBlock2\" dir=in action=block profile=any program=\"C:\\Program Files\\BlueStacks_msi5\\HD-Player.exe");
            this.ExecuteCommand("netsh advfirewall firewall add rule name=\"TemporaryBlock2\" dir=out action=block profile=any program=\"C:\\Program Files\\BlueStacks_msi5\\HD-Player.exe");
            this.notificationLabel.Text = "INJECT";
            Console.Beep();
        }

        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void label3_Click(object sender, EventArgs e)
        {

        }

        private void guna2GradientButton3_Click(object sender, EventArgs e)
        {
            this.ExecuteCommand("netsh advfirewall firewall delete rule name=all program=\"C:\\Program Files\\BlueStacks_nxt\\HD-Player.exe");
            this.ExecuteCommand("netsh advfirewall firewall delete rule name=all program=\"C:\\Program Files\\BlueStacks\\HD-Player.exe");
            this.ExecuteCommand("netsh advfirewall firewall delete rule name=all program=\"C:\\Program Files\\BlueStacks_msi2\\HD-Player.exe");
            this.ExecuteCommand("netsh advfirewall firewall delete rule name=all program=\"C:\\Program Files\\BlueStacks_msi5\\HD-Player.exe");
            this.notificationLabel.Text = "INJECT";
            Console.Beep();
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }
    }
}
