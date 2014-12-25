using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Windows.Forms;
using Amplio.Properties;

namespace Amplio
{
    class ReadOnly
    //This class change the Read Only/Archive (RA) flag in the files to Archive (A) flag only in order to allow file deletion from XBMC 
    {
        internal void RAAll()
        {
            // Begin:Add the last path entered to memory

            Settings.Default.LastPath = Variables.path;
            Settings.Default.Save();

            // End

            var folders = Directory.GetDirectories(Variables.path, "*", SearchOption.AllDirectories);
            foreach (var folder in folders)
            {
                var files = Directory.GetFiles(folder, "*", SearchOption.TopDirectoryOnly);
                foreach (var file in files)
                {
                    if ((File.GetAttributes(file) & FileAttributes.ReadOnly) == FileAttributes.ReadOnly)
                    {
                        File.SetAttributes(file, FileAttributes.Archive);
                    }
                }
            }
        }

        internal void RA()
        {
            var files = Directory.GetFiles(Variables.path, "*", SearchOption.TopDirectoryOnly);
            foreach (var file in files)
            {
                if ((File.GetAttributes(file) & FileAttributes.ReadOnly) == FileAttributes.ReadOnly)
                {  
                     File.SetAttributes(file, FileAttributes.Archive);
                }
            }
        }
    }
}
