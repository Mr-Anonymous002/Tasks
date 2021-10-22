﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace Tasks
{
    class RemoveExt
    {

        public static void RemoveExtension(string path, int Browser)
        {
            // Case 1: Firefox
            // Case 2: Chrome + Edge
    
            switch (Browser)
            {
                case 1:
                    try
                    {
                     File.Delete(path);
                    }
                    catch
                    {
                        // ex
                    }
                    break;

                case 2: // Chrome will say that the extension is corrupted, and will attempt to repair it / restore it. We are working on a fix for this.
                    try 
                    {
                    Directory.Delete(path, true);
                    }
                    catch
                    {
                        // ex
                    }
                    break;
            

            }
        }
        
    }
}
