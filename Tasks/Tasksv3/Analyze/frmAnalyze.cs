﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

// TODO: Cleanup and change the code style
namespace Tasks
{
    public partial class frmAnalyze : Form
    {
        public frmAnalyze()
        {
            InitializeComponent();
        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void frmAnalyze_Load(object sender, EventArgs e)
        {
    RunAnalytics();
    
        }
              
              public void RunAnalytics()
              {
              
                DriveInfo[] allDrives = DriveInfo.GetDrives();

                // will look for setttings, etc, drive space, if the drive is compressed / indexed, settings for windows and more.

                    bool indexedfilespace;
                    bool compression;
                    int drives;
                    string powermode;
                    
              }
              
         
        }
    }
    
