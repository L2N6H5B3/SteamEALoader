using System;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;

namespace SteamEALoader {
    class Program {
        static void Main(string[] args) {

            // Set Arguments
            string gamePath = args[0];
            string gameProcessName = args[1];
            string launcherProcessName = null;

            Console.WriteLine($"gamePath: {gamePath}");
            Console.WriteLine($"gameProcessName: {gameProcessName}");

            // If Launcher Name is provided
            if (args.Length > 2) {
                launcherProcessName = args[2];
                Console.WriteLine($"launcherProcessName: {launcherProcessName}");
            }

            try {
                // Get Running Game Processes
                var runningGameProcesses = Process.GetProcessesByName(gameProcessName);
                // If the Game is not running already
                if (runningGameProcesses.Count() == 0) {
                    // Create New Game Process
                    using (Process gameProcess = new Process()) {
                        gameProcess.StartInfo.FileName = gamePath;
                        gameProcess.Start();

                        Console.WriteLine("Starting Launch Timeout...");

                        // Unfortunate, but needed to be done.
                        Thread.Sleep(1000);

                        Console.WriteLine("Finished Timeout, Wait for game process to start with 60 retries...");

                        // Set Retries
                        int retry = 0;
                        int maxRetries = 60;
                        // While we should retry checking the status
                        while (retry < maxRetries) {
                            Thread.Sleep(1000);
                            Console.WriteLine("Checking Game Process Status...");
                            // Get Running Game Processes
                            runningGameProcesses = Process.GetProcessesByName(gameProcessName);
                            // If the Game is running
                            if (runningGameProcesses.Count() > 0) {
                                Console.WriteLine("Game is running...");
                                retry = maxRetries;
                            } else {
                                Console.WriteLine("Game not running yet...");
                                retry++;
                            }
                        }
                    }
                }

                // Get Running Game Processes
                runningGameProcesses = Process.GetProcessesByName(gameProcessName);
                // If the Game is running
                if (runningGameProcesses.Count() > 0) {

                    // Wait Until the Game Window is Visible
                    Console.WriteLine("Wait for game process to open window with 60 retries...");

                    // Set Retries
                    int retry = 0;
                    int maxRetries = 60;
                    // While we should retry checking the status
                    while (retry < maxRetries) {
                        Thread.Sleep(1000);
                        Console.WriteLine("Checking Game Process Window Status...");

                        // If the Running Game Process has a Window Handle
                        if (runningGameProcesses.FirstOrDefault().MainWindowHandle == IntPtr.Zero) {
                            Console.WriteLine("Game window not visible yet...");
                            retry++;
                        } else {
                            Console.WriteLine("Game window visible...");
                            retry = maxRetries;
                        }
                    }

                    // Set the Game Process Window as Foreground
                    SetForegroundWindow(runningGameProcesses.FirstOrDefault().MainWindowHandle);

                    bool gameRunning = true;
                    // While the Game is still running
                    while (gameRunning) {
                        Thread.Sleep(1000);
                        Console.WriteLine("Checking Game Process Status...");
                        // Get Running Game Processes
                        runningGameProcesses = Process.GetProcessesByName(gameProcessName);
                        // If the Game is running
                        if (runningGameProcesses.Count() > 0) {
                            Console.WriteLine("Game is running...");
                        } else {
                            gameRunning = false;
                        }
                    }

                    // If the Launcher Process Name was passed
                    if (launcherProcessName != null) {

                        Console.WriteLine("Game not running, killing Launcher...");
                        // Kill each instance of the Launcher Process
                        foreach (Process runningLauncherProcess in Process.GetProcessesByName(launcherProcessName)) {
                            runningLauncherProcess.Kill();
                        }
                    }
                }
            } catch (Exception e) {
                Console.WriteLine(e.Message);
            }
        }


        [DllImport("User32.dll", CallingConvention = CallingConvention.StdCall, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool ShowWindow([In] IntPtr hWnd, [In] int nCmdShow);

        [System.Runtime.InteropServices.DllImport("User32.dll")]
        private static extern bool SetForegroundWindow(IntPtr handle);
    }
}
