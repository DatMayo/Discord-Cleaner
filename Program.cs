using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Security.Permissions;

namespace DiscordCleaner
{
    class Program
    {
        static void DrawStartScreen()
        {
            Console.Clear();
            Console.WriteLine("********************************************************************************");
            Console.WriteLine("* Discord Cleaner v0.1 by DatMayo                                Rel. 01.05.18 *");
            Console.WriteLine("* WARNING! All login credentials will be lost and must be re entered!          *");
            Console.WriteLine("*                                                                              *");
            Console.WriteLine("* GitHub: https://github.com/DatMayolein/Discord-Cleaner                       *");
            Console.WriteLine("********************************************************************************");
            Console.WriteLine("");
            Console.Write("Would you like to cleanup your Discord installation? (y/N) ");
            ConsoleKeyInfo key = Console.ReadKey();
            if (key.Key.ToString().ToLower() == "n")
            {
                Console.WriteLine("");
                Console.WriteLine("");
                Console.WriteLine("Operation aborted!");
                Console.ReadKey();
            }
            else if (key.Key.ToString().ToLower() == "y")
                GetProcesses();
            else
                DrawStartScreen();
        }

        static void GetProcesses()
        {
            Console.WriteLine("");
            bool killFailed = false;
            Console.WriteLine("");
            Console.WriteLine("Searching for Discord processes... ");
            Process[] discordHandle = Process.GetProcessesByName("discord");
            Console.WriteLine(string.Format("- {0} Discord process(es)", discordHandle.Length));
            foreach(Process p in discordHandle)
            {
                try
                {
                    Console.Write("-> Killing Discord process...");
                    p.Kill();
                    Console.WriteLine("OK");
                }
                catch
                {
                    Console.WriteLine("Failed");
                    killFailed = true;
                }
            }
            if (killFailed)
            {
                Console.WriteLine("Can not proceed, some discord processes are still running!");
                Console.ReadKey();
            }
            else
            {
                Console.WriteLine("Killing Discord process finished...");
                Console.WriteLine("");
                Console.WriteLine("Searching for Discord-Update processes... ");
                Process[] updateHandle = Process.GetProcessesByName("update");
                Console.WriteLine(string.Format("- {0} Discord-Update process(es)", updateHandle.Length));
                foreach (Process p in updateHandle)
                {
                    try
                    {
                        Console.Write("-> Killing Discord process...");
                        p.Kill();
                        Console.WriteLine("OK");
                    }
                    catch
                    {
                        Console.WriteLine("Failed");
                        killFailed = true;
                    }
                }
                if (killFailed)
                {
                    Console.WriteLine("Can not proceed, some discord processes are still running!");
                    Console.ReadKey();
                }
                else
                {
                    Console.WriteLine("Killing Discord-Update process finished...");
                    Console.WriteLine("");
                    Console.Write("Now waiting for 5 seconds...");
                    System.Threading.Thread.Sleep(5000); //Necessary, for whatever reason
                    Console.WriteLine("OK");
                    Console.WriteLine("");
                    RemoveDiscord();
                }
            }
        }

        static void RemoveDiscord()
        {
            bool deleteFailed = false;
            string discordAppData = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\discord";
            string discordLocalAppData = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + "\\discord";
            Console.WriteLine(string.Format("Checking folder \"{0}\"", discordAppData));
            if(Directory.Exists(discordAppData))
            {
                Console.Write("Found existing discord in appdata-folder, deleting it...");
                try
                {
                    Directory.Delete(discordAppData, true);
                    Console.WriteLine("OK");
                }
                catch (Exception ex)
                {
                    deleteFailed = true;
                    Console.WriteLine("Failed");
                    Console.Write(ex.Message);
                }
            }
            if (deleteFailed)
            {
                Console.WriteLine("Can not proceed, some discord files may remain!");
                Console.ReadKey();
            }
            else
            {
                Console.WriteLine("");
                Console.WriteLine(string.Format("Checking folder \"{0}\"", discordLocalAppData));
                if (Directory.Exists(discordLocalAppData))
                {
                    Console.Write("Found existing discord in localappdata-folder, deleting it...");
                    try
                    {
                        Directory.Delete(discordLocalAppData, true);
                        Console.WriteLine("OK");
                    }
                    catch (Exception ex)
                    {
                        deleteFailed = true;
                        Console.WriteLine("Failed");
                        Console.Write(ex.Message);
                    }
                }
                if (deleteFailed)
                {
                    Console.WriteLine("Can not proceed, some discord files may remain!");
                    Console.ReadKey();
                }
                else
                {
                    Console.WriteLine("");
                    DownloadDiscordSetup();
                }
            }
        }

        static void DownloadDiscordSetup()
        {
            Console.Write("Downloading new Discord-Version (this can take some time)...");
            WebClient client = new WebClient();
            byte[] DiscordSetup = client.DownloadData("https://discordapp.com/api/download?platform=win");
            File.WriteAllBytes("DiscordSetup.exe", DiscordSetup);
            Console.WriteLine("Finished");
            Console.WriteLine("");
            InstallDiscord();
        }

        static void InstallDiscord()
        {
            Console.Write("Starting DiscordSetup.exe...");
            Process process = new Process();
            process.StartInfo.FileName = "DiscordSetup.exe";
            process.Start();
            process.WaitForExit();// Waits here for the process to exit.
            Console.WriteLine("Finished");
            Console.WriteLine("");
            Console.WriteLine("All Done! Discord will start automatically.");
            Console.ReadKey();
        }

        static void DownloadFinished()
        {
            //Console
            Console.ReadKey();
        }

        static void Main(string[] args)
        {
            Console.Title = "Discord Cleaner";
            Console.SetWindowSize(80, 40);
            DrawStartScreen();
        }
    }
}
