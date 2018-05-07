using System;
using System.Diagnostics;
using System.IO;
using System.Net;

namespace DiscordCleaner
{
    class Program
    {
        enum ExitCode : int
        {
            SUCCESS = 0,
            USER_INTERRUPT_DETECTED = 1,
            COULD_NOT_TERMINATE_DISCORD_PROCESS = 2,
            COULD_NOT_TERMINATE_DISCORD_UPDATE_PROCESS = 3,
            COULD_NOT_DELETE_DISCORD_APP_DATA = 4,
            COULD_NOT_DELETE_DISCORD_LOCAL_APP_DATA = 5
        }

        static void DrawStartScreen()
        {
            Console.Clear();
            Console.WriteLine("********************************************************************************");
            Console.WriteLine("* Discord Cleaner v0.2 by DatMayo                                Rel. 07.05.18 *");
            Console.WriteLine("* WARNING! All login credentials will be lost and must be re entered!          *");
            Console.WriteLine("*                                                                              *");
            Console.WriteLine("* GitHub: https://github.com/DatMayolein/Discord-Cleaner                       *");
            Console.WriteLine("* Icon: https://bit.ly/2HVZx0B                                                 *");
            Console.WriteLine("********************************************************************************");
            Console.WriteLine();
        }

        static bool CleanDiscordInstallation()
        {
            Console.Write("Would you like to cleanup your Discord installation? (y/N) ");
            ConsoleKeyInfo key = Console.ReadKey();
            switch (key.Key.ToString().ToLower())
            {
                case "n":
                    Console.WriteLine();
                    Console.WriteLine();
                    Console.WriteLine("Operation aborted!");
                    Console.ReadKey();
                    Environment.Exit((int)ExitCode.USER_INTERRUPT_DETECTED);
                    break;

                case "y":
                    Console.WriteLine();
                    Console.WriteLine();
                    return true;

                default:
                    DrawStartScreen();
                    break;
            }
            return false;
        }

        static void KillProcesses()
        {
            Console.WriteLine("Searching for Discord processes... ");
            Process[] discordHandle = Process.GetProcessesByName("discord");
            Console.WriteLine(string.Format("- {0} Discord process(es)", discordHandle.Length));
            foreach (Process p in discordHandle)
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
                    Console.Write("Process could not be killed, aborting...");
                    Console.ReadKey();
                    Environment.Exit((int)ExitCode.COULD_NOT_TERMINATE_DISCORD_PROCESS);
                }
            }

            Console.WriteLine("Killing Discord process finished...");
            Console.WriteLine();
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
                    Console.Write("Process could not be killed, aborting...");
                    Console.ReadKey();
                    Environment.Exit((int)ExitCode.COULD_NOT_TERMINATE_DISCORD_PROCESS);
                }
            }
            Console.WriteLine("Killing Discord-Update process finished...");
            Console.WriteLine();
            Console.Write("Now waiting for 5 seconds...");
            System.Threading.Thread.Sleep(5000); //Necessary, for whatever reason
            Console.WriteLine("OK");
            Console.WriteLine();
        }

        static void RemoveDiscord()
        {
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
                catch
                {
                    Console.WriteLine("Failed");
                    Console.Write("Process could not be killed, aborting...");
                    Console.ReadKey();
                    Environment.Exit((int)ExitCode.COULD_NOT_DELETE_DISCORD_APP_DATA);
                }
            }

            Console.WriteLine();
            Console.WriteLine(string.Format("Checking folder \"{0}\"", discordLocalAppData));
            if (Directory.Exists(discordLocalAppData))
            {
                Console.Write("Found existing discord in localappdata-folder, deleting it...");
                try
                {
                    Directory.Delete(discordLocalAppData, true);
                    Console.WriteLine("OK");
                }
                catch
                {
                    Console.WriteLine("Failed");
                    Console.Write("Process could not be killed, aborting...");
                    Console.ReadKey();
                    Environment.Exit((int)ExitCode.COULD_NOT_DELETE_DISCORD_LOCAL_APP_DATA);
                }
            }
            Console.WriteLine();
        }

        static void DownloadDiscordSetup()
        {
            Console.Write("Downloading new Discord-Version (this can take some time)... ");
            WebClient client = new WebClient();
            int currentProgress = 0;
            client.DownloadProgressChanged += (s, e) =>
            {
                if(currentProgress < e.ProgressPercentage)
                {
                    currentProgress = e.ProgressPercentage;
                    Console.SetCursorPosition(61, Console.CursorTop);
                    Console.Write(string.Format("{0}%", currentProgress));
                }
            };
            client.DownloadFileCompleted += (s, e) =>
            {
                Console.SetCursorPosition(60, Console.CursorTop);
                Console.WriteLine("Finished!");
                Console.WriteLine();
                InstallDiscord();
            };
            client.DownloadFileAsync(new Uri("https://discordapp.com/api/download?platform=win"), "DiscordSetup.exe");
            while(true)
                Console.ReadKey();
        }

        static void InstallDiscord()
        {
            Console.Write("Starting DiscordSetup.exe...");
            Process process = new Process();
            process.StartInfo.FileName = "DiscordSetup.exe";
            process.Start();
            process.WaitForExit();// Waits here for the process to exit.
            Console.WriteLine("Finished!");
            Console.WriteLine();
            Console.Write("All Done! Discord will start automatically.");
            Console.ReadKey();
        }

        static void Main(string[] args)
        {
            Console.Title = "Discord Cleaner";
            DrawStartScreen();
            if(CleanDiscordInstallation())
            {
                KillProcesses();
                RemoveDiscord();
                DownloadDiscordSetup();
                
            }
        }
    }
}
