﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Windows.Forms;
using ByteSizeLib;

namespace Tasks
{

    public partial class frmCleanup : Form
    {
        public frmCleanup() { InitializeComponent(); }

        [DllImport("Shell32.dll")]
        static extern int SHEmptyRecycleBin(IntPtr hwnd, string pszRootPath, RecycleFlag dwFlags);
        enum RecycleFlag : int
        {
            SHERB_NOCONFIRMATION = 0x00000001, // No confirmation, when emptying
            SHERB_NOPROGRESSUI = 0x00000001, // No progress tracking window during the emptying of the recycle bin
            SHERB_NOSOUND = 0x00000004 // No sound when the emptying of the recycle bin is complete
        }



        private bool DeleteAllFiles(DirectoryInfo directoryInfo)
        {

            foreach (var file in directoryInfo.GetFiles())
            {
                try
                {
                    file.Delete();
                    CleanupLogsLBox.Items.Add("Deleted " + file.FullName);
                }
                catch (Exception ex)
                {
                    CleanupLogsLBox.Items.Add("Exception: " + ex.Message);
                }

            }
            foreach (var dir in directoryInfo.GetDirectories())
            {
                try
                {
                    dir.Delete(true);
                    CleanupLogsLBox.Items.Add("Deleted Folder " + dir.FullName);
                }
                catch (Exception ex)
                {
                    CleanupLogsLBox.Items.Add("Exception: " + ex.Message);
                }

            }

            return true;
        }



        private void btnCleanup_Click(object sender, EventArgs e)
        {
            // List our local directories so we don't need to repeat a lot of code.
            var localappdata = Environment.GetEnvironmentVariable("LocalAppData");
            var roamingappdata = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            var windowstemp = new DirectoryInfo("C:\\Windows\\Temp");
            var usertemp = new DirectoryInfo(Path.GetTempPath());
            var downloads = new DirectoryInfo(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile) + "\\Downloads");

            

            if (cbExplorerDownloads.Checked)
                try
                {
                    if (DeleteAllFiles(downloads)) CleanupLogsLBox.Items.Add("Downloads Folder Cleared.");
                }
                catch (Exception ex)
                {
                    CleanupLogsLBox.Items.Add("Error deleting the Downloads Folder. " + ex);

                }

            if (cbSystemRecycleBin.Checked)
                try
                {
                    // Silently deletes the recycle bin.
                    SHEmptyRecycleBin(IntPtr.Zero, null, RecycleFlag.SHERB_NOSOUND | RecycleFlag.SHERB_NOCONFIRMATION);
                    CleanupLogsLBox.Items.Add("Recycle Bin Cleared.");
                }
                catch (Exception ex)
                {
                    CleanupLogsLBox.Items.Add("Error clearing the Recycle Bin. " + ex);

                }


            if (cbSystemTempFolders.Checked)
            {
                try
                {
                    if (DeleteAllFiles(windowstemp)) CleanupLogsLBox.Items.Add("System Temp Folder Deleted.");
                    if (DeleteAllFiles(usertemp)) CleanupLogsLBox.Items.Add("User Temp Folder Deleted.");

                }
                catch (Exception ex)
                {
                    CleanupLogsLBox.Items.Add("Error while deleting temp folders. " + ex);
                }

            }

      

            if (cbSystemPrefetch.Checked)
            {
                try
                {
                    var directory = new DirectoryInfo("C:\\Windows\\Prefetch");
                    if (DeleteAllFiles(directory)) CleanupLogsLBox.Items.Add("Prefetch Deleted.");
                }
                catch (Exception ex)
                {
                    CleanupLogsLBox.Items.Add("Error while deleting Prefetch. " + ex);
                }
            }



            if (cbChromeCache.Checked)
            {
                try
                {
                    string mainSubdirectory = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile) + "\\AppData\\Local\\Google\\Chrome\\User Data\\";
                    string[] userDataCacheDirs = { "Default\\Cache", "Default\\Code Cache\\", "Default\\GPUCache", "ShaderCache", "Default\\Service Worker\\CacheStorage", "Default\\Service Worker\\ScriptCache", "GrShaderCache\\GPUCache", "\\Default\\File System\\" };
                    List<DirectoryInfo> directoryInfos = new List<DirectoryInfo>();

                    foreach (string subdir in userDataCacheDirs)
                    {
                        // Make a new DirectoryInfo with the info of that subdirectory and then add it into the directoryInfos array
                        directoryInfos.Add(new DirectoryInfo(mainSubdirectory + subdir + "\\"));

                    }

                    bool isDeleted = true;
                    // For each DirectoryInfo inside of the directoryInfos array
                    foreach (DirectoryInfo d in directoryInfos)
                    {
                        try
                        {
                            DeleteAllFiles(d);
                        }
                        catch (Exception ex)
                        {
                            CleanupLogsLBox.Items.Add("Error deleting Chrome Cache. " + ex);
                        }

                        // If DeleteAllFiles returns false, set the isDeleted value to false
                        if (!DeleteAllFiles(d))
                            isDeleted = false;
                    }

                    if (isDeleted)
                        CleanupLogsLBox.Items.Add("Chrome Cache Deleted.");
                }
                catch (Exception)
                {

                }
            }

            if (cbChromeSessions.Checked) //Chrome Session 
            {

                var directory = new DirectoryInfo(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile) + "\\AppData\\Local\\Google\\Chrome\\User Data\\Default\\Sessions");
                var directory2 = new DirectoryInfo(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile) + "\\AppData\\Local\\Google\\Chrome\\User Data\\Default\\Session Storage");
                var directory3 = new DirectoryInfo(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile) + "\\AppData\\Local\\Google\\Chrome\\User Data\\Default\\Extension State");
                if (DeleteAllFiles(directory) & DeleteAllFiles(directory2) & DeleteAllFiles(directory3)) CleanupLogsLBox.Items.Add("Chrome Sessions Deleted.");
            }

            if (cbChromeCookies.Checked) //Chrome cookies
            {
                var directory = new DirectoryInfo(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile) + "\\AppData\\Local\\Google\\Chrome\\User Data\\Default\\IndexedDB\\");
                File.Delete(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile) + "\\AppData\\Local\\Google\\Chrome\\User Data\\Default\\Cookies");
                File.Delete(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile) + "\\AppData\\Local\\Google\\Chrome\\User Data\\Default\\Cookies-journal");
                if (DeleteAllFiles(directory)) CleanupLogsLBox.Items.Add("Chrome Cookies Deleted.");
            }


            if (cbChromeSearchHistory.Checked) //Chrome search history
            {
                File.Delete(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile) + "\\AppData\\Local\\Google\\Chrome\\User Data\\Default\\History");
                File.Delete(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile) + "\\AppData\\Local\\Google\\Chrome\\User Data\\Default\\History Provider Cache");
                File.Delete(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile) + "\\AppData\\Local\\Google\\Chrome\\User Data\\Default\\History-journal");
                CleanupLogsLBox.Items.Add("Chrome Search History Deleted.");
            }



            // Discord

            if (cbDiscordCache.Checked) //Discord cache
            {
                var directory = new DirectoryInfo(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile) + "\\AppData\\Roaming\\discord\\Cache");
                var directory2 = new DirectoryInfo(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile) + "\\AppData\\Roaming\\discord\\Code Cache");
                var directory3 = new DirectoryInfo(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile) + "\\AppData\\Roaming\\discord\\GPUCache");
                if (DeleteAllFiles(directory) & DeleteAllFiles(directory2) & DeleteAllFiles(directory3)) CleanupLogsLBox.Items.Add("Discord Cache Deleted.");
            }


            if (cbDiscordCookies.Checked) //Discord cookies
                try
                {

                    File.Delete(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile) + "\\AppData\\Roaming\\discord\\Cookies");
                    File.Delete(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile) + "\\AppData\\Roaming\\discord\\Cookies-journal");
                    CleanupLogsLBox.Items.Add("Discord Cookies Deleted.");

                }
                catch (Exception ex)
                {
                    CleanupLogsLBox.Items.Add("Error deleting Discord Cookies." + ex);
                }

            //Firefox

            if (cbFirefoxCache.Checked) //Firefox cache
            {
                try
                {

                    var cache = (localappdata + "\\Mozilla\\Firefox\\Profiles\\");
                    foreach (string direc in Directory.EnumerateDirectories(cache))
                    {
                        if (direc.Contains("release") == true)
                        {
                            var cachefile = (direc + "\\cache2");
                            foreach (string file in Directory.EnumerateFiles(cachefile))
                            {
                                try
                                {
                                    File.Delete(file);
                                    CleanupLogsLBox.Items.Add("Firefox Cache Deleted.");
                                }
                                catch (Exception ex)
                                {
                                    CleanupLogsLBox.Items.Add("Exception Error: " + ex);
                                }

                            }
                            foreach (string dir in Directory.EnumerateDirectories(cachefile))
                            {
                                try
                                {
                                    Directory.Delete(dir, true);
                                    CleanupLogsLBox.Items.Add("Firefox Cache Deleted.");
                                }
                                catch (Exception ex)
                                {
                                    CleanupLogsLBox.Items.Add("Exception Error:" + ex);
                                }

                            }
                        }
                    }
                    try
                    {

                        var profile = (roamingappdata + "\\Mozilla\\Firefox\\Profiles\\");
                        foreach (string direc in Directory.EnumerateDirectories(profile))
                        {
                            if (direc.Contains("release") == true)
                            {
                                try
                                {
                                    var shadercache = (direc + "\\shader-cache");
                                    foreach (string file in Directory.EnumerateFiles(shadercache))
                                    {
                                        try
                                        {
                                            File.Delete(file);
                                            CleanupLogsLBox.Items.Add("Deleted File: " + file);
                                        }
                                        catch
                                        {
                                            //do nothing
                                        }

                                    }

                                }
                                catch
                                {

                                    //do nothing

                                }

                            }
                        }

                    }
                    catch (Exception ex)
                    {
                        CleanupLogsLBox.Items.Add("Error trying to delete Firefox Shader Cache. " + ex);
                    }

                }
                catch (Exception ex)
                {
                    CleanupLogsLBox.Items.Add("Error trying to delete firefox cache. " + ex);
                }



            }

            if (cbFirefoxCookies.Checked) //Firefox cookies
            {
                try
                {
                    var cookies = (roamingappdata + "\\Mozilla\\Firefox\\Profiles\\");
                    foreach (string direc in Directory.EnumerateDirectories(cookies))
                    {
                        if (direc.Contains("release") == true)
                        {
                            try
                            {
                                var cookiefile = (direc + "\\cookies.sqlite");
                                File.Delete(cookiefile);
                                CleanupLogsLBox.Items.Add("Firefox Cookies Deleted.");

                            }
                            catch (Exception ex)
                            {

                                CleanupLogsLBox.Items.Add("Error while trying to delete Firefox cookies. " + ex);
                            }

                        }
                    }

                }
                catch (Exception ex)
                {
                    CleanupLogsLBox.Items.Add("Error while trying to delete Firefox cookies. " + ex);
                }


            }
            if (cbFirefoxSearchHistory.Checked) //Firefox search history
            {
                try
                {

                    var cookies = (roamingappdata + "\\Mozilla\\Firefox\\Profiles\\");
                    foreach (string direc in Directory.EnumerateDirectories(cookies))
                    {
                        if (direc.Contains("release") == true)
                        {
                            try
                            {
                                var cookiefile = (direc + "\\places.sqlite");
                                File.Delete(cookiefile);
                                CleanupLogsLBox.Items.Add("Firefox History Deleted.");

                            }
                            catch (Exception ex)
                            {

                                CleanupLogsLBox.Items.Add("Error while trying to delete Firefox History." + ex);

                            }

                        }
                    }

                }
                catch (Exception ex)
                {
                    CleanupLogsLBox.Items.Add("Error when trying to delete Firefox History. " + ex);
                }
            }



            //DNS & ARP
            if (cbSystemDNSCache.Checked) //Clear DNS
            {
                try
                {
                    System.Diagnostics.Process process = new System.Diagnostics.Process();
                    System.Diagnostics.ProcessStartInfo startInfo = new System.Diagnostics.ProcessStartInfo();
                    startInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden;
                    startInfo.FileName = "cmd.exe";
                    startInfo.Arguments = "/c ipconfig /flushdns";
                    startInfo.RedirectStandardError = true;
                    process.StartInfo = startInfo;
                    process.Start();
                    CleanupLogsLBox.Items.Add("DNS Cache Cleared.");
                }
                catch (Exception ex)
                {

                    CleanupLogsLBox.Items.Add("Error while trying to clear DNS cache!" + ex);

                }
            }
            if (cbSystemARPCache.Checked) //Clear ARP
            {
                try
                {
                    System.Diagnostics.Process process = new System.Diagnostics.Process();
                    System.Diagnostics.ProcessStartInfo startInfo = new System.Diagnostics.ProcessStartInfo();
                    startInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden;
                    startInfo.FileName = "cmd.exe";
                    startInfo.Verb = "runas"; //give cmd admin perms
                    startInfo.UseShellExecute = true;
                    startInfo.Arguments = @"/C arp -a -d";
                    process.StartInfo = startInfo;
                    process.Start();
                    CleanupLogsLBox.Items.Add("ARP Cache Cleared.");
                }
                catch (Exception ex)
                {

                    CleanupLogsLBox.Items.Add("Error while trying to clear ARP cache. " + ex);
                    MessageBox.Show(ex.ToString());

                }
            }

            if (cbExplorerRecents.Checked)     //RECENT FILES
            {
                try
                {
                    CleanRecentFiles.CleanRecents.ClearAll();
                    CleanupLogsLBox.Items.Add("Recent Files Cleared.");
                }
                catch (Exception ex)
                {
                    CleanupLogsLBox.Items.Add("Error while clearing Recent Files. " + ex);
                }

            }


            if (cbEdgeSearchHistory.Checked) //Edge search history
            {

                File.Delete(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile) + "\\AppData\\Local\\Microsoft\\Edge\\User Data\\Default\\History");
                CleanupLogsLBox.Items.Add("Edge Search History Deleted.");
            }
            if (cbEdgeCookies.Checked) //Edge cookies
            {

                File.Delete(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile) + "\\AppData\\Local\\Microsoft\\Edge\\User Data\\Default\\Cookies");
                var directory = new DirectoryInfo(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile) + "\\AppData\\Local\\Microsoft\\Edge\\User Data\\Default\\IndexedDB\\");
                if (DeleteAllFiles(directory)) CleanupLogsLBox.Items.Add("Edge Cookies Deleted.");
            }

            if (cbEdgeCache.Checked) //Edge cache
            {
                try
                {
                    var directory = new DirectoryInfo(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile) + "\\AppData\\Local\\Microsoft\\Edge\\User Data\\Default\\Cache\\");
                    var directory2 = new DirectoryInfo(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile) + "\\AppData\\Local\\Microsoft\\Edge\\User Data\\Default\\Code Cache\\");
                    var directory3 = new DirectoryInfo(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile) + "\\AppData\\Local\\Microsoft\\Edge\\User Data\\Default\\GPUCache\\");
                    var directory4 = new DirectoryInfo(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile) + "\\AppData\\Local\\Microsoft\\Edge\\User Data\\ShaderCache\\");
                    var directory5 = new DirectoryInfo(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile) + "\\AppData\\Local\\Microsoft\\Edge\\User Data\\Default\\Service Worker\\CacheStorage\\");
                    var directory6 = new DirectoryInfo(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile) + "\\AppData\\Local\\Microsoft\\Edge\\User Data\\Default\\Service Worker\\ScriptCache\\");
                    var directory7 = new DirectoryInfo(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile) + "\\AppData\\Local\\Microsoft\\Edge\\User Data\\GrShaderCache\\GPUCache\\");
                    var directory8 = new DirectoryInfo(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile) + "\\AppData\\Local\\Microsoft\\Edge\\User Data\\Default\\Service Worker\\Database\\");
                    if (DeleteAllFiles(directory) & DeleteAllFiles(directory2) & DeleteAllFiles(directory3) & DeleteAllFiles(directory4) & DeleteAllFiles(directory5) & DeleteAllFiles(directory6) & DeleteAllFiles(directory7) & DeleteAllFiles(directory8)) CleanupLogsLBox.Items.Add("Edge Cache Deleted.");
                }
                catch
                {
                    CleanupLogsLBox.Items.Add("Error while deleting Edge Cache.");
                }
            }

            if (cbSystemEventLogs.Checked)
            {
                try
                {
                    foreach (var eventLog in EventLog.GetEventLogs())
                    {
                        eventLog.Clear();
                        CleanupLogsLBox.Items.Add("Event Logs Deleted.");
                    }
                }
                catch (Exception ex)
                {
                    CleanupLogsLBox.Items.Add("Error deleting Event Logs. " + ex);
                }


            }

            if (cbEdgeSessions.Checked)
            {
                var directory = new DirectoryInfo(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile) + "\\AppData\\Local\\Microsoft\\Edge\\User Data\\Default\\Sessions");
                var directory2 = new DirectoryInfo(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile) + "\\AppData\\Local\\Microsoft\\Edge\\User Data\\Default\\Session Storage");
                var directory3 = new DirectoryInfo(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile) + "\\AppData\\Local\\Microsoft\\Edge\\User Data\\Default\\Extension State");
                if (DeleteAllFiles(directory) & DeleteAllFiles(directory2) & DeleteAllFiles(directory3)) CleanupLogsLBox.Items.Add("Edge Session Deleted.");
            }

            if (cbSystemDirectXCache.Checked)
            {
                var directory = new DirectoryInfo(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile) + "\\AppData\\Local\\D3DSCache");
                if (DeleteAllFiles(directory)) CleanupLogsLBox.Items.Add("DirectX Shader Cache Deleted.");
            }

            if (cbSystemMemDumps.Checked)
            {
                var directory = new DirectoryInfo(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile) + "\\AppData\\Local\\CrashDumps\\");
                if (DeleteAllFiles(directory)) CleanupLogsLBox.Items.Add("System Memory Dumps Deleted.");
            }


            if (cbSystemErrorReporting.Checked)
            {
                var directory = new DirectoryInfo("C:\\ProgramData\\Microsoft\\Windows\\WER\\ReportArchive");
                if (DeleteAllFiles(directory)) CleanupLogsLBox.Items.Add("Deleted " + directory);
            }

            if (cbExplorerThumbCache.Checked)
            {

                var tc = (Environment.GetFolderPath(Environment.SpecialFolder.UserProfile) + "\\AppData\\Local\\Microsoft\\Windows\\Explorer\\");
                foreach (string file in Directory.EnumerateFiles(tc))

                    if (file.Contains("thumbcache") == true)
                    {
                        try
                        {
                            File.Delete(file);
                            CleanupLogsLBox.Items.Add("Deleted " + file);
                        }
                        catch (Exception)
                        {
                            CleanupLogsLBox.Items.Add("Could not delete " + file);
                        }

                    }
            }

            if (cbExplorerIconCache.Checked)
            {
                var ic = (Environment.GetFolderPath(Environment.SpecialFolder.UserProfile) + "\\AppData\\Local\\Microsoft\\Windows\\Explorer\\");
                foreach (string file in Directory.EnumerateFiles(ic))

                    if (file.Contains("iconcache") == true)
                    {
                        try
                        {
                            File.Delete(file);
                            CleanupLogsLBox.Items.Add("Deleted " + file);
                        }
                        catch (Exception)
                        {
                            CleanupLogsLBox.Items.Add("Could not delete " + file);
                        }

                    }
            }

            if (cbChromeSavedPasswords.Checked)
            {
                try
                {
                    var directory = new DirectoryInfo(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile) + "\\AppData\\Local\\Google\\Chrome\\User Data\\Default\\Login Data\\");
                    if (DeleteAllFiles(directory)) CleanupLogsLBox.Items.Add("Chrome Saved Passwords Deleted.");
                }
                catch
                {
                    CleanupLogsLBox.Items.Add("Error while deleting Chrome Saved Passwords.");
                }
              
            }

            WriteCleanupSummary();
        }


        private void Tabs_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (tabControl1.SelectedTab.Text == "Browser Extensions") // i dont want it to show up in the extensions thing because i'll use a diff button to make the code less messy
            {
                btnCleanup.Visible = false;
             
            }
            else
            {
                if (!btnCleanup.Visible)
                {
                    btnCleanup.Visible = true;

                }
            }
        }
        private void frmCleanup_Load(object sender, EventArgs e)
        {
            tabControl1.SelectedIndexChanged += new EventHandler(Tabs_SelectedIndexChanged);

            DirectoryExists();

            // Extention Finder
            if (Directory.Exists(Dirs.chromeExtDir))
            {
                comboBox1.Items.Add("Google Chrome");
            }

            if (Directory.Exists(Dirs.firefoxDir))
            {
                comboBox1.Items.Add("Mozilla Firefox");
            }

            if (Directory.Exists(Dirs.edgeDir))
            {
                comboBox1.Items.Add("Microsoft Edge");
            }

        }





        private void button1_Click(object sender, EventArgs e) //DisplayDNS
        {

            try
            {
                Process process = new Process();
                process.StartInfo.FileName = "Scripts/BatFiles/displaydns.bat";
                process.Start();
                //Directory.SetCurrentDirectory(@"/");
                //Directory.SetCurrentDirectory(@"Scripts/BatFiles");
                //Process.Start("displaydns.bat");
            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.ToString());

            }

        }

        private void button2_Click(object sender, EventArgs e) //DisplayARP
        {


            try
            {
                Process process = new Process();
                process.StartInfo.FileName = "Scripts/BatFiles/displayarp.bat";
                process.Start();
            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.ToString());

            }



        }


        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

            var g = new Dirs();


            if (comboBox1.Text == "Google Chrome")
            {
                ExtensionsBox.Items.Clear();

                foreach (string ext in Directory.EnumerateDirectories(Dirs.chromeExtDir))

                {

                    FileInfo fi = new FileInfo(ext);
                    ListViewItem extb = ExtensionsBox.Items.Add(fi.Name, 0);
                    DirectoryInfo fol = new DirectoryInfo(ext);
                    fol.EnumerateDirectories();
                    extb.SubItems.Add("~ " + ByteSize.FromBytes(ext.Length).ToString());

                    extb.SubItems.Add(ext);
                }




            }
            else if (comboBox1.Text == "Mozilla Firefox")
            {
                ExtensionsBox.Items.Clear();
                try
                {
                    foreach (string fol in Directory.EnumerateDirectories(Dirs.firefoxExtDir))
                    {
                        if (fol.Contains("-release"))
                        {
                            string prf = (fol + "\\extensions\\");
                            try
                            {
                                foreach (string ext in Directory.EnumerateFiles(prf))
                                {
                                    FileInfo fi = new FileInfo(ext);
                                    ListViewItem extb = ExtensionsBox.Items.Add(fi.Name, 0);
                                    extb.SubItems.Add("~ " + ByteSize.FromBytes(fi.Length).ToString());
                                    extb.SubItems.Add(ext);

                                }

                            }
                            catch (Exception Exc)
                            {
                                MessageBox.Show(Exc.ToString());
                            }
                        }
                    }

                }
                catch (Exception Exc)
                {

                    MessageBox.Show(Exc.ToString());

                }

            }

            else if (comboBox1.Text == "Microsoft Edge")
            {
                ExtensionsBox.Items.Clear();
                foreach (string ext in Directory.EnumerateDirectories(Dirs.edgeExtDir))
                {
                    FileInfo fi = new FileInfo(ext);

                    ListViewItem extb = ExtensionsBox.Items.Add(fi.Name, 0);
                    DirectoryInfo fol = new DirectoryInfo(ext);
                    fol.EnumerateDirectories();
                    extb.SubItems.Add("~ " + ByteSize.FromBytes(ext.Length).ToString());

                    extb.SubItems.Add(ext);
                }
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {

            if (ExtensionsBox.SelectedItems.Count >= 0) //Check if the user selected extensions for deletion.
            {

                /*Process process = new Process();
                process.StartInfo.FileName = "Scripts/BatFiles/killfirefox.bat";
                process.Start();
                process.WaitForExit();*/
                if (comboBox1.Text == "Google Chrome")
                {
                    foreach (ListViewItem eachItem in ExtensionsBox.SelectedItems)
                    {
                        try
                        {
                            Taskkill.Browser(1);
                            Thread.Sleep(75);
                            var item = ExtensionsBox.SelectedItems[0];
                            var subItem = item.SubItems[2].Text;
                            RemoveExt.RemoveExtension(subItem, 2);
                            ExtensionsBox.Items.Remove(eachItem);
                            CleanupLogsLBox.Items.Add("Extension Removed.");

                        }
                        catch (Exception ex)
                        {
                            CleanupLogsLBox.Items.Add("Error while trying to remove extension." + ex);
                        }
                    }
                }

                if (comboBox1.Text == "Mozilla Firefox")
                {
                    Taskkill.Browser(2);
                    Thread.Sleep(75); //Short threadsleep or else the extension deleter would start before firefox is fully killed for some reasons?

                    try
                    {
                        RemoveExt.RemoveExtension(ExtensionsBox.SelectedItems[0].SubItems[2].Text, 1);

                        foreach (ListViewItem eachItem in ExtensionsBox.SelectedItems)
                        {
                            ExtensionsBox.Items.Remove(eachItem);
                            CleanupLogsLBox.Items.Add("Extension Removed.");
                        }
                    }
                    catch
                    {
                        CleanupLogsLBox.Items.Add("Failed to remove extension.");

                    }

                }


                if (comboBox1.Text == "Microsoft Edge")
                {
                    foreach (ListViewItem eachItem in ExtensionsBox.SelectedItems)
                    {
                        try
                        {
                            Taskkill.Browser(3);
                            Thread.Sleep(75);
                            var item = ExtensionsBox.SelectedItems[0];
                            var subItem = item.SubItems[2].Text;
                            RemoveExt.RemoveExtension(subItem, 2);
                            ExtensionsBox.Items.Remove(eachItem);
                            CleanupLogsLBox.Items.Add("Extension Removed.");

                        }
                        catch (Exception ex)
                        {
                            CleanupLogsLBox.Items.Add("Error while trying to remove extension." + ex);
                        }
                    }
                }

            }

            else
            {
                MessageBox.Show("Please select an extension to remove.");
            }

        }

        private void button5_Click(object sender, EventArgs e)
        {
            try { RunFile.RunBat("Scripts/BatFiles/byesolitaire.bat", true); }
            catch (Exception ex) { MessageBox.Show("An error occurred." + ex); }
        }

        private void button7_Click(object sender, EventArgs e)
        {
            try { Process.Start("powershell", "-ExecutionPolicy Bypass -File Scripts/Debloater/DisableCortana.ps1"); }
            catch (Exception ex) { MessageBox.Show("An error occurred." + ex); }
        }

        private void button6_Click(object sender, EventArgs e)
        {
            try { Process.Start("powershell", "-ExecutionPolicy Bypass  -File Scripts/Debloater/UninstallOneDrive.ps1"); }
            catch (Exception ex) { MessageBox.Show("An error occurred." + ex); }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            try { RunFile.RunBat("removeedge.bat", true); }
            catch (Exception ex){ MessageBox.Show("An error occurred." + ex); }
        }
        private void DirectoryExists()
        {
            var localappdata = Environment.GetEnvironmentVariable("LocalAppData");
            var roamingappdata = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            Dirs.chromeDir = localappdata + "\\Google\\Chrome\\";
            Dirs.chromeExtDir = (Environment.GetFolderPath(Environment.SpecialFolder.UserProfile) + "\\AppData\\Local\\Google\\Chrome\\User Data\\Default\\Extensions");
            Dirs.firefoxDir = localappdata + "\\Mozilla\\Firefox\\";
            Dirs.firefoxExtDir = roamingappdata + "\\Mozilla\\Firefox\\Profiles\\";
            Dirs.edgeDir = (Environment.GetFolderPath(Environment.SpecialFolder.UserProfile) + "\\AppData\\Local\\Microsoft\\Edge\\");
            Dirs.edgeExtDir = (Environment.GetFolderPath(Environment.SpecialFolder.UserProfile) + "\\AppData\\Local\\Microsoft\\Edge\\User Data\\Default\\Extensions\\");
            Dirs.discordDir = localappdata + "\\Discord\\";


            if (!Directory.Exists(Dirs.chromeDir))
            {
                cbChromeCache.Enabled = false;
                cbChromeCookies.Enabled = false;
                cbChromeSearchHistory.Enabled = false;
                cbChromeSessions.Enabled = false;
                cbChromeSavedPasswords.Enabled = false;
                lblNotDetected.Visible = true;
            }

            if (!Directory.Exists(Dirs.firefoxDir))
            {
                cbFirefoxCache.Enabled = false;
                cbFirefoxCookies.Enabled = false;
                cbFirefoxSearchHistory.Enabled = false;
                lblNotDetected.Visible = true;
            }

            if (!Directory.Exists(Dirs.discordDir))
            {
                cbDiscordCache.Enabled = false;
                cbDiscordCookies.Enabled = false;
                lblNotDetected.Visible = true;

            }

            if (!Directory.Exists(Dirs.edgeDir))
            {
                cbEdgeCache.Enabled = false;
                cbEdgeCookies.Enabled = false;
                cbEdgeSearchHistory.Enabled = false;
                cbEdgeSessions.Enabled = false;
                lblNotDetected.Visible = true;

            }
        }

        private void cbEdgeCookies_CheckStateChanged(object sender, EventArgs e)
        {
            try { taskDialog1.Show(); }
            catch { Console.WriteLine("An error has occurred."); }
        }
        private void cbFirefoxCookies_CheckStateChanged(object sender, EventArgs e)
        {
            try { taskDialog1.Show(); }
            catch { Console.WriteLine("An error has occurred."); }
        }

        private void cbChromeCookies_CheckStateChanged(object sender, EventArgs e)
        {
            try { taskDialog1.Show(); }
            catch { Console.WriteLine("An error has occurred."); }
        }

        public void WriteCleanupSummary()
        {
            int t = (int)((DateTime.UtcNow - new DateTime(1970, 1, 1)).TotalSeconds);
            File.WriteAllLines(
              Path.Combine(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Tasks"), "Cleanup Summary") + "\\tasks-cleanup-summary-" + t + ".txt",
              CleanupLogsLBox.Items.Cast<string>().ToArray()
            );
            MessageBox.Show("Cleanup is logged at " + Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "Tasks\\" + "Cleanup Summary" +"\\tasks-cleanup-summary-" + t + ".txt");


        }
    }
}
