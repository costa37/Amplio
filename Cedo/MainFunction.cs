using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using Amplio.Properties;
using NUnrar.Archive;
using System.Diagnostics;

namespace Amplio
{
    internal class MainFunction
    {
        internal void OK_ClickLogic()
        {

            /* Ok button event will:
               - Go through every folder in the path
               - Extract every ".rar" file in every folder
               - Moves all the subs (".srt") files into path
               - If a file >=100 M then move it to path
               - Return a log file and message with names of folder that no file was moved from them
             */
            
            // Begin:Add the last path entered to memory
            
            Settings.Default.LastPath = Variables.path;
            Settings.Default.Save();
            
            // End
            Cursor.Current = Cursors.WaitCursor; //Changes the cursor to waiting
            string logFileName = Variables.path + "\\logfile_" + DateTime.Now.ToString("dd-MM-yyyy_hh-mm-ss") + ".txt"; //Log file name

            try // Catches the error that happenes when no path is given by the user before OK buttone is clicked
            {
                using (StreamWriter log = File.CreateText(logFileName)) // Creates a new text file
                {
                    log.WriteLine("Log File {0}", DateTime.Now.ToString("dd-MM-yyyy_hh-mm-ss")); // Log file begin
                    log.WriteLine();
                }

                int counter = 0;

                List<string> messageLog = new List<string>(); //Creation of list that will store the names of folders that did not contained large videos

                var folders = Directory.GetDirectories(Variables.path, "*", SearchOption.AllDirectories); //Gets the directories list of the path

                //Begin: RAR files extract

                string[] rarFile = new string[100]; //Could be only 100 RAR files maximum in each folder
                foreach (var folder in folders)
                {
                    rarFile = Directory.GetFiles(folder, "*.rar", SearchOption.AllDirectories);
                    if (rarFile.Length != 0) // Do only if RAR files exist in the folder
                    {
                        string destination = folder;
                        try
                        {
                         RarArchive.WriteToDirectory(rarFile[0], destination, NUnrar.Common.ExtractOptions.Overwrite);
                            
                        }
                        catch (System.IO.FileNotFoundException)
                        {
                        }
                    }

                    Array.Clear(rarFile, 0, rarFile.Length);
                }                

                //End

                //Begin: Moves all the large videos and subs (.srt) to path

                int df = 0;
                foreach (var folder in folders)
                {
                    DirectoryInfo size = new DirectoryInfo(folder); // Makes reference to directory
                    FileInfo[] fiArr = size.GetFiles(); // Get reference to each file in that directory

                    foreach (FileInfo fi in fiArr)
                    {
                        if (fi.Extension == ".srt")
                        {
                            if (!File.Exists(Variables.path + "\\" + fi.Name))
                            {
                                File.Move(fi.FullName, Variables.path + "\\" + fi.Name); // Moves subs (.srt) to path
                            }
                            else
                            {
                                df++;
                                File.Move(fi.FullName, Variables.path + "\\" + df + fi.Name);
                            }
                        }

                        else
                        {
                            if ((fi.Length / 1000000 > 100) && (fi.Extension != ".rar"))
                            {
                                if (!File.Exists(Variables.path + "\\" + fi.Name))
                                {
                                    File.Move(fi.FullName, Variables.path + "\\" + fi.Name);
                                    counter++; // Provides a count of moved files from each folder
                                }
                                else
                                {
                                    df++;
                                    File.Move(fi.FullName, Variables.path + "\\" + df + fi.Name);
                                    counter++;
                                }
                            }
                        }
                    }

                    switch (counter)
                    {
                        case 0:
                            using (StreamWriter log = new StreamWriter(logFileName, true))
                            {
                                log.WriteLine("The folder {0} does not contain any large files", Path.GetFileName(folder));
                                messageLog.Add(Path.GetFileName(folder)); //Adds the names of the folders that have not contained large videos to the log file
                            }
                            break;
                    }

                    if (counter >= 2)
                    {
                        using (StreamWriter log = new StreamWriter(logFileName, true))
                        {
                            log.WriteLine("The folder {0} contained more then one large file", Path.GetFileName(folder));
                        }
                    }

                    counter = 0;
                }

                using (StreamWriter log = new StreamWriter(logFileName, true))
                {
                    log.WriteLine();
                    log.Write("End of the log file");
                }

                ReadOnly rA = new ReadOnly();
                rA.RA();

                //Begin: Show the user all the folder that no file was moved from them in the message box

                StringBuilder builder = new StringBuilder();
                foreach (string mL in messageLog)
                {
                    builder.Append(mL.ToString()).AppendLine();
                }
                Cursor.Current = Cursors.Default; //Returns the cursor to defoult

                if (builder.Length != 0)
                {
                    MessageBox.Show("Warning! \n" + "Following folders did not contain videos: \n" + builder.ToString(), "Warning!", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
                else
                    MessageBox.Show("The operationd was finished successfully!", "Amplio", MessageBoxButtons.OK, MessageBoxIcon.Information);

                //End

                // Begin: Folder deletion section

                if (MessageBox.Show("Do you want to delete all the folders in the path?","Confirm", MessageBoxButtons.YesNo, MessageBoxIcon.Question ) == DialogResult.Yes)
                {
                    var delFolders = Directory.GetDirectories(Variables.path, "*", SearchOption.TopDirectoryOnly); //Gets the directories list of the path top directoy only
                    foreach (var delFolder in delFolders)
                    {
                        try
                        {
                            Directory.Delete(delFolder, true);
                        }
                        catch (IOException e)
                        {
                            Console.WriteLine(e.Message);
                        }
                    }
                }

                //End

                Application.Exit();
            }

            catch (System.UnauthorizedAccessException)
            {
                MessageBox.Show("Please enter a valid path.", "Warning");
            }
        }
    }
}