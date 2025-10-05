/// <summary>
/// Discord Cleaner - A utility to completely remove and reinstall Discord
/// 
/// This tool helps resolve Discord issues by:
/// 1. Terminating all Discord processes
/// 2. Removing Discord application data
/// 3. Downloading and installing the latest Discord version
/// 
/// Author: DatMayo
/// Version: 0.3
/// Release: 05.10.25
/// Framework: .NET Framework 2.0 (for maximum compatibility)
/// </summary>

using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Threading;

namespace DiscordCleaner
{
    /// <summary>
    /// Main program class containing all Discord cleaning functionality
    /// </summary>
    class Program
    {
        #region Constants
        
        /// <summary>
        /// Time to wait after killing processes before proceeding (in milliseconds)
        /// </summary>
        private const int PROCESS_WAIT_TIME_MS = 5000;
        
        /// <summary>
        /// Official Discord download URL for the latest Windows installer
        /// </summary>
        private const string DISCORD_DOWNLOAD_URL = "https://discord.com/api/downloads/distributions/app/installers/latest?channel=stable&platform=win&arch=x64";
        
        #endregion
        
        #region Exit Codes
        
        /// <summary>
        /// Application exit codes for different failure scenarios
        /// </summary>
        enum ExitCode : int
        {
            /// <summary>Operation completed successfully</summary>
            SUCCESS = 0,
            /// <summary>User chose not to continue with the operation</summary>
            USER_INTERRUPT_DETECTED = 1,
            /// <summary>Failed to terminate Discord main processes</summary>
            COULD_NOT_TERMINATE_DISCORD_PROCESS = 2,
            /// <summary>Failed to terminate Discord update processes</summary>
            COULD_NOT_TERMINATE_DISCORD_UPDATE_PROCESS = 3,
            /// <summary>Failed to delete Discord AppData folder</summary>
            COULD_NOT_DELETE_DISCORD_APP_DATA = 4,
            /// <summary>Failed to delete Discord LocalAppData folder</summary>
            COULD_NOT_DELETE_DISCORD_LOCAL_APP_DATA = 5
        }
        
        #endregion

        #region UI Methods
        
        /// <summary>
        /// Displays the application welcome screen with version info and warnings
        /// </summary>
        static void DrawStartScreen()
        {
            Console.Clear();
            Console.WriteLine("********************************************************************************");
            Console.WriteLine("* Discord Cleaner v0.3 by DatMayo                                Rel. 05.10.25 *");
            Console.WriteLine("* WARNING! All login credentials will be lost and must be re entered!          *");
            Console.WriteLine("*                                                                              *");
            Console.WriteLine("* GitHub: https://github.com/DatMayolein/Discord-Cleaner                       *");
            Console.WriteLine("* Icon: https://bit.ly/2HVZx0B                                                 *");
            Console.WriteLine("********************************************************************************");
            Console.WriteLine();
        }

        /// <summary>
        /// Prompts the user for confirmation to proceed with Discord cleanup
        /// </summary>
        /// <returns>True if user confirms (Y), false otherwise. Exits application on 'N'.</returns>
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
        
        #endregion
        
        #region Process Management
        
        /// <summary>
        /// Terminates all Discord and Discord-Update processes
        /// Includes safety checks to avoid terminating unrelated processes
        /// </summary>
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
                catch (Exception ex)
                {
                    Console.WriteLine("Failed: " + ex.Message);
                    Console.Write("Discord process could not be terminated, aborting...");
                    Console.ReadKey();
                    Environment.Exit((int)ExitCode.COULD_NOT_TERMINATE_DISCORD_PROCESS);
                }
            }

            Console.WriteLine("Killing Discord process finished...");
            Console.WriteLine();
            Console.WriteLine("Searching for Discord-Update processes... ");
            
            // More safely identify Discord update processes
            Process[] allProcesses = Process.GetProcesses();
            int discordUpdateCount = 0;
            foreach (Process p in allProcesses)
            {
                try
                {
                    if (IsDiscordUpdateProcess(p))
                    {
                        discordUpdateCount++;
                        Console.Write("-> Killing Discord-Update process...");
                        p.Kill();
                        Console.WriteLine("OK");
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Failed: " + ex.Message);
                    Console.Write("Discord-Update process could not be terminated, aborting...");
                    Console.ReadKey();
                    Environment.Exit((int)ExitCode.COULD_NOT_TERMINATE_DISCORD_UPDATE_PROCESS);
                }
            }
            
            Console.WriteLine(string.Format("- {0} Discord-Update process(es)", discordUpdateCount));
            Console.WriteLine("Killing Discord-Update process finished...");
            Console.WriteLine();
            Console.Write("Now waiting for 5 seconds...");
            Thread.Sleep(PROCESS_WAIT_TIME_MS);
            Console.WriteLine("OK");
            Console.WriteLine();
        }
        
        /// <summary>
        /// Safely identifies if a process is a Discord update process
        /// Checks process name and executable path to avoid false positives
        /// </summary>
        /// <param name="process">Process to examine</param>
        /// <returns>True if the process is confirmed to be Discord-related</returns>
        static bool IsDiscordUpdateProcess(Process process)
        {
            try
            {
                if (process.ProcessName.ToLower() == "update")
                {
                    string fileName = process.MainModule.FileName;
                    return fileName.ToLower().Contains("discord") || fileName.ToLower().Contains("\\appdata\\local\\discord");
                }
            }
            catch
            {
                // Access denied or process already exited
            }
            return false;
        }
        
        #endregion
        
        #region File System Operations
        
        /// <summary>
        /// Removes Discord application data folders from both AppData and LocalAppData
        /// This includes user settings, cached data, and installation files
        /// </summary>
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
                catch (Exception ex)
                {
                    Console.WriteLine("Failed: " + ex.Message);
                    Console.Write("Discord AppData folder could not be deleted, aborting...");
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
                catch (Exception ex)
                {
                    Console.WriteLine("Failed: " + ex.Message);
                    Console.Write("Discord LocalAppData folder could not be deleted, aborting...");
                    Console.ReadKey();
                    Environment.Exit((int)ExitCode.COULD_NOT_DELETE_DISCORD_LOCAL_APP_DATA);
                }
            }
            Console.WriteLine();
        }
        
        #endregion
        
        #region Download and Installation
        
        /// <summary>
        /// Downloads the latest Discord installer from official servers
        /// Includes progress reporting and error handling
        /// </summary>
        static void DownloadDiscordSetup()
        {
            Console.Write("Downloading new Discord-Version (this can take some time)... ");
            
            using (WebClient client = new WebClient())
            {
                // Note: .NET Framework 2.0 doesn't have Timeout property
                int currentProgress = 0;
                bool downloadCompleted = false;
                Exception downloadError = null;
                
                client.DownloadProgressChanged += delegate(object sender, DownloadProgressChangedEventArgs e)
                {
                    if(currentProgress < e.ProgressPercentage)
                    {
                        currentProgress = e.ProgressPercentage;
                        Console.SetCursorPosition(61, Console.CursorTop);
                        Console.Write(string.Format("{0}%", currentProgress));
                    }
                };
                
                client.DownloadFileCompleted += delegate(object sender, System.ComponentModel.AsyncCompletedEventArgs e)
                {
                    downloadCompleted = true;
                    downloadError = e.Error;
                    
                    if (e.Error == null)
                    {
                        Console.SetCursorPosition(60, Console.CursorTop);
                        Console.WriteLine("Finished!");
                        Console.WriteLine();
                    }
                    else
                    {
                        Console.SetCursorPosition(60, Console.CursorTop);
                        Console.WriteLine("Failed!");
                        Console.WriteLine("Error: " + e.Error.Message);
                    }
                };
                
                try
                {
                    client.DownloadFileAsync(new Uri(DISCORD_DOWNLOAD_URL), "DiscordSetup.exe");
                    
                    // Wait for download completion instead of infinite loop
                    while (!downloadCompleted)
                    {
                        Thread.Sleep(100);
                    }
                    
                    if (downloadError != null)
                    {
                        Console.Write("Download failed, aborting...");
                        Console.ReadKey();
                        Environment.Exit(1);
                    }
                    
                    InstallDiscord();
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Download failed: " + ex.Message);
                    Console.Write("Aborting...");
                    Console.ReadKey();
                    Environment.Exit(1);
                }
            }
        }

        /// <summary>
        /// Executes the downloaded Discord installer and waits for completion
        /// </summary>
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
        
        #endregion
        
        #region Application Entry Point
        
        /// <summary>
        /// Application main entry point
        /// Orchestrates the complete Discord cleaning and reinstallation process
        /// </summary>
        /// <param name="args">Command line arguments (not used)</param>
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
        
        #endregion
    }
}
