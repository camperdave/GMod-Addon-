using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.IO;

namespace GModAddon_
{
    class Program
    {

        private static string gmodDirPath;

        static void Main(string[] args)
        {

            if (args.Length > 0)
            {
                gmodDirPath = args[0];
            }
            else
            {
                Console.Write("GMod Path: ");
                gmodDirPath = Console.ReadLine();
            }

            Console.WriteLine("Starting GMod Addon+\n");
            Console.WriteLine("GMod Path: " + gmodDirPath);

            IEnumerable<string> addonPaths = Directory.EnumerateDirectories(gmodDirPath + "\\addons");
            //for (int addonNum = 0; addonNum < addonPaths.Count(); addonNum++)
            //{
            //    Console.WriteLine("Processing " + addonPaths.
            //}
            foreach (string addonPath in addonPaths)
            {
                Console.WriteLine("===== Processing " + addonPath + "=====\n\n");
                DirectoryInfo addonDir = new DirectoryInfo(addonPath);
                DirectoryInfo[] subDirs = addonDir.GetDirectories();

                foreach (DirectoryInfo subDir in subDirs)
                {
                    Console.WriteLine("Handling SubDir: " + subDir.Name + "\n");
                    if (subDir.Name == ".svn")
                    {
                        // First, do an SVN update.
                        string toRun = Directory.GetCurrentDirectory() + "\\SVN\\svn.exe";
                        string toRunArgs = "update \"" + addonPath + "\"";

                        Process svnUpdateProc = new Process();
                        svnUpdateProc.StartInfo.FileName = toRun;
                        svnUpdateProc.StartInfo.Arguments = toRunArgs;
                        svnUpdateProc.StartInfo.UseShellExecute = false;
                        svnUpdateProc.StartInfo.CreateNoWindow = true;

                        Console.WriteLine(toRun + " " + toRunArgs);

                        svnUpdateProc.Start();
                        svnUpdateProc.WaitForExit();
                        
                        //Console.ReadLine();
                    }
                    else
                    {
                        checkFolder(addonPath, subDir.Name);
                    }
                }
            }

        }


        private static void checkFolder(string addonPath, string folderName)
        {
            // First, handle any subdirectories of the folder:
            DirectoryInfo mainDir = new DirectoryInfo(addonPath + "\\" + folderName);
            DirectoryInfo[] subDirs = mainDir.GetDirectories();
            foreach (DirectoryInfo dI in subDirs)
            {
                string dupPath = gmodDirPath + "\\" + folderName + "\\" + dI.Name;
                if(Directory.Exists(dupPath))
                {
                    // Uh oh! check for duplicates.
                    Console.WriteLine("potential duplicates found in: " + dI.Name);
                    FileInfo[] files = dI.GetFiles();
                    foreach(FileInfo file in files)
                    {
                        string toTest = dupPath + "\\" + file.Name;
                        if (File.Exists(toTest))
                        {
                            Console.WriteLine(toTest + " - DELETED!");
                            File.Delete(toTest);
                        }
                    }
                }
            }

        }
    }
}
