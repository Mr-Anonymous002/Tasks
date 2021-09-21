﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.Win32;
using System.IO;
using System.Management;
using System.Diagnostics;
using System.Threading;

// TODO: Cleanup and change the code style
namespace Tasks
{

    public partial class frmStartupPrograms : Form
    {
        public frmStartupPrograms()
        {
            InitializeComponent();
            RenderStartupsOnListWiew();
        }

        private void ClearStartupList()
        {
            StartupProcesses.Items.Clear();
        }

        private void RenderStartupsOnListWiew()
        {
            ManagementClass mangnmt = new ManagementClass("Win32_StartupCommand");
            ManagementObjectCollection mcol = mangnmt.GetInstances();
            foreach (ManagementObject strt in mcol)
            {
                ListViewItem oo = StartupProcesses.Items.Add(strt["Name"].ToString(), 0);  //Changed the way the items work so we can add more than one subitem.
                oo.SubItems.Add(strt["Location"].ToString());
            }


        }


        private void button1_Click(object sender, EventArgs e)
        {
    // idk
            // update, i still dont know can someone please help me with this
          



        }

        private void frmStartupPrograms_Load(object sender, EventArgs e)
        {
            txtTargetPath.Text = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile) + "\\AppData\\Roaming\\Microsoft\\Windows\\Start Menu\\Programs\\Startup\\";
        }

        private void StartupProcesses_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void button3_Click(object sender, EventArgs e)
        {
            try
            {
                // will add a method to auto update the list when the window closes
                Process.Start("explorer.exe", @Environment.GetFolderPath(Environment.SpecialFolder.UserProfile) + "\\AppData\\Roaming\\Microsoft\\Windows\\Start Menu\\Programs\\Startup");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex);
            }

        }

        private void button2_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog ofd = new OpenFileDialog() { Filter = "All|*.*" })
            {
                if (ofd.ShowDialog() == DialogResult.OK)
                txtFileName.Text = ofd.FileName;
                FileInfo fileInfo = new FileInfo(txtFileName.Text);
                txtTargetPath.Text = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile) + "\\AppData\\Roaming\\Microsoft\\Windows\\Start Menu\\Programs\\Startup", fileInfo.Name);
                File.Copy(txtFileName.Text, txtTargetPath.Text, true);
                Thread.Sleep(30);
                ClearStartupList();
                RenderStartupsOnListWiew();

            }

 
               
            

        }

        private void button4_Click(object sender, EventArgs e)
        {
           
        }
        public class StartUpProgram

        {

            public string Name { get; set; }


            public string Path { get; set; }

            //show name in checkboxitem

            public override string ToString()

            {

                return Name;

            }

        }
    }
}
